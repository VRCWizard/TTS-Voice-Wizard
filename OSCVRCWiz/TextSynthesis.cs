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
        public static async void speechTTTS(VoiceWizardWindow MainForm, string fromLanguageFullname)//speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {
                var speechConfig = SpeechConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
                var fromLanguage = "en-US";
                switch (fromLanguageFullname)
                {
                    case "Arabic [ar-EG]": fromLanguage = "ar-EG"; break;
                    case "Chinese [zh-CN]": fromLanguage = "zh-CN"; break;
                    case "Danish [da-DK]": fromLanguage = "da-DK"; break;
                    case "Dutch [nl-NL]": fromLanguage = "nl-NL"; break;
                    case "English [en-US] (Default)": fromLanguage = "en-US"; break;
                    case "Filipino [fil-PH]": fromLanguage = "fil-PH"; break;
                    case "Finnish [fi-FI]": fromLanguage = "fi-FI"; break;
                    case "French [fr-FR]": fromLanguage = "fr-FR"; break;
                    case "German [de-DE]": fromLanguage = "de-DE"; break;
                    case "Hendi [hi-IN]": fromLanguage = "hi-IN"; break;
                    case "Irish [ga-IE]": fromLanguage = "ga-IE"; break;
                    case "Italian [it-IT]": fromLanguage = "it-IT"; break;
                    case "Japanese [ja-JP]": fromLanguage = "ja-JP"; break;
                    case "Korean [ko-KR]": fromLanguage = "ko-KR"; break;
                    case "Norwegian [nb-NO]": fromLanguage = "nb-NO"; break;
                    case "Polish [pl-PL]": fromLanguage = "pl-PL"; break;
                    case "Portuguese [pt-BR]": fromLanguage = "pt-BR"; break;
                    //place holder^^
                    case "Russian [ru-RU]": fromLanguage = "ru-RU"; break;
                    case "Spanish [es-MX]": fromLanguage = "es-MX"; break;
                    //place holder^^
                    case "Swedish [sv-SE]": fromLanguage = "sv-SE"; break;
                    case "Thai [th-TH]": fromLanguage = "th-TH"; break;
                    case "Ukrainian [uk-UA]": fromLanguage = "uk-UA"; break;
                    case "Vietnamese [vi-VN]": fromLanguage = "vi-VN"; break;
                    default: fromLanguage = "en-US"; break; // if translation to english happens something is wrong
                }

                speechConfig.SpeechRecognitionLanguage = fromLanguage ;

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

                ///Phrase List
                var phraseList = PhraseListGrammar.FromRecognizer(speechRecognizer);
                if ( MainForm.rjTogglePhraseList.Checked == true)
                {
                    string words = MainForm.richTextBox6.Text.ToString();

                    string[] split = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in split)
                    {

                        if (s.Trim() != "")
                            phraseList.AddPhrase(s);
                        System.Diagnostics.Debug.WriteLine(s);

                    }
             
                    

                }
                if (MainForm.rjTogglePhraseList.Checked == false)
                {
                    phraseList.Clear();
                }




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
                if (MainForm.rjToggleButton2.Checked == true)
                {
                    ot.outputLog(MainForm, MainForm.dictationString);
                }
                if (MainForm.rjToggleButtonDisableTTS.Checked == false)
                {
                    Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm.dictationString, emotion, rate, pitch, volume, voice));
                }
                //Send Text to Vrchat
                if (MainForm.rjToggleButton4.Checked == true)
                {
                    //ot.outputVRChat(MainForm, MainForm.dictationString);
                    Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString));
                }
                //Send Text to TTS

               // Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm.dictationString, emotion, rate, pitch, volume, voice));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No valid subscription key given or speech service has been disabled");

            }
        }
        public static async void translationSTTTS(VoiceWizardWindow MainForm,string toLanguageFullname, string fromLanguageFullname)//translate speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
         //   try
         //   {
                var translationConfig = SpeechTranslationConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);


                var fromLanguage = "en-US";
                var toLanguage = "en";

            switch (fromLanguageFullname)
            {
                case "Arabic [ar-EG]": fromLanguage = "ar-EG"; break;
                case "Chinese [zh-CN]": fromLanguage = "zh-CN"; break;
                case "Danish [da-DK]": fromLanguage = "da-DK"; break;
                case "Dutch [nl-NL]": fromLanguage = "nl-NL"; break;
                case "English [en-US] (Default)": fromLanguage = "en-US"; break;
                case "Filipino [fil-PH]": fromLanguage = "fil-PH"; break;
                case "Finnish [fi-FI]": fromLanguage = "fi-FI"; break;
                case "French [fr-FR]": fromLanguage = "fr-FR"; break;
                case "German [de-DE]": fromLanguage = "de-DE"; break;
                case "Hendi [hi-IN]": fromLanguage = "hi-IN"; break;
                case "Irish [ga-IE]": fromLanguage = "ga-IE"; break;
                case "Italian [it-IT]": fromLanguage = "it-IT"; break;
                case "Japanese [ja-JP]": fromLanguage = "ja-JP"; break;
                case "Korean [ko-KR]": fromLanguage = "ko-KR"; break;
                case "Norwegian [nb-NO]": fromLanguage = "nb-NO"; break;
                case "Polish [pl-PL]": fromLanguage = "pl-PL"; break;
                case "Portuguese [pt-BR]": fromLanguage = "pt-BR"; break;
                    //place holder^^
                case "Russian [ru-RU]": fromLanguage = "ru-RU"; break;
                case "Spanish [es-MX]": fromLanguage = "es-MX"; break;
                //place holder^^
                case "Swedish [sv-SE]": fromLanguage = "sv-SE"; break;
                case "Thai [th-TH]": fromLanguage = "th-TH"; break;
                case "Ukrainian [uk-UA]": fromLanguage = "uk-UA"; break;
                case "Vietnamese [vi-VN]": fromLanguage = "vi-VN"; break;
                default: fromLanguage = "en-US"; break; // if translation to english happens something is wrong
            }


            switch (toLanguageFullname)
                {
                    case "Arabic [ar]": toLanguage = "ar"; break;
                    case "Chinese [zh]": toLanguage = "zh-Hans"; break;
                    case "Danish [da]": toLanguage = "da"; break;
                    case "Dutch [nl]": toLanguage = "nl"; break;
                    case "English [en]": toLanguage = "en"; break;
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

            // Phrase list
            var phraseList = PhraseListGrammar.FromRecognizer(translationRecognizer);
            if (MainForm.rjTogglePhraseList.Checked == true)
            {
                string words = MainForm.richTextBox6.Text.ToString();

                string[] split = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string s in split)
                {

                    if (s.Trim() != "")
                        phraseList.AddPhrase(s);
                    System.Diagnostics.Debug.WriteLine(s);

                }



            }
            if (MainForm.rjTogglePhraseList.Checked == false)
            {
                phraseList.Clear();
            }



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
                if (MainForm.rjToggleButton2.Checked == true)
                {
                 ot.outputLog(MainForm, MainForm.dictationString + " [" + fromLanguage+ ">"+ toLanguage + "]: " + "[" + translatedString + "]");
                }
            //Send Text to TTS
            if (MainForm.rjToggleButtonDisableTTS.Checked == false)
            {
                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(translatedString, emotion, rate, pitch, volume, voice));

            }
            
                //Send Text to Vrchat
                if (MainForm.rjToggleButton4.Checked == true)
                {
                    if (MainForm.rjToggleButtonTextAsTranslated.Checked == true) //changed from checkbox7
                    {
                    Task.Run(() => ot.outputVRChat(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]"));

                    }
                    else
                    {
                    Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]"));

                    }
                
                
                // Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString + "[" + toLanguage + "]"));
            }
                //Send Text to TTS

               // Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(translatedString, emotion, rate, pitch, volume, voice));
           // }
          //  catch (Exception ex)
         //   {
            //    MessageBox.Show("No valid subscription key given or speech service has been disabled");
//
         //   }
        }
    }
}
