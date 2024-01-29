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
        static bool voiceDetected = false;


        static string previousLangcode = "";
        static string previousModel = "";

        //static bool useGPU = true;



        static double MsBeforeAutoStop = 0.01;//10ms
        static OperatingMode NoiseOperatingModel = OperatingMode.HighQuality;
        static double minDuration = 1.5;//1.5 seconds
        static double maxDuration = 10;//10 seconds
        static DateTime silenceGracePeriodInitialisationTime;
        static DateTime silenceMaxDurationPeriodInitialisationTime;
       
        static bool WhisperEnabled = false;


        static private Stopwatch stopwatch;
        static private Stopwatch stopwatchClipDuration;

        private static CancellationTokenSource whisperCt = new();


        public static async void startUp()
        {
            stopwatch = new Stopwatch();
            stopwatchClipDuration = new Stopwatch();

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
        public static async void StopWhisper()
        {
            if (recording == true)
            {
                if (!VoiceWizardWindow.MainFormGlobal.rjToggleWhisperContinuous.Checked) { DoSpeech.speechToTextOffSound(); }
                waveIn.StopRecording();
                stopwatchClipDuration.Stop();

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
        }
        public static void StopWhisperNow()
        {
            StopWhisper();
            whisperCt.Cancel();
        }
        public static void StartWhisper()
        {
            whisperCt = new();
            DoSpeech.speechToTextOnSound();
            OutputText.outputLog("[Whisper Listening]");
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                OSC.OSCSender.Send(sttListening);
            }

            minDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text.ToString(), CultureInfo.InvariantCulture); //1
            maxDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text.ToString(), CultureInfo.InvariantCulture); //8    
            MsBeforeAutoStop = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text.ToString(), CultureInfo.InvariantCulture);  //1


            totalBytes = 0;
            rawDataArray = new byte[0];
            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();
            isDetecting = false;
            voiceDetected = false;
            recording = true;
            stopwatchClipDuration.Reset();
            stopwatchClipDuration.Start();
            waveIn.StartRecording();
            silenceGracePeriodInitialisationTime = DateTime.Now.AddSeconds(minDuration);
            silenceMaxDurationPeriodInitialisationTime = DateTime.Now.AddSeconds(maxDuration);

            // firstDataAvailable = false;
            //  isDetecting = false;
            // voiceDetected = false;
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

                    //can not dispose of this before it has chance to trancribe or will cause un-catchable app crash (access violation)
                    OutputText.outputLog("[Stopping Whisper... Please Wait");
                   /* if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("[Waiting for Whisper to finish transcribing]");
                    }*/
                    await recordingTaskCompletionSource2.Task;

                  /*  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("[Disposing of Whisper factory and processor from memory]");
                    }*/
                    //   Task.Delay(1000);
                    whisperFactory?.Dispose();
                    whisperProcessor?.Dispose();
                    whisperFactory = null;
                    whisperProcessor = null;
                   // whisperCt.Cancel();

                    //   DoSpeech.speechToTextOffSound();
                }
                else if (!recording)
                {
                    if (CheckForModel())
                    {
                        // GgmlModelFileName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text;
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
                    if (CheckForModel())
                    {
                        WhisperEnabled = true;
                    }
                    else
                    {
                        WhisperEnabled = false;
                        OutputText.outputLog("[Whisper Stopped Listening]");
                        DoSpeech.speechToTextOffSound();
                    }
                    // InitialiseLocalTranscription();

                    while (WhisperEnabled)
                    {
                       


                            minDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text.ToString(), CultureInfo.InvariantCulture); //1
                            maxDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text.ToString(), CultureInfo.InvariantCulture); //8    
                            MsBeforeAutoStop = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text.ToString(), CultureInfo.InvariantCulture);  //1



                            //crashing was caused by model path not being set :), it was only set by checkformodel lol
                            // recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
                            whisperCt = new();
                            isDetecting = false;
                            voiceDetected = false;
                            recording = true;
                            recordingTaskCompletionSource2 = new TaskCompletionSource<bool>();
                            totalBytes = 0;
                            rawDataArray = new byte[0];
                            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();
                        stopwatchClipDuration.Reset();
                        stopwatchClipDuration.Start();
                        waveIn.StartRecording();
                            silenceGracePeriodInitialisationTime = DateTime.Now.AddSeconds(minDuration);
                            silenceMaxDurationPeriodInitialisationTime = DateTime.Now.AddSeconds(maxDuration);


                            await recordingTaskCompletionSource2.Task;


                       
                        // await recordingTaskCompletionSource2.Task;


                    }



                }

                else
                {
                    // must allow canceling of recognition             
                    WhisperEnabled = false;
                    StopWhisper();
                    DoSpeech.speechToTextOffSound();

                    OutputText.outputLog("[Stopping Whisper... Please Wait");
                    //can not dispose of this before it has chance to trancribe or will cause un-catchable app crash (access violation)
                   /* if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("[Waiting for Whisper to finish transcribing]");
                    }*/
                    await recordingTaskCompletionSource2.Task;
                   /* if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("[Disposing of Whisper factory and processor from memory]");
                    }
                    */
                    //  await Task.Delay(5000);
                    whisperFactory?.Dispose();
                    whisperProcessor?.Dispose();
                    whisperFactory = null;
                    whisperProcessor = null;
                   // whisperCt.Cancel();

                    OutputText.outputLog("[Whisper Stopped Listening");
                }
            }



        }


        //tatic bool firstDataAvailable = false;
        static bool isDetecting = false;
        private static TimeSpan startTime = DateTime.MinValue.TimeOfDay;
        private static TimeSpan endTime = DateTime.MinValue.TimeOfDay;
        //ublic static bool isVoiceDetected = false;
        public static List<Tuple<TimeSpan, TimeSpan>> voiceActivationTimes = new List<Tuple<TimeSpan, TimeSpan>>();
        private static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (whisperCt.Token.IsCancellationRequested)
            {
                voiceDetected = false;
                waveIn.StopRecording();
            }
            /* if (deepgramCt.Token.IsCancellationRequested)
             {
                 validAudioClip = false;
                 waveSource.StopRecording();
             }*/
            //must use cancellation token here to prevent this going after form closes
            //if (recording)
            // {

            // copying audio data to raw data array
            byte[] newArray = new byte[rawDataArray.Length + e.BytesRecorded];
            System.Buffer.BlockCopy(rawDataArray, 0, newArray, 0, rawDataArray.Length);
            System.Buffer.BlockCopy(e.Buffer, 0, newArray, rawDataArray.Length, e.BytesRecorded);
            rawDataArray = newArray;
            totalBytes += e.BytesRecorded;

          


            var buffer = e.Buffer.Take(frameSize).ToArray();



            if (vad.HasSpeech(buffer))
            {

                if (!isDetecting)
                {
                    voiceDetected = true;
                    isDetecting = true;
                    startTime = DateTime.Now.TimeOfDay;//update on inital detect
                  if (!whisperCt.Token.IsCancellationRequested)
                  {
                  VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                  {
                      VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.Green;
                  });
                  }
                }
                // Update the end time constantly while detecting
                endTime = DateTime.Now.TimeOfDay;
                //  OutputText.outputLog("[detected]");

            }
            else
            {
                if (isDetecting)
                {
                    isDetecting = false;
                    if (!whisperCt.Token.IsCancellationRequested)
                    {


                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                        });
                    }
                    voiceActivationTimes.Add(new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
                    startTime = DateTime.MinValue.TimeOfDay;
                    endTime = DateTime.MinValue.TimeOfDay;

                }
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
                            if (isDetecting)//incase it doesnt get a chance to stop
                            {
                                isDetecting = false;
                                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                {
                                    VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                                });
                                voiceActivationTimes.Add(new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
                                startTime = DateTime.MinValue.TimeOfDay;
                                endTime = DateTime.MinValue.TimeOfDay;

                            }
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
                if (isDetecting)//incase it doesnt get a chance to stop
                {
                    isDetecting = false;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                    });
                    voiceActivationTimes.Add(new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
                    startTime = DateTime.MinValue.TimeOfDay;
                    endTime = DateTime.MinValue.TimeOfDay;

                }
                StopWhisper();


            }
            //  }
        }

        private static async void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                // WaveFileWriter waveWriter = new WaveFileWriter(outputStream, new WaveFormat(16000, 16, 1));
                //waveWriter.Write(rawDataArray, 0, totalBytes);
                // waveWriter.Flush();

                using (WaveFileWriter waveWriter = new WaveFileWriter(new IgnoreDisposeStream(outputStream), new WaveFormat(16000, 16, 1)))
                {
                   
                    waveWriter.Write(rawDataArray, 0, totalBytes);
                    
                    
                    // waveWriter.Flush();
                    waveWriter.Dispose();
                }
                //



                // if(totalDurationOfAllSpeechMillis>500)
                // this was if (voicedetected) but that detection wasn't great for short utterances.
                if (voiceDetected)
                {
                    int totalDurationOfAllSpeechMillis = 0;
                    foreach (var utterance in voiceActivationTimes)
                    {
                        totalDurationOfAllSpeechMillis += (int)(utterance.Item2 - utterance.Item1).TotalMilliseconds;
                    }
                    voiceActivationTimes.Clear();

                    // OutputText.outputLog("Transcribing audio stream...");

                    stopwatch.Reset();
                    stopwatch.Start();
                    string result = "";
                    try
                    {
                        result = await TranscribeLocalAsync(outputStream);
                        stopwatch.Stop();

                       /* if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked)
                        {
                            result = purifyOutput(result);
                        }*/
                        if (result == "" || result == " ")
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                            {
                                OutputText.outputLog($"[No voice detected]", Color.DarkOrange);
                            }
                        }
                        else 
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                            {
                                OutputText.outputLog($"[Processing Time: {Math.Round(stopwatch.Elapsed.TotalMilliseconds)} ms]", Color.Purple);
                                OutputText.outputLog($"[Clip Duration: {Math.Round(stopwatchClipDuration.Elapsed.TotalMilliseconds)} ms]", Color.Purple);

                            }

                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {
                                VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                            });
                           


                            if (totalDurationOfAllSpeechMillis > ((float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinVADDuration.Text.ToString(), CultureInfo.InvariantCulture)*1000))
                            {
                                //stopwatch.Elapsed.TotalSeconds;
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                                {
                                    OutputText.outputLog($"[VAD Duration: {totalDurationOfAllSpeechMillis} ms]", Color.Purple);
                                }
                                TTSMessageQueue.QueueMessage(result, "Whisper");
                            }
                            else
                            {
                                //OutputText.outputLog($"[voice detected for a total of {totalDurationOfAllSpeechMillis} ms]");
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                                {
                                    OutputText.outputLog($"[VAD Filtered: {result} VAD Duration: {totalDurationOfAllSpeechMillis} ms]", Color.DarkOrange);
                                }
                            }
                        }
                      
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog($"Failed to transcribe: {ex.Message}", Color.Red);

                        StopWhisper();
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleWhisperContinuous.Checked)
                        {
                            WhisperEnabled = false;
                            DoSpeech.speechToTextOffSound();
                            OutputText.outputLog("[Whisper Stopped Listening");
                        }
                        return;
                    }


                    /* transcriptionTask.ContinueWith(t =>
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
                             if(result != "")
                             {
                                 if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                                 {
                                     OutputText.outputLog($"Whisper Processing Time: {stopwatch.Elapsed.TotalSeconds}");
                                 }
                                 if (totalDurationOfAllSpeechMillis > 998)
                                 {
                                     //stopwatch.Elapsed.TotalSeconds;
                                     TTSMessageQueue.QueueMessage(result, "Whisper");
                                 }
                                 else
                                 {
                                     OutputText.outputLog($"[voice detected for a total of {totalDurationOfAllSpeechMillis} ms]");
                                     OutputText.outputLog($"[Whisper Invalid Clip Prevented: {result}]");
                                 }
                             }


                         }


                     });
                 } */

                }


            }
            recordingTaskCompletionSource2.SetResult(true);
        }


        private static string purifyOutput(string input)
        {
            string result = input;

            // Remove bracketed sections such as [ EMPTY ] and ( SILENCE )
            //result = Regex.Replace(result, @"\[.*?\]|\(.*?\)", string.Empty);
            result = Regex.Replace(result, @"\[.*?\]|\(.*?\)", string.Empty);
            result = Regex.Replace(result, @"\*.*?\*", string.Empty);

            // strip duplicated whitespace and any on the ends.
            result = result.Replace("[BLANK_AUDIO]", "");
            result = result.Replace("[MUSIC PLAYING]", "");
            result = result.Replace("  ", " ").Trim();
            result = result.Replace("  ", " ").Trim();
            if(result.Contains("[")|| result.Contains("(") || result.Contains("【") || result.Contains("】"))
            {
                result = "";
            }
            result = result.Replace("\"", "");
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
                //   OutputText.outputLog($"Probability: {result.Probability}");         
                // OutputText.outputLog($"Min Probability: {result.MinProbability}");
                //  OutputText.outputLog($"Max Probability: {result.MaxProbability}");
                //  OutputText.outputLog($"Text: {result.Text}");
                string purified=purifyOutput(result.Text);
                if (purified != " " && purified != "" && purified != " ." && purified != ".")
                {
                    if (result.Probability > (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinConfidence.Text.ToString(), CultureInfo.InvariantCulture))
                    {
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                        {
                            OutputText.outputLog($"[Result: {result.Text}]", Color.Purple);
                            OutputText.outputLog($"[Confidence: {result.Probability}]", Color.Purple);
                            OutputText.outputLog($"[Min: {result.MinProbability} Max: {result.MaxProbability}]", Color.Purple);
                        }
                        if(result.Text.Contains("Thank you") && (result.MaxProbability ==1))
                        {
                            OutputText.outputLog($"[Known Hallucination Prevented: {result.Text}]", Color.Green);
                            return "";
                        }
                        textResult += result.Text + " ";
                    }
                    else
                    {
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                        {
                            OutputText.outputLog($"[Confidence Filtered: '{result.Text}' Confidence: {result.Probability} Min: {result.MinProbability} Max: {result.MaxProbability}]]", Color.DarkOrange);
                        }
                    }
                }
            }

            return textResult;
        }

        private static bool CheckForModel()
        {
            GgmlModelFileName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text;
            FileInfo fInfo = new FileInfo(GgmlModelFileName);
           /* if (fInfo.Exists)
            {
                long lengthInBytes = fInfo.Length;

                // None of the models are <65mb to my knowledge so it's broken if that small.
                if (lengthInBytes < 65000000)
                {
                    File.Delete(GgmlModelFileName);
                }
            }*/

            if (!File.Exists(GgmlModelFileName))
            {
                OutputText.outputLog("[No Model Found. Attempting to auto installing selected Whisper model. To manually download higher accuracy Whisper model navigate to Speech Provider > Local > Whisper.cpp Model and download/select a bigger model]", Color.Red);
                WhisperRecognition.downloadWhisperModel();
                return false;
            }
            else
            {
                return true;
            }

        }

        private static async Task InitialiseLocalTranscription()
        {
            // whisperFactory = null;
            // whisperProcessor = null;
            langcode = LanguageSelect.fromLanguageNew(VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString(), "sourceLanguage", "Whisper");
            if ((langcode != previousLangcode) || (GgmlModelFileName != previousModel) || (whisperFactory == null))
            {
                previousLangcode = langcode;
                previousModel = GgmlModelFileName;
              /*  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                {
                    OutputText.outputLog($"[Disposing of Whisper Factory and Processor from memory and rebuilding]");
                }*/
                whisperFactory?.Dispose();
                whisperProcessor?.Dispose();
                whisperFactory = null;
                whisperProcessor = null;


                // if (whisperFactory == null)//this was preventing me from switching models
                // {
                // OutputText.outputLog("Creating whisper factory for transcription");
                GgmlModelFileName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text;

                try
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    string dllPath = System.IO.Path.Combine(appPath, "runtimes", "win-x64", "whisper.dll");
                    // OutputText.outputLog("Attempting to load dll at " + dllPath);
                    whisperFactory = WhisperFactory.FromPath(GgmlModelFileName, libraryPath: dllPath, useGpu:true);
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"[Error creating WhisperFactory: {ex.Message}]", Color.Red);
                    // OutputText.outputLog("Model location: " + GgmlModelFileName, Color.Red);
                    OutputText.outputLog(ex.Message, Color.Red);
                    return;
                }

                whisperProcessor = whisperFactory.CreateBuilder().WithLanguage(langcode).WithProbabilities().Build();
            }
            //  }







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
