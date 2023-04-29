using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
//using CSCore;
//using CSCore.MediaFoundation;
using System.Media;
using System.Net;
//using CSCore.CoreAudioAPI;
using OSCVRCWiz.Text;
using Resources;
using NAudio.Wave;
using Microsoft.VisualBasic;
using Windows.Storage.Streams;
using System.Collections;
using Microsoft.Extensions.Primitives;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;
using System.Diagnostics;
using Amazon.Polly;
using Polly.Caching;
using OSCVRCWiz.Resources;
using System.Reflection.PortableExecutable;


namespace OSCVRCWiz.TTS
{
    public class NovelAITTS
    {
        // public static WaveOut TikTokOutput=null;
       

        public static async Task NovelAITextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);

           
           
            byte[] result = null;
            try
            {
                Debug.WriteLine("trying0");
                result = await CallNovelAIAPIAsync(TTSMessageQueued.text, TTSMessageQueued.Voice);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[NovelAI TTS Error: " + ex.Message + "]", Color.Red);
             

            }


            try
            {
                //  File.WriteAllBytes("TikTokTTS.mp3", result);          
                //  Task.Run(() => PlayAudioHelper());

                MemoryStream memoryStream = new MemoryStream(result);

             




                MemoryStream memoryStream2 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream2);


                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                StreamMediaFoundationReader wav = new StreamMediaFoundationReader(memoryStream);
                //Mp3FileReader mp3Reader = new Mp3FileReader(wav);


                memoryStream2.Flush();
                memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
                StreamMediaFoundationReader wav2 = new StreamMediaFoundationReader(memoryStream2);

        


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
                   // delayTime = rateFloat;
                }


                var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
                VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(useTempo, false));
                speedControl.PlaybackRate = pitchFloat;
                var AnyOutput = new WaveOut();
                AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                AnyOutput.Init(speedControl);
                AnyOutput.Play();
                ct.Register(async () => AnyOutput.Stop());

                WaveOut AnyOutput2 = null;
                VarispeedSampleProvider speedControl_2 = null;
                WaveChannel32 wave32_2 = null;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
                {
                    wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                    wave32_2.PadWithZeroes = false;
                    speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                    speedControl_2.PlaybackRate = pitchFloat;//output 2
                    AnyOutput2 = new WaveOut();
                    AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                    AnyOutput2.Init(speedControl_2);
                    AnyOutput2.Play();
                    ct.Register(async () => AnyOutput2.Stop());
                }


                //this is where i would save files
                //  WaveFileWriter.CreateWaveFile(@"TextOut\file.wav", speedControl.ToWaveProvider());

                ct.Register(async () => TTSMessageQueue.PlayNextInQueue());
             //   ct.Register(async () => Thread.Sleep(50));
              //  ct.Register(async () => ct = new());
                float delayTime = pitchFloat;
                if (rate != 5){ delayTime = rateFloat;}
                int delayInt = (int)Math.Ceiling((int)wave32.TotalTime.TotalMilliseconds / delayTime);
                Thread.Sleep(delayInt);
                // VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
                Thread.Sleep(100);
                


                AnyOutput.Stop();
                AnyOutput.Dispose();
                AnyOutput = null;
                speedControl.Dispose();
                speedControl = null;
                wave32.Dispose();
                wave32 = null;
                wav.Dispose();
                wav = null;
                memoryStream.Dispose();
               // synthesizerLite.Dispose();
                memoryStream = null;
               // synthesizerLite = null;

                if (AnyOutput2 != null)
                {
                    AnyOutput2.Stop();
                    AnyOutput2.Dispose();
                    AnyOutput2 = null;
                    speedControl_2.Dispose();
                    speedControl_2 = null;
                    wave32_2.Dispose();
                    wave32_2 = null;
                    wav2.Dispose();
                    wav2 = null;
                }


                if(!ct.IsCancellationRequested)
                {
                    TTSMessageQueue.PlayNextInQueue();
                }
                


               
             //   Debug.WriteLine("disposed of all");

               



            }
            catch (Exception ex)
            {
                OutputText.outputLog("[NovelAI TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
                TTSMessageQueue.PlayNextInQueue();
            }
            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallNovelAIAPIAsync(string text, string voice)
        {

           //    Debug.WriteLine("trying1");
             

          //     string fixText = text.Replace(" ", "%20");
             //  Debug.WriteLine(fixText);
             //  var base64String = "";
            // HttpClientHandler httpClientHandler = new HttpClientHandler();
            // httpClientHandler.UseCookies = false;
            //    var httpClient = new HttpClient(httpClientHandler);

            //   var httpClient = new HttpClient();
            //  httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36");

            //   httpClient.DefaultRequestHeaders.Referrer = new Uri($"https://api.novelai.net/ai/generate-voice?text={fixText}&seed=range_28_jumble&voice=0&opus=false&version=v2");
            // Set accept headers to indicate that the client can handle various content types
            //  httpClient.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            // Add other headers as needed
            // httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            //  httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

            /*   httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
               var response = await httpClient.GetAsync($"https://api.novelai.net/ai/generate-voice?text={fixText}&seed=range_28_jumble&voice=0&opus=false&version=v2");
                Debug.WriteLine("trying");
                if (response.IsSuccessStatusCode)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var memoryStream = new MemoryStream();
                    await contentStream.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    base64String = Convert.ToBase64String(bytes);
                    Debug.WriteLine("output: "+base64String);
                }
                else
                {
                    Debug.WriteLine($"Request failed with status code {response.StatusCode} {response.ReasonPhrase}");
                }*/
            // System.Diagnostics.Debug.WriteLine(audioInBase64);*/
            //string test = $"https://api.novelai.net/ai/generate-voice?text={fixText}&seed=range_28_jumble&voice=0&opus=false&version=v2";

          /*  Debug.WriteLine("wherestuck1");
            Debug.WriteLine("wherestuck2");
          //  IWebDriver driver = new FirefoxDriver();
            Debug.WriteLine("wherestuck3");
            driver.Navigate().GoToUrl(test);
            Debug.WriteLine("wherestuck4");
            // Find the audio element and get the source URL
            IWebElement audioElement = driver.FindElement(By.CssSelector("audio[src$='.mp3']"));
            string audioSrc = audioElement.GetAttribute("src");

            // Use JavaScript to fetch the audio file as a byte array
            string script = @"
                var xhr = new XMLHttpRequest();
                xhr.open('GET', arguments[0], false);
                xhr.responseType = 'arraybuffer';
                xhr.send();
                return new Uint8Array(xhr.response);
            ";
            byte[] mp3Bytes = (byte[])((IJavaScriptExecutor)driver).ExecuteScript(script, audioSrc);

            // Create a MemoryStream from the byte array
            MemoryStream ms = new MemoryStream(mp3Bytes);

            // Convert the MP3 file to a base64 string
            string base64String = Convert.ToBase64String(ms.ToArray());

            // Close the browser and the MemoryStream
          //  driver.Quit();
         //   ms.Close();
           // ms.Dispose();

            /* Debug.WriteLine(test);
         //  Uri url = new Uri(test);
             Process.Start(new ProcessStartInfo
             {
                 FileName = "C:\\Program Files\\Mozilla Firefox\\firefox.exe",
                 Arguments = "-new-window " + test,
                 WindowStyle = ProcessWindowStyle.Normal
             });*/




              var base64String = "";
            return Convert.FromBase64String(base64String);

        }
        public static string GetNovelAIVoice(string voice)
        {
            string apiName = "en_us_001";
            switch (voice)
            {
                case "English US Female": apiName = "en_us_001"; break;
                case "English US Male 1": apiName = "en_us_006"; break;
                case "English US Male 2": apiName = "en_us_007"; break;
                case "English US Male 3": apiName = "en_us_009"; break;
                case "English US Male 4": apiName = "en_us_010"; break;

                case "English UK Male 1": apiName = "en_uk_001"; break;
                case "English UK Male 2": apiName = "en_uk_003"; break;

                case "English AU Female": apiName = "en_au_001"; break;
                case "English AU Male": apiName = "en_au_002"; break;

                case "French Male 1": apiName = "fr_001"; break;
                case "French Male 2": apiName = "fr_002"; break;

                case "German Female": apiName = "de_001"; break;
                case "German Male": apiName = "de_002"; break;


                case "Spanish Male": apiName = "es_002"; break;
                case "Spanish MX Male": apiName = "es_mx_002"; break;


                case "Portuguese BR Female 1": apiName = "br_003"; break;
                case "Portuguese BR Female 2": apiName = "br_004"; break;
                case "Portuguese BR Male": apiName = "br_005"; break;

                case "Indonesian Female": apiName = "id_001"; break;
                case "Japanese Female 1": apiName = "jp_001"; break;
                case "Japanese Female 2": apiName = "jp_003"; break;
                case "Japanese Female 3": apiName = "jp_005"; break;
                case "Japanese Male": apiName = "jp_006"; break;

                case "Korean Male 1": apiName = "kr_002"; break;
                case "Korean Male 2": apiName = "kr_004"; break;

                case "Korean Female": apiName = "kr_003"; break;



                case "Ghostface (Scream)": apiName = "en_us_ghostface"; break;
                case "Chewbacca (Star Wars)": apiName = "en_us_chewbacca"; break;
                case "C3PO (Star Wars)": apiName = "en_us_c3po"; break;
                case "Stitch (Lilo & Stitch)": apiName = "en_us_stitch"; break;
                case "Stormtrooper (Star Wars)": apiName = "en_us_stormtrooper"; break;
                case "Rocket (Guardians of the Galaxy)": apiName = "en_us_rocket"; break;

                case "Alto": apiName = "en_female_f08_salut_damour"; break;
                case "Tenor": apiName = "en_male_m03_lobby"; break;
                case "Sunshine Soon": apiName = "en_male_m03_sunshine_soon"; break;
                case "Warmy Breeze": apiName = "en_female_f08_warmy_breeze"; break;
                case "Glorious": apiName = "en_female_ht_f08_glorious"; break;
                case "It Goes Up": apiName = "en_male_sing_funny_it_goes_up"; break;
                case "Chipmunk": apiName = "en_male_m2_xhxs_m03_silly"; break;
                case "Dramatic": apiName = "en_female_ht_f08_wonderful_world"; break;
                case "Funny": apiName = "en_male_funny"; break;
                case "Emotional": apiName = "en_female_emotional"; break; //added back
                case "Narrator": apiName = "en_male_narration"; break;


                default: break;
            }
            return apiName;
        }


        
    }



}
