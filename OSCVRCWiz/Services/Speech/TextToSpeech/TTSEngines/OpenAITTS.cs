using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using System.Net.Http.Json;
using System.Net;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class OpenAITTS
    {

        private class InputBody
        {
            public string model { get; set; }
            public string input { get; set; }
            public string voice { get; set; }

        }
        private static readonly HttpClient client = new HttpClient();
        public static async Task OpenAItts(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            try
            {
                InputBody input = new InputBody();
                input.input = TTSMessageQueued.text;
                input.voice = TTSMessageQueued.Voice;
                input.model = "gpt-4o-mini-tts";


                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://api.openai.com/v1/audio/speech"),
                    Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), $"Bearer {VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text.ToString()}" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" }
                },
                    Content = JsonContent.Create(input)
                };

                var response = await client.SendAsync(httpRequestMessage);
                var byteArray = await response.Content.ReadAsByteArrayAsync();

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    OutputText.outputLog("[" + response.StatusCode + "]", Color.Red);
                    OutputText.outputLog("[" + response.Headers + "]", Color.Red);
                }

                MemoryStream memoryStream = new MemoryStream(byteArray);
                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);

                memoryStream.Dispose();
            }
            catch (Exception ex)
            {

                OutputText.outputLog("[OpenAI TTS Error: " + ex.Message + "]", Color.Red);
                if (ex.InnerException != null)
                {
                    OutputText.outputLog("[OpenAI TTS Inner Exception: " + ex.InnerException.Message + "]", Color.Red);
                }
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());
            }

        }


        public static async void OpenAIPlayAudioPro(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {

            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();


        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;


            SynthesisGetAvailableVoicesAsync(voices, accents.Text.ToString());
            // comboBox2.SelectedIndex = 0;
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }



        public static async Task SynthesisGetAvailableVoicesAsync(ComboBox voices, string fromLanguageFullname)
        {

            voices.Items.Clear();


            voices.Items.Add("alloy");
            voices.Items.Add("echo");
            voices.Items.Add("fable");
            voices.Items.Add("onyx");
            voices.Items.Add("nova");
            voices.Items.Add("shimmer");
            //new voices
            voices.Items.Add("ash");
           // voices.Items.Add("ballad"); //not producing audio
            voices.Items.Add("coral");
            voices.Items.Add("sage");
            //voices.Items.Add("verse");
            voices.SelectedIndex = 0;
         

        }





    }
}
