using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class DeepgramAuraTTS
    {



        public static async void AuraPlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
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

            var voiceAccents = new List<string>()
                    {

                        "English (US)",
                        "English (UK)",
                        "English (Ireland)",
                    };
            foreach (var accent in voiceAccents)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedItem = "English (US)";


          SynthesisGetAvailableVoicesAsync(voices, accents.Text.ToString());
            // comboBox2.SelectedIndex = 0;
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }



        public static async Task SynthesisGetAvailableVoicesAsync(ComboBox voices, string fromLanguageFullname)
        {

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();

                switch (fromLanguageFullname)
                {
                    case "English (US)":
                    voices.Items.Add("aura-asteria-en");
                    voices.Items.Add("aura-hera-en");
                    voices.Items.Add("aura-luna-en");
                    voices.Items.Add("aura-stella-en");
                    voices.Items.Add("aura-zeus-en");
                    voices.Items.Add("aura-orion-en");
                    voices.Items.Add("aura-arcas-en");
                    voices.Items.Add("aura-perseus-en");
                    voices.Items.Add("aura-orpheus-en");
                    break;

                    case "English (UK)":
                    voices.Items.Add("aura-athena-en");
                    voices.Items.Add("aura-helios-en");
                    break;

                    case "English (Ireland)":
                    voices.Items.Add("aura-angus-en");
                    break;



                default:
                    voices.Items.Add("error"); break; // if translation to english happens something is wrong
            


                }



            voices.SelectedIndex = 0;
         

        }





    }
}
