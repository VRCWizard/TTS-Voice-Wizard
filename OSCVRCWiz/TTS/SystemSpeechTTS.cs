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
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using OSCVRCWiz.Resources;

namespace TTS
{
    public class SystemSpeechTTS
    {

       public static List<string> systemSpeechVoiceList = new List<string>();
        //public static string currentLiteVoice = "";
        

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
       
        public static async void systemTTSAction(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            //  var semitone = Math.Pow(2, 1.0/24);
            //   var upOneTone = semitone;
            // var downOneTone = 1.0 / upOneTone;



           
            



            try
           {
                string phrase = TTSMessageQueued.Voice;
                string[] words = phrase.Split('|');
                int counter = 1;
                var voice = "none";

                foreach (var word in words)
                {
                    if (counter == 1)
                    {
                        //synthesizerLite.SelectVoice(word);
                        voice = word;
                        // System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");

                    }
                    if (counter == 2)
                    {
                        //CultureSelected = word;
                        //  System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");
                    }
                    counter++;
                }


                System.Speech.Synthesis.SpeechSynthesizer synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
                synthesizerLite.SelectVoice(voice);

                MemoryStream memoryStream = new MemoryStream();
                synthesizerLite.SetOutputToWaveStream(memoryStream);
                synthesizerLite.Speak(TTSMessageQueued.text);

                AudioDevices.playWaveStream(memoryStream, TTSMessageQueued, ct);
                memoryStream.Dispose();

                synthesizerLite.Dispose();
                synthesizerLite = null;

   


            }
            catch (Exception ex)
            {
              OutputText.outputLog("[System Speech TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
               {
                  OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }




        }
       
       
    }
}
