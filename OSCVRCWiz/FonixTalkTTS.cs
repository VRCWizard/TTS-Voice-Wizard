using System;
using System.Collections.Generic;
using System.Text;
using SharpTalk;
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using System.Media;


namespace OSCVRCWiz
{
    public class FonixTalkTTS
    {

        public void FonixTTS(string text)
        {
            var tts = new FonixTalkEngine();
            string name = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
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

            //   tts.Speak(phrase); //ONLY WORKS IF PROJECT > PROPERTIES > BUILD > PLATFORM TARGET  is set to x86 due to the FonixTalk.dll being 32 bit only
            tts.SpeakToWavFile("MEMEspeech.wav", text);
            tts.Dispose();
            Task.Run(() => PlayAudioHelper());

           // var ot = new OutputText();
        



        }
        public void PlayAudioHelper()
        {
            var stream = new MemoryStream(File.ReadAllBytes("MEMEspeech.wav"));
            var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) };
            var waveSource = new MediaFoundationDecoder(stream);
            waveOut.Initialize(waveSource);
            waveOut.Play();
            waveOut.WaitForStopped();

        }




        }
}
