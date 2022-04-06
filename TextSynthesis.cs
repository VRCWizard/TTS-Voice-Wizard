using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace OSCVRCWiz
{
    public class TextSynthesis
    {
        public static async void speechTTTS(VoiceWizardWindow MainForm)//speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {
                var speechConfig = SpeechConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);


                speechConfig.SpeechRecognitionLanguage = "en-US";

                if (MainForm.profanityFilter == false)
                {
                    speechConfig.SetProfanity(ProfanityOption.Raw);
                }
                if (MainForm.profanityFilter == true)
                {
                    speechConfig.SetProfanity(ProfanityOption.Masked);
                }

                //To recognize speech from an audio file, use `FromWavFileInput` instead of `FromDefaultMicrophoneInput`:
                //using var audioConfig = AudioConfig.FromWavFileInput("YourAudioFile.wav");
                var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                var speechRecognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);

                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                //OutputSpeechRecognitionResult(speechRecognitionResult);


                MainForm.dictationString = speechRecognitionResult.Text; //Dictation string
                string emotion = "Normal";
                string rate = "default";
                string pitch = "default";
                string volume = "default";
                string voice = "Sara";
                MainForm.Invoke((MethodInvoker)delegate ()
                {
                    if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString()))
                    {
                        emotion = "Normal";

                    }
                    else
                    {
                        emotion = MainForm.comboBox1.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString()))
                    {
                        rate = "default";

                    }
                    else
                    {
                        rate = MainForm.comboBoxRate.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString()))
                    {
                        pitch = "default";

                    }
                    else
                    {
                        pitch = MainForm.comboBoxPitch.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString()))
                    {
                        volume = "default";

                    }
                    else
                    {
                        volume = MainForm.comboBoxVolume.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString()))
                    {
                        voice = "Sara";

                    }
                    else
                    {
                        voice = MainForm.comboBox2.Text.ToString();
                    }


                });
                var ot = new OutputText();
                if (MainForm.checkBox2.Checked == true)
                {
                    ot.outputLog(MainForm, MainForm.dictationString);
                }
                //Send Text to Vrchat
                if (MainForm.checkBox1.Checked == true)
                {
                    Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString));
                }
                //Send Text to TTS

                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm.dictationString, emotion, rate, pitch, volume, voice));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No valid subscription key given or speech service has been disabled");

            }
        }
        public static async void translationSTTTS(VoiceWizardWindow MainForm,string languageFullname)//translate speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
         //   try
         //   {
                var translationConfig = SpeechTranslationConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);


                var fromLanguage = "en-US";
                var toLanguage = "en";


                switch (languageFullname)
                {
                    case "Arabic [ar]": toLanguage = "ar"; break;
                    case "Chinese [zh]": toLanguage = "zh-Hans"; break;
                    case "Danish [da]": toLanguage = "da"; break;
                    case "Dutch [nl]": toLanguage = "nl"; break;
                    case "Filipino [fil]": toLanguage = "fil"; break;
                    case "Finnish [fi]": toLanguage = "fi"; break;
                    case "French [fr]": toLanguage = "fr"; break;
                    case "German [de]": toLanguage = "de"; break;
                    case "Hendi [hi]": toLanguage = "hi"; break;
                    case "Irish [ga]": toLanguage = "ga"; break;
                    case "Italian [it]": toLanguage = "it"; break;
                    case "Japanese [ja]": toLanguage = "ja"; break;
                    case "Korean [ko]": toLanguage = "ko"; break;
                    case "Norwegian [nb]": toLanguage = "nb"; break;
                    case "Polish [pl]": toLanguage = "pl"; break;
                    case "Portuguese [pt]": toLanguage = "pt"; break;
                    case "Russian [ru]": toLanguage = "ru"; break;
                    case "Spanish [es]": toLanguage = "es"; break;
                    case "Swedish [sv]": toLanguage = "sv"; break;
                    case "Thai [th]": toLanguage = "th"; break;
                    case "Ukrainian [uk]": toLanguage = "uk"; break;
                    case "Vietnamese [vi]": toLanguage = "vi"; break;
                    default: toLanguage = "en"; break; // if translation to english happens something is wrong
                }




                translationConfig.SpeechRecognitionLanguage = fromLanguage;
                translationConfig.AddTargetLanguage(toLanguage);

                if (MainForm.profanityFilter == false)
                {
                    translationConfig.SetProfanity(ProfanityOption.Raw);
                }
                if (MainForm.profanityFilter == true)
                {
                    translationConfig.SetProfanity(ProfanityOption.Masked);
                }

                //To recognize speech from an audio file, use `FromWavFileInput` instead of `FromDefaultMicrophoneInput`:
                //using var audioConfig = AudioConfig.FromWavFileInput("YourAudioFile.wav");
                var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                using var translationRecognizer = new TranslationRecognizer(translationConfig, audioConfig);



                //OutputSpeechRecognitionResult(speechRecognitionResult);

                System.Diagnostics.Debug.WriteLine($"Say something in '{fromLanguage}' and ");
                System.Diagnostics.Debug.WriteLine($"we'll translate into '{toLanguage}'.\n");

                var speechRecognitionResult = await translationRecognizer.RecognizeOnceAsync();
                if (speechRecognitionResult.Reason == ResultReason.TranslatedSpeech)
                {
                    System.Diagnostics.Debug.WriteLine($"Recognized: \"{speechRecognitionResult.Text}\"");
                    System.Diagnostics.Debug.WriteLine($"Translated into '{toLanguage}': {speechRecognitionResult.Translations[toLanguage]}");
                }

                MainForm.dictationString = speechRecognitionResult.Text; //Dictation string
                string translatedString = speechRecognitionResult.Translations[toLanguage]; //Dictation string
                string emotion = "Normal";
                string rate = "default";
                string pitch = "default";
                string volume = "default";
                string voice = "Sara";
                MainForm.Invoke((MethodInvoker)delegate ()
                {
                    if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString()))
                    {
                        emotion = "Normal";

                    }
                    else
                    {
                        emotion = MainForm.comboBox1.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString()))
                    {
                        rate = "default";

                    }
                    else
                    {
                        rate = MainForm.comboBoxRate.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString()))
                    {
                        pitch = "default";

                    }
                    else
                    {
                        pitch = MainForm.comboBoxPitch.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString()))
                    {
                        volume = "default";

                    }
                    else
                    {
                        volume = MainForm.comboBoxVolume.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString()))
                    {
                        voice = "Sara";

                    }
                    else
                    {
                        voice = MainForm.comboBox2.Text.ToString();
                    }


                });
                var ot = new OutputText();
                if (MainForm.checkBox2.Checked == true)
                {
                    ot.outputLog(MainForm, MainForm.dictationString + " [" + toLanguage + "]: " + "[" + translatedString + "]");
                }
                //Send Text to Vrchat
                if (MainForm.checkBox1.Checked == true)
                {
                    Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString + "[" + toLanguage + "]"));
                }
                //Send Text to TTS

                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(translatedString, emotion, rate, pitch, volume, voice));
           // }
          //  catch (Exception ex)
         //   {
            //    MessageBox.Show("No valid subscription key given or speech service has been disabled");
//
         //   }
        }
    }
}
