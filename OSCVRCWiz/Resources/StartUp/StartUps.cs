using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Speech_Recognition;

namespace OSCVRCWiz.Resources.StartUp
{
    public class StartUps
    {
        public static int fontSize = 20;

        public static void SetTextBoxFontSize()
        {
            if (fontSize >= 1)
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.richTextBox3.Font = new Font("Segoe UI", fontSize);
                });
            }
        }
        public static void OnAppStart() //immediate
        {
            AudioDevices.InitializeAudioDevices();
            SystemSpeechTTS.InitializeSystemSpeech();
            OSC.InitializeOSC();
           

        }

        public static void OnFormLoad() //delayed until form is loaded correctly
        {
            AudioDevices.InitializeAudioDevices();
            SystemSpeechTTS.InitializeSystemSpeech();
            OSC.InitializeOSC();

            VRChatListener.initiateTimer();
            WhisperRecognition.initiateTimer();
            ToastNotification.initiateTimer();
            OutputText.initiateTextTimers();

            Updater.getGithubInfo();
            Hotkeys.InitiateHotkeys();
            SetTextBoxFontSize();
            OSCListener.OnStartUp();
            VRChatListener.OnStartUp();
            WindowsMedia.getWindowsMedia();
            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();
            ToastNotification.ToastListen();
            MinimizeSystemTray.StartInSystemTray();
            OutputText.EmptyTextOutput();
            WindowsMedia.addSoundPad();


            OutputText.outputLog("[QuickStart Guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide ]");

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == false)
            {
                OutputText.outputLog("[Consider becoming a VoiceWizardPro member for instant access to all the best voices: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
            }
            else
            {
                //make get pro button white
                VoiceWizardWindow.MainFormGlobal.iconButton17.ForeColor = Color.White;
                VoiceWizardWindow.MainFormGlobal.iconButton17.IconColor = Color.White;
            }

        }



    }
}
