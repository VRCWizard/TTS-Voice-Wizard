using OSCVRCWiz.Text;
using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;
using static OSCVRCWiz.VoiceWizardWindow;

namespace Settings
{
    public class VoicePresets
    {

        public static int presetnum = 0;
        private static Dictionary<string, voicePreset> presetDict = new Dictionary<string, voicePreset>();
        static bool editingPreset = false;
        public static string presetsStored = "";



        public static void presetSaveButton()
        {
            if (editingPreset == false)
            {
                presetnum++;
                voicePreset saveThisPreset = new voicePreset();
                string nameToCheck = "preset " + presetnum;

                while (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Contains(nameToCheck))
                {
                    presetnum++;
                    nameToCheck = "preset " + presetnum;

                }
                saveThisPreset.PresetName = nameToCheck;
                saveThisPreset.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem.ToString();
                saveThisPreset.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedItem.ToString();
                saveThisPreset.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedItem.ToString();
                saveThisPreset.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedItem.ToString();
                saveThisPreset.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();
                saveThisPreset.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedItem.ToString();
                saveThisPreset.Pitch = "";
                saveThisPreset.Volume = "";
                saveThisPreset.Speed = "";
                saveThisPreset.PitchNew = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value; 
                saveThisPreset.VolumeNew = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                saveThisPreset.SpeedNew = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;

                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Add(saveThisPreset.PresetName);

                presetDict.Add(saveThisPreset.PresetName, saveThisPreset);
            }
            else // edit true
            {
                presetDict.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString());
                // comboBoxPreset.Items.Remove();

                voicePreset saveThisPreset = new voicePreset();
                string nameToCheck = VoiceWizardWindow.MainFormGlobal.textBoxRename.Text.ToString();
                int counter = 0;
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString());//deleting name from list first make more sense so it can have the same same as it did before
                while (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Contains(nameToCheck))
                {
                    counter++;
                    nameToCheck = VoiceWizardWindow.MainFormGlobal.textBoxRename.Text.ToString() + " " + counter;

                }
                saveThisPreset.PresetName = nameToCheck;
                saveThisPreset.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem.ToString();
                saveThisPreset.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedItem.ToString();
                saveThisPreset.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedItem.ToString();
                saveThisPreset.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedItem.ToString();
                saveThisPreset.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();
                saveThisPreset.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedItem.ToString();
                saveThisPreset.Pitch = "";
                saveThisPreset.Volume = "";
                saveThisPreset.Speed = "";
                saveThisPreset.PitchNew = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                saveThisPreset.VolumeNew = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                saveThisPreset.SpeedNew = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;



                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Add(saveThisPreset.PresetName);
                presetDict.Add(saveThisPreset.PresetName, saveThisPreset);

                VoiceWizardWindow.MainFormGlobal.textBoxRename.Visible = false;
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Enabled = true;
                VoiceWizardWindow.MainFormGlobal.buttonEditPreset.Enabled = true;
                VoiceWizardWindow.MainFormGlobal.buttonDeletePreset.Enabled = true;
                editingPreset = false;

            }
            VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count - 1;

        }
        public static void presetEditButton()
        {
            VoiceWizardWindow.MainFormGlobal.textBoxRename.Visible = true;
            VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Enabled = false;
            VoiceWizardWindow.MainFormGlobal.buttonEditPreset.Enabled = false;
            VoiceWizardWindow.MainFormGlobal.buttonDeletePreset.Enabled = false;
            editingPreset = true;
            VoiceWizardWindow.MainFormGlobal.textBoxRename.Text = VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString();

        }

        public static void presetDeleteButton()
        {
            if (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex != 0)
            {
                presetDict.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString());
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Remove(VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString());
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count - 1;
            }
        }

        public static void setPreset()//enables preset when selected
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {



                foreach (var kvp in presetDict)
                {
                    if (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem.ToString() == kvp.Key)
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem = kvp.Value.TTSMode;
                        VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedItem = kvp.Value.Accent;
                        Thread.Sleep(10);
                        VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedItem = kvp.Value.Voice;
                         Thread.Sleep(10);
                        
                        VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedItem = kvp.Value.SpokenLang;
                        VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem = kvp.Value.TranslateLang;
                        VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedItem = kvp.Value.Style;
                        // VoiceWizardWindow.MainFormGlobal.comboBoxPitch.SelectedItem = kvp.Value.Pitch;
                        //  VoiceWizardWindow.MainFormGlobal.comboBoxVolume.SelectedItem = kvp.Value.Volume;
                        //  VoiceWizardWindow.MainFormGlobal.comboBoxRate.SelectedItem = kvp.Value.Speed;
                        if (kvp.Value.Pitch == "" && kvp.Value.Volume == "" && kvp.Value.Speed == "")
                        {
                            VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value = kvp.Value.PitchNew;
                            VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value = kvp.Value.VolumeNew;
                            VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value = kvp.Value.SpeedNew;
                        }
                        else
                        {
                            //legacy 
                            VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value = 5;
                            VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value = 5;
                            VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value = 5;

                        }
                        VoiceWizardWindow.MainFormGlobal.updateAllTrackBarLabels();
                        if (kvp.Value.TTSMode == "Azure")
                        {
                            // OutputText.outputLog("If Azure Voice Accent/Language is being loaded for the first time this session then preset will not select Voice properly. Simply re-select the preset.", Color.DarkOrange);
                           // Thread.Sleep(500);
                          //  VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedItem = kvp.Value.Voice;
                          ///  VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedItem = kvp.Value.Style;

                        }
                    }
                }

            });
        }


        public static void presetsLoad()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();
            string words = presetsStored;
            string[] split = words.Split(new char[] { ';' });
            foreach (string s in split)
            {
                if (s.Trim() != "")
                {
                    string words2 = s;
                    int count = 1;
                    voicePreset saveThisPreset = new voicePreset();

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
                            saveThisPreset.TTSMode = s2;
                            System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }
                        if (count == 3)
                        {
                            saveThisPreset.Voice = s2;
                            System.Diagnostics.Debug.WriteLine("typeadded: " + s2);

                        }
                        if (count == 4)
                        {
                            saveThisPreset.Accent = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 5)
                        {
                            saveThisPreset.SpokenLang = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 6)
                        {
                            saveThisPreset.TranslateLang = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 7)
                        {
                            saveThisPreset.Style = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 8)
                        {
                            saveThisPreset.Pitch = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 9)
                        {
                            saveThisPreset.Volume = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 10)
                        {
                            saveThisPreset.Speed = s2;
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 11)
                        {
                            saveThisPreset.PitchNew = Int32.Parse(s2);
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 12)
                        {
                            saveThisPreset.VolumeNew = Int32.Parse(s2);
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        if (count == 13)
                        {
                            saveThisPreset.SpeedNew = Int32.Parse(s2);
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }


                        count++;
                    }
                    try
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Add(saveThisPreset.PresetName);
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
            presetsStored = "";
            foreach (var kvp in presetDict)
            {

                presetsStored += $"{kvp.Value.PresetName}:{kvp.Value.TTSMode}:{kvp.Value.Voice}:{kvp.Value.Accent}:{kvp.Value.SpokenLang}:{kvp.Value.TranslateLang}:{kvp.Value.Style}:{kvp.Value.Pitch}:{kvp.Value.Volume}:{kvp.Value.Speed}:{kvp.Value.PitchNew}:{kvp.Value.VolumeNew}:{kvp.Value.SpeedNew};";
            }
        }
    }
}
