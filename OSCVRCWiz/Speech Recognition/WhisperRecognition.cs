using NAudio.Wave;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using Resources;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Whisper;
using Whisper.Internal;


namespace OSCVRCWiz.Speech_Recognition
{
	public class WhisperRecognition
	{
       static bool WhisperEnabled = false;
        public static string WhisperString = "";
       
        public static void toggleWhisper()
        {
            if (WhisperEnabled == false)
            {
                
                WhisperEnabled = true;

                string UseThisMic = getWhisperInputDevice().ToString();

                string[] args = { 
                "-c",UseThisMic,
                "-m",  VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text,
              //  "-ml", "300"
                };
                Task.Run(() => doWhisper(args));
                
                OutputText.outputLog("[Whisper Listening]");
            }

            else
            {
                WhisperString = "";
                CaptureThread.stopWhisper();
                WhisperEnabled = false;
                OutputText.outputLog("[Whisper Stopped Listening]");

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




        public static int doWhisper(string[] args)
		{


            try
            {
                CommandLineArgs cla;
                try
                {
                    cla = new CommandLineArgs(args);
                //cla.max_len = 300;
                    
                  //  cla.captureDeviceIndex = 0;
                  //  cla.model = @"C:\Users\\bdw10\Downloads\base.en.pt";
                    

                    //i can set all the cla argument here very easily
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
                if (cla.diarize)
                    cp.flags |= eCaptureFlags.Stereo;
                using iAudioCapture captureDev = mf.openCaptureDevice(devices[cla.captureDeviceIndex], cp);
                
                using iModel model = Library.loadModel(cla.model);
                using Whisper.Context context = model.createContext();


                //attempted fix will break program
              //  context.parameters.audioContextSize = 150;
           //   context.parameters.n_max_text_ctx = 300;
           //    context.parameters.max_tokens = 300;

                


                cla.apply(ref context.parameters);
                

                CaptureThread thread = new CaptureThread(cla, context, captureDev);
                thread.join();

                


                context.timingsPrint();
                Debug.WriteLine("finished");
                return 0;
            }
            catch (Exception ex)
            {
                // Console.WriteLine( ex.Message );
                // Debug.WriteLine(ex.ToString());
                OutputText.outputLog("Whisper Error: " + ex.Message.ToString(), Color.Red);
                WhisperEnabled = false;
                // return;
                return ex.HResult;
            }
        }
			
			
				
			
    }
}
