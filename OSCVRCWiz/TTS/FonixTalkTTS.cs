using System;
using System.Collections.Generic;
using System.Text;
using System.Media;
using Resources;
using OSCVRCWiz.Text;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;
using OSCVRCWiz.Resources;
using Microsoft.VisualBasic.Devices;


namespace OSCVRCWiz.TTS
{
    public class FonixTalkTTS
    {
        private static readonly HttpClient client = new HttpClient();
        // private static bool Moonbase = false;
        public static Process pro;



        public static void FonixTTS(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
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

                    OutputText.outputLog("[Moonbase TTS Startup Error: " + ex.Message + "]", Color.Red);

                    OutputText.outputLog("[Something prevented the program from running the MoonbaseVoice.exe console app included inside the TTSVoiceWizard download folder. Make sure that 'MoonbaseVoices.exe' exists in the download folder and has not been renamed. Try running TTS Voice Wizard as Administrator]", Color.DarkOrange);

                    TTSMessageQueue.PlayNextInQueue();

                }

               




                // Moonbase = true;
                // Task.Delay(2000).Wait();
            }
            Task<string> stringTask = MoonBase(TTSMessageQueued);
            string audio = stringTask.Result;
            MoonBasePlayAudio(audio, TTSMessageQueued, ct);


        }
        public static async void MoonBasePlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            try
            {

               
                var audiobytes = Convert.FromBase64String(audioString);
                MemoryStream memoryStream = new MemoryStream(audiobytes);

                AudioDevices.playMoonbaseStream(memoryStream, TTSMessageQueued, ct);
                memoryStream.Dispose();



            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Moonbase TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);

                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }
        }

            public static async Task<string> MoonBase(TTSMessageQueue.TTSMessage TTSMessageQueued)
        {
            try
            {
 
           


                var url = $"http://localhost:54027/audio?voice={TTSMessageQueued.Voice}&text={TTSMessageQueued.text}";
              

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
                OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Moonbase-TTS ]", Color.DarkOrange);
                //  MessageBox.Show("FonixTalk Error: "+ex.Message);
                TTSMessageQueue.PlayNextInQueue();
                return "";
                
            }



}




    }
}
