using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
//using CSCore;
//using CSCore.MediaFoundation;
using System.Media;
using System.Net;
//using CSCore.CoreAudioAPI;
using OSCVRCWiz.Text;
using Resources;
using NAudio.Wave;
using Microsoft.VisualBasic;
using Windows.Storage.Streams;
using System.Collections;
using Microsoft.Extensions.Primitives;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;

namespace OSCVRCWiz.TTS
{
    public class TikTokTTS
    {
       // public static WaveOut TikTokOutput=null;

        public static async Task TikTokTextAsSpeech(string text)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
            string voice = "";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
            });
            System.Diagnostics.Debug.WriteLine("tiktok speech ran " + voice);
            byte[] result = null;
            try
            {
                result = await CallTikTokAPIAsync(text, voice);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TikTok TTS Error: " + ex.Message + "]", Color.Red);
             

            }


            try
            {
                //  File.WriteAllBytes("TikTokTTS.mp3", result);          
                //  Task.Run(() => PlayAudioHelper());

                MemoryStream memoryStream = new MemoryStream(result);
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                Mp3FileReader wav = new Mp3FileReader(memoryStream); //it does not have a wav file header so it is mp3 formate unless systemspeech, and fonixtalk


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

                var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
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
                OutputText.outputLog("[TikTok TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }

            }
            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {


            var url = "https://tiktok-tts.weilnet.workers.dev/api/generation";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            var data = "{\"text\":\"" + text + "\",\"voice\":\"" + voice + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string audioInBase64 = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                audioInBase64 = dataHere.ToString();

                System.Diagnostics.Debug.WriteLine(result);
            }

            System.Diagnostics.Debug.WriteLine(httpResponse.StatusCode);



            System.Diagnostics.Debug.WriteLine(audioInBase64);
            return Convert.FromBase64String(audioInBase64);

        }



        
    }



}
