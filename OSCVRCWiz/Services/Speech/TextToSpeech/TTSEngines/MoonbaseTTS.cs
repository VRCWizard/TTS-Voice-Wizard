using System.Diagnostics;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class MoonbaseTTS
    {
        private static readonly HttpClient client = new HttpClient();
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
                catch (Exception ex)
                {

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


            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);

            // AudioDevices.playMoonbaseStream(memoryStream, TTSMessageQueued, ct);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Raw);
            memoryStream.Dispose();




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
                    Debug.WriteLine("Moonbase: " + response.StatusCode);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Debug.WriteLine("Moonbase: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                    return "";
                }




                //  return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Moonbase Error: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Moonbase-TTS ]", Color.DarkOrange);
                //  MessageBox.Show("FonixTalk Error: "+ex.Message);
                TTSMessageQueue.PlayNextInQueue();
                return "";

            }



        }

        public static void CloseMoonbaseTerminal()
        {
            try
            {
                if (pro != null)
                {
                    pro.Kill();
                }
            }
            catch (Exception ex) { }
        }

        public static void SetVoices(ComboBox voices, ComboBox styles)
        {
            voices.Items.Clear();
            voices.Items.Add("Betty");
            voices.Items.Add("Dennis");
            voices.Items.Add("Frank");
            voices.Items.Add("Harry");
            voices.Items.Add("Kit");
            voices.Items.Add("Paul");
            voices.Items.Add("Rita");
            voices.Items.Add("Ursula");
            voices.Items.Add("Wendy");
            voices.SelectedIndex = 0;

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;

            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;

        }






    }
}
