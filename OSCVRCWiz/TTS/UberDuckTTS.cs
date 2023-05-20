using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VarispeedDemo.SoundTouch;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static OSCVRCWiz.Resources.TTSMessageQueue;
using System.Net.Http.Headers;

namespace OSCVRCWiz.TTS
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
            VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Clear();
            if (UberfirstVoiceLoad == true)
            {






                // replace with the path to the JSON file
                string jsonFilePath = "voices/uberduckVoices.json";

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
                    catch (System.ArgumentException e) { }
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
                        VoiceWizardWindow.MainFormGlobal.comboBox5.Items.Add(categoryName);
                    }

                    // Do something for each voice
                    //   Console.WriteLine("Voice " + voice.Key + " belongs to category " + categoryName);
                }
                VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedIndex = 0;
                foreach (KeyValuePair<string, string> voice in UberNameAndCategory)
                {
                    if (voice.Value == VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedItem.ToString())
                    {
                        //Console.WriteLine("Voice " + voice.Key + " belongs to category " + currentCategory);
                        VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.Key);
                    }
                }




                UberfirstVoiceLoad = false;



            }



            else
            {
                /*  foreach (string voice in voiceList)
                  {
                      VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice);
                  }*/
                if (changedMethods)
                {


                    foreach (var cat in seenCategories)
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBox5.Items.Add(cat);
                    }
                }


                foreach (KeyValuePair<string, string> voice in UberNameAndCategory)
                {
                    if (voice.Value == currentCategory)
                    {
                        //Console.WriteLine("Voice " + voice.Key + " belongs to category " + currentCategory);
                        VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.Key);
                    }
                }

            }

            VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex = 0;







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

                var content = new StringContent(JsonConvert.SerializeObject(new { speech = text, voicemodel_uuid = voicemodel_uuid }));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var byteArray = System.Text.Encoding.ASCII.GetBytes($"{authKey}:{authSecret}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var response = await client.PostAsync("speak", content);

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
                OutputText.outputLog("[Uberduck TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }

            //   return base64String;
            //  return "";
        }
        public static async void UberPlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            try
            {


                var audiobytes = Convert.FromBase64String(audioString);
                MemoryStream memoryStream = new MemoryStream(audiobytes);

                AudioDevices.playWaveStream(memoryStream, TTSMessageQueued, ct);
                memoryStream.Dispose();



            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Uberduck TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);

                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }
        }

    }
}
