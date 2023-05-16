
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
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;

namespace OSCVRCWiz.TTS

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
           
            VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Clear();
          

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
                        // replace with the path to the JSON file
                        string jsonFilePath = "voices/googleVoices.json";

                        // read the JSON data from the file
                        string jsonData = File.ReadAllText(jsonFilePath);

                        // deserialize the JSON data into an array of Voice objects
                        GoogleVoice[] voices = JsonSerializer.Deserialize<GoogleVoice[]>(jsonData);

                        // replace with the desired locale
                       // string locale = "en-GB";

                        foreach (var voice in voices)
                        {
                            if (voice.LanguageCodes[0] == locale)
                            {
                               


                               // AllVoices4Language.Add(voice.ShortName, styleList.ToArray());
                                VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice.Name +" | " +voice.SsmlGender);
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
                    VoiceWizardWindow.MainFormGlobal.comboBox2.Items.Add(voice);
                }
            }
         //   if (GooglefirstVoiceLoad == false)
           // {
              //  VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DEBUG: setting voice]");
                VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex = 0;
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
            try
            {


                var audiobytes = Convert.FromBase64String(audioString);
                MemoryStream memoryStream = new MemoryStream(audiobytes);



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
                    audioFiles.writeAudioToOutputMp3(memoryStream3);
                }


                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                Mp3FileReader wav = new Mp3FileReader(memoryStream);

                memoryStream2.Flush();
                memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
                Mp3FileReader wav2 = new Mp3FileReader(memoryStream2);







                var volume = 5;
                int pitch = 5;
                int rate = 5;
                var volumeFloat = 1f;
                var pitchFloat = 1f;
                var rateFloat = 1f;

                volume = TTSMessageQueued.Volume;
                pitch = TTSMessageQueued.Pitch;
                rate = TTSMessageQueued.Speed;

                volumeFloat = 0.5f + volume * 0.1f;
                pitchFloat = 0.5f + pitch * 0.1f;
                rateFloat = 0.5f + rate * 0.1f;

                bool useTempo = false;
                if (rate != 5)//if rate is changed will use only rate, else use pitch which also changes rate.
                {
                    useTempo = true;
                    pitchFloat = rateFloat;
                }

                var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
                VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(useTempo, false));
                speedControl.PlaybackRate = pitchFloat;
                var AnyOutput = new WaveOut();
                AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                AnyOutput.Init(speedControl);
                AnyOutput.Play();
                ct.Register(async () => AnyOutput.Stop());

                WaveOut AnyOutput2 = new WaveOut();
                VarispeedSampleProvider speedControl_2 = null;
                WaveChannel32 wave32_2 = null;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
                {
                    wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                    wave32_2.PadWithZeroes = false;
                    speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                    speedControl_2.PlaybackRate = pitchFloat;//output 2
                                                             // AnyOutput2 = new WaveOut();
                    AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                    AnyOutput2.Init(speedControl_2);
                    AnyOutput2.Play();
                    ct.Register(async () => AnyOutput2.Stop());
                }

                ct.Register(async () => TTSMessageQueue.PlayNextInQueue());
                float delayTime = pitchFloat;
                if (rate != 5) { delayTime = rateFloat; }

                int delayInt = (int)Math.Ceiling((int)wave32.TotalTime.TotalMilliseconds / delayTime);
                Thread.Sleep(delayInt);
                //  Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds * 2);// VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
                Thread.Sleep(100);

                //   WaveFileWriter.CreateWaveFile(@"TextOut\file.wav", speedControl.ToWaveProvider());

                AnyOutput.Stop();
                AnyOutput.Dispose();
                //  AnyOutput = null;
                speedControl.Dispose();
                speedControl = null;
                wave32.Dispose();
                wave32 = null;
                wav.Dispose();
                wav = null;
                memoryStream.Dispose();
                //   synthesizerLite.Dispose();
                memoryStream = null;
                //    synthesizerLite = null;


                AnyOutput2.Stop();
                AnyOutput2.Dispose();
                //  AnyOutput2 = null;
                if (wave32_2 != null)
                {
                    speedControl_2.Dispose();
                    speedControl_2 = null;
                    wave32_2.Dispose();
                    wave32_2 = null;
                    wav2.Dispose();
                    wav2 = null;
                }
                if (!ct.IsCancellationRequested)
                {
                    TTSMessageQueue.PlayNextInQueue();
                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Google TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);

                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }
        }



    }
}