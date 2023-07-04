using System.Net;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class GladosTTS
    {
        public static async Task GladosTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
            byte[] result = null;
            try
            {
                result = await CallGladosAPIAsync(TTSMessageQueued.text);

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Glados TTS Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.ToString() == "No connection could be made because the target machine actively refused it. [::ffff:127.0.0.1]:8124 (127.0.0.1:8124)")
                {
                    OutputText.outputLog("[You did not setup Glados TTS follow the instructions on the wiki here: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS]", Color.DarkOrange);
                }

                Task.Run(() => TTSMessageQueue.PlayNextInQueue());

            }
            //  File.WriteAllBytes("TikTokTTS.mp3", result);          
            //  Task.Run(() => PlayAudioHelper());

            MemoryStream memoryStream = new MemoryStream(result);

            //  AudioDevices.playWaveStream(memoryStream, TTSMessageQueued, ct);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
            memoryStream.Dispose();




            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallGladosAPIAsync(string text)
        {


            var url = "http://127.0.0.1:8124/synthesize/";

            string audioInBase64 = "";
            WebRequest request = WebRequest.Create("http://127.0.0.1:8124/synthesize/" + "?" + text);
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {

                using (Stream stream = response.GetResponseStream())
                {

                    using (var streamReader = new StreamReader(stream))
                    {
                        var result = streamReader.ReadToEnd();
                        // var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                        // audioInBase64 = dataHere.ToString();

                        System.Diagnostics.Debug.WriteLine(result.ToString());
                        audioInBase64 = result.ToString();

                    }

                }
            }
            // System.Diagnostics.Debug.WriteLine(audioInBase64);
            return Convert.FromBase64String(audioInBase64);

        }





    }
}
