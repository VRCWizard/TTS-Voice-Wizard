using CoreOSC;
using NAudio.Wave;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using Resources;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vosk;
using Whisper;
//using Whisper;
//using Whisper.Internal;
using static System.Net.WebRequestMethods;


namespace OSCVRCWiz.Speech_Recognition
{
	public class WhisperRecognition
	{
       static bool WhisperEnabled = false;
        public static string WhisperString = "";
        public static string WhisperPrevText = "";
        private static string langcode = "en";
        private static bool WhisperError = false;
       
        public static void toggleWhisper()
        {
            if (WhisperEnabled == false)
            {



                
                WhisperEnabled = true;

                string UseThisMic = getWhisperInputDevice().ToString();

              //  var language = "";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    fromLanguageID(VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedItem.ToString());//set lang code for recognition
                  //  language = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();

                });

               
                
             //   if(language!= "English[en]")
            //   {

                    string[] args = {
                "-c",UseThisMic,
                "-m",  VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text,
                "-l", langcode,
              //  "-tr", VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperToEnglish.Checked.ToString()
              //  "-ml", "300"
                 };
                    Task.Run(() => doWhisper(args));

                    OutputText.outputLog("[Whisper Listening]");
                // }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }


            }

            else
            {
                try
                {
                    WhisperString = "";
                    CaptureThread.stopWhisper();
                    WhisperEnabled = false;
                    OutputText.outputLog("[Whisper Stopped Listening]");
                    WhisperError = false;

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }
                }
                catch(Exception ex) {
                    OutputText.outputLog("[Error Stopping Whisper]");
                }

            }
        }
        public static void autoStopWhisper()
        {
            try
            {
                if (WhisperEnabled == true)
                {
                    WhisperString = "";
                    CaptureThread.stopWhisper();
                    WhisperEnabled = false;
                    OutputText.outputLog("[Whisper Stopped Listening]");
                    WhisperError = false;
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }

                }
           
              }
                catch(Exception ex) {
                OutputText.outputLog("[Error Stopping Whisper]");
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

        public static void fromLanguageID(string fullname)
        {
            langcode = "en-US";
            switch (fullname)
            {
                case "Arabic [ar-EG]": langcode = "ar"; break;
                case "Chinese [zh-CN]": langcode = "zh"; break;
                case "Czech [cs-CZ]": langcode = "cs"; break;
                case "Danish [da-DK]": langcode = "da"; break;
                case "Dutch [nl-NL]": langcode = "nl"; break;
                case "English [en-US] (Default)": langcode = "en"; break;
                case "Estonian [et-EE]": langcode = "et"; break;
                case "Filipino [fil-PH]": langcode = "tl"; break;
                case "Finnish [fi-FI]": langcode = "fi"; break;
                case "French [fr-FR]": langcode = "fr"; break;
                case "German [de-DE]": langcode = "de"; break;
                case "Hindi [hi-IN]": langcode = "hi"; break;
                case "Hungarian [hu-HU]": langcode = "hu"; break;
                case "Indonesian [id-ID]": langcode = "id"; break;
                case "Irish [ga-IE]": langcode = "ga"; break;
                case "Italian [it-IT]": langcode = "it"; break;
                case "Japanese [ja-JP]": langcode = "ja"; break;
                case "Korean [ko-KR]": langcode = "ko"; break;
                case "Norwegian [nb-NO]": langcode = "nb"; break;
                case "Polish [pl-PL]": langcode = "pl"; break;
                case "Portuguese [pt-BR]": langcode = "pt"; break;
                //place holder^^
                case "Russian [ru-RU]": langcode = "ru"; break;
                case "Spanish [es-MX]": langcode = "es"; break;
                //place holder^^
                case "Swedish [sv-SE]": langcode = "sv"; break;
                case "Thai [th-TH]": langcode = "th"; break;
                case "Ukrainian [uk-UA]": langcode = "uk"; break;
                case "Vietnamese [vi-VN]": langcode = "vi"; break;
                default: langcode = "en"; break; // if translation to english happens something is wrong
            }
        }




        public static int doWhisper(string[] args)
		{


            try
           {
                CommandLineArgs cla;
                try
                {
                    cla = new CommandLineArgs(args);

                }
                catch (OperationCanceledException)
                {
                    return 1;
                }
                const eLoggerFlags loggerFlags = eLoggerFlags.UseStandardError | eLoggerFlags.SkipFormatMessage;
               Library.setLogSink(eLogLevel.Debug, loggerFlags);

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
                        OutputText.outputLog("[WARNING: Error Occured loading Whisper custom values. Forcing defaults]", Color.DarkOrange);
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
                using iAudioCapture captureDev = mf.openCaptureDevice(devices[cla.captureDeviceIndex], cp);
                
                using iModel model = Library.loadModel(cla.model);
                using Whisper.Context context = model.createContext();

              //  context.parameters.duration_ms = 1000;

                //attempted fix will break program
              //  context.parameters.audioContextSize = 150;
           //   context.parameters.n_max_text_ctx = 300;
           //    context.parameters.max_tokens = 300;
          // context.parameters.hold

                


                cla.apply(ref context.parameters);
                

                CaptureThread thread = new CaptureThread(cla, context, captureDev);
                thread.join();

                


                context.timingsPrint();
                Debug.WriteLine("finished");
                return 0;
              }
            catch (Exception ex)
            { 
                
               
                OutputText.outputLog("[Whisper Error: " + ex.Message.ToString()+ "]", Color.Red);
                OutputText.outputLog("[Whisper Setup Guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Whisper ", Color.DarkOrange);


               WhisperEnabled = false;

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }

                return ex.HResult;
            }
        }
			
			
				
			
    }
}
