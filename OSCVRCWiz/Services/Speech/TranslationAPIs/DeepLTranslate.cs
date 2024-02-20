using System;
using System.Collections.Generic;
using System.Text;
using DeepL;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;

namespace OSCVRCWiz.Services.Speech.TranslationAPIs
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
                    fullFromLanguage = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                    fullToLanguage = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.SelectedItem.ToString();
                });

                var from = LanguageSelect.fromLanguageNew(fullFromLanguage, "sourceLanguage", "DeepL");
                var to = LanguageSelect.fromLanguageNew(fullToLanguage, "targetLanguage", "DeepL");

                switch (to)
                {

                    case "en": to = "en-US"; break;
                }


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
                OutputText.outputLog("[Learn how to get a Language Translation Key: https://ttsvoicewizard.com/docs/Translation/DeepL ]", Color.DarkOrange);
                return "";
            }
        }
      
    }
}
