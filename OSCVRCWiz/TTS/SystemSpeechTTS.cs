using CSCore.MediaFoundation;
using CSCore;
using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Speech;//free windows

using System.Speech.Synthesis;//free windows
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using CSCore.SoundIn;
using CSCore.CoreAudioAPI;
using NAudio.Wave;
using Resources;

namespace TTS
{
    public class SystemSpeechTTS
    {
        private static System.Speech.Synthesis.SpeechSynthesizer synthesizerLite;//free
        private static MemoryStream stream;
       public static List<string> systemSpeechVoiceList = new List<string>();
        public static string currentLiteVoice = "";

        public static void getVoices()
        {
            synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
            foreach (var voice in synthesizerLite.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                System.Diagnostics.Debug.WriteLine($"Id: {info.Id} | Name: {info.Name} | Age: {info.Age} | Gender: {info.Gender} | Culture: {info.Culture}");
                systemSpeechVoiceList.Add(info.Name + "|" + info.Culture);
                // comboBoxLite.Items.Add(info.Name + "|" + info.Culture);
            }
        }

        public static void systemTTSAction(string text)
        {
            // stream = new MemoryStream();
            // synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
            // synthesizerLite.SetOutputToWaveStream(stream);
            synthesizerLite.SelectVoice(currentLiteVoice);
           
            stream = new MemoryStream();
            synthesizerLite.SetOutputToWaveStream(stream);
            synthesizerLite.Speak(text);
            //      var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
            var waveSource = new MediaFoundationDecoder(stream);
            //  waveOut.Initialize(waveSource);
            //  waveOut.Play();
            //  waveOut.WaitForStopped();

            var testOut = new CSCore.SoundOut.WasapiOut();
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                if (endpoint.DeviceID == AudioDevices.currentOutputDevice)
                {
                    testOut.Device = endpoint;
                }
            }
            testOut.Initialize(waveSource);
            testOut.Play();
            testOut.WaitForStopped();




        }
    }
}
