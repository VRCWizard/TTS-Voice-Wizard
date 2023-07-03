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
            WinKey = 8
        }



        public static string modifierKeySTTTS = "Control";
        public static string normalKeySTTTS = "G";

        public static string modifierKeyStopTTS = "";
        public static string normalKeyStopTTS = "";

        public static string modifierKeyQuickType = "Control";
        public static string normalKeyQuickType = "J";



        public static void CUSTOMRegisterHotKey(int id, string modifierKey, string normalKey)
        {
            //  int id = 0;// The id of the hotkey. 
            if (id == 0 && VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked == false) { return; }
            if (id == 1 && VoiceWizardWindow.MainFormGlobal.rjToggleButton12.Checked == false) { return; }
            if (id == 2 && VoiceWizardWindow.MainFormGlobal.rjToggleButtonQuickTypeEnabled.Checked == false) { return; }
            KeyModifier modkey;
            Enum.TryParse(modifierKey, out modkey);

            Keys normKey;
            Enum.TryParse(normalKey, out normKey);

            RegisterHotKey(VoiceWizardWindow.MainFormGlobal.Handle, id, (int)modkey, normKey.GetHashCode());
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

                if (id == 0)//sttts
                {
                    Task.Run(() => DoSpeech.MainDoSpeechTTS());
                }
                if (id == 1)//stop
                {
                    Task.Run(() => DoSpeech.MainDoStopTTS());

                }
                if (id == 2)//game keyboard
                {
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
