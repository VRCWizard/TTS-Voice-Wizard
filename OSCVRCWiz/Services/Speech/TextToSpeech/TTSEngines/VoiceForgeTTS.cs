using Newtonsoft.Json.Linq;
using System.Net;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Text.Json;
using Windows.Media.Protection.PlayReady;
using static System.Net.WebRequestMethods;
using Octokit;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using Amazon.Polly;
using SpotifyAPI.Web.Http;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{

    public class VoiceForgeTTS
    {
        private static readonly HttpClient client = new HttpClient();

        // charLimit: 540?

        public static async Task VoiceForgeTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            // bro, where do u even get one of these keys
            // borrowed from chrisjp/tts who borrowed from Wrapper-Offline/Wrapper-Offline
            string key = "8b3f76a8539";

            string email = "-";
           


            MemoryStream memoryStream = new MemoryStream();

            Task<Stream> streamTask = CallVoiceForgeAPIAsync(key, TTSMessageQueued.text, TTSMessageQueued.Voice, email);
           Stream stream = streamTask.Result;


          //  OutputText.outputLog("stream: " + stream);


            AudioDevices.PlayAudioStream(stream, TTSMessageQueued, ct, true, AudioFormat.Wav);
            memoryStream.Dispose();




        }

        public static async Task<Stream> CallVoiceForgeAPIAsync(string key, string text, string voice,string email)
        {

            var url = $"https://api.voiceforge.com/swift_engine?voice={voice}&msg={text}&email={email}";


            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("HTTP_X_API_KEY", key);

            // request.Content = JsonContent.Create(new { voice = name, text = textIn });
            //  request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "voice", name }, { "text", textIn } });



            HttpResponseMessage response = await client.SendAsync(request);

           // OutputText.outputLog(response.StatusCode.ToString());
            if (response.IsSuccessStatusCode)
            {
               // OutputText.outputLog("Vpog: " + response.StatusCode+" "+await response.Content.ReadAsStringAsync());
                // Debug.WriteLine("Moonbase: " + response.StatusCode);


                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                OutputText.outputLog("VoiceForgeTTS call failed: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                return null;
            }


        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");   
            accents.SelectedIndex= 0;



            SynthesisGetAvailableVoicesAsync(voices);

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;

            styles.Enabled = false;
            voices.Enabled = true;
        }



        public static async Task SynthesisGetAvailableVoicesAsync(ComboBox voices)
        {

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();


            voices.Items.Add("Conrad");
            voices.Items.Add("Designer");
            voices.Items.Add("Diesel");
            voices.Items.Add("Dog");
            voices.Items.Add("Evilgenius");
            voices.Items.Add("Frank");
            voices.Items.Add("French-fry");
            voices.Items.Add("Gregory");
            voices.Items.Add("Jerkface");
            voices.Items.Add("JerseyGirl");
            voices.Items.Add("Kayla");
            voices.Items.Add("Kevin");
            voices.Items.Add("Kidaroo");
            voices.Items.Add("Princess");
            voices.Items.Add("RansomNote");
            voices.Items.Add("Robot");
            voices.Items.Add("Shygirl");
            voices.Items.Add("Susan");
            voices.Items.Add("Tamika");
            voices.Items.Add("TopHat");
            voices.Items.Add("Vixen");
            // voices.Items.Add("Vlad");
            voices.Items.Add("Warren");
            voices.Items.Add("Wiseguy");
            voices.Items.Add("Zach");
            voices.Items.Add("Obama");

            voices.SelectedIndex = 0;


        }








    }



}
