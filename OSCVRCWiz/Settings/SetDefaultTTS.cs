using System;
using System.Collections.Generic;
using System.Text;

namespace OSCVRCWiz
{
    public class SetDefaultTTS
    {
        public static void SetVoicePresets()
        {
            VoiceWizardWindow.emotion = "normal";
            VoiceWizardWindow.rate = "default";
            VoiceWizardWindow.pitch = "default";
            VoiceWizardWindow.volume = "default";
            VoiceWizardWindow.voice = "Sara";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString())) { VoiceWizardWindow.emotion = "normal"; }
                else { VoiceWizardWindow.emotion = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString(); }
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxRate.Text.ToString())) { VoiceWizardWindow.rate = "default"; } 
                else { VoiceWizardWindow.rate = VoiceWizardWindow.MainFormGlobal.comboBoxRate.Text.ToString(); }
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxPitch.Text.ToString())) { VoiceWizardWindow.pitch = "default"; }
                else { VoiceWizardWindow.pitch = VoiceWizardWindow.MainFormGlobal.comboBoxPitch.Text.ToString(); }
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxVolume.Text.ToString())) { VoiceWizardWindow.volume = "default"; }
                else { VoiceWizardWindow.volume = VoiceWizardWindow.MainFormGlobal.comboBoxVolume.Text.ToString(); }
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString())) { VoiceWizardWindow.voice = "Sara"; }
                else { VoiceWizardWindow.voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString(); }


            });
        }
    }
}
