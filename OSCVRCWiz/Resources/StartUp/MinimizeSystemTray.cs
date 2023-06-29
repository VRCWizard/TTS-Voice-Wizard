using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class MinimizeSystemTray
    {

        public static void StartInSystemTray()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked == true)
            {
                if (System.Diagnostics.Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {
                    MessageBox.Show("TTS Voice Wizard (System Tray Launch): This application is already running!");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }

                // bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
                VoiceWizardWindow.MainFormGlobal.WindowState = FormWindowState.Minimized;
                if (VoiceWizardWindow.MainFormGlobal.WindowState == FormWindowState.Minimized)
                {
                    VoiceWizardWindow.MainFormGlobal.ShowInTaskbar = false;
                    VoiceWizardWindow.MainFormGlobal.notifyIcon1.Visible = true;
                    VoiceWizardWindow.MainFormGlobal.Hide();
                    // int id = 0;
                    Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
                    Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
                    Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
                }

            }
        }



    }

}
