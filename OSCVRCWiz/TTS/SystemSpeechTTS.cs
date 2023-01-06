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

namespace TTS
{
    public class SystemSpeechTTS
    {

        public static void systemTTSAction(string text)
        {
            VoiceWizardWindow.MainFormGlobal.stream = new MemoryStream();
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.SetOutputToWaveStream(VoiceWizardWindow.MainFormGlobal.stream);
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.Speak(text);
            //      var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
            var waveSource = new MediaFoundationDecoder(VoiceWizardWindow.MainFormGlobal.stream);
            //  waveOut.Initialize(waveSource);
            //  waveOut.Play();
            //  waveOut.WaitForStopped();

            var testOut = new CSCore.SoundOut.WasapiOut();
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                if (endpoint.DeviceID == VoiceWizardWindow.MainFormGlobal.currentOutputDevice)
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
