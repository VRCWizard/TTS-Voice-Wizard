using System;
using System.Collections.Generic;
using System.Text;
using SharpTalk;
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using System.Media;
using CSCore.CoreAudioAPI;
using Resources;


namespace OSCVRCWiz.TTS
{
    public class FonixTalkTTS
    {

        public static void FonixTTS(string text)
        {
            try
            {

                var tts = new FonixTalkEngine();
                string name = "";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    name = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                });
                switch (name)
                {
                    case "Betty": tts.Voice = TtsVoice.Betty; break;
                    case "Dennis": tts.Voice = TtsVoice.Dennis; break;
                    case "Frank": tts.Voice = TtsVoice.Frank; break;
                    case "Harry": tts.Voice = TtsVoice.Harry; break;
                    case "Kit": tts.Voice = TtsVoice.Kit; break;
                    case "Paul": tts.Voice = TtsVoice.Paul; break;
                    case "Rita": tts.Voice = TtsVoice.Rita; break;
                    case "Ursula": tts.Voice = TtsVoice.Ursula; break;
                    case "Wendy": tts.Voice = TtsVoice.Wendy; break;
                    default: break;
                }

                //////////////////// //   tts.Speak(phrase); //ONLY WORKS IF PROJECT > PROPERTIES > BUILD > PLATFORM TARGET  is set to x86 due to the FonixTalk.dll being 32 bit only
                tts.SpeakToWavFile("MEMEspeech.wav", text);
                tts.Dispose();
                Task.Run(() => PlayAudioHelper());
            }
             catch (Exception ex)
            {
                MessageBox.Show("FonixTalk Error: Note that FonixTalk only works on the x86 build. "+ex.Message);
            }






        }
        public static void PlayAudioHelper()
        {
            var stream = new MemoryStream(File.ReadAllBytes("MEMEspeech.wav"));
            //    var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) };
            var waveSource = new MediaFoundationDecoder(stream);
            //  waveOut.Initialize(waveSource);
            //  waveOut.Play();
            //  waveOut.WaitForStopped();
            var testOut = new WasapiOut();
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
