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
                 var output = new WaveOut();
                output.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                output.Init(wav);
                 output.Play();

            }
           catch (Exception ex)
            {
                OutputText.outputLog("[System Speech TTS *AUDIO* Error: " + ex.Message + "]",Color.Red);
            }




        }
       
    }
}
