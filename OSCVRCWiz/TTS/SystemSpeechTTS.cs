//using CSCore.MediaFoundation;
//using CSCore;
using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Speech;//free windows

using System.Speech.Synthesis;//free windows

//using NAudio.Wave;
//using NAudio.CoreAudioApi;
using Resources;
using OSCVRCWiz.Text;
using NAudio.Wave;
using System.IO;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;
using Windows.Storage.Streams;
using Newtonsoft.Json.Serialization;
using System.Windows;
using System.Windows.Forms.VisualStyles;

namespace TTS
{
    public class SystemSpeechTTS
    {

       public static List<string> systemSpeechVoiceList = new List<string>();
        public static string currentLiteVoice = "";
        

        public static void getVoices()
        {
            System.Speech.Synthesis.SpeechSynthesizer synthesizerVoices = new System.Speech.Synthesis.SpeechSynthesizer();

          // synthesizerLite.c += new EventHandler<SpeakCompletedEventArgs>(reader_SpeakCompleted);
            foreach (var voice in synthesizerVoices.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                System.Diagnostics.Debug.WriteLine($"Id: {info.Id} | Name: {info.Name} | Age: {info.Age} | Gender: {info.Gender} | Culture: {info.Culture}");
                systemSpeechVoiceList.Add(info.Name + "|" + info.Culture);
                // comboBoxLite.Items.Add(info.Name + "|" + info.Culture);
            }

        }
       
        public static async void systemTTSAction(string text)
        {
        //  var semitone = Math.Pow(2, 1.0/24);
       //   var upOneTone = semitone;
        // var downOneTone = 1.0 / upOneTone;

            try
            {
                System.Speech.Synthesis.SpeechSynthesizer synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
                synthesizerLite.SelectVoice(currentLiteVoice);

                MemoryStream memoryStream = new MemoryStream();
                synthesizerLite.SetOutputToWaveStream(memoryStream);
               synthesizerLite.Speak(text);

                memoryStream.Flush();
               memoryStream.Seek(0, SeekOrigin.Begin);
                 WaveFileReader wav = new WaveFileReader(memoryStream);


                var volume = "";
                var pitch = "";
                var volumeFloat = 1f;
                var pitchFloat = 1f;
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    volume = VoiceWizardWindow.MainFormGlobal.comboBoxVolume.Text.ToString();
                    pitch = VoiceWizardWindow.MainFormGlobal.comboBoxPitch.Text.ToString();
                });
                switch (volume)
                {
                    case "x-soft": volumeFloat = .5f; break;
                    case "soft": volumeFloat = .75f; break;
                    case "default": volumeFloat = 1f; break;
                    case "loud": volumeFloat = 1.25f; break;
                    case "x-loud": volumeFloat = 1.50f; break;
                    default:
                        break;
                }
                switch (pitch)
                {
                    case "x-low": pitchFloat = .5f; break;
                    case "low": pitchFloat = .75f; break;
                    case "slightly lower": pitchFloat = .9f; break;
                    case "default": pitchFloat = 1f; break;
                    case "slightly higher": pitchFloat = 1.10f; break;
                    case "high": pitchFloat = 1.25f; break;
                    case "x-high": pitchFloat = 1.50f; break;
                    default:
                        break;
                }

                var wave32 = new WaveChannel32(wav, volumeFloat,0f);  //1f volume is normal, keep pan at 0 for audio through both ears
                VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(false, false));
                speedControl.PlaybackRate = pitchFloat; 


                VoiceWizardWindow.AnyOutput = new WaveOut();
                VoiceWizardWindow.AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                VoiceWizardWindow.AnyOutput.Init(speedControl);
                VoiceWizardWindow.AnyOutput.Play();
                while (VoiceWizardWindow.AnyOutput.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(2000);
                }

            }
           catch (Exception ex)
            {
                OutputText.outputLog("[System Speech TTS *AUDIO* Error: " + ex.Message+"]",Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
            }




        }
       
    }
}
