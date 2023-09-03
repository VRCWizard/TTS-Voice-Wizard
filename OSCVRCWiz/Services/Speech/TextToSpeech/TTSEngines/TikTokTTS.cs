using Newtonsoft.Json.Linq;
using System.Net;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Text.Json;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class TikTokTTS
    {
        // public static WaveOut TikTokOutput=null;

        public static async Task TikTokTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
          



            byte[] result = null;
            try
            {
                result = await CallTikTokAPIAsync(TTSMessageQueued.text, TTSMessageQueued.Voice);
            }
            catch (Exception ex)
            {



                OutputText.outputLog("[TikTok TTS Error: " + ex.Message+ "]", Color.Red);
                if (ex.InnerException != null)
                {
                    OutputText.outputLog("[TikTok TTS Inner Exception: " + ex.InnerException.Message + "]", Color.Red);
                }
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());


            }



            //  File.WriteAllBytes("TikTokTTS.mp3", result);          
            //  Task.Run(() => PlayAudioHelper());

            MemoryStream memoryStream = new MemoryStream(result);



            //   AudioDevices.playMp3Stream(memoryStream, TTSMessageQueued, ct);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();






            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {
           

            var url = "https://tiktok-tts.weilnet.workers.dev/api/generation";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            //httpRequest.Timeout = 180000;// httpclient-an-error-occurred-while-sending-the-request attempt fix (3 minutes)
           // httpRequest.KeepAlive = false;// httpclient-an-error-occurred-while-sending-the-request attempt fix                 

            var apiVoice = GetTikTokVoice(voice);

            var data = "{\"text\":\"" + text + "\",\"voice\":\"" + apiVoice + "\"}";

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
          //  OutputText.outputLog("[TikTok TTS Error: " + httpResponse.StatusCode + "]", Color.Red);



            System.Diagnostics.Debug.WriteLine(audioInBase64);
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


                //string jsonFilePath = "Assets/voices/TiktokVoices.json";

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
                    TikTokVoice[] voices = JsonSerializer.Deserialize<TikTokVoice[]>(jsonData);

                    

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

        public static void SetVoices(ComboBox voices, ComboBox styles)
        {
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
