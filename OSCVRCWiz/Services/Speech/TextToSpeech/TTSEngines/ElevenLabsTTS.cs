using Newtonsoft.Json;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class ElevenLabsTTS
    {

        private static readonly HttpClient client = new HttpClient();
        public static Dictionary<string, string> voiceDict = null;
        public static bool elevenFirstLoad = true;
        public static async Task ElevenLabsTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);

            var voiceID = voiceDict.FirstOrDefault(x => x.Value == TTSMessageQueued.Voice).Key;
            //  byte[] result = await CallTikTokAPIAsync(text, voice);
            //  File.WriteAllBytes("TikTokTTS.mp3", result);          
            //  Task.Run(() => PlayAudioHelper());

            MemoryStream memoryStream = new MemoryStream();

            Task<Stream> streamTask = CallElevenLabsAPIAsync(TTSMessageQueued.text, voiceID);
            Stream stream = streamTask.Result;

            AmazonPollyTTS.WriteSpeechToStream(stream, memoryStream);


            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();





            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());v
        }

        public static async Task<Stream> CallElevenLabsAPIAsync(string textIn, string voice)
        {

            try
            {

                //modified from https://github.com/connorbutler44/bingbot/blob/main/Service/ElevenLabsTextToSpeechService.cs

                int optimize = 0;
                int stabilities = 0;
                int similarities = 0;
                int styles = 0;
                bool boost = true;
                string modelID = "eleven_monolingual_v1";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {


                    optimize = int.Parse(VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedItem.ToString());
                    stabilities = VoiceWizardWindow.MainFormGlobal.trackBarStability.Value;
                    similarities = VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value;
                    modelID = VoiceWizardWindow.MainFormGlobal.comboBoxLabsModelID.SelectedItem.ToString();
                    styles = VoiceWizardWindow.MainFormGlobal.trackBarStyleExaggeration.Value;
                    boost= VoiceWizardWindow.MainFormGlobal.rjToggleSpeakerBoost.Checked;

                    Debug.WriteLine(optimize);
                    Debug.WriteLine(stabilities);
                    Debug.WriteLine(similarities);
                    Debug.WriteLine(modelID);


                });


                var similarityFloat = similarities * 0.01f;
                var stabilityFloat = stabilities * 0.01f;
                var styleFloat = styles * 0.01f;

                var url = $"https://api.elevenlabs.io/v1/text-to-speech/{voice}?optimize_streaming_latency={optimize}";
                //  var url = $"https://api.elevenlabs.io/v1/text-to-speech/{voice}";
                var apiKey = Settings1.Default.elevenLabsAPIKey;

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                request.Content = JsonContent.Create(new
                {
                    text = textIn,
                    model_id = modelID,
                    voice_settings = new
                    {
                        stability = stabilityFloat,
                        similarity_boost = similarityFloat,
                        style = styleFloat,
                        use_speaker_boost = boost
                    }
                });


                request.Headers.Add("xi-api-key", apiKey);
                request.Headers.Add("Accept", "audio/mpeg");

                //  var data = "{\"text\":\"" + text + "\"}";

                HttpResponseMessage response = await client.SendAsync(request);

                Debug.WriteLine("Eleven Response:" + response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {

                    //  System.Diagnostics.Debug.WriteLine("Eleven Response:" + response.StatusCode);
                    string json = response.Content.ReadAsStringAsync().Result.ToString();

                    dynamic data = JsonConvert.DeserializeObject(json);
                    string status = data.detail.status;

                    if (status != "invalid_api_key")
                    {
                        OutputText.outputLog("[ElevenLabs TTS Error: " + response.StatusCode + ": " + json + "]", Color.Red);
                    }
                    else
                    {
                        OutputText.outputLog("[YOU COPIED YOUR ELEVEN LABS API KEY INCORRECTLY]", Color.Red);
                    }




                }


                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ElevenLabs TTS Error: " + ex.Message + "]", Color.Red);
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                return null;


            }

        }
        public class Voice
        {
            public string voice_id { get; set; }
            public string name { get; set; }
        }
        public class Voices
        {
            public List<Voice> voices { get; set; }
        }


        public static void CallElevenVoices()
        {
            try
            {

                //modified from https://github.com/connorbutler44/bingbot/blob/main/Service/ElevenLabsTextToSpeechService.cs


                var url = $"https://api.elevenlabs.io/v1/voices";
                var apiKey = Settings1.Default.elevenLabsAPIKey;





                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("xi-api-key", apiKey);
                using (WebResponse response = request.GetResponse())
                {

                    using (Stream stream = response.GetResponseStream())
                    {

                        using (var streamReader = new StreamReader(stream))
                        {
                            var result = streamReader.ReadToEnd();
                            // var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                            // audioInBase64 = dataHere.ToString();

                            Debug.WriteLine(result.ToString());
                            var json = result.ToString();

                            Voices voices = JsonConvert.DeserializeObject<Voices>(json);
                            voiceDict = voices.voices.ToDictionary(v => v.voice_id, v => v.name);
                        }

                    }
                }
                /*foreach (KeyValuePair<string, string> kvp in dict)
                  {
                      Console.WriteLine("Voice ID: " + kvp.Key + ", Name: " + kvp.Value);
                  }*/

                // System.Diagnostics.Debug.WriteLine(audioInBase64);
                // return Convert.FromBase64String(audioInBase64);
                elevenFirstLoad = false;
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ElevenLabs Voice Load Error: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[You appear to be using an incorrect ElevenLabs Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/ElevenLabs-TTS ]", Color.DarkOrange);
                TTSMessageQueue.PlayNextInQueue();

            }

        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {

            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;

            voices.Items.Clear();

            try
            {
                if (ElevenLabsTTS.elevenFirstLoad == true)
                {
                    ElevenLabsTTS.CallElevenVoices();
                }

                if (ElevenLabsTTS.voiceDict != null)
                {
                    foreach (KeyValuePair<string, string> kvp in ElevenLabsTTS.voiceDict)
                    {
                        voices.Items.Add(kvp.Value);

                    }
                }
                else
                {
                    voices.Items.Add("error");
                }
            }
            catch (Exception ex)
            {
                voices.Items.Add("error");
                OutputText.outputLog("[ElevenLabs Load1 Error: " + ex.Message + "]", Color.Red);

            }

            voices.SelectedIndex = 0;

            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;


        }




        }
}
