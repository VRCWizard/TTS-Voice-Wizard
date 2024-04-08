using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class IBMWatsonTTS
    {

        public static async void WatsonPlayAudio(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            var audiobytes = Convert.FromBase64String(audioString);
            MemoryStream memoryStream = new MemoryStream(audiobytes);
            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Mp3);
            memoryStream.Dispose();
        }

        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();

            var voiceAccents = new List<string>()
                    {

                        "Dutch [nl]",
                        "English [en]",
                        "French [fr]",
                        "German [de]",
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Portuguese [pt]",
                        "Spanish [es]",
                    };
            foreach (var accent in voiceAccents)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedItem = "English [en]";


            SynthesisGetAvailableVoicesAsync(voices, accents.Text.ToString());
            // comboBox2.SelectedIndex = 0;
            styles.SelectedIndex = 0;
            styles.Enabled = true;
            voices.Enabled = true;
        }



        public static async Task SynthesisGetAvailableVoicesAsync(ComboBox voices, string fromLanguageFullname)
        {

            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();

            switch (fromLanguageFullname)
            {

                case "Dutch [nl]":

                    voices.Items.Add("nl-NL_MerelV3Voice");
                    break;
                case "English [en]":



                    voices.Items.Add("en-US_AllisonV3Voice");
                    voices.Items.Add("en-US_EmilyV3Voice");
                    voices.Items.Add("en-US_HenryV3Voice");
                    voices.Items.Add("en-US_KevinV3Voice");
                    voices.Items.Add("en-US_LisaV3Voice");
                    voices.Items.Add("en-US_MichaelV3Voice");
                    voices.Items.Add("en-US_OliviaV3Voice");

                    voices.Items.Add("en-US_AllisonExpressive");
                    voices.Items.Add("en-US_EmmaExpressive");
                    voices.Items.Add("en-US_LisaExpressive");
                    voices.Items.Add("en-US_MichaelExpressive");

                    voices.Items.Add("en-GB_CharlotteV3Voice");
                    voices.Items.Add("en-GB_JamesV3Voice");
                    voices.Items.Add("en-GB_KateV3Voice");

                    voices.Items.Add("en-AU_HeidiExpressive");
                    voices.Items.Add("en-AU_JackExpressive");

                    break;

                case "French [fr]":
                    voices.Items.Add("fr-CA_LouiseV3Voice");
                    voices.Items.Add("fr-FR_NicolasV3Voice");
                    voices.Items.Add("fr-FR_ReneeV3Voice");

                    break;
                case "German [de]":
                    voices.Items.Add("de-DE_BirgitV3Voice");
                    voices.Items.Add("de-DE_DieterV3Voice");
                    voices.Items.Add("de-DE_ErikaV3Voice");


                    break;


                case "Italian [it]":
                    voices.Items.Add("it-IT_FrancescaV3Voice");
                    break;

                case "Japanese [ja]":

                    voices.Items.Add("ja-JP_EmiV3Voice");
                    break;

                case "Korean [ko]":
                    voices.Items.Add("ko-KR_JinV3Voice");
                    break;

                case "Portuguese [pt]":
                    voices.Items.Add("pt-BR_IsabelaV3Voice");
                    break;

                case "Spanish [es]":

                    voices.Items.Add("es-ES_EnriqueV3Voice");
                    voices.Items.Add("es-ES_LauraV3Voice");
                    voices.Items.Add("es-LA_SofiaV3Voice");
                    voices.Items.Add("es-US_SofiaV3Voice");
                    break;

                default:
                    voices.Items.Add("error"); break; // if translation to english happens something is wrong

            }

            voices.SelectedIndex = 0;
        }

    }
}
