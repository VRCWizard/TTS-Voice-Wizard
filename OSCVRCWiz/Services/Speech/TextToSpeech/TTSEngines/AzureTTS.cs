﻿using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSCVRCWiz.Settings;
using NAudio.Wave;
using System.IO;
using Swan.Logging;
using System.Text.Json;
using Octokit;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines

{
    public class Voice
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string LocalName { get; set; }
        public string ShortName { get; set; }
        public string Gender { get; set; }
        public string Locale { get; set; }
        public string LocaleName { get; set; }
        public string SampleRateHertz { get; set; }
        public string VoiceType { get; set; }
        public string Status { get; set; }
        public string WordsPerMinute { get; set; }
        public List<string> StyleList { get; set; }
    }
    public class AzureTTS
    {
        // public static Microsoft.CognitiveServices.Speech.SpeechSynthesizer synthesizerVoice;

        //TTS
        public static Dictionary<string, string[]> AllVoices4Language = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> RememberLanguageVoices = new Dictionary<string, string[]>();
        public static bool firstVoiceLoad = true;
        // public static SpeechSynthesizer synthesizerVoice;

        public static async Task SynthesisGetAvailableVoicesAsync(string fromLanguageFullname)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();
            //  var ot = new OutputText();

            if (!RememberLanguageVoices.ContainsKey(fromLanguageFullname))
            {
                //var config = SpeechConfig.FromSubscription(AzureRecognition.YourSubscriptionKey, AzureRecognition.YourServiceRegion);

                // Creates a speech synthesizer
                //  using (var synthesizer = new SpeechSynthesizer(config, null as AudioConfig))
                //   {
                //  var ts = new AzureRecognition();
                //   var language = ts.toLanguageID(fromLanguageFullname);

                List<string> localList = new List<string>();  //keep commented voices and release if they are widely requested (idea with new releasing all voices is to reduce load time)
                switch (fromLanguageFullname)
                {
                    case "Afrikaans [af]": localList.Add("af-ZA"); break;
                    case "Arabic [ar]":
                        //   localList.Add("ar-AE");

                        //  localList.Add("ar-BH");
                        localList.Add("ar-DZ");
                        localList.Add("ar-EG");
                        //  localList.Add("ar-IL");
                        localList.Add("ar-IQ");
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
                    case "Armenian [hy]": localList.Add("hy-AM"); break;
                    case "Azerbaijani [az]": localList.Add("az-AZ"); break;
                    case "Bosnian [bs]": localList.Add("bs-BA"); break;
                    case "Bulgarian [bg]": localList.Add("bg-BG"); break;
                    case "Cantonese [yue]": localList.Add("yue-CN"); break;
                    case "Catalan [ca]": localList.Add("ca-ES"); break;


                    case "Chinese [zh]":
                        localList.Add("zh-CN");
                        localList.Add("zh-CN-SICHUAN");
                        localList.Add("zh-HK");
                        localList.Add("zh-TW"); break;
                    case "Croatian [hr]": localList.Add("hr-HR"); break;
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

                         localList.Add("en-GH");
                          localList.Add("en-HK");
                          localList.Add("en-IE");
                           localList.Add("en-IN");
                           localList.Add("en-KE");
                            localList.Add("en-NZ");
                        localList.Add("en-PH");
                           localList.Add("en-SG");
                           localList.Add("en-TZ");
                           localList.Add("en-ZA");

                        break;
                    case "Estonian [et]": localList.Add("et-EE"); break;
                    case "Filipino [fil]": localList.Add("fil-PH"); break;
                    case "Finnish [fi]": localList.Add("fi-FI"); break;
                    case "French [fr]":
                        localList.Add("fr-FR");
                        localList.Add("fr-BE");
                        localList.Add("fr-CA");
                        localList.Add("fr-CH");

                        break;
                    case "Galician [gl]": localList.Add("gl-ES"); break;
                    case "German [de]":
                        localList.Add("de-AT");
                        localList.Add("de-CH");
                        localList.Add("de-DE");
                        break;
                    case "Greek [el]": localList.Add("el-GR"); break;
                    case "Hebrew [he]": localList.Add("he-IL"); break;
                    


                    case "Hindi [hi]": localList.Add("hi-IN"); break;
                    case "Hungarian [hu]": localList.Add("hu-HU"); break;
                    case "Icelandic [is]": localList.Add("is-IS"); break;
                    case "Indonesian [id]": localList.Add("id-ID"); break;

                    case "Irish [ga]": localList.Add("ga-IE"); break;
                    case "Italian [it]": localList.Add("it-IT"); break;

                    case "Japanese [ja]": localList.Add("ja-JP"); break;

                    case "Kannada [kn]": localList.Add("kn-IN"); break;
                    case "Kazakh [kk]": localList.Add("kk-KZ"); break;
                 

                    case "Korean [ko]": localList.Add("ko-KR"); break;
                    case "Latvian [lv]": localList.Add("lv-LV"); break;
                    case "Lithuanian [lt]": localList.Add("lt-LT"); break;
                    case "Macedonian [mk]": localList.Add("mk-MK"); break;
                    case "Malay [ms]": localList.Add("ms-MY"); break;
                    case "Marathi [mr]": localList.Add("mr-IN"); break;
                    case "Nepali [ne]": localList.Add("ne-NP"); break;
                 


                    case "Norwegian [nb]": localList.Add("nb-NO"); break;

                    case "Persian [fa]": localList.Add("fa-IR"); break;

                    case "Polish [pl]": localList.Add("pl-PL"); break;
                    case "Portuguese [pt]":
                        localList.Add("pt-BR");
                        localList.Add("pt-PT"); break;
                   // case "Punjabi [pa]": localList.Add(""); break;
                    case "Romanian [ro]": localList.Add(""); break;

                    case "Russian [ru]": localList.Add("ru-RU"); break;
                    case "Serbian [sr]": 
                        localList.Add("sr-LATN-RS");
                        localList.Add("sr-RS");
                        break;
                    case "Slovak [sk]": localList.Add("sk-SK"); break;
                    case "Slovenian [sl]": localList.Add("sl-SI"); break;
       
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
                    case "Swahili [sw]": 
                        localList.Add("sw-KE");
                        localList.Add("sw-TZ");
                        break;
                    case "Swedish [sv]": localList.Add("sv-SE"); break;
                    case "Tamil [ta]": 
                        localList.Add("ta-IN");
                        localList.Add("ta-LK");
                        localList.Add("ta-MY");
                        localList.Add("ta-IN");
                        break;
                    case "Telugu [te]": localList.Add("te-IN"); break;
 
                    case "Thai [th]": localList.Add("th-TH"); break;
                    case "Turkish [tr]": localList.Add("tr-TR"); break;
                    case "Ukrainian [uk]": localList.Add("uk-UA"); break;
                    case "Urdu [ur]": 
                        localList.Add("ur-IN");
                        localList.Add("ur-PK");
                        break;
                    case "Uzbek [uz]": localList.Add("uz-UZ"); break;
                    case "Vietnamese [vi]": localList.Add("vi-VN"); break;
                    case "Welsh [cy]": localList.Add("cy-GB"); break;


                    default: localList.Add("en-US"); break; // if translation to english happens something is wrong
                }
                List<string> voiceList = new List<string>();
                /*   foreach (var local in localList)
                   {
                       using (var result = await synthesizer.GetVoicesAsync(local))
                       {
                           if (result.Reason == ResultReason.VoicesListRetrieved)
                           {
                               OutputText.outputLog("[Voices successfully retrieved from Azure]", Color.Green);


                               foreach (var voice in result.Voices)
                               {

                                   //  ot.outputLog(MainForm,voice.LocalName);
                                   AllVoices4Language.Add(voice.ShortName, voice.StyleList);
                                   VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.ShortName);
                                   voiceList.Add(voice.ShortName);
                                   //   foreach (KeyValuePair<string, string[]> kvp in AllVoices4Language)
                                   //  {
                                   //   ot.outputLog(MainForm, kvp.Key.ToString());
                                   // foreach (string style in kvp.Value)
                                   //  {
                                   //  ot.outputLog(MainForm, style);
                                   //  }
                                   //    }
                                   //  ot.outputLog(MainForm, voice.ShortName);
                                   //ot.outputLog(MainForm, voice.StyleList);
                                   //  foreach (var style in voice.StyleList)
                                   // {
                                   //     ot.outputLog(MainForm, style);
                                   // }
                               }
                           }
                           else if (result.Reason == ResultReason.Canceled)
                           {
                               OutputText.outputLog($"CANCELED: ErrorDetails=[{result.ErrorDetails}]", Color.Red);
                               OutputText.outputLog($"CANCELED: Did you update the Azure subscription info?", Color.Red);
                           }
                       }
                   }*/
                foreach (var locale in localList)
                {
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;

                    string relativePath = "Assets/voices/azureVoices.json";

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


                    /*    if (string.IsNullOrWhiteSpace(jsonData))
                        {
                            OutputText.outputLog("[Error accessing voice files, try running as admin or moving entire VoiceWizardFolder to new location]", Color.Red);
                        }*/

                    // deserialize the JSON data into an array of Voice objects
                    Voice[] voices = JsonSerializer.Deserialize<Voice[]>(jsonData);

                    // replace with the desired locale
                    // string locale = "en-GB";

                    foreach (var voice in voices)
                    {
                        if (voice.Locale == locale)
                        {
                            List<string> styleList = new List<string>();

                            if (voice.StyleList != null)
                            {
                                styleList.AddRange(voice.StyleList);
                            }


                            AllVoices4Language.Add(voice.ShortName, styleList.ToArray());
                            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice.ShortName);
                            voiceList.Add(voice.ShortName);
                        }

                    }

                }
                RememberLanguageVoices.Add(fromLanguageFullname, voiceList.ToArray());


            }
            else
            {
                //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: Voices successfully reloaded locally]");
                foreach (string voice in RememberLanguageVoices[fromLanguageFullname])
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add(voice);
                }
            }
            if (firstVoiceLoad == false)
            {
                //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice]");
                VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;
            }

            if (firstVoiceLoad == true)
            {
                //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice and style to saved values]");
                try
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                    VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
                }
                catch (Exception ex) 
                {
                    OutputText.outputLog("[Error selecting voice presets (Consider editing and re-saving your voice presets): " + ex.Message + "]", Color.Red);
                }
                firstVoiceLoad = false;

            }

        }

        public static async Task SynthesizeAudioAsync(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default) //TTS Outputs through speakers //can not change voice style
        {
            try
            {
                string style = TTSMessageQueued.Style;
                int rate = TTSMessageQueued.Speed;
                int pitch = TTSMessageQueued.Pitch;
                int volume = TTSMessageQueued.Volume;
                string voice = TTSMessageQueued.Voice;






                var config = SpeechConfig.FromSubscription(AzureRecognition.YourSubscriptionKey, AzureRecognition.YourServiceRegion);
                // config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw16Khz16BitMonoTrueSilk);
                config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);

                //   config.SetProperty(PropertyId.Speech_LogFilename, "logfile.txt");

                // Note: if only language is set, the default voice of that language is chosen.
                // config.SpeechSynthesisLanguage = "<your-synthesis-language>"; // For example, "de-DE"
                // The voice setting will overwrite the language setting.
                // The voice setting will not overwrite the voice element in input SSML.
                // config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                // config.SpeechSynthesisVoiceName = "en-US-SaraNeural";

                // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#speaker-recognition

                var ratePercent = rate;
                var pitchPercent = pitch;
                var volumePercent = (int)Math.Floor((volume * 0.1f - 1) * 100);

                string rateString = "<prosody rate=\"" + ratePercent + "%\">"; //1
                string pitchString = "<prosody pitch=\"" + pitchPercent + "%\">"; //1
                string volumeString = "<prosody volume=\"" + volumePercent + "%\">"; //1

                Debug.WriteLine("rate: " + ratePercent);
                Debug.WriteLine("pitch: " + pitchPercent);
                Debug.WriteLine("volume: " + volumePercent);
                Debug.WriteLine("voice: " + voice);
                Debug.WriteLine("style: " + style);
                Debug.WriteLine("text: " + TTSMessageQueued.text);


                //  var audioConfig = AudioConfig.FromSpeakerOutput(AudioDevices.currentOutputDevice);
                AudioOutputStream stream = AudioOutputStream.CreatePullStream();//this allows for instant synthesis for naudio output
                var audioConfig = AudioConfig.FromStreamOutput(stream);//this allows for instant synthesis for naudio output
                                                                       //  if (AudioDevices.currentOutputDeviceName == "Default")
                                                                       //  {
                                                                       //      audioConfig = AudioConfig.FromDefaultSpeakerOutput();
                                                                       //  }


                var synthesizerVoice = new SpeechSynthesizer(config, audioConfig);


                string ssml0 = "<speak version=\"1.0\"";
                ssml0 += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
                if (style != "normal") { ssml0 += " xmlns:mstts=\"https://www.w3.org/2001/mstts\""; }
                ssml0 += " xml:lang=\"en-US\">";

                string thisVoice = "<voice name=\"" + voice + "\">";//
                ssml0 += thisVoice;


                if (style != "normal")
                {
                    ssml0 += "<mstts:express-as style=\"" + style + "\">";

                }
                if (rate != 0)//5 = default /middle of track bar
                {
                    ssml0 += rateString;


                }
                if (pitch != 0)
                {
                    ssml0 += pitchString;


                }
                if (volume != 10)
                {
                    ssml0 += volumeString;


                }
                ssml0 += TTSMessageQueued.text;
                if (rate != 0) { ssml0 += "</prosody>"; }
                if (pitch != 0) { ssml0 += "</prosody>"; }
                if (volume != 10) { ssml0 += "</prosody>"; }
                if (style != "normal") { ssml0 += "</mstts:express-as>"; }
                ssml0 += "</voice>";
                ssml0 += "</speak>";

                Debug.WriteLine("DEBUG OUTPUT HERE");
                Debug.WriteLine(ssml0);

                // var result = await synthesizerVoice.

                var result = await synthesizerVoice.SpeakSsmlAsync(ssml0);

                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {


                    MemoryStream memoryStream = new MemoryStream(result.AudioData);

                    AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, false, AudioFormat.Wav);
                    memoryStream.Dispose();





                }
                else if (result.Reason == ResultReason.Canceled)
                {
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    Debug.WriteLine($"[CANCELED: Reason={cancellation.Reason}]");
                    OutputText.outputLog($"[CANCELED: Reason={cancellation.Reason}]", Color.Red);

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Debug.WriteLine($"[CANCELED: ErrorCode={cancellation.ErrorCode}]");
                        OutputText.outputLog($"[CANCELED: ErrorCode={cancellation.ErrorCode}]", Color.Red);
                        Debug.WriteLine($"[CANCELED: ErrorDetails={cancellation.ErrorDetails}]");
                        OutputText.outputLog($"[CANCELED: ErrorDetails={cancellation.ErrorDetails}]", Color.Red);
                        Debug.WriteLine($"[CANCELED: Did you update the subscription info?]");
                        OutputText.outputLog($"[CANCELED: Did you update the subscription info?]", Color.Red);
                    }
                    TTSMessageQueue.PlayNextInQueue();
                }



            }
            catch (Exception ex)
            {
                //  MessageBox.Show("No valid subscription key given or speech service has been disabled; " + ex.Message.ToString());
                OutputText.outputLog("[Azure Error: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[You may be missing an Azure Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                TTSMessageQueue.PlayNextInQueue();
            }
        }
        public static async void AzurePlayAudioPro(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            try
            {
                var audiobytes = Convert.FromBase64String(audioString);
                MemoryStream memoryStream = new MemoryStream(audiobytes);

                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, false, AudioFormat.Wav);
                memoryStream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Azure Ouput Device *AUDIO* Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();
            }
        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {

            accents.Items.Clear();

            var voiceAccents = new List<string>()
                    {
                       "Afrikaans [af]",
                        "Arabic [ar]",
                        "Armenian [hy]",
                        "Azerbaijani [az]",
                        "Bosnian [bs]",
                        "Bulgarian [bg]",
                        "Cantonese [yue]",
                        "Catalan [ca]",
                        "Chinese [zh]",
                        "Croatian [hr]",
                        "Czech [cs]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                        "Estonian [et]",
                        "Filipino [fil]",
                        "Finnish [fi]",
                        "French [fr]",
                        "Galician [gl]",
                        "German [de]",
                        "Greek [el]",
                        "Hebrew [he]",
                        "Hindi [hi]",
                        "Hungarian [hu]",
                        "Icelandic [is]",
                        "Indonesian [id]",
                        "Irish [ga]",
                        "Italian [it]",
                        "Japanese [ja]",
                        "Kannada [kn]",
                        "Kazakh [kk]",
                        "Korean [ko]",
                        "Latvian [lv]",
                        "Lithuanian [lt]",
                        "Macedonian [mk]",
                        "Malay [ms]",
                        "Marathi [mr]",
                        "Nepali [ne]",
                        "Norwegian [nb]",
                        "Persian [fa]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                        "Romanian [ro]",
                        "Russian [ru]",
                        "Serbian [sr]",
                        "Slovak [sk]",
                        "Slovenian [sl]",
                        "Spanish [es]",
                        "Swahili [sw]",
                        "Swedish [sv]",
                        "Tamil [ta]",
                        "Telugu [te]",
                        "Thai [th]",
                        "Turkish [tr]",
                        "Ukrainian [uk]",
                        "Urdu [ur]",
                        "Uzbek [uz]",
                        "Vietnamese [vi]",
                        "Welsh [cy]",
                    };
            foreach (var accent in voiceAccents)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedItem = "English [en]";


            AzureTTS.SynthesisGetAvailableVoicesAsync(accents.Text.ToString());
            // comboBox2.SelectedIndex = 0;
            styles.Enabled = true;
            voices.Enabled = true;

        }

        }
}