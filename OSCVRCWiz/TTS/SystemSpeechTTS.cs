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
       
        public static async void systemTTSAction(string text, CancellationToken ct = default)
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


                MemoryStream memoryStream2 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream2);

          
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                WaveFileReader wav = new WaveFileReader(memoryStream);


                memoryStream2.Flush();
                memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
                WaveFileReader wav2 = new WaveFileReader(memoryStream2);



            var volume = 5;
                int pitch = 5;
                int rate = 5;
                var volumeFloat = 1f;
                var pitchFloat = 1f;
                var rateFloat = 1f;
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                    pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                    rate = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                });

                volumeFloat = 0.5f + volume * 0.1f;
                pitchFloat = 0.5f + pitch * 0.1f;
                rateFloat = 0.5f + rate * 0.1f;

                bool useTempo = false;
                if(rate !=5)//if rate is changed will use only rate, else use pitch which also changes rate.
                {
                    useTempo= true;
                    pitchFloat = rateFloat;
                }

                var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
                wave32.PadWithZeroes = false;
                VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 2000, new SoundTouchProfile(useTempo, false));
                speedControl.PlaybackRate = pitchFloat;
           

            var AnyOutput = new WaveOut();
            AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();


            AnyOutput.Init(speedControl);
            AnyOutput.Play();
            ct.Register(async () => AnyOutput.Stop());


            WaveOut AnyOutput2 = null;
            VarispeedSampleProvider speedControl_2 = null;
            WaveChannel32 wave32_2 = null;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {
                wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                wave32_2.PadWithZeroes = false;
                speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                speedControl_2.PlaybackRate = pitchFloat;//output 2
                AnyOutput2 = new WaveOut();
                AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                AnyOutput2.Init(speedControl_2);
                AnyOutput2.Play();
                ct.Register(async () => AnyOutput2.Stop());
            }
           
           

           
            Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds*2);// VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
             Thread.Sleep(500);


              
                AnyOutput.Stop();
                AnyOutput.Dispose();        
                AnyOutput = null;          
                speedControl.Dispose();           
                speedControl = null;
                wave32.Dispose();            
                wave32= null;              
                wav.Dispose();           
                wav = null;           
                memoryStream.Dispose();
                synthesizerLite.Dispose();
                memoryStream = null;
                synthesizerLite= null;

            if (AnyOutput2 != null) {
                AnyOutput2.Stop();
                AnyOutput2.Dispose();
                AnyOutput2 = null;
                speedControl_2.Dispose();
                speedControl_2 = null;
                wave32_2.Dispose();
                wave32_2 = null;
                wav2.Dispose();
                wav2 = null;
            }
            ct = new();
            
            
         



            Debug.WriteLine("disposed of all");
                    


                    //    }
                    //    }      
                    //  while (VoiceWizardWindow.AnyOutput.PlaybackState == PlaybackState.Playing )
                    //   {
                    //    Debug.WriteLine("testing if it's still broken");
                    // Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds);
                    //  }

                
           }
            catch (Exception ex)
            {
              OutputText.outputLog("[System Speech TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
               {
                  OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
            }




        }
       
       
    }
}
