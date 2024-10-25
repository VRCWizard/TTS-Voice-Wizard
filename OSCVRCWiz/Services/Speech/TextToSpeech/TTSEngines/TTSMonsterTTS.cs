using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Net.Http.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web.Http;
using Octokit.Internal;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class TTSMonsterTTS
    {

        private class InputBody
        {
            public string voice_id { get; set; }
            public string message { get; set; }
            public string return_usage { get; set; }

        }
        private static readonly HttpClient client = new HttpClient();
        public static async Task TTSMonstertts(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            try
           {
                InputBody input = new InputBody();
                input.voice_id = GetTTSMonsterVoice(TTSMessageQueued.Voice);
                input.message = TTSMessageQueued.text;
                input.return_usage = "false";


                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api.console.tts.monster/generate"),
                    Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"<key>" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" }
                },
                    Content = JsonContent.Create(input)
                };

                var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(false);

                string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
               // OutputText.outputLog(responseContent);
                JObject responseObject = JObject.Parse(responseContent);
               // string status = responseObject["status"].ToString();
               // OutputText.outputLog(status);
                string audioURL = responseObject["url"].ToString();
               
               // OutputText.outputLog(audioURL);
                byte[] audioData = await client.GetByteArrayAsync(audioURL);

                MemoryStream memoryStream = new MemoryStream(audioData);
                   AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);

                  memoryStream.Dispose();
            }
            catch (Exception ex)
            {

                OutputText.outputLog("[TTSMonster TTS Error: " + ex.Message + ex.StackTrace+"]", Color.Red);
                if (ex.InnerException != null)
                {
                    OutputText.outputLog("[TTSMonster TTS Inner Exception: " + ex.InnerException.Message + "]", Color.Red);
                }
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());
            }

        }


        public static string GetTTSMonsterVoice(string voice)
        {
            string apiName = "en_us_001";

            apiName = TTSMonsterRememberVoices[voice];

            return apiName;
        }

        public class TTSMonsterVoice
        {
            public string name { get; set; }
            public string voice_id { get; set; }

        }

        public static Dictionary<string, string> TTSMonsterRememberVoices = new Dictionary<string, string>();
        public static bool TTSMonsterfirstVoiceLoad = true;

        public static async Task SynthesisGetAvailableVoicesAsync(ComboBox comboboxVoices)
        {

            if (TTSMonsterfirstVoiceLoad)
            {


                // replace with the path to the JSON file
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = "Assets/voices/ttsMonsterVoices.json";

                string jsonFilePath = Path.Combine(basePath, relativePath);

                // read the JSON data from the file
                string jsonData = "";
                try
                {
                    jsonData = File.ReadAllText(jsonFilePath);
                }
                catch (Exception ex)
                {
                    OutputText.outputLog("[Could not find directory, try running TTSVoiceWizard as admin or moving the entire folder to a new location. (if it's on the desktop move it to documents or where your games are stored for example)]", Color.Red);
                }



                // deserialize the JSON data into an array of Voice objects
                TTSMonsterVoice[] voices = System.Text.Json.JsonSerializer.Deserialize<TTSMonsterVoice[]>(jsonData);



                foreach (var voice in voices)
                {
                    comboboxVoices.Items.Add(voice.name);
                    TTSMonsterRememberVoices.Add(voice.name, voice.voice_id);




                }



                TTSMonsterfirstVoiceLoad = false;

            }
            else
            {
                //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: Voices successfully reloaded locally]");
                foreach (string voice in TTSMonsterRememberVoices.Keys)
                {
                    comboboxVoices.Items.Add(voice);
                }
            }

            //  VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;





        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;


            voices.Items.Clear();



            SynthesisGetAvailableVoicesAsync(voices);

            voices.SelectedIndex = 0;

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;

            styles.Enabled = false;
            voices.Enabled = true;

        }



    





}
}
