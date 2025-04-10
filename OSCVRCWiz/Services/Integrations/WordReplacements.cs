using static OSCVRCWiz.VoiceWizardWindow;
using System.Text.RegularExpressions;
using OSCVRCWiz.Services.Text;
using System.Windows.Input;

namespace OSCVRCWiz.Services.Integrations
{
    public class WordReplacements
    {
        private static Dictionary<string, string> replaceDict = new Dictionary<string, string>();

        public static string wordReplacemntsStored = "";


        public static string MainDoWordReplacement(string text)
        {
            MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                int index = 0;
                foreach (var kvp in replaceDict)
                {

                    if (text.Contains(kvp.Key.ToString(), StringComparison.OrdinalIgnoreCase))
                    {

                        //this implementation may be more fault proof but is more costly than using index
                        //index = replaceDict.Values.ToList().IndexOf(kvp.Key);
                        if (MainFormGlobal.checkedListBoxReplacements.GetItemCheckState(index) == CheckState.Checked)
                        {

                            string pattern = Regex.Escape(kvp.Key.ToString());
                            string key = kvp.Key.ToString();
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleUseWordBoundaries.Checked)
                            {
                                // pattern = $@"\b{Regex.Escape(kvp.Key.ToString())}\b"; //does not work if the word starts with a special character like $
                                // pattern = $@"(?<!\S){Regex.Escape(kvp.Key.ToString())}(?!\S)"; //no longer works if punctuation is touching the word...
                                // pattern = $@"(?<![\w]){Regex.Escape(kvp.Key.ToString())}(?![\w])";

                                if ((char.IsLetterOrDigit(key[0]) || key[0] == '_') && (char.IsLetterOrDigit(key[^1]) || key[^1] == '_')) // Word characters: [a-zA-Z0-9_] //added check for last character too
                                {
                                    // Use standard word boundary pattern
                                    pattern = $@"\b{Regex.Escape(key)}\b";
                                }
                                else
                                {
                                    // Use custom pattern for special characters
                                    pattern = $@"(?<![\w]){Regex.Escape(key)}(?![\w])";
                                }
                            }
                            text = Regex.Replace(text, pattern, kvp.Value.ToString(), RegexOptions.IgnoreCase);
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
                MainFormGlobal.checkedListBoxReplacements.Items.Add($"{MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
                replacementSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public static void removeWordReplacementAt(int index)
        {
            replaceDict.Remove(replaceDict.ElementAt(index).Key);
            MainFormGlobal.checkedListBoxReplacements.Items.RemoveAt(index);

        }

        public static void clearWordReplacement()
        {
            replaceDict.Clear();
            MainFormGlobal.checkedListBoxReplacements.Items.Clear();

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
                        MainFormGlobal.checkedListBoxReplacements.Items.Add($"{MainFormGlobal.checkedListBoxReplacements.Items.Count + 1} | {wordKey} ---> {wordValue}", true);
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
