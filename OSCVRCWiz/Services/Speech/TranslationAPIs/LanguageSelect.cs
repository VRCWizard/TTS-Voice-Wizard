using DeepL;
using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.AppBroadcasting;
using static OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines.TikTokTTS;
using System.Text.Json;
using static OSCVRCWiz.Services.Speech.TranslationAPIs.LanguageSelect;
using System.Diagnostics;

namespace OSCVRCWiz.Services.Speech.TranslationAPIs
{
    public class LanguageSelect
    {
        //azure
      /*  public static string fromLanguageID(string fullname, string source)
        {
            string fromLanguage = "en-US";
            switch (source)
            {
              case "Azure":
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
                    case "Hungarian [hu-HU]": fromLanguage = "hu-HU"; break;
                    case "Indonesian [id-ID]": fromLanguage = "id-ID"; break;
                    case "Irish [ga-IE]": fromLanguage = "ga-IE"; break;
                    case "Italian [it-IT]": fromLanguage = "it-IT"; break;
                    case "Japanese [ja-JP]": fromLanguage = "ja-JP"; break;
                    case "Korean [ko-KR]": fromLanguage = "ko-KR"; break;
                    case "Norwegian [nb-NO]": fromLanguage = "nb-NO"; break;

                    case "Persian [fa-IR]": fromLanguage = "fa-IR"; break;//new
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
            break;
                case "DeepL":

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

                        //8/25
                        case "Latvian ": fromLanguage = LanguageCode.Ukrainian; break;


                        default:
                            fromLanguage = LanguageCode.English;
                            OutputText.outputLog("This language does not support text translations with DeepL", Color.Red);
                            OutputText.outputLog("[Consider becoming a VoiceWizardPro member for access to this translation language: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                            break; // if translation to english happens something is wrong
                    }
                    break;

                case "Whisper":

                    switch (fullname)
                    {
                        case "Arabic [ar-EG]": fromLanguage = "ar"; break;
                        case "Chinese [zh-CN]": fromLanguage = "zh"; break;
                        case "Czech [cs-CZ]": fromLanguage = "cs"; break;
                        case "Danish [da-DK]": fromLanguage = "da"; break;
                        case "Dutch [nl-NL]": fromLanguage = "nl"; break;
                        case "English [en-US] (Default)": fromLanguage = "en"; break;
                        case "Estonian [et-EE]": fromLanguage = "et"; break;
                        case "Filipino [fil-PH]": fromLanguage = "tl"; break;
                        case "Finnish [fi-FI]": fromLanguage = "fi"; break;
                        case "French [fr-FR]": fromLanguage = "fr"; break;
                        case "German [de-DE]": fromLanguage = "de"; break;
                        case "Hindi [hi-IN]": fromLanguage = "hi"; break;
                        case "Hungarian [hu-HU]": fromLanguage = "hu"; break;
                        case "Indonesian [id-ID]": fromLanguage = "id"; break;
                        case "Irish [ga-IE]": fromLanguage = "ga"; break;
                        case "Italian [it-IT]": fromLanguage = "it"; break;
                        case "Japanese [ja-JP]": fromLanguage = "ja"; break;
                        case "Korean [ko-KR]": fromLanguage = "ko"; break;
                        case "Norwegian [nb-NO]": fromLanguage = "no"; break;

                        case "Persian [fa-IR]": fromLanguage = "fa"; break;//new
                        case "Polish [pl-PL]": fromLanguage = "pl"; break;
                        case "Portuguese [pt-BR]": fromLanguage = "pt"; break;

                        //place holder^^
                        case "Russian [ru-RU]": fromLanguage = "ru"; break;
                        case "Spanish [es-MX]": fromLanguage = "es"; break;
                        //place holder^^
                        case "Swedish [sv-SE]": fromLanguage = "sv"; break;
                        case "Thai [th-TH]": fromLanguage = "th"; break;
                        case "Ukrainian [uk-UA]": fromLanguage = "uk"; break;
                        case "Vietnamese [vi-VN]": fromLanguage = "vi"; break;
                        default: fromLanguage = "en"; break; // if translation to english happens something is wrong
                    }

                    break;


        }
        return fromLanguage;
        }

        //azure //whisper (from)
        public static string toLanguageID(string fullname, string source)
        {
            string toLanguage = "en";


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
                case "Hungarian [hu]": toLanguage = "hu"; break;
                case "Indonesian [id]": toLanguage = "id"; break;
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
            return toLanguage;
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
        } */

        public class LanguageJson
        {
            public string name { get; set; }
            public string sourceName { get; set; }
            public string targetName { get; set; }
            public string azureCode { get; set; }
            public string whisperCode { get; set; }
            public string deepLCode { get; set; }
            public string proCode { get; set; }

        }

        public static void loadLanguages(ComboBox InputLanguage, ComboBox OutputLanguage)
        {
            // Replace with the path to the JSON file
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = "Assets/languages/languages.json";
            string jsonFilePath = Path.Combine(basePath, relativePath);

            // Read the JSON data from the file
            string jsonData = "";
            try
            {
                jsonData = File.ReadAllText(jsonFilePath);
            }
            catch (Exception ex)
            {
             //  Debug.WriteLine("[Could not load languages: " + ex.Message + " ]");
                OutputText.outputLog("[Could not load languages: " + ex.Message + " ]", Color.Red);
            }

            // Deserialize the JSON data into an array of LanguageJson objects

                LanguageJson[] languageSelection = JsonSerializer.Deserialize<LanguageJson[]>(jsonData);




            // Populate the InputLanguage ComboBox with sourceNames
            foreach (var language in languageSelection)
             {
                if (language.sourceName.ToString() !="")
                {
                    InputLanguage.Items.Add(language.sourceName.ToString());
                }
                if (language.targetName.ToString() != "")
                {
                    OutputLanguage.Items.Add(language.targetName.ToString());
                }

                   
             }


        }


        public static string fromLanguageNew(string language, string inputCodeType,string outputCodeType)
        {
            string languageCode = "en";



            // replace with the path to the JSON file
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = "Assets/languages/languages.json";
            string jsonFilePath = Path.Combine(basePath, relativePath);



            // read the JSON data from the file
            string jsonData = "";
            try
            {
                jsonData = File.ReadAllText(jsonFilePath);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Could not read languages: "+ex.Message+ " ]", Color.Red);
            }



            // deserialize the JSON data into an array of Voice objects
            LanguageJson[] languageSelection = JsonSerializer.Deserialize<LanguageJson[]>(jsonData);
            /*  foreach (var lang in languageSelection)
              {
            

              }*/
            //how would i select the "azureCode" with the "sourceName" that matches "language"
            // Find the matching sourceName and get the corresponding azureCode

           LanguageJson selectedLanguage = null;

           switch (inputCodeType) //source or target language
            {
                case "sourceLanguage":
                    selectedLanguage = languageSelection.FirstOrDefault(lang => lang.sourceName == language);
                    break;

                case "targetLanguage":
                    selectedLanguage = languageSelection.FirstOrDefault(lang => lang.targetName == language);
                    break;
 
            }

            if (selectedLanguage != null)
            {
                switch (outputCodeType)// select translation API
                {
                    case "Azure":
                        languageCode = selectedLanguage.azureCode;
                        break;

                    case "Whisper":
                        languageCode = selectedLanguage.whisperCode;
                        break;

                    case "DeepL":
                        languageCode = selectedLanguage.deepLCode;
                        break;

                    case "Pro":
                        languageCode = selectedLanguage.proCode;
                        break;

                }
                if (languageCode == "")
                {
                    OutputText.outputLog($"[{language} is not available as a {inputCodeType} for translation with {outputCodeType}]", Color.Red);
                }


                return languageCode;
            }
            else
            {
                return "en";
            }
                
        }
    

    }
}
