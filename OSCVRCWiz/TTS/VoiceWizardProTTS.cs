using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using Octokit;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VarispeedDemo.SoundTouch;
using Windows.Media.Protection.PlayReady;
using static System.Net.Mime.MediaTypeNames;
using System.Buffers.Text;
using System.Diagnostics;
using Swan.Logging;
using OSCVRCWiz.Settings;

namespace OSCVRCWiz.TTS
{
    public class VoiceWizardProTTS
    {

        public static async Task<string> VoiceWizardProTextAsSpeech(string apiKey, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
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
                (result, translation) = await Task.Run(() => CallVoiceProAPIAsync(apiKey, TTSMessageQueued));
                audioString = result;
                translationString = translation;


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

        public static async Task<(string, string)> CallVoiceProAPIAsync(string apiKey, TTSMessageQueue.TTSMessage message)
        {
            string voiceWizardAPITranslationString = "";

            bool translate = false;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true)
            {
                translate = true;
            }

            //var url = $"http://localhost:54029/api/tts?" +
            var url = $"https://ttsvoicewizard.herokuapp.com/api/tts?" +
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


            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // request.Content = JsonContent.Create(new { voice = name, text = textIn });
            //  request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "voice", name }, { "text", textIn } });

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);

            //   System.Diagnostics.Debug.WriteLine("Fonix:" + response.StatusCode);

            // System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + response.StatusCode +" "+ response.ReasonPhrase);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                OutputText.outputLog("VoiceWizardPro API Error: " + response.StatusCode + ": " + errorMessage, Color.Red);
                return (null, "");
            }


            var json = response.Content.ReadAsStringAsync().Result.ToString();
            System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + json);

            var dataHere = JObject.Parse(json).SelectToken("audioString").ToString();

            //  var TTSModeUsed = JObject.Parse(json).SelectToken("TTSMethod").ToString();

            var charUsed = JObject.Parse(json).SelectToken("charUsed").ToString();
            var charLimit = JObject.Parse(json).SelectToken("charLimit").ToString();
            var transCharUsed = JObject.Parse(json).SelectToken("transCharUsed").ToString();
            var transCharLimit = JObject.Parse(json).SelectToken("transCharLimit").ToString();


            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {




                VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text = $"TTS Characters Used: {charUsed}/{charLimit}";
                VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text = $"Translation Characters Used: {transCharUsed}/{transCharLimit}";
                Settings1.Default.charsUsed = VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text.ToString();
                Settings1.Default.transCharsUsed = VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text.ToString();
                Settings1.Default.Save();
            });


            voiceWizardAPITranslationString = JObject.Parse(json).SelectToken("translationText").ToString();
            var audioInBase64 = dataHere.ToString();
            System.Diagnostics.Debug.WriteLine("audio string: " + dataHere);
            return (audioInBase64, voiceWizardAPITranslationString);



        }
    }
}
