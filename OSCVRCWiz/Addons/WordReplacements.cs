using OSCVRCWiz.Text;
using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;
using static OSCVRCWiz.VoiceWizardWindow;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Globalization;
using OSCVRCWiz.Addons;
using System.Text.RegularExpressions;

namespace Addons
{
    public class WordReplacements
    {
        private static Dictionary<string, string> replaceDict = new Dictionary<string, string>();
       
        public static string wordReplacemntsStored = "";


        public static string MainDoWordReplacement(string text)
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
               int index = 0;
                foreach (var kvp in replaceDict)
                {
                    
                    if (text.Contains(kvp.Key.ToString(), StringComparison.OrdinalIgnoreCase))
                    {

                        //this implementation may be more fault proof but is more costly than using index
                        //index = replaceDict.Values.ToList().IndexOf(kvp.Key);
                        if (VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.GetItemCheckState(index) == CheckState.Checked)
                        {
                            //text = text.Replace(kvp.Key.ToString(), kvp.Value.ToString());

                           
                           // string pattern = @"[*#()\[\]]"; // Match any of these characters
                           
                            text = Regex.Replace(text, kvp.Key.ToString(), kvp.Value.ToString(), RegexOptions.IgnoreCase);
                        }
                        
                    }
                    index++;
                }

            });
            return text;
        }

        public static void addWordReplacement(string wordKey, string wordValue)
        {


            try
            {
                
                replaceDict.Add(wordKey, wordValue);//it is important to catch the thing that will actually break first in this case*** (fixes but with phantom entry after error occurs)
                VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.Add($"{VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public static void removeWordReplacementAt(int index)
        {
            replaceDict.Remove(replaceDict.ElementAt(index).Key);
            VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.RemoveAt(index);

        }

        public static void clearWordReplacement()
        {
            replaceDict.Clear();
            VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.Clear();

        }

        public static void replacementsLoad()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();
            string words = wordReplacemntsStored;
            string[] split = words.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    string words2 = s;
                    int count = 1;
                    string wordKey = "";
                    string wordValue = "";
                    string[] split2 = words2.Split(new char[] { '¬' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s2 in split2)
                    {




                        if (count == 1)
                        {
                            wordKey = s2;
                            System.Diagnostics.Debug.WriteLine("Phrase Added: " + s2);

                        }
                        if (count == 2)
                        {
                            wordValue = s2;
                            System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }
                      


                        count++;
                    }
                    try
                    {
                        //VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Add(saveThisPreset.PresetName);
                        VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.Add($"{VoiceWizardWindow.MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
                        replaceDict.Add(wordKey, wordValue);
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog("Error Loading Word Replacements / No Word Replacements Found", Color.DarkOrange);
                    }
                }
            }

        }
        public static void replacementSave()
        {
            wordReplacemntsStored = "";
            foreach (var kvp in replaceDict)
            {

                wordReplacemntsStored += $"{kvp.Key}¬{kvp.Value};";
            }
        }
    }
}
