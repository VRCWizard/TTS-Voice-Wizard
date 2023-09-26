using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;
using static OSCVRCWiz.VoiceWizardWindow;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Services.Integrations.Media;

namespace Settings
{
    public class MediaPresets
    {

        private struct mediaPreset //use then when setting up presets
        {
            public string PresetName;
            public string mediaText;
            public string updateInterval;
            public string OutputContinuously;
            public string StopPaused;
            public string SpamLog;

        }

        private static int mediapresetnum = 0;
        private static Dictionary<string, mediaPreset> presetDict = new Dictionary<string, mediaPreset>();
        static bool editingPreset = false;
        public static string mediaPresetsStored = "";

        private static string boolToString(bool theBool)
        {
            if (theBool) { return "T"; }
            else { return "F"; }

        }
        private static bool stringToBool(string theString)
        {
            if (theString=="T") { return true; }
            else { return false; }
        }


        public static void presetSaveButton()
        {
            string text = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString().Replace(":", "{colon}").Replace(";", "{semi}");

            if (editingPreset == false)
            {
                mediapresetnum++;
                mediaPreset saveThisPreset = new mediaPreset();
                string nameToCheck = "preset " + mediapresetnum;

                while (VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Contains(nameToCheck))
                {
                    mediapresetnum++;
                    nameToCheck = "preset " + mediapresetnum;

                }
                saveThisPreset.PresetName = nameToCheck;
                saveThisPreset.mediaText = text;
                saveThisPreset.updateInterval = SpotifyAddon.spotifyInterval;
                saveThisPreset.OutputContinuously = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked);
                saveThisPreset.StopPaused = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked);
                saveThisPreset.SpamLog = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked);


                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Add(saveThisPreset.PresetName);

                presetDict.Add(saveThisPreset.PresetName, saveThisPreset);
            }
            else // edit true
            {
                presetDict.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString());
                // comboBoxPreset.Items.Remove();

                mediaPreset saveThisPreset = new mediaPreset();
                string nameToCheck = VoiceWizardWindow.MainFormGlobal.textBoxMediaPresetEdit.Text.ToString();
                nameToCheck = nameToCheck.Replace(":", "").Replace(";", "");
                int counter = 0;
                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString());//deleting name from list first make more sense so it can have the same same as it did before
                while (VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Contains(nameToCheck))
                {
                    counter++;
                    nameToCheck = VoiceWizardWindow.MainFormGlobal.textBoxMediaPresetEdit.Text.ToString() + " " + counter;

                }
                saveThisPreset.PresetName = nameToCheck;
               // saveThisPreset.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem.ToString();
                saveThisPreset.mediaText = text;
                saveThisPreset.updateInterval = SpotifyAddon.spotifyInterval;
                saveThisPreset.OutputContinuously = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked);
                saveThisPreset.StopPaused = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked);
                saveThisPreset.SpamLog = boolToString(VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked);




                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Add(saveThisPreset.PresetName);
                presetDict.Add(saveThisPreset.PresetName, saveThisPreset);

                VoiceWizardWindow.MainFormGlobal.textBoxMediaPresetEdit.Visible = false;
                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Enabled = true;
                VoiceWizardWindow.MainFormGlobal.buttonMediaPresetEditNew.Enabled = true;
                VoiceWizardWindow.MainFormGlobal.buttonMediaPresetDeleteNew.Enabled = true;
                editingPreset = false;

            }
            VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedIndex = VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Count - 1;
            presetsSave();
        }
        public static void presetEditButton()
        {
            VoiceWizardWindow.MainFormGlobal.textBoxMediaPresetEdit.Visible = true;
            VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Enabled = false;
            VoiceWizardWindow.MainFormGlobal.buttonMediaPresetEditNew.Enabled = false;
            VoiceWizardWindow.MainFormGlobal.buttonMediaPresetDeleteNew.Enabled = false;
            editingPreset = true;
            VoiceWizardWindow.MainFormGlobal.textBoxMediaPresetEdit.Text = VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString();

        }

        public static void presetDeleteButton()
        {
            if (VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedIndex != 0)
            {
                presetDict.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString());
                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString());
                VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedIndex = VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Count - 1;
            }
        }

        public static void setPreset()//enables preset when selected
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {



                foreach (var kvp in presetDict)
                {
                    if (VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedItem.ToString() == kvp.Key)
                    {

                       

                      VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text =kvp.Value.mediaText.ToString().Replace("{colon}", ":").Replace("{semi}", ";"); ;
                      SpotifyAddon.spotifyInterval = kvp.Value.updateInterval;
                        VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked = stringToBool(kvp.Value.OutputContinuously);
                        VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked = stringToBool(kvp.Value.StopPaused);
                        VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked = stringToBool(kvp.Value.SpamLog);


                    }
                }

            });
        }


        public static void presetsLoad()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();
            string words = mediaPresetsStored;
            string[] split = words.Split(new char[] { ';' });
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    string words2 = s;
                    int count = 1;
                    mediaPreset saveThisPreset = new mediaPreset();

                    string[] split2 = words2.Split(new char[] { ':' });
                    foreach (string s2 in split2)
                    {




                        if (count == 1)
                        {
                            saveThisPreset.PresetName = s2;
                            System.Diagnostics.Debug.WriteLine("Phrase Added: " + s2);

                        }
                        if (count == 2)
                        {
                            saveThisPreset.mediaText = s2;
                             System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }
                        if (count == 3)
                        {
                            saveThisPreset.updateInterval = s2;
                            System.Diagnostics.Debug.WriteLine("typeadded: " + s2);

                        }
                        if (count == 4)
                        {
                            saveThisPreset.OutputContinuously = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 5)
                        {
                            saveThisPreset.StopPaused = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 6)
                        {
                            saveThisPreset.SpamLog = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                       


                        count++;
                    }
                    try
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.Items.Add(saveThisPreset.PresetName);
                        presetDict.Add(saveThisPreset.PresetName, saveThisPreset);
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog("Error Loading Presets / No Presets Found", Color.DarkOrange);
                    }
                }
            }
        }
        public static void presetsSave()
        {
            mediaPresetsStored = "";
            foreach (var kvp in presetDict)
            {

                mediaPresetsStored += $"{kvp.Value.PresetName}:{kvp.Value.mediaText}:{kvp.Value.updateInterval}:{kvp.Value.OutputContinuously}:{kvp.Value.StopPaused};";
            }
        }
    }
}
