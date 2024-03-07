using OSCVRCWiz.RJControls;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class Hotkeys
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static IntPtr prevFocusedWindow = IntPtr.Zero;
        public static bool captureEnabled = false;
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8,
            Norepeat = 0x4000
        }


        public static string modifierKeySTTTS = "Control";
        public static string normalKeySTTTS = "G";

        public static string modifierKeyStopTTS = "";
        public static string normalKeyStopTTS = "";

        public static string modifierKeyQuickType = "Control";
        public static string normalKeyQuickType = "J";

        public static string modifierKeyScrollUp = "";
        public static string normalKeyScrollUp = "";

        public static string modifierKeyScrollDown = "";
        public static string normalKeyScrollDown = "";


        public static void HotkeyEdit(TextBox textBoxMod, TextBox textBoxNorm, Button editButton, Button saveButton)
        {
            textBoxMod.Clear();
            textBoxNorm.Clear();
            saveButton.Enabled = true;
            editButton.Enabled = false;
            textBoxMod.Enabled = true;
            textBoxNorm.Enabled = true;

        }
        public static (string, string) HotkeySave(TextBox textBoxMod, TextBox textBoxNorm, Button editButton, Button saveButton, int id)
        {
            textBoxMod.Enabled = false;
            textBoxNorm.Enabled = false;
            saveButton.Enabled = false;
            editButton.Enabled = true;

            string ModKey = textBoxMod.Text.ToString();
            string NormKey = textBoxNorm.Text.ToString();
            Hotkeys.UnregisterHotKey(VoiceWizardWindow.MainFormGlobal.Handle, id);
            Hotkeys.CUSTOMRegisterHotKey(id, ModKey, NormKey);

            return (ModKey, NormKey);
        }
        public static void HotkeyEnableChanged(RJToggleButton rjButton, string ModKey, string NormKey, int id)
        {
            if (rjButton.Checked)
            {
                Hotkeys.CUSTOMRegisterHotKey(id, ModKey, NormKey);
            }
            else
            {
                Hotkeys.UnregisterHotKey(VoiceWizardWindow.MainFormGlobal.Handle, id);
            }

        }

        public static void HotkeyKeyDown(TextBox KeyTextbox, KeyEventArgs e, bool mod)
        {
            if (mod)
            {
                if (KeyTextbox.Enabled == true)
                {
                    Keys modifierKeys = e.Modifiers;
                    KeyTextbox.Text = modifierKeys.ToString();

                }

            }
            else
            {
                if (KeyTextbox.Enabled == true)
                {
                    Keys modifierKeys = e.Modifiers;
                    Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys
                    var converter = new KeysConverter();
                    KeyTextbox.Text = converter.ConvertToString(pressedKey);

                }

            }

        }

        public static void CUSTOMRegisterHotKey(int id, string modifierKey, string normalKey)
        {
            //  int id = 0;// The id of the hotkey. 
            if (id == 0 && VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked == false) { return; }
            if (id == 1 && VoiceWizardWindow.MainFormGlobal.rjToggleButton12.Checked == false) { return; }
            if (id == 2 && VoiceWizardWindow.MainFormGlobal.rjToggleButtonQuickTypeEnabled.Checked == false) { return; }

            if (id == 3 && VoiceWizardWindow.MainFormGlobal.rjToggleSwitchVoicePresetsBind.Checked == false) { return; }
            if (id == 4 && VoiceWizardWindow.MainFormGlobal.rjToggleSwitchVoicePresetsBind.Checked == false) { return; }
            KeyModifier modkey;
            Enum.TryParse(modifierKey, out modkey);

            Keys normKey;
            Enum.TryParse(normalKey, out normKey);

            RegisterHotKey(VoiceWizardWindow.MainFormGlobal.Handle, id, (int)modkey | (int)KeyModifier.Norepeat, normKey.GetHashCode());
        }

        public static void CatchHotkey(ref Message m)
        {
            //link to implementation https://www.fluxbytes.com/csharp/how-to-register-a-global-hotkey-for-your-application-in-c/ 
            //additional links https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp

            //  System.Diagnostics.Debug.WriteLine("-------------get key press id: " + m.Result.ToString());
            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */


                Keys key = (Keys)((int)m.LParam >> 16 & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                System.Diagnostics.Debug.WriteLine("-------------get key press id: " + key.ToString());

                switch (id)
                {
                    case 0: Task.Run(() => DoSpeech.MainDoSpeechTTS()); break;
                    case 1: Task.Run(() => DoSpeech.MainDoStopTTS()); break;
                    case 2:
                        if (captureEnabled == false) // not capturing so turn it on
                        {
                            // Save the handle of the previously focused window
                            prevFocusedWindow = GetForegroundWindow();

                            // Activate and bring the form to the front
                            VoiceWizardWindow.MainFormGlobal.Activate();
                            VoiceWizardWindow.MainFormGlobal.BringToFront();
                            VoiceWizardWindow.MainFormGlobal.richTextBox3.Text = "";
                            VoiceWizardWindow.MainFormGlobal.mainTabControl.SelectTab(VoiceWizardWindow.MainFormGlobal.tabPage1);//sttts
                            VoiceWizardWindow.MainFormGlobal.richTextBox3.Select();


                            captureEnabled = true;


                        }
                        else if (captureEnabled == true)//is capturing so turn it off
                        {

                            // Activate and bring the previous window to the front
                            if (prevFocusedWindow != IntPtr.Zero)
                            {
                                SetForegroundWindow(prevFocusedWindow);
                            }

                            captureEnabled = false;


                        }
                        break;
                    case 3:
                        int index = (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex - 1 + VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count) % VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count;
                        if ( index ==0)
                        {
                            index = (index- 1 + VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count) % VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count;
                        }
                        VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = index;
                        break;
                    case 4:
                        index= (VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex + 1) % VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count;
                        if ( index ==0)
                        {
                            index = (index + 1) % VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Count;
                        }
                        VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = index;
                        break;
                }
              
            }

        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetForegroundWindow(IntPtr hWnd);




        public static void InitiateHotkeys()
        {
            try
            {
                modifierKeySTTTS = Settings1.Default.modHotKey;
                normalKeySTTTS = Settings1.Default.normalHotKey;
                CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);

                modifierKeyStopTTS = Settings1.Default.modHotkeyStop;
                normalKeyStopTTS = Settings1.Default.normalHotkeyStop;
                CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);

                modifierKeyQuickType = Settings1.Default.modHotkeyQuick;
                normalKeyQuickType = Settings1.Default.normalHotkeyQuick;
                CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);

                modifierKeyScrollUp = Settings1.Default.modifierKeyScrollUp;
                normalKeyScrollUp = Settings1.Default.normalKeyScrollUp;
                CUSTOMRegisterHotKey(3, modifierKeyScrollUp, normalKeyScrollUp);


                modifierKeyScrollDown = Settings1.Default.modifierKeyScrollDown;
                normalKeyScrollDown = Settings1.Default.normalKeyScrollDown;
                CUSTOMRegisterHotKey(4, modifierKeyScrollDown, normalKeyScrollDown);
            }
            catch (Exception ex) {

                DialogResult result = MessageBox.Show("Hotkey Startup Error: \n\nYour config file (where settings are stored) may have been corrupted.\n Would you like to navigate to C:\\Users\\<user>\\AppData\\Local\\TTSVoiceWizard and delete the files in this directory to reset your settings?",
                                    "Error",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    // Open the directory containing the config files
                    var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TTSVoiceWizard");
                    Process.Start("explorer.exe", appPath);
                }


            }

        }
    }
}
