using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;//free Windows
using NAudio.Wave;
//using CSCore.SoundIn;
using CSCore.XAudio2;
using Resources;
using System.Collections;
using System.Linq;
using OSCVRCWiz.Addons;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using OSCVRCWiz.Text;


//using NAudio.Wave;

namespace OSCVRCWiz
{
    public class SystemSpeechRecognition
    {
        static bool listeningCurrently = false;
        static WaveInEvent waveIn;
        static SpeechStreamer audioStream = new(12800);
        static SpeechRecognitionEngine rec;
        static Dictionary<string, int> AlternateInputDevices = new Dictionary<string, int>();



        public static void startListeningNow()
        {
                string cultureHere = "en-US";// system speech only for en-us, it's not worth using over alternatives
            //  cultureHere = MainForm.CultureSelected;


            rec = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(cultureHere));
                 
            // Create and load a dictation grammar.  
            rec.LoadGrammar(new DictationGrammar());
             // Add a handler for the speech recognized event.  
            rec.SpeechRecognized +=new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            // Setting to Correct Input Device
            int waveInDevices = WaveIn.DeviceCount;
            AlternateInputDevices.Clear();
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                AlternateInputDevices.Add(deviceInfo.ProductName, waveInDevice);
            }
            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            foreach (var kvp in AlternateInputDevices)
            {
                if (AudioDevices.currentInputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    waveIn.DeviceNumber = kvp.Value;
                    System.Diagnostics.Debug.WriteLine("Input device worked"+kvp.Key);
                } 
            }

            // Start Listening
            waveIn.WaveFormat = new WaveFormat(48000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;
            waveIn?.StartRecording();
            rec.SetInputToAudioStream(audioStream, new(48000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono));
            rec.RecognizeAsync(RecognizeMode.Multiple);

        }
        public static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)//lite version, WindowsBuiltInSTTTS Help
        {
            System.Diagnostics.Debug.WriteLine("Recognized text: " + e.Result.Text);
            string text = e.Result.Text.ToString();
            Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "System Speech"));
        }

        private static void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            audioStream.Write(e.Buffer, 0, e.BytesRecorded);

        }

        public static void speechTTSButtonLiteClick()
        {

                if (listeningCurrently == false)
                {
                OutputText.outputLog("[System Speech Started Listening]");
                listeningCurrently = true;
                Task.Run(() => startListeningNow());

                }
                else
                {
                OutputText.outputLog("[System Speech Stopped Listening]");
                listeningCurrently = false;
                waveIn.StopRecording();
                rec.RecognizeAsyncStop();             
                }




        }
    }
}
