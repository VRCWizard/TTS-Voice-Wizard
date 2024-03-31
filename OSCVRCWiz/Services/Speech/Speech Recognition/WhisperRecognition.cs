using Amazon.Runtime.Internal.Util;
using CoreOSC;
using NAudio.Wave;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Resources.Whisper;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using SpotifyAPI.Web;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Windows.Shapes;
using WebRtcVadSharp;
using Whisper;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OSCVRCWiz.Speech_Recognition
{
    public class WhisperRecognition
	{
       static bool WhisperEnabled = false;
        public static string WhisperString = "";
        public static string WhisperPrevText = "";
        private static string langcode = "en";
        private static bool WhisperError = false;
        private static bool WhisperAllowStop = false;
        public static TimeSpan WhisperStartTime = TimeSpan.Zero;


        private static WebRtcVad vad;
        private static WaveInEvent waveIn;
      //  static OperatingMode NoiseOperatingModel = OperatingMode.VeryAggressive;
        private static FrameLength frameLength = FrameLength.Is30ms;
        private static int frameSize;



        private static CaptureThread? ctt;
        public static void toggleWhisper()
        {
            if (WhisperEnabled == false )
            {

                //GetGPUs();
                if (VoiceWizardWindow.MainFormGlobal.rjToggleVAD.Checked)
                {
                    try
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            vad.OperatingMode = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;
                        });
                    }
                    catch (Exception ex)
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex = 0;
                        });
                        vad.OperatingMode = OperatingMode.HighQuality;
                        OutputText.outputLog("[Error selecting VAD mode, defaulting to 0]");
                    }
                    
                    waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();
                    waveIn.StartRecording();
                }

                DoSpeech.speechToTextOnSound();
                WhisperEnabled = true;
                string UseThisMic = getWhisperInputDevice().ToString();
                OutputText.outputLog("[Whisper Mic Selected]");
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    langcode = LanguageSelect.fromLanguageNew(VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString(), "sourceLanguage", "Whisper");
                   // fromLanguageID(VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString());//set lang code for recognition

                });
                OutputText.outputLog("[Whisper Language Selected]");

                string[] args = {
                "-c",UseThisMic,
                "-m",  VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text,
                "-l", langcode,
                 };
                OutputText.outputLog("[Starting Whisper]");
                WhisperStartTime = DateTime.Now.TimeOfDay;
                Task.Run(() => doWhisper(args));

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }


            }

            else
            {
                
                if (ctt != null && WhisperAllowStop==true)
                {
                    waveIn?.StopRecording();
                    DoSpeech.speechToTextOffSound();
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text = $"Whisper Debug: ";
                    });
                    try
                    {
                        WhisperString = "";
                        StopWhisper();
                        WhisperEnabled = false;
                        OutputText.outputLog("[Whisper Stopping Listening]");
                        WhisperError = false;

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {
                            var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                            OSC.OSCSender.Send(sttListening);
                        }
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog("[Error Stopping Whisper (manual): "+ ex.Message+ " ]", System.Drawing.Color.Red);
                    }
                }
                else
                {
                    OutputText.outputLog("[Could not stop whisper (slow down)]", System.Drawing.Color.Red);
                }

            }
        }
        public static void autoStopWhisper()
        {
            try
            {
                // if (WhisperEnabled == true && WhisperAllowStop == true)
                if (WhisperEnabled == true && WhisperAllowStop == true)
                {
                    waveIn?.StopRecording();//fix for when you switch st methods and switch back while whisper was running
                    WhisperString = "";
                        StopWhisper();
                        WhisperEnabled = false;
                        OutputText.outputLog("[Whisper Stopped Listening]");
                        WhisperError = false;
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {
                            var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                            OSC.OSCSender.Send(sttListening);
                        }
                        DoSpeech.speechToTextOffSound();
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text = $"Whisper Debug: ";
                        });
                    

                }
               

              }
                catch(Exception ex) {
                OutputText.outputLog("[Error Stopping Whisper (auto): " + ex.Message + " ]", System.Drawing.Color.Red);
            }
}

            public static int getWhisperInputDevice()
        {
            

            // Setting to Correct Input Device
            using iMediaFoundation mf = Library.initMediaFoundation();
            CaptureDeviceId[] devices = mf.listCaptureDevices() ??
                throw new ApplicationException("This computer has no audio capture devices");

            for (int i = 0; i < devices.Length; i++)
            {
               // Debug.WriteLine("#{0}: {1}", i, devices[i].displayName);
                if (AudioDevices.currentInputDeviceName.ToString() == devices[i].displayName.ToString())
                {
                    return i;

                }
            }
           
            return 0;

        }

     /*   public static string[] GetGPUs()
        {
            string[] gpuList = Library.listGraphicAdapters();

            // Using Console.WriteLine for simple logging
            Debug.WriteLine("List of GPUs:");
            foreach (var gpu in gpuList)
            {
                Debug.WriteLine(gpu);
            }
            return gpuList;

        }*/
     

        public static void StopWhisper()
        {
      
            ctt?.Stop();
            ctt = null;
        }

        // changed to static here -chrisk
        static CommandLineArgs cla;
        static Whisper.Context context;
        static iAudioCapture captureDev;
      //  static CaptureThread whisperThread;

        // added: set language -chrisk
        public static void setLanguage(string language)
        {
            if (context != null && WhisperEnabled==true)
            {
                langcode = LanguageSelect.fromLanguageNew(language, "sourceLanguage", "Whisper");
                eLanguage? elang = Library.languageFromCode(langcode);
                if (elang != null)
                {
                    Stopwatch stopwatch = new Stopwatch();

                    // Start the stopwatch
                    stopwatch.Start();
                    if (ctt != null && WhisperAllowStop == true)
                    {
                       // DoSpeech.speechToTextOffSound();
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text = $"Whisper Debug: ";
                        });
                        try
                        {
                            WhisperString = "";
                            StopWhisper();
                            OutputText.outputLog("[Whisper Switching Languages]");

                        }
                        catch (Exception ex)
                        {
                            OutputText.outputLog("[Error switch input language (manual): " + ex.Message + " ]", System.Drawing.Color.Red);
                        }
                    }
                    else
                    {
                        OutputText.outputLog("[Could not switch input language (slow down)]", System.Drawing.Color.Red);
                    }

                    context.parameters.language = (eLanguage)elang;

                    stopwatch.Stop();
                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    elapsedTime = stopwatch.Elapsed;
                  //  OutputText.outputLog($"Startup Time: {elapsedTime.TotalMilliseconds} ms");
                    DoSpeech.speechToTextOnSound();
                    ctt = new CaptureThread(cla, context, captureDev);
                   
                    ctt?.join();

                }
            }
        }



        public static int doWhisper(string[] args)
		{


            try
           {
               // Stopwatch stopwatch = new Stopwatch();

                // Start the stopwatch
              //  stopwatch.Start();
                // CommandLineArgs cla;
                try
                {
                    cla = new CommandLineArgs(args);

                }
                catch (OperationCanceledException)
                {
                    return 1;
                }

                using iMediaFoundation mf = Library.initMediaFoundation();
                CaptureDeviceId[] devices = mf.listCaptureDevices() ??
                    throw new ApplicationException("This computer has no audio capture devices");


                       
                if (cla.captureDeviceIndex < 0 || cla.captureDeviceIndex >= devices.Length)
                    throw new ApplicationException($"Capture device index is out of range; the valid range is [ 0 .. {devices.Length - 1} ]");
                
                sCaptureParams cp = new sCaptureParams();
            try
            {
                
                cp.minDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text.ToString(), CultureInfo.InvariantCulture); //1
                cp.maxDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text.ToString(), CultureInfo.InvariantCulture); //8
                cp.dropStartSilence = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text.ToString(), CultureInfo.InvariantCulture);   // 250 ms
                cp.pauseDuration = (float)Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text.ToString(), CultureInfo.InvariantCulture);  //1
                //we need culture invariant or for some languages like german 8.0 will be converted to 80 because they use "," instead of "."
            }
            catch (Exception ex)
            {
                    cp.minDuration = 1.0f;
                    cp.maxDuration = 8.0f;
                    cp.dropStartSilence = 0.25f;
                    cp.pauseDuration = 1.0f;
                    if (WhisperError == false)
                    {
                        OutputText.outputLog("[WARNING: Error Occured loading Whisper custom values. Forcing defaults]", System.Drawing.Color.DarkOrange);
                    }
                    WhisperError = true;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        
                  
                    VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text = "1.0";
                    VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text = "8.0";
                    VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text = "0.25";
                    VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text = "1.0";
                    });

                }

                if (cla.diarize)
                    cp.flags |= eCaptureFlags.Stereo;
                captureDev = mf.openCaptureDevice(devices[cla.captureDeviceIndex], cp);

                // string selectedGPU = "default";

                /* VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                 {
                     string selectedGPU = VoiceWizardWindow.MainFormGlobal.comboBoxGPUSelection.SelectedItem.ToString();
                 });

                 if (selectedGPU =="default")
                 {
                     using iModel model = Library.loadModel(cla.model);
                     context = model.createContext();
                     OutputText.outputLog($"[Whisper Loaded with default GPU]");

                 }
                 else
                 {

                     using iModel model = Library.loadModel(cla.model, adapter: selectedGPU);
                     context = model.createContext();
                     OutputText.outputLog($"[Whisper Loaded with GPU: {selectedGPU}]");
                 }*/
                using iModel model = Library.loadModel(cla.model);
                context = model.createContext();


                cla.apply(ref context.parameters);
                WhisperAllowStop = false;
                ctt = new CaptureThread(cla, context, captureDev);
                Thread.Sleep(500);
                WhisperAllowStop = true;


                ctt?.join();


                


                //context.timingsPrint();
                OutputText.outputLog("[Whisper Stopped]");
                //model.Dispose();
               // context.
               //ctt.
                return 0;
              }
            catch (Exception ex)
            {


                //  OutputText.outputLog("[Whisper Error: " + ex.Message.ToString()+ "]", System.Drawing.Color.Red);
                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("[Whisper Error: " + errorMsg + "]", System.Drawing.Color.Red);
                //System.Windows.Forms.MessageBox.Show("FormLoad Error: " + errorMsg);
                OutputText.outputLog("[Whisper Setup Guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Whisper ", System.Drawing.Color.DarkOrange);


               WhisperEnabled = false;

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
                DoSpeech.speechToTextOffSound();

                return ex.HResult;
            }
        }



        public static System.Threading.Timer whisperTimer;

        public static void initiateWhisper()
        {
            whisperTimer = new System.Threading.Timer(whispertimertick);
            whisperTimer.Change(Timeout.Infinite, Timeout.Infinite);

            waveIn = new WaveInEvent();
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.WaveFormat = new WaveFormat(16000, 1);

           // NoiseOperatingModel = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADModel.SelectedIndex;//this needs to be in a try catch incase it's blank for someone
            vad = new WebRtcVad()
            {
                OperatingMode = OperatingMode.VeryAggressive,
                FrameLength = frameLength,
                SampleRate = SampleRate.Is16kHz,
            };
            frameSize = (int)vad.SampleRate / 1000 * 2 * (int)frameLength;

          /* // string[] GPUs = GetGPUs();
          //  VoiceWizardWindow.MainFormGlobal.comboBoxGPUSelection.Items.Add("default");
            foreach (var gpu in GPUs)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxGPUSelection.Items.Add(gpu);
            }
            VoiceWizardWindow.MainFormGlobal.comboBoxGPUSelection.SelectedItem = Settings1.Default.WhisperGPU;*/



        }


        private static TimeSpan startTime = DateTime.MinValue.TimeOfDay;
        private static TimeSpan endTime = DateTime.MinValue.TimeOfDay;
        public static bool isVoiceDetected = false;
        public static List<Tuple<TimeSpan, TimeSpan>> voiceActivationTimes = new List<Tuple<TimeSpan, TimeSpan>>();
        private static int maxVoiceActivationTimes = 25; // Set the maximum number of voice activation times
        private static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = e.Buffer.Take(frameSize).ToArray();

            if (vad.HasSpeech(buffer))
            {
                if (!isVoiceDetected)
                {
                    // Voice has just started
                    startTime = DateTime.Now.TimeOfDay;
                    isVoiceDetected = true;

                    //VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text += "🎙️";
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.Green;
                    });
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("VAD Start Time: " + startTime);
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked == true)
                    {
                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                        OSC.OSCSender.Send(typingbubble);

                    }
                }
                // Update the end time while voice is detected
                endTime = DateTime.Now.TimeOfDay;
            }
            else
            {
                if (isVoiceDetected)
                {
                    // Voice has stopped
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                    });
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                    {
                        OutputText.outputLog("VAD End Time: " + endTime);
                    }

                    isVoiceDetected = false;

                    //  if ((endTime - startTime).TotalSeconds >= 0.5)
                    //  {
                    if (voiceActivationTimes.Count >= maxVoiceActivationTimes)
                    {
                        // Remove the oldest voice activation times
                        int removeCount = voiceActivationTimes.Count - maxVoiceActivationTimes + 1;
                        voiceActivationTimes.RemoveRange(0, removeCount);
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                        {
                            OutputText.outputLog("[Removing old whisper activation timestamps from memory]");
                        }

                    }


                    voiceActivationTimes.Add(new Tuple<TimeSpan, TimeSpan>(startTime, endTime));
                 //   }

                    startTime = DateTime.MinValue.TimeOfDay;
                    endTime = DateTime.MinValue.TimeOfDay;
                }
            }
        }
        public static  void whispertimertick(object sender)
        {

            Thread t = new Thread(doWhisperTimerTick);
            t.IsBackground = true; // Set the thread as a background thread
            t.Start();
        }

        private static void doWhisperTimerTick() //Whisper on timer to prevent double outputs at the same time
        {

            string text = WhisperRecognition.WhisperString;

            TTSMessageQueue.QueueMessage(text, "Whisper");
            WhisperRecognition.WhisperPrevText = WhisperRecognition.WhisperString;
            WhisperRecognition.WhisperString = "";

        }

        public static void downloadWhisperModel()
        {
            string address = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/";
            string path = "Assets/models/";


            switch (VoiceWizardWindow.MainFormGlobal.comboBoxWhisperModelDownload.Text.ToString())
            {
                case "ggml-tiny.bin (75 MB)":
                    path += "ggml-tiny.bin";
                    address += "ggml-tiny.bin";

                    break;

                case "ggml-base.bin (142 MB)":
                    path += "ggml-base.bin";
                    address += "ggml-base.bin";

                    break;

                case "ggml-small.bin (466 MB)":
                    path += "ggml-small.bin";
                    address += "ggml-small.bin";

                    break;

                case "ggml-medium.bin (1.5 GB)":
                    path += "ggml-medium.bin";
                    address += "ggml-medium.bin";

                    break;

                default: break;
            }

            if (!System.IO.File.Exists(path))
            {
                VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = System.Drawing.Color.DarkOrange;
                VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model downloading... PLEASE WAIT";


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
