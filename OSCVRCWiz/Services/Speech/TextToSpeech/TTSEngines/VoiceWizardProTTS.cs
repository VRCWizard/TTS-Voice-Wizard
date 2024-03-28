using Newtonsoft.Json.Linq;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Services.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class VoiceWizardProTTS
    {
        private static readonly HttpClient client = new HttpClient();//reusing client save so much time!!! around 100ms

        public static async Task<string> VoiceWizardProTextAsSpeech(string apiKey, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);

         //   Stopwatch stopwatch = new Stopwatch();
            
            if (apiKey == "")
            {
                OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                return "";
            }


            string result = null;
            string translation = null;
            string audioString = "";
            string translationString = "";

            // byte[] result = null;
            try
            {
               // stopwatch.Start();
                (result, translation) = await CallVoiceProAPIAsync(apiKey, TTSMessageQueued);
              //  stopwatch.Stop();
             //   OutputText.outputLog($"Full Runtime (Outside): {stopwatch.ElapsedMilliseconds}", Color.Yellow);

                audioString = result;
                translationString = translation;

               // OutputText.outputLog("[AudioString: " + audioString + "]", Color.Red);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[VoiceWizardPro API Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();
                return "";

            }


     

            switch (TTSMessageQueued.TTSMode)
            {
                /* case "Moonbase":
                 // code to execute when expression is equal to value1
                 Task.Run(() => FonixTalkTTS.MoonBasePlayAudio(audioString, TTSMessageQueued, ct));

                     break;*/
                case "Azure":
                    // code to execute when expression is equal to value2
                    Task.Run(() => AzureTTS.AzurePlayAudioPro(audioString, TTSMessageQueued, ct));

                    break;
                case "Amazon Polly":
                    // code to execute when expression is equal to value2
                    Task.Run(() => AmazonPollyTTS.AmazonPlayAudioPro(audioString, TTSMessageQueued, ct));

                    break;

                case "Google (Pro Only)":
                    // code to execute when expression is equal to value2
                    Task.Run(() => GoogleTTS.GooglePlayAudio(audioString, TTSMessageQueued, ct));

                    break;

                case "IBM Watson (Pro Only)":
                    // code to execute when expression is equal to value2
                    Task.Run(() => IBMWatsonTTS.WatsonPlayAudio(audioString, TTSMessageQueued, ct));

                    break;
                case "Deepgram Aura (Pro Only)":
                    // code to execute when expression is equal to value2
                    Task.Run(() => DeepgramAuraTTS.AuraPlayAudio(audioString, TTSMessageQueued, ct));

                    break;

                case "Corqui (Pro Only)":
                    // code to execute when expression is equal to value2
                    //Task.Run(() => DeepgramAuraTTS.AuraPlayAudio(audioString, TTSMessageQueued, ct));

                    break;
                case "OpenAI":
                    // code to execute when expression is equal to value2
                    Task.Run(() => OpenAITTS.OpenAIPlayAudioPro(audioString, TTSMessageQueued, ct));

                    break;
                /*case "Uberduck (Pro Only)":
                    // code to execute when expression is equal to value2
                    Task.Run(() => UberDuckTTS.UberPlayAudio(audioString, TTSMessageQueued, ct));*/

                //break;
                default:
                    // code to execute when expression is not equal to any of the values
                    break;
            }
            

            return translationString;
            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }
       
        private static async Task<(string, string)> CallVoiceProAPIAsync(string apiKey, TTSMessageQueue.TTSMessage message)
        {
          //  Stopwatch stopwatchfull = new Stopwatch();
          //  stopwatchfull.Start();
            string voiceWizardAPITranslationString = "";

            bool translate = false;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true)
            {
                translate = true;
            }
            var branch = "eastus";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                branch = VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.Text.ToString();
            });
            var url = $"https://ttsvoicewizard.herokuapp.com/api/tts?";
           switch (branch)
            {
                case "eastus": url = $"https://ttsvoicewizard.herokuapp.com/api/tts?"; break;
                case "dev":    url = $"https://ttsvoicewizard-playground.herokuapp.com/api/tts?"; break;
                case "local": url = $"http://localhost:54029/api/tts?"; break;
                default: break;
            }

            url +=
              $"apiKey={apiKey}" +
                $"&TTSMode={message.TTSMode}" +
                $"&text={message.text}" +
                $"&voice={message.Voice}" +
                $"&style={message.Style}" +
                $"&speed={message.Speed}" +
                $"&pitch={message.Pitch}" +
                $"&volume={message.Volume}" +
                $"&fromLang={message.SpokenLang}" +
                $"&toLang={message.TranslateLang}" +
                $"&transAudio={translate}";

           // Stopwatch stopwatch = new Stopwatch();
           // stopwatch.Start();

            var response = await client.PostAsync(url, null).ConfigureAwait(false);

          //  stopwatch.Stop();
          //  OutputText.outputLog($"API RESPONSE TIME:{stopwatch.ElapsedMilliseconds}", Color.Yellow);
 

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                OutputText.outputLog("VoiceWizardPro API Error: " + response.StatusCode + ": " + errorMessage, Color.Red);
                return ("", "");
            }


            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + json);

            var dataHere = JObject.Parse(json).SelectToken("audioString").ToString();

            var charUsed = JObject.Parse(json).SelectToken("charUsed").ToString();
            var charLimit = JObject.Parse(json).SelectToken("charLimit").ToString();
            var transCharUsed = JObject.Parse(json).SelectToken("transCharUsed").ToString();
            var transCharLimit = JObject.Parse(json).SelectToken("transCharLimit").ToString();


 
            _ = Task.Run(() => //code don't wait, making this a task saves 30ms
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text = $"TTS Characters Used: {charUsed}/{charLimit}";
                    VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text = $"Translation Characters Used: {transCharUsed}/{transCharLimit}";
                    Settings1.Default.charsUsed = VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text.ToString();
                    Settings1.Default.transCharsUsed = VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text.ToString();
                    Settings1.Default.Save();
                });
            });


            voiceWizardAPITranslationString = JObject.Parse(json).SelectToken("translationText").ToString();
            var audioInBase64 = dataHere.ToString();
            System.Diagnostics.Debug.WriteLine("audio string: " + dataHere);

          //  stopwatchfull.Stop();
         //   OutputText.outputLog($"Function Runtime (from inside): {stopwatchfull.ElapsedMilliseconds}", Color.Yellow);
            return (audioInBase64, voiceWizardAPITranslationString);



        }

        public static async Task<string> CallVoiceProAPIGPT(string apiKey, string text)
        {

            var branch = "eastus";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                branch = VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.Text.ToString();
            });

            if (VoiceWizardWindow.MainFormGlobal.rjToggleGPTUsePrompt.Checked)
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    text = VoiceWizardWindow.MainFormGlobal.richTextBoxGPTPrompt.Text.ToString().Trim() + ": " + text;
                });
            }

            var url = $"https://ttsvoicewizard.herokuapp.com/api/tts?";
            switch (branch)
            {
                case "eastus": url = $"https://ttsvoicewizard.herokuapp.com/api/gpt?"; break;
                case "dev": url = $"https://ttsvoicewizard-playground.herokuapp.com/api/gpt?"; break;
                case "local": url = $"http://localhost:54029/api/gpt?"; break;
                default: break;
            }

            url +=
              $"apiKey={apiKey}" +
                $"&text={text}";


            var response = await client.PostAsync(url, null).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                OutputText.outputLog("VoiceWizardPro ChatGPT API Error: " + response.StatusCode + ": " + errorMessage, Color.Red);
                return ("");
            }


            var json = response.Content.ReadAsStringAsync().Result.ToString();
            System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + json);



            var GPTUsed = JObject.Parse(json).SelectToken("gptUsed").ToString();
            var GPTLimit = JObject.Parse(json).SelectToken("gptLimit").ToString();

            string responseText = JObject.Parse(json).SelectToken("responseString").ToString();

            _ = Task.Run(() => //code don't wait, making this a task saves 30ms
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.labelChatGPTCharacters.Text = $"ChatGPT Characters Used: {GPTUsed}/{GPTLimit}";
                    Settings1.Default.GPTUsageLabel = VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text.ToString();
                    Settings1.Default.Save();
                });
            });

            OutputText.outputLog($"ChatGPT Characters Used: {GPTUsed}/{GPTLimit}");

            return responseText;



        }
    }
}
