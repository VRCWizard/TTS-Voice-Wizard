using System;
using System.Collections.Generic;
using System.Text;
using DeepL;
using OSCVRCWiz;

namespace DeepL_Translation
{
    public class DeepLC
    {
        private static string fromLanguage = "en-US";
        private static string toLanguage = "en-US";
        public static string DeepLTranslationText="";

        public async void translateTextDeepL(string text)
        {
            var authKey = ""; // Replace with your key
            var translator = new Translator(authKey);
            var fullFromLanguage = "";
            var fullToLanguage = "";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
               fullFromLanguage = VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedText;
               fullToLanguage = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedText;
            });

            fromLanguageIDDeepL(fullFromLanguage);
            toLanguageIDDeepL(fullToLanguage);

            var translatedText = await translator.TranslateTextAsync(text, LanguageCode.English, LanguageCode.French);
          System.Diagnostics.Debug.WriteLine(translatedText);
            System.Diagnostics.Debug.WriteLine(LanguageCode.English);
            System.Diagnostics.Debug.WriteLine(LanguageCode.French);

            DeepLTranslationText = translatedText.ToString();
        }
        private void fromLanguageIDDeepL(string fullname)
        {
            fromLanguage = "en-US";
            switch (fullname)
            {
                case "Arabic [ar-EG]": fromLanguage = "ar-EG"; break;
                case "Chinese [zh-CN]": fromLanguage = "zh-CN"; break;
                case "Czech [cs-CZ]": fromLanguage = "cs-CZ"; break;
                case "Danish [da-DK]": fromLanguage = "da-DK"; break;
                case "Dutch [nl-NL]": fromLanguage = "nl-NL"; break;
                case "English [en-US] (Default)": fromLanguage = "en-US"; break;
                case "Estonian [et-EE]": fromLanguage = "et-EE"; break;
                case "Filipino [fil-PH]": fromLanguage = "fil-PH"; break;
                case "Finnish [fi-FI]": fromLanguage = "fi-FI"; break;
                case "French [fr-FR]": fromLanguage = "fr-FR"; break;
                case "German [de-DE]": fromLanguage = "de-DE"; break;
                case "Hindi [hi-IN]": fromLanguage = "hi-IN"; break;
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
            //  return fromLanguage;

        }

        private void toLanguageIDDeepL(string fullname)
        {
            toLanguage = "en-US";

            switch (fullname)
            {
                case "Arabic [ar]": toLanguage = "ar"; break;
                case "Chinese [zh]": toLanguage = "zh-Hans"; break;
                case "Czech [cs]": toLanguage = "cs"; break;
                case "Danish [da]": toLanguage = "da"; break;
                case "Dutch [nl]": toLanguage = "nl"; break;
                case "English [en]": toLanguage = "en"; break;
                case "Estonian [et]": toLanguage = "et"; break;
                case "Filipino [fil]": toLanguage = "fil"; break;
                case "Finnish [fi]": toLanguage = "fi"; break;
                case "French [fr]": toLanguage = "fr"; break;
                case "German [de]": toLanguage = "de"; break;
                case "Hindi [hi]": toLanguage = "hi"; break;
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
                default: toLanguage = "en-US"; break; // if translation to english happens something is wrong
            }
        }
    }
}
