using CoreOSC;
using EmbedIO.Utilities;
using MeaMod.DNS.Model;
using NAudio;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Octokit;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
using OSCVRCWiz.Services.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using WebRtcVadSharp;
using Whisper.net;
using Whisper.net.Ggml;
using Windows.Graphics.Printing3D;
using Windows.Storage.Streams;
using static System.Windows.Forms.DataFormats;

namespace OSCVRCWiz.Speech_Recognition
{
    public class WhisperRecognition
    {


        static private WebRtcVad vad;
        static private byte[] rawDataArray;
        static private int totalBytes;
        static private FrameLength frameLength = FrameLength.Is30ms;
        static private int frameSize;
        static private WaveInEvent waveIn;

        static bool AutoStopAfterSilence = true;
        static private DateTime lastVoiceDetectedTimeStamp;
        static bool recording = false;
        private static string langcode = "en";
        static bool voiceDetected = true;

        //static bool useGPU = true;



        static double MsBeforeAutoStop = 0.01;//10ms
        static OperatingMode NoiseOperatingModel = OperatingMode.HighQuality;
        static double minDuration = 1.5;//1.5 seconds
        static double maxDuration = 10;//10 seconds
        static DateTime silenceGracePeriodInitialisationTime;
        static DateTime silenceMaxDurationPeriodInitialisationTime;

        static bool WhisperEnabled =false;


        static private Stopwatch stopwatch;


        public static async void startUp()
        {
            stopwatch = new Stopwatch();

            waveIn = new WaveInEvent();
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += waveIn_RecordingStopped;
            waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
            silenceGracePeriodInitialisationTime = DateTime.Now.AddSeconds(minDuration);
            silenceMaxDurationPeriodInitialisationTime = DateTime.Now.AddSeconds(maxDuration);

            vad = new WebRtcVad()
            {
                OperatingMode = NoiseOperatingModel,
                FrameLength = frameLength,
                SampleRate = SampleRate.Is16kHz,
            };

            frameSize = (int)vad.SampleRate / 1000 * 2 * (int)frameLength;
          //  await InitialiseLocalTranscription();

        }
        public static void StopWhisper()
        {
            if (!VoiceWizardWindow.MainFormGlobal.rjToggleWhisperContinuous.Checked) { DoSpeech.speechToTextOffSound(); }
            waveIn.StopRecording();
            recording = false;
            if (!VoiceWizardWindow.MainFormGlobal.rjToggleWhisperContinuous.Checked)
            {
                OutputText.outputLog("[Whisper Stopped Listening");
            }

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                OSC.OSCSender.Send(sttListening);
            }
        }
        public static void StartWhisper()
        {
            DoSpeech.speechToTextOnSound();
            OutputText.outputLog("[Whisper Listening]");
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                OSC.OSCSender.Send(sttListening);
            }
            totalBytes = 0;
            rawDataArray = new byte[0];
            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();
            waveIn.StartRecording();
            silenceGracePeriodInitialisationTime = DateTime.Now.AddSeconds(minDuration);
            silenceMaxDurationPeriodInitialisationTime = DateTime.Now.AddSeconds(maxDuration);
            recording = true;
           // firstDataAvailable = false;
            voiceDetected = false;
        }

        //static bool WhisperContinuous =false;
        static TaskCompletionSource<bool> recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
        public static async Task ToggleWhisper()
        {


            
            if (!VoiceWizardWindow.MainFormGlobal.rjToggleWhisperContinuous.Checked)
            {

                if (recording)
                {
                    StopWhisper();
                 //   DoSpeech.speechToTextOffSound();
                }
                else if (!recording)
                {
                    if (CheckForModel())
                    {
                        recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
                        StartWhisper();
                      

                    }
                }
            }
            else
            {
                if (!WhisperEnabled)
                {
                    OutputText.outputLog("[Whisper Listening (Continuous)]");
                    DoSpeech.speechToTextOnSound();
                    WhisperEnabled = true;
                   // InitialiseLocalTranscription();

                    while (WhisperEnabled)
                        {
                            if (CheckForModel())
                            {

                            
                            minDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text.ToString(), CultureInfo.InvariantCulture); //1
                            maxDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text.ToString(), CultureInfo.InvariantCulture); //8    
                            MsBeforeAutoStop = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text.ToString(), CultureInfo.InvariantCulture);  //1


                            
                            //crashing was caused by model path not being set :), it was only set by checkformodel lol
                            // recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
                              voiceDetected = false;
                            recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
                            totalBytes = 0;
                            rawDataArray = new byte[0];
                            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();
                            waveIn.StartRecording();
                            silenceGracePeriodInitialisationTime = DateTime.Now.AddSeconds(minDuration);
                            silenceMaxDurationPeriodInitialisationTime = DateTime.Now.AddSeconds(maxDuration);
                            recording = true;

                             
                              await recordingTaskCompletionSource2.Task;


                            }
                            else
                            {
                                WhisperEnabled = false;
                            }
                          // await recordingTaskCompletionSource2.Task;


                    }
                   
                    
                     
                }
                      
                else
                {
                    // must allow canceling of recognition             
                    WhisperEnabled = false;
                    StopWhisper();
                    DoSpeech.speechToTextOffSound();
                    OutputText.outputLog("[Whisper Stopped Listening");
                }
            }
           
        

        }


        static bool firstDataAvailable = false;

        private static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
           /* if (firstDataAvailable = false)
            {
                voiceDetected = false;
                firstDataAvailable = true;
            }*/
           
            // copying audio data to raw data array
            byte[] newArray = new byte[rawDataArray.Length + e.BytesRecorded];
            System.Buffer.BlockCopy(rawDataArray, 0, newArray, 0, rawDataArray.Length);
            System.Buffer.BlockCopy(e.Buffer, 0, newArray, rawDataArray.Length, e.BytesRecorded);
            rawDataArray = newArray;
            totalBytes += e.BytesRecorded;



            var buffer = e.Buffer.Take(frameSize).ToArray();
            if (vad.HasSpeech(buffer)) 
            {
                voiceDetected = true;
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.Green;
                });

                //  OutputText.outputLog("[detected]");

            }
            else
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                });
            }


            // checking for silence, but only after a grace period at the start
            if (DateTime.Now > silenceGracePeriodInitialisationTime)//minimum silence duration
            {
               // var buffer = e.Buffer.Take(frameSize).ToArray();
                if (!vad.HasSpeech(buffer))
                {
                    if (recording && AutoStopAfterSilence)
                    {
                        // Set 'silence' progress bar
                       // TimeSpan silentPeriod = DateTime.Now - lastVoiceDetectedTimeStamp;
                       // double silentPeriodInMilliseconds = silentPeriod.TotalMilliseconds;


                        if (DateTime.Now > lastVoiceDetectedTimeStamp.AddMilliseconds(MsBeforeAutoStop))
                        {
                          //  OutputText.outputLog("[Whisper: Silence Detected, Auto-Stopping");
                            StopWhisper();
                            

                        }

                    }
                }
                else
                {                  
                    lastVoiceDetectedTimeStamp = DateTime.Now;
                }
            }
            if (recording && (DateTime.Now > silenceMaxDurationPeriodInitialisationTime))
            {
               // OutputText.outputLog("[Whisper: Max Audio Duration Reached, Auto-Stopping");
                StopWhisper();

            }
        }

        private static void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                WaveFileWriter waveWriter = new WaveFileWriter(outputStream, new WaveFormat(16000, 16, 1));

                waveWriter.Write(rawDataArray, 0, totalBytes);
                waveWriter.Flush();


                // this was if (voicedetected) but that detection wasn't great for short utterances.
                if (voiceDetected)
                {
                    // OutputText.outputLog("Transcribing audio stream...");

                    stopwatch.Reset();
                    stopwatch.Start();
                    Task<string> transcriptionTask = TranscribeLocalAsync(outputStream);
                    transcriptionTask.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            OutputText.outputLog($"Failed to transcribe: {t.Exception}", Color.Red);
                            return;
                        }
                        else
                        {

                            if (t.Result.Trim() == "[ERROR]")
                            {
                                return;
                            }

                            stopwatch.Stop();


                            if (t.Result == null)
                            {
                                OutputText.outputLog("[Whisper: No text found]");
                                return;
                            }

                            string result = t.Result;
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked)
                            {
                                result = purifyOutput(result);
                            }
                            //  if(result != "")
                            //  {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                            {
                                OutputText.outputLog($"Whisper Processing Time: {stopwatch.Elapsed.TotalSeconds}");
                            }
                            //stopwatch.Elapsed.TotalSeconds;
                            TTSMessageQueue.QueueMessage(result, "Whisper");
                         //   }

                           
                        }


                    });
                }

            }
            recordingTaskCompletionSource2.SetResult(true);
        }


        private static string purifyOutput(string input)
        {
            string result = input;

            // Remove bracketed sections such as [ EMPTY ] and ( SILENCE )
            result = Regex.Replace(result, @"\[.*?\]|\(.*?\)", string.Empty);

            // strip duplicated whitespace and any on the ends.
            result = result.Replace("[BLANK_AUDIO]", "");
            result = result.Replace("[MUSIC PLAYING]", "");
            result = result.Replace("  ", " ").Trim();
            result = result.Replace("  ", " ").Trim();
            return result;
        }


        private static string GgmlModelFileName;
        private static WhisperFactory whisperFactory;
        private static WhisperProcessor whisperProcessor;

        public static async Task<string> TranscribeLocalAsync(MemoryStream audioStream)
        {

            await InitialiseLocalTranscription();

            if (whisperFactory == null)
            {
                OutputText.outputLog("[Whisper Error: Failed to initialise whisperFactory]", Color.Red);
                return "[ERROR]";
            }


            audioStream.Flush();
            audioStream.Seek(0, SeekOrigin.Begin);
            var textResult = "";
            // OutputText.outputLog("[Whisper: Transcribing]");
            await foreach (var result in whisperProcessor.ProcessAsync(audioStream))
            {
                textResult += result.Text + " ";
            }

            return textResult;
        }

        private static bool CheckForModel()
        {
            GgmlModelFileName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text;
            FileInfo fInfo = new FileInfo(GgmlModelFileName);
            if (fInfo.Exists)
            {
                long lengthInBytes = fInfo.Length;

                // None of the models are <65mb to my knowledge so it's broken if that small.
                if (lengthInBytes < 65000000)
                {
                    File.Delete(GgmlModelFileName);
                }
            }

            if (!File.Exists(GgmlModelFileName))
            {
                OutputText.outputLog("[No Model Found. Auto installing selected Whisper model. To download higher accuracy Whisper model navigate to Speech Provider > Local > Whisper.cpp Model and download/select a bigger model]", Color.Red);
                downloadWhisperModel();
                return false;
            }
            else
            {
                return true;
            }

        }

        private static async Task InitialiseLocalTranscription()
        {



          //  if (whisperFactory == null)//this was preventing me from switching models
          //  {
                // OutputText.outputLog("Creating whisper factory for transcription");
                GgmlModelFileName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text;

                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string dllPath = System.IO.Path.Combine(appPath, "runtimes", "win-x64", "whisper.dll");
                    // OutputText.outputLog("Attempting to load dll at " + dllPath);
                    whisperFactory = WhisperFactory.FromPath(GgmlModelFileName, libraryPath: dllPath, useGpu: VoiceWizardWindow.MainFormGlobal.rjToggleWhisperUseGPU.Checked);
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"[Error creating WhisperFactory: {ex.Message}]", Color.Red);
                  // OutputText.outputLog("Model location: " + GgmlModelFileName, Color.Red);
                    OutputText.outputLog(ex.Message, Color.Red);
                    return;
                }


                
           // }
            langcode = LanguageSelect.fromLanguageNew(VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString(), "sourceLanguage", "Whisper");
            whisperProcessor = whisperFactory.CreateBuilder().WithLanguage(langcode).Build();





        }


        public static void downloadWhisperModel()
        {
            string address = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/";

            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativePath = "Assets\\models\\";


            switch (VoiceWizardWindow.MainFormGlobal.comboBoxWhisperModelDownload.Text.ToString())
            {
                case "ggml-tiny.bin (77.7 MB)":
                    relativePath += "ggml-tiny.bin";
                    address += "ggml-tiny.bin";

                    break;
                case "ggml-tiny-q5_1.bin (32.2 MB)":
                    relativePath += "ggml-tiny-q5_1.bin";
                    address += "ggml-tiny-q5_1.bin";

                    break;

                case "ggml-base.bin (148 MB)":
                    relativePath += "ggml-base.bin";
                    address += "ggml-base.bin";

                    break;

                case "ggml-base-q5_1.bin (59.7 MB)":
                    relativePath += "ggml-base-q5_1.bin";
                    address += "ggml-base-q5_1.bin";

                    break;

                case "ggml-small.bin (488 MB)":
                    relativePath += "ggml-small.bin";
                    address += "ggml-small.bin";

                    break;

                case "ggml-small-q5_1.bin (190 MB)":
                    relativePath += "ggml-small-q5_1.bin";
                    address += "ggml-small-q5_1.bin";

                    break;

                case "ggml-medium.bin (1.5 GB)":
                    relativePath += "ggml-medium.bin";
                    address += "ggml-medium.bin";

                    break;

                case "ggml-medium-q5_0.bin (539 MB)":
                    relativePath += "ggml-medium-q5_0.bin";
                    address += "ggml-medium-q5_0.bin";

                    break;

                default: break;
            }

            string path = Path.Combine(basePath, relativePath);

            if (!System.IO.File.Exists(path))
            {
                VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = System.Drawing.Color.DarkOrange;
                VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model downloading... PLEASE WAIT";
                OutputText.outputLog("[Whisper model downloading... PLEASE WAIT]", Color.DarkOrange);

                WebClient client = new WebClient();
                Uri uri = new Uri(address);

                // Call DownloadFileCallback2 when the download completes.
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback2);

                // Specify a progress notification handler here ...

                client.DownloadFileAsync(uri, path);
            }
            VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text = path;
        }
        private static void DownloadFileCallback2(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Console.WriteLine("File download cancelled.");
                MessageBox.Show("File download cancelled");
                OutputText.outputLog("[Whisper Model Download Cancelled: Model Download was cancelled, If this was un-intentional try manually downloading the model from here]", System.Drawing.Color.Red);
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = System.Drawing.Color.Red;
                    VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model error";
                });
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("Error while downloading file.");
                OutputText.outputLog("[Whisper Model Download Error: " + e.Error.Message + "]", System.Drawing.Color.Red);
                //Console.WriteLine(e.Error.ToString());
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = System.Drawing.Color.Red;
                    VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model error";
                });
                return;
            }
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = System.Drawing.Color.Green;
                VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model downloaded";
            });
            OutputText.outputLog("[Your Whisper Model has completed downloading]", System.Drawing.Color.Green);
            MessageBox.Show("Your Whisper Model has completed downloading");



        }


    }
}
