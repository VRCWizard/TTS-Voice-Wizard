using Newtonsoft.Json.Linq;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Text;
using System.Net.Http.Headers;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class TikTokTTS
    {
        // public static WaveOut TikTokOutput=null;
        private static readonly HttpClient client = new HttpClient();//reusing client save so much time!!! around 100ms


        public static async Task TikTokTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
           // Stopwatch stopwatch = new Stopwatch();
            byte[] result = null;
            try
            {
               // stopwatch.Start();
                result = await CallTikTokAPIAsync(TTSMessageQueued.text, TTSMessageQueued.Voice);

            }
            catch (Exception ex)
            {

                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("TikTok TTS Error: " + errorMsg, Color.Red);
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());


            }

            MemoryStream memoryStream = new MemoryStream(result);

           // stopwatch.Stop();
          //  OutputText.outputLog($"Processing/Response time:{stopwatch.ElapsedMilliseconds}", Color.Yellow);

            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);

            memoryStream.Dispose();

        }


    /*    public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {
            var audioInBase64 = "";
            // var url = "https://tiktok-tts.weilnet.workers.dev/api/generation"; //deprecated
            var url = "https://tiktok-tts.weilbyte.dev/api/generate";
            var apiVoice = GetTikTokVoice(voice);
            var input = "{\"text\":\"" + text + "\",\"voice\":\"" + apiVoice + "\"}";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.SendAsync(request).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var audioBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
                    return audioBytes;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    OutputText.outputLog($"TikTok Error: {response.StatusCode.ToString()} {errorContent}", Color.Red);
                    return null;
                }

        }*/

        public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {
            var audioInBase64 = "";
            var url = "https://tiktok-tts.weilnet.workers.dev/api/generation";
            var apiVoice = GetTikTokVoice(voice);
            var input = "{\"text\":\"" + text + "\",\"voice\":\"" + apiVoice + "\"}";
            var content = new StringContent(input, Encoding.UTF8, "application/json");

            using (MemoryStream ms = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(ms))
                {
                    streamWriter.Write(input);
                    streamWriter.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    var request = new HttpRequestMessage(HttpMethod.Post, url);
                    request.Content = new StreamContent(ms);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.SendAsync(request).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        OutputText.outputLog($"TikTok Error: {response.StatusCode.ToString()} {errorContent}", Color.Red);
                    }

                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject responseObject = JObject.Parse(responseContent);
                    audioInBase64 = responseObject["data"].ToString();
                }
            }
            return Convert.FromBase64String(audioInBase64);

        }
        public static string GetTikTokVoice(string voice)
    {
        string apiName = "en_us_001";

        apiName = TiktokRememberVoices[voice];

        return apiName;
    }

    public class TikTokVoice
    {
        public string name { get; set; }
        public string voice_id { get; set; }

    }

    public static Dictionary<string, string> TiktokRememberVoices = new Dictionary<string, string>();
    public static bool TiktokfirstVoiceLoad = true;

    public static async Task SynthesisGetAvailableVoicesAsync(ComboBox comboboxVoices)
    {

        if (TiktokfirstVoiceLoad)
        {


            // replace with the path to the JSON file
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativePath = "Assets/voices/TiktokVoices.json";

            string jsonFilePath = Path.Combine(basePath, relativePath);

            // read the JSON data from the file
            string jsonData = "";
            try
            {
                jsonData = System.IO.File.ReadAllText(jsonFilePath);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Could not find directory, try running TTSVoiceWizard as admin or moving the entire folder to a new location. (if it's on the desktop move it to documents or where your games are stored for example)]", Color.Red);
            }



            // deserialize the JSON data into an array of Voice objects
            TikTokVoice[] voices = System.Text.Json.JsonSerializer.Deserialize<TikTokVoice[]>(jsonData);



            foreach (var voice in voices)
            {
                comboboxVoices.Items.Add(voice.name);
                TiktokRememberVoices.Add(voice.name, voice.voice_id);




            }



            TiktokfirstVoiceLoad = false;

        }
        else
        {
            //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: Voices successfully reloaded locally]");
            foreach (string voice in TiktokRememberVoices.Keys)
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
