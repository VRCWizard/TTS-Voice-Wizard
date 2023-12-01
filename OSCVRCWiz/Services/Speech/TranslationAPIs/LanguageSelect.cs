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

        public static (string LanguageName, string LanguageCode) ExtractLanguageNameAndCode(string languageString)
        {
            int startIndex = languageString.IndexOf('[');
            int endIndex = languageString.IndexOf(']');

            if (startIndex != -1 && endIndex != -1)
            {
                string languageName = languageString.Substring(0, startIndex).Trim();
                string languageCode = languageString.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

                return (languageName, languageCode);

            }
            else
            {
                return (string.Empty, string.Empty);
            }
        }


    }
}
