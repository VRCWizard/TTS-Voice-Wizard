using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;
using Resources;

namespace OSCVRCWiz.TTS
{
    public class AzureTTS
    {
        // public static Microsoft.CognitiveServices.Speech.SpeechSynthesizer synthesizerVoice;

        //TTS
        public static Dictionary<string, string[]> AllVoices4Language = new Dictionary<string, string[]>();
        public static Dictionary<string, string[]> RememberLanguageVoices = new Dictionary<string, string[]>();
        public static bool firstVoiceLoad = true;

        public static async Task SynthesisGetAvailableVoicesAsync(string fromLanguageFullname)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".
            VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Clear();
          //  var ot = new OutputText();

            if (!RememberLanguageVoices.ContainsKey(fromLanguageFullname))
            {
                var config = SpeechConfig.FromSubscription(AzureRecognition.YourSubscriptionKey, AzureRecognition.YourServiceRegion);

                // Creates a speech synthesizer
                using (var synthesizer = new SpeechSynthesizer(config, null as AudioConfig))
                {
                    var ts = new AzureRecognition();
                    //   var language = ts.toLanguageID(fromLanguageFullname);

                    List<string> localList = new List<string>();  //keep commented voices and release if they are widely requested (idea with new releasing all voices is to reduce load time)
                    switch (fromLanguageFullname)
                    {
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

                        case "Chinese [zh]":
                            localList.Add("zh-CN");
                            localList.Add("zh-CN-SICHUAN");
                            localList.Add("zh-HK");
                            localList.Add("zh-TW"); break;
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
                            //   localList.Add("en-PH");
                            //    localList.Add("en-SG");
                            //   localList.Add("en-TZ");
                            //   localList.Add("en-ZA");

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
                        case "German [de]":
                            localList.Add("de-AT");
                            localList.Add("de-CH");
                            localList.Add("de-DE");
                            break;

                        case "Hindi [hi]": localList.Add("hi-IN"); break;
                        case "Hungarian [hu]": localList.Add("hu-HU"); break;
                        case "Indonesian [id]": localList.Add("id-ID"); break;

                        case "Irish [ga]": localList.Add("ga-IE"); break;
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
                    foreach (var local in localList)
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
                    }
                    RememberLanguageVoices.Add(fromLanguageFullname, voiceList.ToArray());

                }
            }
            else
            {
              //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: Voices successfully reloaded locally]");
                foreach (string voice in RememberLanguageVoices[fromLanguageFullname])
                {
                    VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice);
                }
            }
            if (firstVoiceLoad == false)
            {
              //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice]");
                VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex = 0;
            }

            if (firstVoiceLoad == true)
            {
              //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice and style to saved values]");
                VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
                firstVoiceLoad = false;

            }

        }

        public static async Task SynthesizeAudioAsync(string text) //TTS Outputs through speakers //can not change voice style
        {
            try
            {
                string style = "normal";
                string rate = "default";
                string pitch = "default";
                string volume = "default";
                string voice = "Sara";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString())) { style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString(); }
                    if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxRate.Text.ToString())) { rate = VoiceWizardWindow.MainFormGlobal.comboBoxRate.Text.ToString(); }
                    if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxPitch.Text.ToString())) { pitch = VoiceWizardWindow.MainFormGlobal.comboBoxPitch.Text.ToString(); }
                    if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxVolume.Text.ToString())) { volume = VoiceWizardWindow.MainFormGlobal.comboBoxVolume.Text.ToString(); }
                    if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString())) { voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString(); }


                });


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

                string ratexslow = "<prosody rate=\"x-slow\">"; //1
                string rateslow = "<prosody rate=\"slow\">"; //2
                string ratemedium = "<prosody rate=\"medium\">"; //3
                string ratefast = "<prosody rate=\"fast\">"; //4
                string ratexfast = "<prosody rate=\"x-fast\">"; //5

                string pitchxlow = "<prosody pitch=\"x-low\">"; //1
                string pitchlow = "<prosody pitch=\"low\">"; //2
                string pitchmedium = "<prosody pitch=\"medium\">"; //3
                string pitchhigh = "<prosody pitch=\"high\">"; //4
                string pitchxhigh = "<prosody pitch=\"x-high\">"; //5

                string volumexlow = "<prosody volume=\"x-soft\">"; //1
                string volumelow = "<prosody volume=\"soft\">"; //2
                string volumemedium = "<prosody volume=\"medium\">"; //3
                string volumehigh = "<prosody volume=\"loud\">"; //4
                string volumexhigh = "<prosody volume=\"x-loud\">"; //5

                Debug.WriteLine("rate: " + rate);
                Debug.WriteLine("pitch: " + pitch);
                Debug.WriteLine("volume: " + volume);
                Debug.WriteLine("voice: " + voice);
                Debug.WriteLine("style: " + style);
                Debug.WriteLine("text: " + text);


                var audioConfig = AudioConfig.FromSpeakerOutput(AudioDevices.currentOutputDevice);
                if (AudioDevices.currentOutputDeviceName == "Default")
                {
                    audioConfig = AudioConfig.FromDefaultSpeakerOutput();

                }
               

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
                if (rate != "default")
                {
                    if (rate == "x-slow") { ssml0 += ratexslow; }
                    if (rate == "slow") { ssml0 += rateslow; }
                    if (rate == "medium") { ssml0 += ratemedium; }
                    if (rate == "fast") { ssml0 += ratefast; }
                    if (rate == "x-fast") { ssml0 += ratexfast; }

                }
                if (pitch != "default")
                {
                    if (pitch == "x-low") { ssml0 += pitchxlow; }
                    if (pitch == "low") { ssml0 += pitchlow; }
                    if (pitch == "medium") { ssml0 += pitchmedium; }
                    if (pitch == "high") { ssml0 += pitchhigh; }
                    if (pitch == "x-high") { ssml0 += pitchxhigh; }

                }
                if (volume != "default")
                {
                    if (volume == "x-soft") { ssml0 += volumexlow; }
                    if (volume == "soft") { ssml0 += volumelow; }
                    if (volume == "medium") { ssml0 += volumemedium; }
                    if (volume == "loud") { ssml0 += volumehigh; }
                    if (volume == "x-loud") { ssml0 += volumexhigh; }

                }
                ssml0 += text;
                if (rate != "default") { ssml0 += "</prosody>"; }
                if (pitch != "default") { ssml0 += "</prosody>"; }
                if (volume != "default") { ssml0 += "</prosody>"; }
                if (style != "normal") { ssml0 += "</mstts:express-as>"; }
                ssml0 += "</voice>";
                ssml0 += "</speak>";

                Debug.WriteLine(ssml0);

                
                var result = await synthesizerVoice.SpeakSsmlAsync(ssml0).ConfigureAwait(false);
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.WriteLine($"[Speech synthesized to speaker for text: {text}]");
                    // ot.outputLog(MainForm, $"[Azure Speech Synthesized]");
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
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("No valid subscription key given or speech service has been disabled; " + ex.Message.ToString());
            }
        }
    }
}