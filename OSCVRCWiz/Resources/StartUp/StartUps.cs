using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Integrations.Heartrate;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Speech_Recognition;
using System.Configuration;

namespace OSCVRCWiz.Resources.StartUp
{
    public class StartUps
    {
        public static int fontSize = 20;
        public static bool safeStart = true;

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

         



        }



        public static void OnFormLoad() //delayed until form is loaded correctly
        {
            OSC.InitializeOSC();

            OutputText.loadTextDelays();
            OutputText.initiateTextTimers();
            VRChatListener.initiateTimer();
            SpotifyAddon.initiateTimer();
            WhisperRecognition.initiateTimer();
            ToastNotification.initiateTimer();
           // HeartratePulsoid.initiateTimer();

            //timers
            



            Updater.getGithubInfo();
            Hotkeys.InitiateHotkeys();
            SetTextBoxFontSize();
            OSCListener.OnStartUp();
            VRChatListener.OnStartUp();
            HeartratePulsoid.OnStartUp();
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

        public static void saveBackupOfSettings()
        {
            //https://stackoverflow.com/questions/42708868/user-config-corruption
            //Took me way too long to discover this solution

            string configPathBackup;
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
                configPathBackup = config.FilePath + ".bak";
                config.SaveAs(configPathBackup, ConfigurationSaveMode.Full, true);
              //  System.Windows.Forms.MessageBox.Show("Backup Created");
            }
            catch (ConfigurationErrorsException ex)
            {
                safeStart = false;
                string filename = ex.Filename;
                configPathBackup = filename + ".bak";
                //_logger.Error(ex, "Cannot open config file");

                if (File.Exists(filename) == true)
                {
                    //_logger.Error("Config file {0} content:\n{1}", filename, File.ReadAllText(filename));
                    File.Delete(filename);

                    if (!string.IsNullOrEmpty(configPathBackup) && File.Exists(configPathBackup))
                    {
                        File.Copy(configPathBackup, filename, true);
                    }

                }
                //System.Windows.Forms.MessageBox.Show("Corrupted");
                //  Kinesis.Properties.Settings.Default.Reload();
                //_logger.Error("Config file {0} does not exist", filename);
            }

        }
        public static void BackupStatus()
        {
            if(safeStart) 
            {
                OutputText.outputLog("[A backup of your settings was successfully created]", Color.Purple);
            }
            else
            {
                OutputText.outputLog("[Configuration system failed to initialize - Your settings were loaded from a backup (changes to settings from your last session may have been lost)]", Color.MediumVioletRed);
            }
        }



    }
}
