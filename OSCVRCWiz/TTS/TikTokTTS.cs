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

namespace OSCVRCWiz.TTS
{
    public class TikTokTTS
    {
       

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
            try
            {
                byte[] result = await CallTikTokAPIAsync(text, voice);
                //  File.WriteAllBytes("TikTokTTS.mp3", result);          
                //  Task.Run(() => PlayAudioHelper());

                MemoryStream memoryStream = new MemoryStream(result);
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                Mp3FileReader wav = new Mp3FileReader(memoryStream); //it does not have a wav file header so it is mp3 formate unless systemspeech, and fonixtalk
                var output = new WaveOut();
                output.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                output.Init(wav);
                output.Play();


            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TikTok TTS Error: " + ex.Message + "]", Color.Red);

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
