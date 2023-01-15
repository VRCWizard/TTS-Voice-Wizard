using System;
using System.Collections.Generic;
using System.Text;
using SharpTalk;
//using CSCore;
//using CSCore.MediaFoundation;
//using CSCore.SoundOut;
using System.Media;
//using CSCore.CoreAudioAPI;
using Resources;
using OSCVRCWiz.Text;
using NAudio.Wave;

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
                MemoryStream memoryStream = new MemoryStream();
                tts.SpeakToStream(memoryStream, text);
                tts.Dispose();

                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                var wav = new RawSourceWaveStream(memoryStream, new WaveFormat(11000, 16, 1)); //11000 and 16 seemed to be the closest to the original
                var output = new WaveOut();
                output.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                output.Init(wav);
                output.Play();
                //Task.Run(() => PlayAudioHelper());
            }
             catch (Exception ex)
            {
                OutputText.outputLog("[Reminder that FonixTalk only works on the x86 build of TTS Voice Wizard]", Color.Red);
                MessageBox.Show("FonixTalk Error: "+ex.Message);
                
            }



}




    }
}
