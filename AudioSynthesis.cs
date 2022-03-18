using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz
{
    public class AudioSynthesis
    {
      /*  public static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult) //STT
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    System.Diagnostics.Debug.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                    break;
                case ResultReason.NoMatch:
                    System.Diagnostics.Debug.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    System.Diagnostics.Debug.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        System.Diagnostics.Debug.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        System.Diagnostics.Debug.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        System.Diagnostics.Debug.WriteLine($"CANCELED: Double check the speech resource key and region.");
                    }
                    break;
            }
        } */
        //TTS
        public static async Task SynthesizeAudioAsync(string text, string style, string rate, string pitch, string volume, string voice, int outputIndex) //TTS Outputs through speakers //can not change voice style
        {
            try
            {
                var config = SpeechConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
                // Note: if only language is set, the default voice of that language is chosen.
                //  config.SpeechSynthesisLanguage = "<your-synthesis-language>"; // For example, "de-DE"
                // The voice setting will overwrite the language setting.
                // The voice setting will not overwrite the voice element in input SSML.
                //config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                // config.SpeechSynthesisVoiceName = "en-US-SaraNeural";


                // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#speaker-recognition
                string angry = "<mstts:express-as style=\"angry\">"; //1
                string happy = "<mstts:express-as style=\"cheerful\">"; //2
                string sad = "<mstts:express-as style=\"sad\">";//3

                string chat = "<mstts:express-as style=\"chat\">"; //1
                string customerservice = "<mstts:express-as style=\"customerservice\">"; //2
                string empathetic = "<mstts:express-as style=\"empathetic\">";//3
                string narrationProf = "<mstts:express-as style=\"narration-professional\">"; //1
                string newscastCas = "<mstts:express-as style=\"newscast-casual\">"; //2
                string newscastForm = "<mstts:express-as style=\"newscast-formal\">";//3
                string newscast = "<mstts:express-as style=\"newscast\">"; //1
                string assistant = "<mstts:express-as style=\"assistant\">"; //2


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


                string sara = "<voice name=\"en-US-SaraNeural\">";//1
                string jenny = "<voice name=\"en-US-JennyNeural\">";//2
                string guy = "<voice name=\"en-US-GuyNeural\">";//3
                string amber = "<voice name=\"en-US-AmberNeural\">";//4
                string ana = "<voice name=\"en-US-AnaNeural\">";//5
                string aria = "<voice name=\"en-US-AriaNeural\">";//6
                string ashley = "<voice name=\"en-US-AshleyNeural\">";//7
                string brandon = "<voice name=\"en-US-BrandonNeural\">";//8
                string christopher = "<voice name=\"en-US-ChristopherNeural\">";//9
                string cora = "<voice name=\"en-US-CoraNeural\">";//10
                string elizabeth = "<voice name=\"en-US-ElizabethNeural\">";//11
                string eric = "<voice name=\"en-US-EricNeural\">";//12
                string jacob = "<voice name=\"en-US-JacobNeural\">";//13
                string michelle = "<voice name=\"en-US-MichelleNeural\">";//14
                string monica = "<voice name=\"en-US-MonicaNeural\">";//15
                string davis = "<voice name=\"en-US-DavisNeural\">";//16


                System.Diagnostics.Debug.WriteLine(rate);
                System.Diagnostics.Debug.WriteLine(pitch);
                System.Diagnostics.Debug.WriteLine(volume);








                var synthesizer = new Microsoft.CognitiveServices.Speech.SpeechSynthesizer(config);

                string ssml0 = "<speak version=\"1.0\"";
                ssml0 += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
                if (style != "Normal") { ssml0 += " xmlns:mstts=\"https://www.w3.org/2001/mstts\""; }
                ssml0 += " xml:lang=\"en-US\">";

                switch (voice)
                {
                    case "Sara": ssml0 += sara; break;
                    case "Jenny": ssml0 += jenny; ; break;
                    case "Guy": ssml0 += guy; ; break;
                    case "Amber": ssml0 += amber; ; break;
                    case "Ana": ssml0 += ana; ; break;
                    case "Aria": ssml0 += aria; ; break;
                    case "Ashley": ssml0 += ashley; ; break;
                    case "Brandon": ssml0 += brandon; ; break;
                    case "Christopher": ssml0 += christopher; ; break;
                    case "Cora": ssml0 += cora; ; break;
                    case "Elizabeth": ssml0 += elizabeth; ; break;
                    case "Eric": ssml0 += eric; ; break;
                    case "Jacob": ssml0 += jacob; ; break;
                    case "Michelle": ssml0 += michelle; ; break;
                    case "Monica": ssml0 += monica; ; break;
                    case "Davis": ssml0 += davis; ; break;


                    default: ssml0 += "<voice name=\"en-US-SaraNeural\">"; break;
                }

                if (style != "Normal")
                {

                    if (style == "Angry") { ssml0 += angry; }
                    if (style == "Happy") { ssml0 += happy; }
                    if (style == "Sad") { ssml0 += sad; }
                    if (style == "Chat") { ssml0 += chat; }
                    if (style == "Customer Service") { ssml0 += customerservice; }
                    if (style == "Empathetic") { ssml0 += empathetic; }
                    if (style == "Narration (Professional)") { ssml0 += narrationProf; }
                    if (style == "Newscast (Casual)") { ssml0 += newscastCas; }
                    if (style == "Newscast (Formal)") { ssml0 += newscastForm; }
                    if (style == "Newscast") { ssml0 += newscast; }
                    if (style == "Assistant") { ssml0 += assistant; }

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

                if (style != "Normal") { ssml0 += "</mstts:express-as>"; }
                ssml0 += "</voice>";
                ssml0 += "</speak>";

                System.Diagnostics.Debug.WriteLine(ssml0);
                await synthesizer.SpeakSsmlAsync(ssml0);
                //using var result = await synthesizer.SpeakSsmlAsync(ssml0);

            }
            catch (Exception ex)
            {
                MessageBox.Show("No valid subscription key given or speech service has been disabled");

            }



        }
    }
}
