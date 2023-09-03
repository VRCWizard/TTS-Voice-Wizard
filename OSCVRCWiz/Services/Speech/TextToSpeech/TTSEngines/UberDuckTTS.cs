﻿using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static OSCVRCWiz.Services.Speech.TextToSpeech.TTSMessageQueue;
using System.Net.Http.Headers;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{

    public class UberVoice
    {
        public string display_name { get; set; }

        public string name { get; set; }
        public string voicemodel_uuid { get; set; }

        public string category { get; set; }

    }
    public class UberDuckTTS
    {


        // public static Microsoft.CognitiveServices.Speech.SpeechSynthesizer synthesizerVoice;

        //TTS
        //  public static Dictionary<string, string[]> AllVoices4Language = new Dictionary<string, string[]>();
        public static Dictionary<string, string> UberVoiceNameAndID = new Dictionary<string, string>();
        public static Dictionary<string, string> UberNameAndCategory = new Dictionary<string, string>();
        public static bool UberfirstVoiceLoad = true;
        public static HashSet<string> seenCategories = new HashSet<string>();
        //    static List<string> voiceList = new List<string>();
        // public static SpeechSynthesizer synthesizerVoice;

        public static async Task SynthesisGetAvailableVoicesAsync(string currentCategory, bool changedMethods)
        {
            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();
            if (UberfirstVoiceLoad == true)
            {


                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = "Assets/voices/uberduckVoices.json";
                string jsonFilePath = Path.Combine(basePath, relativePath);


                // read the JSON data from the file
                string jsonData = File.ReadAllText(jsonFilePath);

                // deserialize the JSON data into an array of Voice objects
                UberVoice[] voices = System.Text.Json.JsonSerializer.Deserialize<UberVoice[]>(jsonData);

                // replace with the desired locale
                // string locale = "en-GB";

                foreach (var voice in voices)
                {




                    // AllVoices4Language.Add(voice.ShortName, styleList.ToArray());
                    //  VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.display_name);
                    //  voiceList.Add(voice.display_name);
                    try
                    {
                        UberVoiceNameAndID.Add(voice.display_name, voice.voicemodel_uuid);
                        UberNameAndCategory.Add(voice.display_name, voice.category);
                    }
                    catch (ArgumentException e) { }
                }

                var sortedList = UberNameAndCategory.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

                foreach (KeyValuePair<string, string> voice in sortedList)
                {
                    string categoryName = voice.Value;

                    // If this is a new category, do something
                    if (!seenCategories.Contains(categoryName))
                    {
                        seenCategories.Add(categoryName);

                        // Do something for each new category found
                        // Console.WriteLine("New category found: " + categoryName);
                        VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.Items.Add(categoryName);
                    }

                    // Do something for each voice
                    //   Console.WriteLine("Voice " + voice.Key + " belongs to category " + categoryName);
                }
                VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedIndex = 0;
                foreach (KeyValuePair<string, string> voice in UberNameAndCategory)
                {
                    if (voice.Value == VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedItem.ToString())
                    {
                        //Console.WriteLine("Voice " + voice.Key + " belongs to category " + currentCategory);
                        VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice.Key);
                    }
                }




                UberfirstVoiceLoad = false;



            }



            else
            {
              
                if (changedMethods)
                {


                    foreach (var cat in seenCategories)
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.Items.Add(cat);
                    }
                }


                foreach (KeyValuePair<string, string> voice in UberNameAndCategory)
                {
                    if (voice.Value == currentCategory)
                    {
                        //Console.WriteLine("Voice " + voice.Key + " belongs to category " + currentCategory);
                        VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice.Key);
                    }
                }

            }

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;







        }

        public static async Task uberduckTTS(TTSMessage message, CancellationToken ct = default)
        {
            try
           {

                var authKey = VoiceWizardWindow.MainFormGlobal.textBoxUberKey.Text.ToString();
                var authSecret = VoiceWizardWindow.MainFormGlobal.textBoxUberSecret.Text.ToString();

                if (string.IsNullOrWhiteSpace(authKey) || string.IsNullOrWhiteSpace(authKey))
                {
                    OutputText.outputLog("[Uberduck API Key or Secret not provided]", Color.Red);
                    return;
                }


                // string apiKey = "your_api_key_here";
                string voicemodel_uuid = message.Voice;
                string text = message.text;
                string audio_uuid = "";


                var client = new HttpClient();

                client.BaseAddress = new Uri("https://api.uberduck.ai/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(new { speech = text, voicemodel_uuid }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var byteArray = System.Text.Encoding.ASCII.GetBytes($"{authKey}:{authSecret}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.PostAsync("speak", content);

                var responseCode = response.StatusCode;
                if (!response.IsSuccessStatusCode)
                {
                    OutputText.outputLog("[Uberduck Error: " + responseCode + "]", Color.Red);
                }

                if (response.IsSuccessStatusCode)
                {
                    string json = response.Content.ReadAsStringAsync().Result.ToString();
                    audio_uuid = JObject.Parse(json).SelectToken("uuid").ToString();
                    // Console.WriteLine(audio_uuid);
                }



                string audioUrl = null;

                for (int i = 0; i < 10; i++)
                {

                    await Task.Delay(1000); // check status every second.
                    var response2 = await client.GetAsync($"https://api.uberduck.ai/speak-status?uuid={audio_uuid}");
                    Console.WriteLine(response2.Content.ReadAsStringAsync().Result.ToString());

                    audioUrl = JObject.Parse(await response2.Content.ReadAsStringAsync())["path"].ToString();


                    // audioUrl = JObject.Parse(statusContent)["path"]?.ToString();

                    if (audioUrl != null && audioUrl != "")
                    {

                        Console.WriteLine("printing: " + audioUrl.ToString());
                        break;

                    }
                }



                // read the audio file into a byte array
                client = new HttpClient();
                byte[] audioBytes = await client.GetByteArrayAsync(audioUrl);

                // convert the byte array to a base64-encoded string
                string base64String = Convert.ToBase64String(audioBytes);
                UberPlayAudio(base64String, message, ct);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Uberduck TTS  Error: " + ex.Message + "]", Color.Red);
              
               PlayNextInQueue();
            }

             //  return base64String;
             // return "";
        }
        public static async void UberPlayAudio(string audioString, TTSMessage TTSMessageQueued, CancellationToken ct)
        {



            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);

            //  AudioDevices.playWaveStream(memoryStream, TTSMessageQueued, ct);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
            memoryStream.Dispose();




        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            UberDuckTTS.SynthesisGetAvailableVoicesAsync(accents.Text.ToString(), true);
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }

      }
}
