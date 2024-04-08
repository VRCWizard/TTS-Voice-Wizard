using System.Net;
using System.Web;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;
using Windows.Media.Protection.PlayReady;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class GladosTTS
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task GladosTextAsSpeech(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {

            byte[] result = null;
            try
            {
                result = await CallGladosAPIAsync(TTSMessageQueued.text);

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Locally Hosted TTS Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.ToString() == "No connection could be made because the target machine actively refused it. [::ffff:127.0.0.1]:8124 (127.0.0.1:8124)")
                {
                    OutputText.outputLog("[You did not setup The locally hostd option follow the instructions on the wiki here: https://ttsvoicewizard.com/docs/TTSMethods/LocallyHosted ]", Color.DarkOrange);
                }

                Task.Run(() => TTSMessageQueue.PlayNextInQueue());

            }


            MemoryStream memoryStream = new MemoryStream(result);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
            memoryStream.Dispose();

        }

        public static async Task<byte[]> CallGladosAPIAsync(string text)
        {


            /*  var url = "http://127.0.0.1:8124/synthesize/";

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

                          System.Diagnostics.Debug.WriteLine(result.ToString());
                          audioInBase64 = result.ToString();

                      }

                  }
              }
              return Convert.FromBase64String(audioInBase64);*/

            string url = "http://127.0.0.1:8124/synthesize/";
           // string textParam = HttpUtility.UrlEncode(text);
            string requestUrl = $"{url}?{text}";

            var response = await client.GetAsync(requestUrl).ConfigureAwait(false);
            

            string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
          // System.Diagnostics.Debug.WriteLine(result);

            return Convert.FromBase64String(result);

        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            accents.Items.Add("default");
            accents.SelectedIndex = 0;

            voices.Items.Clear();
            voices.Items.Add("Local 1");
            voices.SelectedIndex = 0;

            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;

            styles.Enabled = false;
            voices.Enabled = true;

        }





    }
}
