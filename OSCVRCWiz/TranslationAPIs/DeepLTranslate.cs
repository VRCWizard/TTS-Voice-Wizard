using System;
using System.Collections.Generic;
using System.Text;
using DeepL;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;

namespace OSCVRCWiz.TranslationAPIs
{
    public class DeepLTranslate
    {
        // private LanguageCode fromLanguage = LanguageCode.English;
        //   private LanguageCode toLanguage = "en";
        //public static string DeepLTranslationText="";
      //  public static string DeepLKey = "";
     

        public static async Task<string> translateTextDeepL(string text)
        {
            try
            {

                var translator = new Translator(Settings1.Default.deepLKeysave);
                var fullFromLanguage = "";
                var fullToLanguage = "";
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    fullFromLanguage = VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedItem.ToString();
                    fullToLanguage = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();
                });

                var from = fromLanguageIDDeepL(fullFromLanguage);
                var to = toLanguageIDDeepL(fullToLanguage);

                var translatedText = await translator.TranslateTextAsync(text, from, to);
                System.Diagnostics.Debug.WriteLine(translatedText);
                //System.Diagnostics.Debug.WriteLine(LanguageCode.English);
                // System.Diagnostics.Debug.WriteLine(LanguageCode.French);
                //VoiceWizardWindow.MainFormGlobal.ot.outputLog("[DeepL Input Text]: " + text);
                return translatedText.ToString();
                //  System.Diagnostics.Debug.WriteLine("DeepL: " + VoiceWizardWindow.MainFormGlobal.deepLString);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("DeepL Translation Error: " + ex.Message);
                OutputText.outputLog("[Translation API Error: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[You are attempting to translate from one language to another. If this is not your intent then switch 'Translation Language' back to 'No Translation (Default)'. If you are intending to translate then get a VoiceWizardPro key or a DeepL key. ]", Color.DarkOrange);
                OutputText.outputLog("[Learn how to get a Language Translation Key: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/DeepL-Translation-API]", Color.DarkOrange);
                return "";
            }
        }
        private static string fromLanguageIDDeepL(string fullname)
        {
            var fromLanguage = "en";
            switch (fullname)
            {
                //   case "Arabic [ar-EG]": fromLanguage = Lan; break;
                case "Chinese [zh-CN]": fromLanguage = LanguageCode.Chinese; break;
                case "Czech [cs-CZ]": fromLanguage = LanguageCode.Czech; break;
                case "Danish [da-DK]": fromLanguage = LanguageCode.Danish; break;
                case "Dutch [nl-NL]": fromLanguage = LanguageCode.Dutch; break;
                case "English [en-US] (Default)": fromLanguage = LanguageCode.English; break;
                case "Estonian [et-EE]": fromLanguage = LanguageCode.Estonian; break;
                //  case "Filipino [fil-PH]": fromLanguage = LanguageCode.fi; break;
                case "Finnish [fi-FI]": fromLanguage = LanguageCode.Finnish; break;
                case "French [fr-FR]": fromLanguage = LanguageCode.French; break;
                case "German [de-DE]": fromLanguage = LanguageCode.German; break;
                // case "Hindi [hi-IN]": fromLanguage = LanguageCode.hin; break;
                case "Hungarian [hu-HU]": fromLanguage = LanguageCode.Hungarian; break;
                case "Indonesian [id-ID]": fromLanguage = LanguageCode.Indonesian; break;
                // case "Irish [ga-IE]": fromLanguage = LanguageCode.iris; break;
                case "Italian [it-IT]": fromLanguage = LanguageCode.Italian; break;
                case "Japanese [ja-JP]": fromLanguage = LanguageCode.Japanese; break;
                 case "Korean [ko-KR]": fromLanguage = LanguageCode.Korean; break;
                 case "Norwegian [nb-NO]": fromLanguage = LanguageCode.Norwegian; break;
                case "Polish [pl-PL]": fromLanguage = LanguageCode.Polish; break;
                case "Portuguese [pt-BR]": fromLanguage = LanguageCode.PortugueseBrazilian; break;
                //place holder^^
                case "Russian [ru-RU]": fromLanguage = LanguageCode.Russian; break;
                case "Spanish [es-MX]": fromLanguage = LanguageCode.Spanish; break;
                //place holder^^
                case "Swedish [sv-SE]": fromLanguage = LanguageCode.Swedish; break;
                //   case "Thai [th-TH]": fromLanguage = LanguageCode. break;
                case "Ukrainian [uk-UA]": fromLanguage = LanguageCode.Ukrainian; break;
                //  case "Vietnamese [vi-VN]": fromLanguage = LanguageCode.vie break;
                default:
                    fromLanguage = LanguageCode.English;
                    OutputText.outputLog("This language does not support text translations with DeepL", Color.Red);
                    OutputText.outputLog("[Consider becoming a VoiceWizardPro member for access to this translation language: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    break; // if translation to english happens something is wrong
            }
            return fromLanguage;

        }

        private static string toLanguageIDDeepL(string fullname)
        {
            var toLanguage = "en-US";

            switch (fullname)
            {
                //   case "Arabic [ar-EG]": fromLanguage = Lan; break;
                case "Chinese [zh]": toLanguage = LanguageCode.Chinese; break;
                case "Czech [cs]": toLanguage = LanguageCode.Czech; break;
                case "Danish [da]": toLanguage = LanguageCode.Danish; break;
                case "Dutch [nl]": toLanguage = LanguageCode.Dutch; break;
                case "English [en]": toLanguage = LanguageCode.EnglishAmerican; break;
                case "Estonian [et]": toLanguage = LanguageCode.Estonian; break;
                //  case "Filipino [fil]": toLanguage = LanguageCode.fi; break;
                case "Finnish [fi]": toLanguage = LanguageCode.Finnish; break;
                case "French [fr]": toLanguage = LanguageCode.French; break;
                case "German [de]": toLanguage = LanguageCode.German; break;
                // case "Hindi [hi]": toLanguage = LanguageCode.hin; break;
                case "Hungarian [hu]": toLanguage = LanguageCode.Hungarian; break;
                case "Indonesian [id]": toLanguage = LanguageCode.Indonesian; break;
                // case "Irish [ga]": toLanguage = LanguageCode.iris; break;
                case "Italian [it]": toLanguage = LanguageCode.Italian; break;
                case "Japanese [ja]": toLanguage = LanguageCode.Japanese; break;
                 case "Korean [ko]": toLanguage = LanguageCode.Korean; break;
                  case "Norwegian [nb]": toLanguage = LanguageCode.Norwegian; break;
                case "Polish [pl]": toLanguage = LanguageCode.Polish; break;
                case "Portuguese [pt]": toLanguage = LanguageCode.PortugueseBrazilian; break;
                //place holder^^
                case "Russian [ru]": toLanguage = LanguageCode.Russian; break;
                case "Spanish [es]": toLanguage = LanguageCode.Spanish; break;
                //place holder^^
                case "Swedish [sv]": toLanguage = LanguageCode.Swedish; break;
                //   case "Thai [th-]": toLanguage = LanguageCode. break;
                case "Ukrainian [uk]": toLanguage = LanguageCode.Ukrainian; break;
                //  case "Vietnamese [vi]": toLanguage = LanguageCode.vie break;
                default:
                    toLanguage = LanguageCode.EnglishAmerican;
                    OutputText.outputLog("This language does not support text translations with DeepL", Color.Red);
                    break; // if translation to english happens something is wrong
            }
            return toLanguage;
        }
    }
}
