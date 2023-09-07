using System.Text.Json;
using System.Windows;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines

{
    public class GoogleVoice
    {
        public string Name { get; set; }
        public string SsmlGender { get; set; }
        public string[] LanguageCodes { get; set; }

    }
    public class GoogleTTS
    {
        // public static Microsoft.CognitiveServices.Speech.SpeechSynthesizer synthesizerVoice;

        //TTS
        //  public static Dictionary<string, string[]> AllVoices4Language = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> GoogleRememberLanguageVoices = new Dictionary<string, string[]>();
        public static bool GooglefirstVoiceLoad = true;
        // public static SpeechSynthesizer synthesizerVoice;

        public static async Task SynthesisGetAvailableVoicesAsync(string fromLanguageFullname)
        {

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();


            if (!GoogleRememberLanguageVoices.ContainsKey(fromLanguageFullname))
            {

                //   var language = ts.toLanguageID(fromLanguageFullname);

                List<string> localList = new List<string>();  //keep commented voices and release if they are widely requested (idea with new releasing all voices is to reduce load time)
                switch (fromLanguageFullname)
                {
                    case "Arabic [ar]":
                        localList.Add("ar-XA");
                        //   localList.Add("ar-AE");

                        //  localList.Add("ar-BH");
                        //  localList.Add("ar-DZ");
                        //  localList.Add("ar-EG");
                        //  localList.Add("ar-IL");
                        //    localList.Add("ar-IQ");
                        //  localList.Add("ar-JO");
                        //   localList.Add("ar-KW");
                        //  localList.Add("ar-LB");
                        //  localList.Add("ar-LY");
                        //  localList.Add("ar-MA");
                        //  localList.Add("ar-OM");
                        //  localList.Add("ar-PS");
                        //  localList.Add("ar-QA");
                        //  localList.Add("ar-SA");
                        // localList.Add("ar-SY");
                        //  localList.Add("ar-TN");
                        //  localList.Add("ar-YE");
                        break;

                    case "Chinese [zh]":
                        localList.Add("cmn-CN");
                        localList.Add("cmn-TW");
                        // localList.Add("zh-CN");
                        // localList.Add("zh-CN-SICHUAN");
                        //  localList.Add("zh-HK");
                        //  localList.Add("zh-TW");
                        break;
                    case "Czech [cs]": localList.Add("cs-CZ"); break;
                    case "Danish [da]": localList.Add("da-DK"); break;
                    case "Dutch [nl]":
                        localList.Add("nl-BE");
                        localList.Add("nl-NL"); break;
                    case "English [en]":
                        localList.Add("en-US");
                        localList.Add("en-GB");
                        localList.Add("en-AU");
                        localList.Add("en-CA");

                        // localList.Add("en-GH");
                        //   localList.Add("en-HK");
                        //   localList.Add("en-IE");
                        //   localList.Add("en-IN");
                        //   localList.Add("en-KE");
                        //    localList.Add("en-NZ");
                        localList.Add("en-PH");
                        //    localList.Add("en-SG");
                        //   localList.Add("en-TZ");
                        //   localList.Add("en-ZA");

                        break;
                    // case "Estonian [et]": localList.Add("et-EE"); break;
                    case "Filipino [fil]": localList.Add("fil-PH"); break;
                    case "Finnish [fi]": localList.Add("fi-FI"); break;
                    case "French [fr]":
                        localList.Add("fr-FR");
                        localList.Add("fr-BE");
                        localList.Add("fr-CA");
                        localList.Add("fr-CH");

                        break;
                    case "German [de]":
                        localList.Add("de-AT");
                        localList.Add("de-CH");
                        localList.Add("de-DE");
                        break;

                    case "Hindi [hi]": localList.Add("hi-IN"); break;
                    case "Hungarian [hu]": localList.Add("hu-HU"); break;
                    case "Indonesian [id]": localList.Add("id-ID"); break;

                    //  case "Irish [ga]": localList.Add("ga-IE"); break;
                    case "Italian [it]": localList.Add("it-IT"); break;

                    case "Japanese [ja]": localList.Add("ja-JP"); break;
                    case "Korean [ko]": localList.Add("ko-KR"); break;
                    case "Norwegian [nb]": localList.Add("nb-NO"); break;
                    case "Polish [pl]": localList.Add("pl-PL"); break;
                    case "Portuguese [pt]":
                        localList.Add("pt-BR");
                        localList.Add("pt-PT"); break;
                    case "Russian [ru]": localList.Add("ru-RU"); break;
                    case "Spanish [es]":
                        localList.Add("es-MX");
                        localList.Add("es-ES");
                        localList.Add("es-US");
                        //  localList.Add("es-AR");
                        // localList.Add("es-BO");
                        // localList.Add("es-CL");
                        //localList.Add("es-CO");
                        // localList.Add("es-CR");
                        // localList.Add("es-CU");
                        // localList.Add("es-DO");
                        // localList.Add("es-EC");

                        //  //localList.Add("es-GQ");
                        // localList.Add("es-GT");
                        //  localList.Add("es-HN");

                        //  localList.Add("es-PE");
                        //   localList.Add("es-PR");
                        //  localList.Add("es-PY");
                        //  localList.Add("es-SV");

                        //   localList.Add("es-UY");
                        //   localList.Add("es-VE");
                        //  localList.Add("es-AR");
                        // 
                        break;
                    case "Swedish [sv]": localList.Add("sv-SE"); break;
                    case "Thai [th]": localList.Add("th-TH"); break;
                    case "Ukrainian [uk]": localList.Add("uk-UA"); break;
                    case "Vietnamese [vi]": localList.Add("vi-VN"); break;


                    default: localList.Add("en-US"); break; // if translation to english happens something is wrong
                }
                List<string> voiceList = new List<string>();

                foreach (var locale in localList)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    string relativePath = "Assets/voices/googleVoices.json";

                    string fullPath = Path.Combine(basePath, relativePath);
                    // replace with the path to the JSON file
                    string jsonFilePath = fullPath;

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
                    GoogleVoice[] voices = JsonSerializer.Deserialize<GoogleVoice[]>(jsonData);

                    // replace with the desired locale
                    // string locale = "en-GB";

                    foreach (var voice in voices)
                    {
                        if (voice.LanguageCodes[0] == locale)
                        {



                            // AllVoices4Language.Add(voice.ShortName, styleList.ToArray());
                            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice.Name + " | " + voice.SsmlGender);
                            voiceList.Add(voice.Name + " | " + voice.SsmlGender);
                        }

                    }

                }
                GoogleRememberLanguageVoices.Add(fromLanguageFullname, voiceList.ToArray());


            }
            else
            {
                //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: Voices successfully reloaded locally]");
                foreach (string voice in GoogleRememberLanguageVoices[fromLanguageFullname])
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice);
                }
            }
            //   if (GooglefirstVoiceLoad == false)
            // {
            //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice]");
            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;
            //  }

            /* if (GooglefirstVoiceLoad == true)
             {
               //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice and style to saved values]");
                 VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                 VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
                 GooglefirstVoiceLoad = false;

             }*/

        }
        public static async void GooglePlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {



            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);


            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();



        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            // comboBox2.Items.Add("");

            var voiceAccentsGoogle = new List<string>()
                    {
                        "Arabic [ar]",
                        "Chinese [zh]",
                        "Czech [cs]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                       // "Estonian [et]",
                        "Filipino [fil]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Hungarian [hu]",
                        "Indonesian [id]",
                       // "Irish [ga]",
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Thai [th]",
                        "Ukrainian [uk]",
                        "Vietnamese [vi]"
                    };
            foreach (var accent in voiceAccentsGoogle)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedIndex = 5;


            GoogleTTS.SynthesisGetAvailableVoicesAsync(accents.Text.ToString());
            // comboBox2.SelectedIndex = 0;
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }



        }
}