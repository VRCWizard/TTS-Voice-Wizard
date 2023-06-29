using Newtonsoft.Json.Linq;
//using CSCore;
//using CSCore.MediaFoundation;
using System.Net;
//using CSCore.CoreAudioAPI;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class TikTokTTS
    {
        // public static WaveOut TikTokOutput=null;

        public static async Task TikTokTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);



            byte[] result = null;
            try
            {
                result = await CallTikTokAPIAsync(TTSMessageQueued.text, TTSMessageQueued.Voice);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TikTok TTS Error: " + ex.Message + "]", Color.Red);


            }



            //  File.WriteAllBytes("TikTokTTS.mp3", result);          
            //  Task.Run(() => PlayAudioHelper());

            MemoryStream memoryStream = new MemoryStream(result);



            //   AudioDevices.playMp3Stream(memoryStream, TTSMessageQueued, ct);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();






            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {


            var url = "https://tiktok-tts.weilnet.workers.dev/api/generation";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            var apiVoice = GetTikTokVoice(voice);

            var data = "{\"text\":\"" + text + "\",\"voice\":\"" + apiVoice + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string audioInBase64 = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                audioInBase64 = dataHere.ToString();

                System.Diagnostics.Debug.WriteLine(result);
            }

            System.Diagnostics.Debug.WriteLine(httpResponse.StatusCode);



            System.Diagnostics.Debug.WriteLine(audioInBase64);
            return Convert.FromBase64String(audioInBase64);

        }
        public static string GetTikTokVoice(string voice)
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

        public static void SetVoices(ComboBox voices, ComboBox styles)
        {
            voices.Items.Clear();

            var tiktokVoices = new List<string>()
                    {
             "English US Female",
             "English US Male 1",
              "English US Male 2",
              "English US Male 3",
              "English US Male 4",
             "English UK Male 1",
              "English UK Male 2",
               "English AU Female",
               "English AU Male",
                "French Male 1",
               "French Male 2",
               "German Female",
               "German Male",
               "Spanish Male",
             "Spanish MX Male",
                "Portuguese BR Female 1",
               "Portuguese BR Female 2",
                 "Portuguese BR Male",
                "Indonesian Female",
                "Japanese Female 1",
                 "Japanese Female 2",
                "Japanese Female 3",
                "Japanese Male",
                "Korean Male 1",
               "Korean Male 2",
                 "Korean Female",
                 "Ghostface (Scream)",
                "Chewbacca (Star Wars)",
               "C3PO (Star Wars)",
              "Stitch (Lilo & Stitch)",
              "Stormtrooper (Star Wars)",
             "Rocket (Guardians of the Galaxy)",
              "Alto",
               "Tenor",
              "Sunshine Soon",
            "Warmy Breeze",
             "Glorious",
            "It Goes Up",
             "Chipmunk",
             "Dramatic",
            "Funny",
            "Emotional",
            "Narrator",
                    };
            foreach (var voice in tiktokVoices)
            {
                voices.Items.Add(voice);
            }

            voices.SelectedIndex = 0;

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;

            styles.Enabled = false;
            voices.Enabled = true;

        }



        }



}
