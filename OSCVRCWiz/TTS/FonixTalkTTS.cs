using System;
using System.Collections.Generic;
using System.Text;
//using SharpTalk;
//using CSCore;
//using CSCore.MediaFoundation;
//using CSCore.SoundOut;
using System.Media;
//using CSCore.CoreAudioAPI;
using Resources;
using OSCVRCWiz.Text;
using NAudio.Wave;
using OSCVRCWiz.Settings;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using Windows.Media.Protection.PlayReady;
//using Amazon.Polly;

namespace OSCVRCWiz.TTS
{
    public class FonixTalkTTS
    {
        private static readonly HttpClient client = new HttpClient();
        // private static bool Moonbase = false;
        public static Process pro;



        public static void FonixTTS(string text)
        {
            Process[] pname = Process.GetProcessesByName("MoonbaseVoices");
            if (pname.Length == 0)
            {
                ProcessStartInfo psi = new ProcessStartInfo("MoonbaseVoices.exe");
                psi.WindowStyle = ProcessWindowStyle.Minimized;
                try
                {
                    pro = Process.Start(psi);
                }
                catch (Exception ex){

                    OutputText.outputLog("[Moonbase Error: " + ex.Message + ". Something prevented the program from running the MoonbaseVoice.exe console app included inside the TTSVoiceWizard download folder]", Color.Red);

                }

                

                
               // Moonbase = true;
               // Task.Delay(2000).Wait();
            }
           
        

            Task<string> stringTask = MoonBase(text);
            string audio = stringTask.Result;
            var audiobytes = Convert.FromBase64String(audio);
            MemoryStream memoryStream = new MemoryStream(audiobytes);

            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            var wav = new RawSourceWaveStream(memoryStream, new WaveFormat(11000, 16, 1)); //11000 and 16 seemed to be the closest to the original
            var output = new WaveOut();
            output.DeviceNumber = AudioDevices.getCurrentOutputDevice();
            output.Init(wav);
            output.Play();
            while (output.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(2000);
            }


        }
        public static async Task<string> MoonBase(string textIn)
        {
            try
            {
                string name = "";
               VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    name = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                   
                });
           


                var url = $"http://localhost:54027/audio?voice={name}&text={textIn}";
              

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                  // request.Content = JsonContent.Create(new { voice = name, text = textIn });
              //  request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "voice", name }, { "text", textIn } });



                HttpResponseMessage response = await client.SendAsync(request);

                //   System.Diagnostics.Debug.WriteLine("Fonix:" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Moonbase: " + response.StatusCode);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Moonbase: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                    return "";
                }




              //  return await response.Content.ReadAsStringAsync();
            }
             catch (Exception ex)
            {
                OutputText.outputLog("[Moonbase Error: "+ex.Message+"]", Color.Red);
              //  MessageBox.Show("FonixTalk Error: "+ex.Message);
                return "";
                
            }



}




    }
}
