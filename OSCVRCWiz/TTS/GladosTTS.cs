using NAudio.Wave;
using Newtonsoft.Json.Linq;
using OSCVRCWiz.Text;
using OSCVRCWiz;
using Resources;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static Siticone.Desktop.UI.Native.WinApi;
using EmbedIO;

namespace TTS
{
    public class GladosTTS
    {
        public static async Task GladosTextAsSpeech(string text)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
            string voice = "";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
            });
            System.Diagnostics.Debug.WriteLine("glados speech ran " + voice);
            try
           {
                byte[] result = await CallGladosAPIAsync(text);
                //  File.WriteAllBytes("TikTokTTS.mp3", result);          
                //  Task.Run(() => PlayAudioHelper());

                MemoryStream memoryStream = new MemoryStream(result);
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);
                WaveFileReader wav = new WaveFileReader(memoryStream); //it does not have a wav file header so it is mp3 formate unless systemspeech, and fonixtalk
                var output = new WaveOut();
                output.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                output.Init(wav);
                output.Play();


            }
            catch (Exception ex)
           {
                if (ex.Message.ToString() == "No connection could be made because the target machine actively refused it. [::ffff:127.0.0.1]:8124 (127.0.0.1:8124)")
                {
                    OutputText.outputLog("[You did not setup Glados TTS follow the instructions on the wiki here: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS]", Color.Red);
                }
                else
                {
                    OutputText.outputLog("[Glados TTS Error: " + ex.Message + "]", Color.Red);
                }
           }
            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallGladosAPIAsync(string text)
        {


            var url = "http://127.0.0.1:8124/synthesize/";

            string audioInBase64 = "";
            WebRequest request = WebRequest.Create("http://127.0.0.1:8124/synthesize/" + "?"+text);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {

                using (Stream stream = response.GetResponseStream())
                {
                    
                    using (var streamReader = new StreamReader(stream))
                    {
                        var result = streamReader.ReadToEnd();
                       // var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                       // audioInBase64 = dataHere.ToString();

                        System.Diagnostics.Debug.WriteLine(result.ToString());
                        audioInBase64 = result.ToString();

                    }

                }
            }
           // System.Diagnostics.Debug.WriteLine(audioInBase64);
            return Convert.FromBase64String(audioInBase64);

        }




    
}
}
