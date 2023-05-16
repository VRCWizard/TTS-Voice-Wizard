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
using NAudio.Wave;
using System.IO;
using Swan.Logging;
using OSCVRCWiz.Resources;
using System.Text.Json;
using Octokit;

namespace OSCVRCWiz.TTS

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
            VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Clear();
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
                               localList.Add("en-PH");
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
                        // replace with the path to the JSON file
                        string jsonFilePath = "voices/azureVoices.json";

                        // read the JSON data from the file
                        string jsonData = File.ReadAllText(jsonFilePath);

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
                                VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.ShortName);
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

                var ratePercent = (int)Math.Floor(((0.5f + rate * 0.1f) - 1) * 100);
                var pitchPercent = (int)Math.Floor(((0.5f + pitch * 0.1f) - 1) * 100);
                var volumePercent = (int)Math.Floor(((volume * 0.1f) - 1) * 100);

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
                if (rate != 5)//5 = default /middle of track bar
                {
                    ssml0 += rateString;


                }
                if (pitch != 5)
                {
                    ssml0 += pitchString;


                }
                if (volume != 10)
                {
                    ssml0 += volumeString;


                }
                ssml0 += TTSMessageQueued.text;
                if (rate != 5) { ssml0 += "</prosody>"; }
                if (pitch != 5) { ssml0 += "</prosody>"; }
                if (volume != 10) { ssml0 += "</prosody>"; }
                if (style != "normal") { ssml0 += "</mstts:express-as>"; }
                ssml0 += "</voice>";
                ssml0 += "</speak>";

                Debug.WriteLine(ssml0);

               // var result = await synthesizerVoice.

                var result = await synthesizerVoice.SpeakSsmlAsync(ssml0);
                
                if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    Debug.WriteLine($"[Speech synthesized to speaker for text: {TTSMessageQueued.text}]");
                    // ot.outputLog(MainForm, $"[Azure Speech Synthesized]");
                   
                        try
                        {
                        MemoryStream memoryStream = new MemoryStream(result.AudioData);

                        MemoryStream memoryStream2 = new MemoryStream();
                        memoryStream.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                        memoryStream.CopyTo(memoryStream2);


                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
                        {
                            MemoryStream memoryStream3 = new MemoryStream();
                            memoryStream.Flush();
                            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                            memoryStream.CopyTo(memoryStream3);

                            memoryStream3.Flush();
                            memoryStream3.Seek(0, SeekOrigin.Begin);// go to begining before copying
                            audioFiles.writeAudioToOutputWav(memoryStream3);
                        }
                        ////


                        memoryStream.Flush();
                        memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                        WaveFileReader wav = new WaveFileReader(memoryStream);


                        memoryStream2.Flush();
                        memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
                        WaveFileReader wav2 = new WaveFileReader(memoryStream2);



                        var AnyOutput = new WaveOut();
                        AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                        AnyOutput.Init(wav);
                        AnyOutput.Play();
                        ct.Register(async () => AnyOutput.Stop());
                        var AnyOutput2 = new WaveOut();
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
                        {
                         //  AnyOutput2 = new WaveOut();
                            AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                            AnyOutput2.Init(wav2);
                            AnyOutput2.Play();
                           
                            ct.Register(async () => AnyOutput2.Stop());
                            while (AnyOutput2.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(2000);
                            }
                        }
                        while (AnyOutput.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(2000);
                        }
                        if(AnyOutput.PlaybackState == PlaybackState.Stopped)
                        {
                         
                            AnyOutput.Stop();
                            AnyOutput.Dispose();
                           
                           // AnyOutput = null;
                          //  if (AnyOutput2 != null)
                          //  {
                                AnyOutput2.Stop();
                                AnyOutput2.Dispose();
                            //    AnyOutput2 = null;
                         //   }
                            memoryStream.Dispose();
                            memoryStream = null;
                          //  memoryStream2.Dispose();
                            wav.Dispose();
                            wav2.Dispose();
                            wav = null;
                            wav2 = null;
                            synthesizerVoice.Dispose();
                            synthesizerVoice = null;
                            stream.Dispose();
                            stream = null;

                            ct = new();
                            Debug.WriteLine("azure dispose successful");
                            TTSMessageQueue.PlayNextInQueue();
                        }
                    }
                        catch(Exception ex)
                        {
                            OutputText.outputLog("[Azure Ouput Device *AUDIO* Error: " + ex.Message + "]", Color.Red);
                        TTSMessageQueue.PlayNextInQueue();
                    }
                    
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

                AudioDevices.playSSMLWaveStream(memoryStream, TTSMessageQueued, ct);
                memoryStream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Azure Ouput Device *AUDIO* Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();
            }
        }

        }
}