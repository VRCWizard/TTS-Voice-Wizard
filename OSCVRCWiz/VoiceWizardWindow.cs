﻿//Wizard
#region Using Statements
using OSCVRCWiz.Settings;
using System.Diagnostics;
using Settings;
using OSCVRCWiz.Speech_Recognition;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.Themes;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Resources.StartUp;
using OSCVRCWiz.Resources.StartUp.StartUp;
using FontAwesome.Sharp;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
#endregion


namespace OSCVRCWiz
{

    public partial class VoiceWizardWindow : Form
    {

        public static VoiceWizardWindow MainFormGlobal;
        bool forceClose = false;


        public VoiceWizardWindow()
        {
            try { InitializeComponent(); }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show("Initalization Error: " + ex.Message); }
            MainFormGlobal = this;



            try
            {
                mainTabControl.Appearance = TabAppearance.FlatButtons;
                mainTabControl.ItemSize = new System.Drawing.Size(0, 1);
                mainTabControl.SizeMode = TabSizeMode.Fixed;
                labelCharCount.Text = richTextBox3.Text.ToString().Length.ToString();
                navbarHome.BackColor = SelectedNavBar;//make home button appear selected   

                StartUps.OnAppStart();
                LanguageSelect.loadLanguages(comboBoxSpokenLanguage, comboBoxTranslationLanguage);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                System.Windows.Forms.MessageBox.Show("Startup Error: " + errorMsg);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                try
                {
                   LoadSettings.LoadingSettings();// this is the source of the configuration error //add a try catch
                }
                catch (Exception ex)
                {

                    string ErrorMessage = ex.Message;
                    if (ex.StackTrace != null)
                    {
                        ErrorMessage += "\n \n" + ex.StackTrace;
                    }


                    DialogResult result = System.Windows.Forms.MessageBox.Show("Error Loading Settings: \n \n" + ErrorMessage

                        + "\n \nYour config file (where settings are stored) may have been corrupted.\n \nWould you like to be redirected to the AppData/Local/TTSVoiceWizard directory to manually delete the config files? Deleting these files will reset your settings. \n \n(If this does not solve your issue please post a screenshot of your error message in the #help channel of the Discord server)", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TTSVoiceWizard");
                        Process.Start("explorer.exe", appPath);
                        forceClose = true;
                        Application.Exit();
                    }
                    if (result == DialogResult.No)
                    {

                    }
                }
                StartUps.OnFormLoad();
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                System.Windows.Forms.MessageBox.Show("FormLoad Error: " + errorMsg);
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hotkeys.UnregisterHotKey(this.Handle, 0);
            Hotkeys.UnregisterHotKey(this.Handle, 1);
            Hotkeys.UnregisterHotKey(this.Handle, 2);
            if (!forceClose)
            {
                SaveSettings.SavingSettings();
            }
            MoonbaseTTS.CloseMoonbaseTerminal();
            StopAllRecogntion();


        }

        #region Resize
        private void VoiceWizardWindow_Resize(object sender, EventArgs e)
        {
            if (rjToggleButtonSystemTray.Checked == true)
            {
                bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);

                if (this.WindowState == FormWindowState.Minimized && cursorNotInBar)
                {
                    this.ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                    this.Hide();


                    Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
                    Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
                    Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
                }

            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
                this.Show();


                Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
                Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
                Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
                this.WindowState = FormWindowState.Normal;

            }


        }
        #endregion

        #region Hotkeys
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            Hotkeys.CatchHotkey(ref m);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TTSButton.PerformClick();

                e.Handled = true;


            }
        }

        private void rjToggleButtonQuickTypeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonQuickTypeEnabled.Checked == true)
            {
                Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
            }
            if (rjToggleButtonQuickTypeEnabled.Checked == false)
            {
                Hotkeys.UnregisterHotKey(this.Handle, 2);
            }
        }





        private void textBoxQuickType1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBoxQuickType1.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;
                textBoxQuickType1.Text = modifierKeys.ToString();

            }
        }

        private void textBoxQuickType2_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBoxQuickType2.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;
                Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys
                var converter = new KeysConverter();
                textBoxQuickType2.Text = converter.ConvertToString(pressedKey);

            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox4.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;
                textBox4.Text = modifierKeys.ToString();

            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;

                Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys

                //do stuff with pressed and modifier keys
                var converter = new KeysConverter();

                textBox1.Text = converter.ConvertToString(pressedKey);
            }

        }
        #endregion

        #region Textboxes

        public void ShowHidePassword(TextBox password, IconButton eye)
        {
            if (eye.IconChar == FontAwesome.Sharp.IconChar.EyeSlash)
            {
                password.PasswordChar = '\0';
                eye.IconChar = FontAwesome.Sharp.IconChar.Eye;
            }
            else
            {
                password.PasswordChar = '*';
                eye.IconChar = FontAwesome.Sharp.IconChar.EyeSlash;
            }
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            int length = richTextBox3.Text.ToString().Length;

            if (rjToggleButtonChatBox.Checked == true && (richTextBox3.Text.ToString().Length > length))
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                OSC.OSCSender.Send(typingbubble);
            }
            // TTSBoxText = richTextBox3.Text.ToString();
            labelCharCount.Text = length.ToString();

            if (rjToggleButtonAutoSend.Checked)
            {
                Task.Run(() => DoSpeech.TTSButonClick());
            }

        }

        public void logLine(string line, Color? color = null)
        {
            try
            {

                if (InvokeRequired)
                {
                    this.Invoke(new Action<string, Color?>(logLine), new object[] { line, color });
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox1.Select(0, 0);
                    if (rjToggleDarkMode.Checked == true) { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.White); }
                    else { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.Black); }

                    richTextBox1.SelectedText = line + "\r\n";
                });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Logline Error: " + ex.Message + ". This occured while trying to output: " + line + ". This message is colorless in the case that the issue is caused by colored messages. If you get this error report it in the #tts-voice-wizard-bug channel in discord.]");


            }


        }

        public void ClearTextBox()
        {
            try
            {

                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBox));
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox1.Text = "";
                });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ClearTextBox Error: " + ex.Message + "]", Color.Red);

            }

        }

        public void ClearTextBoxTTS()
        {
            try
            {

                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBoxTTS));
                    return;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox3.Text = "";
                });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ClearTextBoxTTS Error: " + ex.Message + "]", Color.Red);

            }

        }
        public void ClearTypingBox()
        {
            try
            {

                if (InvokeRequired)
                {
                    this.Invoke(new Action(ClearTextBox));
                    return;
                }

                richTextBox9.Text = "";
                if (rjToggleButtonOSC.Checked == true)
                {

                    var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
                    OSC.OSCSender.Send(message0);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ClearTypingBox Error: " + ex.Message + ". Try moving folder location.]", Color.Red);

            }

        }

        #endregion

        #region Navbar

        public static Color SelectedNavBar = Color.FromArgb(68, 72, 111);

        Color UnSelectedNavBar = Color.FromArgb(31, 30, 68);

        private void allButtonColorReset()
        {
            navbarTextToSpeech.BackColor = UnSelectedNavBar;
            navbarSpeechProvider.BackColor = UnSelectedNavBar;
            navbarSettings.BackColor = UnSelectedNavBar;
            navbarHome.BackColor = UnSelectedNavBar;
            navbarIntegrations.BackColor = UnSelectedNavBar;
            navbarTextToText.BackColor = UnSelectedNavBar;

        }

        private void iconButton1_Click(object sender, EventArgs e) // Dashboard
        {
            allButtonColorReset();
            navbarHome.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage4);//Dashboard
            webView21.Show();


        }
        private void iconButton2_Click(object sender, EventArgs e)// Text to Speech
        {
            allButtonColorReset();
            navbarTextToSpeech.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage1);//sttts
            webView21.Hide();


        }

        private void iconButton23_Click(object sender, EventArgs e)// Text to Text
        {
            allButtonColorReset();
            navbarTextToText.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabPage3);//ttt
            webView21.Hide();
        }


        private void iconButton5_Click(object sender, EventArgs e) // Settings
        {
            allButtonColorReset();
            navbarSettings.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(General);//settings
            webView21.Hide();
        }



        private void iconButton3_Click(object sender, EventArgs e) // Integrations
        {
            allButtonColorReset();
            navbarIntegrations.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(tabAddons); //addon
            webView21.Hide();


        }

        private void iconButton4_Click(object sender, EventArgs e) // Speech Provider
        {
            allButtonColorReset();
            navbarSpeechProvider.BackColor = SelectedNavBar;
            mainTabControl.SelectTab(APIs);//provider
            webView21.Hide();
        }


        private void iconButton7_Click(object sender, EventArgs e) // Discord
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard");
        }

        private void iconButton6_Click(object sender, EventArgs e) //Github
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://discord.gg/YjgR9SWPnW");

        }

        private void iconButton12_Click(object sender, EventArgs e) //Ko-fi
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard/tiers");
        }

        private void iconButton8_Click(object sender, EventArgs e)//Updater Button
        {
            Updater.UpdateButtonClicked();
        }


        private void versionLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/releases");
        }


        #endregion

        #region Expandable Log

        public bool logPanelExtended = true;
        public bool logPanelExtended2 = true;


        private void richTextBox1_TextChanged(object sender, EventArgs e) // auto log clear
        {
            if (richTextBox1.Lines.Count() >= 2000)
            {
                ClearTextBox();
                OutputText.outputLog("Log exceeded limit and was automatically cleared");
            }


        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)// click links in log
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }


        private void button45_Click(object sender, EventArgs e)//expand button
        {
            if (logPanelExtended == true)
            {
                logPanel.Size = new System.Drawing.Size(20, logPanel.Height);
                button45.Text = "🢀🢀🢀";
                logPanelExtended = false;
            }
            else
            {
                logPanel.Size = new System.Drawing.Size(300, logPanel.Height);
                button45.Text = "🢂🢂🢂";
                logPanelExtended = true;
            }

        }



        private void logTrash_Click(object sender, EventArgs e) // trash can button
        {
            ClearTextBox();
        }


        #endregion




        #region Home Screen Tab


        private void pictureBox4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard/tiers");
        }

        #region Socials
        private void iconButton13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/wBRUcx9EWes");

        }

        private void iconButton14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://twitter.com/Wizard_VR");

        }

        private void iconButton26_Click(object sender, EventArgs e)//trello button home page
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://trello.com/b/cUhN6eF0/ttsvoicewizard-planned-features");
        }
        #endregion

        #region WebViewer Buttons

        private void button9_Click(object sender, EventArgs e)//Refresh
        {
            Uri uri = new Uri("https://voicewizardpro.carrd.co/");
            webView21.Source = uri;
        }

        private void button10_Click(object sender, EventArgs e)//Close
        {
            webView21.Dispose();
            button10.Dispose();
            button9.Dispose();
        }
        #endregion

        #region Bottom Buttons

        private void iconButton33_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/Q4kaXcA74Bo");
        }

        private void iconButton34_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://discord.gg/YjgR9SWPnW");
        }

        private void iconButton17_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard/tiers");
        }

        #endregion



        #endregion 

        #region TextToSpeech Tab


        #region Text to Speech Section




        private async void TTSButton_Click(object sender, EventArgs e)//TTS
        {
            Task.Run(() => DoSpeech.TTSButonClick());
        }

        private void button38_Click(object sender, EventArgs e)
        {
            DoSpeech.MainDoStopTTS();
        }


        private void buttonQueueClear_Click(object sender, EventArgs e)
        {
            TTSMessageQueue.queueTTS.Clear();
            labelQueueSize.Text = TTSMessageQueue.queueTTS.Count.ToString();
        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {

            Task.Run(() => DoSpeech.MainDoSpeechTTS());

        }
        private void iconButton36_Click(object sender, EventArgs e) //Increase Font Size
        {
            StartUps.fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) + 1;
            richTextBox3.Font = new Font("Segoe UI", StartUps.fontSize);

        }

        private void iconButton37_Click(object sender, EventArgs e)// Decrease Font Size
        {
            if (Int32.Parse(richTextBox3.Font.Size.ToString()) >= 1)
            {
                StartUps.fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) - 1;
                richTextBox3.Font = new Font("Segoe UI", StartUps.fontSize);
            }


        }

        private void ttsTrash_Click(object sender, EventArgs e) // trash can button
        {
            ClearTextBoxTTS();
        }

        #endregion

        #region Voice Customization Options

        private void buttonImportVoicePresets_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBoxPreset.Items.Clear();
                comboBoxPreset.Items.Add("- None Selected -");
                VoicePresets.presetsStored = Import_Export.importFile(openFileDialog1.FileName);
                VoicePresets.presetsLoad();
                comboBoxPreset.SelectedIndex = 0;

            }
        }

        private void buttonExportVoicePresets_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "VoicePreset", VoicePresets.presetsStored);
        }

        private void comboBoxTTSMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "Moonbase":
                    MoonbaseTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Moonbase";
                    OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Moonbase-TTS ]", Color.DarkOrange);


                    break;
                case "TikTok":

                    TikTokTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "TikTok";


                    break;
                case "System Speech":
                    SystemSpeechTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "System Speech";

                    break;
                case "Azure":
                    AzureTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Azure";

                    if (textBoxAzureKey.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


                    break;

                case "Google (Pro Only)":
                    GoogleTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Google (Pro Only)";

                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


                    break;

                case "IBM Watson (Pro Only)":
                    IBMWatsonTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "IBM Watson (Pro Only)";

                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


                    break;

                case "Uberduck":
                    UberDuckTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Uberduck";


                    break;

                case "Locally Hosted":

                    comboBoxVoiceSelect.Items.Clear();
                    comboBoxVoiceSelect.Items.Add("Local 1");
                    comboBoxVoiceSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.Enabled = false;
                    comboBoxVoiceSelect.Enabled = true;


                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Locally Hosted";

                    OutputText.outputLog("[Here is an example of a project that can be used with Local: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS . This method works by sending a GET request to http://127.0.0.1:8124/synthesize/ with the string parameter 'text'. If you create compatible projects or models, feel free to share them in the Discord server.]", Color.DarkOrange);

                    break;

                case "ElevenLabs":

                    ElevenLabsTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect);

                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "ElevenLabs";

                    if (textBoxElevenLabsKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an ElevenLabs Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/ElevenLabs-TTS ]", Color.DarkOrange);
                    }

                    break;


                case "Amazon Polly":

                    AmazonPollyTTS.SetVoices(comboBoxVoiceSelect, comboBoxStyleSelect, comboBoxAccentSelect);

                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    DoSpeech.TTSModeSaved = "Amazon Polly";

                    if (textBoxAmazonKey.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Amazon Polly Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Amazon-Polly ]", Color.DarkOrange);
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }

                    break;





                default:
                    DoSpeech.TTSModeSaved = "No TTS";
                    comboBoxVoiceSelect.Items.Clear();
                    comboBoxVoiceSelect.Items.Add("no voice");
                    comboBoxVoiceSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("default");
                    comboBoxStyleSelect.SelectedIndex = 0;
                    comboBoxStyleSelect.Enabled = false;
                    comboBoxVoiceSelect.Enabled = false;
                    comboBoxTranslationLanguage.Enabled = true;
                    comboBoxAccentSelect.Enabled = false;
                    trackBarPitch.Enabled = false;
                    trackBarVolume.Enabled = false;
                    trackBarSpeed.Enabled = false;
                    break;
            }
            updateAllTrackBarLabels();
        }

        private void button50_Click(object sender, EventArgs e)
        {

            if (logPanelExtended2 == true)
            {
                panelCustomize.Size = new System.Drawing.Size(20, logPanel.Height);
                button50.Text = "🢀🢀🢀";
                logPanelExtended2 = false;
            }
            else
            {
                panelCustomize.Size = new System.Drawing.Size(315, logPanel.Height);
                button50.Text = "🢂🢂🢂";
                logPanelExtended2 = true;
            }

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTranslationLanguage.SelectedItem != null && comboBoxSpokenLanguage.SelectedItem != null)
            {
                // Get the language code from the selected spoken language
                string spokenLanguageCode = comboBoxSpokenLanguage.SelectedItem.ToString().Substring(0, comboBoxSpokenLanguage.SelectedItem.ToString().IndexOf(' '));

                // Get the language code from the selected translation language
                string translationLanguageCode = comboBoxTranslationLanguage.SelectedItem.ToString().Substring(0, comboBoxTranslationLanguage.SelectedItem.ToString().IndexOf(' '));

                // Check if the selected spoken language is the same as the selected translation language
                if (spokenLanguageCode == translationLanguageCode)
                {
                    // Set the translation language to position 0 (no translation)
                    comboBoxTranslationLanguage.SelectedIndex = 0;
                }
                // added: change language -chrisk
                switch (comboBoxSTT.Text.ToString())
                {
                    case "Whisper":
                        string language = "";

                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            language = comboBoxSpokenLanguage.SelectedItem.ToString();
                        });
                        Task.Run(() => WhisperRecognition.setLanguage(language));

                        break;
                }

            }


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTranslationLanguage.SelectedItem != null && comboBoxSpokenLanguage.SelectedItem != null)
            {
                // Get the language code from the selected spoken language
                string spokenLanguageCode = comboBoxSpokenLanguage.SelectedItem.ToString().Substring(0, comboBoxSpokenLanguage.SelectedItem.ToString().IndexOf(' '));

                // Get the language code from the selected translation language
                string translationLanguageCode = comboBoxTranslationLanguage.SelectedItem.ToString().Substring(0, comboBoxTranslationLanguage.SelectedItem.ToString().IndexOf(' '));

                // Check if the selected spoken language is the same as the selected translation language
                if (spokenLanguageCode == translationLanguageCode)
                {
                    // Set the translation language to position 0 (no translation)
                    comboBoxTranslationLanguage.SelectedIndex = 0;
                }

            }
        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//Voice Select
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "Moonbase": break;

                case "ElevenLabs": break;

                case "TikTok": break;

                case "NovelAI": break;

                case "System Speech":


                    break;
                case "Azure":
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("normal");
                    foreach (string style in AzureTTS.AllVoices4Language[comboBoxVoiceSelect.Text.ToString()])
                    {
                        comboBoxStyleSelect.Items.Add(style);
                    }
                    comboBoxStyleSelect.SelectedIndex = 0; break;


                case "Amazon Polly":
                    comboBoxStyleSelect.Items.Clear();
                    comboBoxStyleSelect.Items.Add("normal");
                    if (!VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString().EndsWith("($Neural)"))
                    {
                        comboBoxStyleSelect.Items.Add("auto-breaths");
                        comboBoxStyleSelect.Items.Add("soft");
                        comboBoxStyleSelect.Items.Add("whispered");
                    }
                    comboBoxStyleSelect.SelectedIndex = 0;
                    break;


                case "Google (Pro Only)": break;





                default:

                    break;
            }

        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)//Accents/Languages Select
        {
            if (DoSpeech.TTSModeSaved == "Azure")
            {
                AzureTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString());

            }
            if (DoSpeech.TTSModeSaved == "Amazon Polly")
            {
                AmazonPollyTTS.SynthesisGetAvailableVoices(comboBoxAccentSelect.Text.ToString());

            }
            if (DoSpeech.TTSModeSaved == "Google (Pro Only)")
            {
                GoogleTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString());

            }
            if (DoSpeech.TTSModeSaved == "Uberduck")
            {
                UberDuckTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString(), false);

            }
            if (DoSpeech.TTSModeSaved == "IBM Watson (Pro Only)")
            {
                IBMWatsonTTS.SynthesisGetAvailableVoicesAsync(comboBoxVoiceSelect, comboBoxAccentSelect.Text.ToString());

            }



        }
        private void trackBarPitch_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }
        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            updateAllTrackBarLabels();
        }
        public void updateAllTrackBarLabels()
        {
            //float value1 = 0.5f + trackBarPitch.Value * 0.1f;
            // labelPitchNum.Text = "x" + Math.Round(value1, 1).ToString();

            labelPitchNum.Text = trackBarPitch.Value + "%";


            //  float value2 = 0.5f + trackBarSpeed.Value * 0.1f;
            //  labelSpeedNum.Text =  Math.Round(value2, 1).ToString();

            labelSpeedNum.Text = trackBarSpeed.Value + "%";

            float value3 = trackBarVolume.Value * 0.1f;
            labelVolumeNum.Text = (Math.Round(value3, 1) * 100).ToString("0.#") + "%";

            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";
        }

        #region Voice Presets
        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)//preset selected
        {
            if (comboBoxPreset.SelectedIndex == 0)
            {
                buttonEditPreset.Enabled = false;
                buttonDeletePreset.Enabled = false;
            }
            else
            {
                buttonEditPreset.Enabled = true;
                buttonDeletePreset.Enabled = true;
                Task.Run(() => VoicePresets.setPreset());
            }
            Settings1.Default.saveVoicePresetIndex = comboBoxPreset.SelectedIndex;
            Settings1.Default.Save();




        }

        private void button15_Click(object sender, EventArgs e)
        {
            VoicePresets.presetSaveButton();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            VoicePresets.presetEditButton();
        }
        private void button25_Click_1(object sender, EventArgs e)
        {
            VoicePresets.presetDeleteButton();

        }
        #endregion


        #endregion

        #endregion

        #region TextToText Tab

        private void richTextBox9_TextChanged(object sender, EventArgs e)
        {
            OutputText.typingBox = true;
            var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
            OSC.OSCSender.Send(typingbubble);
        }

        private void iconButton22_Click(object sender, EventArgs e)
        {
            ClearTypingBox();
        }


        #endregion

        #region Settings Tab
        #region General Settings

        private void iconButton2_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Settings");
        }

        private void button14_Click_2(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "Output\\Exports");
        }



        private void rjToggleButtonLog_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonClear_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonSystemTray_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonStopCurrentTTS_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBoxQueueDelayBeforeNext_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxDelayAfterNoTTS_TextChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonQueueTypedText_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonRefocus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }



        private void buttonQuickTypeEdit_Click(object sender, EventArgs e)
        {
            textBoxQuickType1.Clear();
            textBoxQuickType2.Clear();
            buttonQuickTypeSave.Enabled = true;
            buttonQuickTypeEdit.Enabled = false;
            textBoxQuickType1.Enabled = true;
            textBoxQuickType2.Enabled = true;
        }

        private void buttonQuickTypeSave_Click(object sender, EventArgs e)
        {
            textBoxQuickType1.Enabled = false;
            textBoxQuickType2.Enabled = false;
            buttonQuickTypeSave.Enabled = false;
            buttonQuickTypeEdit.Enabled = true;
            Hotkeys.modifierKeyQuickType = textBoxQuickType1.Text.ToString();
            Hotkeys.normalKeyQuickType = textBoxQuickType2.Text.ToString();
            Hotkeys.UnregisterHotKey(this.Handle, 2);
            Hotkeys.CUSTOMRegisterHotKey(2, Hotkeys.modifierKeyQuickType, Hotkeys.normalKeyQuickType);
        }


        private void rjToggleButtonQueueSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonQueueSystem.Checked == true)
            {
                label81.Visible = true;
                labelQueueSize.Visible = true;
                buttonQueueClear.Visible = true;
            }
            else
            {
                label81.Visible = false;
                labelQueueSize.Visible = false;
                buttonQueueClear.Visible = false;
            }

        }

        private void rjToggleButton6_CheckedChanged(object sender, EventArgs e)//Minimize Taskbar
        {
            if (rjToggleButton6.Checked == true)
            {
                panel1.SetBounds(0, 0, 65, 731);
                panel2Logo.SetBounds(0, 0, 220, 55);
                pictureBox1.SetBounds(0, 0, 55, 55);
                navbarHome.Text = "";
                navbarTextToSpeech.Text = "";
                navbarTextToText.Text = "";
                navbarSettings.Text = "";
                navbarIntegrations.Text = "";
                navbarDiscord.Text = "";
                navbarGithub.Text = "";
                navbarDonate.Text = "";
                navbarUpdates.Text = "";
                navbarSpeechProvider.Text = "";
            }
            if (rjToggleButton6.Checked == false)
            {
                panel1.SetBounds(0, 0, 159, 731);
                panel2Logo.SetBounds(0, 0, 159, 105);
                pictureBox1.SetBounds(12, -8, 129, 113);
                navbarHome.Text = "Home";
                navbarTextToSpeech.Text = "Text to Speech";
                navbarTextToText.Text = "Text to Text";
                navbarSpeechProvider.Text = "Speech Provider";
                navbarSettings.Text = "Settings";
                navbarIntegrations.Text = "Addon";
                navbarDiscord.Text = "Discord";
                navbarGithub.Text = "Github";
                navbarDonate.Text = "Donate";
                navbarUpdates.Text = "Update";
                navbarUpdates.Text = "Speech Provider";

            }


        }


        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonOnTop2.Checked == true)
            {
                TopMost = true;

            }
            if (rjToggleButtonOnTop2.Checked == false)
            {
                TopMost = false;

            }
        }

        private void button47_Click(object sender, EventArgs e) //Open Config Folder
        {
            var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TTSVoiceWizard");
            Process.Start("explorer.exe", appPath);
        }

        private void button38_Click_1(object sender, EventArgs e) //Manually Save Config
        {
            SaveSettings.SavingSettings();
        }

        private void rjToggleButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton9.Checked == true)
            {
                Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
            }
            if (rjToggleButton9.Checked == false)
            {
                Hotkeys.UnregisterHotKey(this.Handle, 0);
            }

        }
        private void button28_Click(object sender, EventArgs e)//STTTS hotkey edit key
        {
            textBox4.Clear();
            textBox1.Clear();
            button27.Enabled = true;
            button28.Enabled = false;
            textBox1.Enabled = true;
            textBox4.Enabled = true;
        }
        private void button27_Click(object sender, EventArgs e)//STTTS hotkey save key
        {
            textBox1.Enabled = false;
            textBox4.Enabled = false;
            button27.Enabled = false;
            button28.Enabled = true;
            Hotkeys.modifierKeySTTTS = textBox4.Text.ToString();
            Hotkeys.normalKeySTTTS = textBox1.Text.ToString();
            Hotkeys.UnregisterHotKey(this.Handle, 0);
            Hotkeys.CUSTOMRegisterHotKey(0, Hotkeys.modifierKeySTTTS, Hotkeys.normalKeySTTTS);
        }
        private void button39_Click(object sender, EventArgs e)
        {
            textBoxStopTTS1.Clear();
            textBoxStopTTS2.Clear();
            button40.Enabled = true;
            button39.Enabled = false;
            textBoxStopTTS1.Enabled = true;
            textBoxStopTTS2.Enabled = true;
        }

        private void button40_Click(object sender, EventArgs e)
        {
            textBoxStopTTS1.Enabled = false;
            textBoxStopTTS2.Enabled = false;
            button40.Enabled = false;
            button39.Enabled = true;
            Hotkeys.modifierKeyStopTTS = textBoxStopTTS1.Text.ToString();
            Hotkeys.normalKeyStopTTS = textBoxStopTTS2.Text.ToString();
            Hotkeys.UnregisterHotKey(this.Handle, 1);
            Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
        }

        private void rjToggleButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton12.Checked == true)
            {
                Hotkeys.CUSTOMRegisterHotKey(1, Hotkeys.modifierKeyStopTTS, Hotkeys.normalKeyStopTTS);
            }
            if (rjToggleButton12.Checked == false)
            {
                Hotkeys.UnregisterHotKey(this.Handle, 1);
            }
        }

        private void textBoxStopTTS1_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBoxStopTTS1.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;
                textBoxStopTTS1.Text = modifierKeys.ToString();

            }
        }

        private void textBoxStopTTS2_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBoxStopTTS2.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;
                Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys
                var converter = new KeysConverter();
                textBoxStopTTS2.Text = converter.ConvertToString(pressedKey);

            }
        }

        private void rjToggleButtonDisableWindowsMedia_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.WindowsMediaDisable = rjToggleButtonDisableWindowsMedia.Checked;
            Settings1.Default.Save();

            OutputText.outputLog("Restart required for changes to take effect (Disabling Windows Media may solve 'random' crashing). If you just restarted, Windows Media mode is already disabled so disregard this message.", Color.DarkOrange);

        }

        #region DarkMode
        Color DarkModeColor = Color.FromArgb(31, 30, 68);
        Color LightModeColor = Color.FromArgb(68, 72, 111);
        private void rjToggleButton7_CheckedChanged_1(object sender, EventArgs e) //Dark Mode
        {


            if (rjToggleDarkMode.Checked == true)//dark mode
            {
                DarkTitleBarClass.UseImmersiveDarkMode(Handle, true);
                foreach (var thisControl in GetAllChildren(this).OfType<TextBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<RichTextBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<ComboBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<CheckedListBox>())
                {
                    thisControl.BackColor = DarkModeColor;
                    thisControl.ForeColor = Color.White;
                }
                labelCharCount.BackColor = DarkModeColor;
                ttsTrash.BackColor = DarkModeColor;
                logTrash.BackColor = DarkModeColor;
                iconButton22.BackColor = DarkModeColor;
                iconButton36.BackColor = DarkModeColor;

                iconButton37.BackColor = DarkModeColor;

                ShowAmazonKeyPassword.BackColor = DarkModeColor;
                ShowDeepLPassword.BackColor = DarkModeColor;
                ShowAmazonSecretPassword.BackColor = DarkModeColor;
                ShowAzurePassword.BackColor = DarkModeColor;
                ShowElevenLabsPassword.BackColor = DarkModeColor;
                ShowSpotifyPassword.BackColor = DarkModeColor;
                ProShowKey.BackColor = DarkModeColor;
                UberDuckShowPassword.BackColor = DarkModeColor;
                UberDuckShowSecretPassword.BackColor = DarkModeColor;



                labelCharCount.ForeColor = Color.White;
                ttsTrash.IconColor = Color.White;
                logTrash.IconColor = Color.White;
                iconButton22.IconColor = Color.White;

                iconButton36.IconColor = Color.White;
                iconButton37.IconColor = Color.White;
                iconButton36.ForeColor = Color.White;
                iconButton37.ForeColor = Color.White;

                ShowAmazonKeyPassword.IconColor = Color.White;
                ShowDeepLPassword.IconColor = Color.White;
                ShowAmazonSecretPassword.IconColor = Color.White;
                ShowAzurePassword.IconColor = Color.White;
                ShowElevenLabsPassword.IconColor = Color.White;
                ShowSpotifyPassword.IconColor = Color.White;
                ProShowKey.IconColor = Color.White;
                UberDuckShowPassword.IconColor = Color.White;
                UberDuckShowSecretPassword.IconColor = Color.White;

            }
            if (rjToggleDarkMode.Checked == false)//light mode
            {
                DarkTitleBarClass.UseImmersiveDarkMode(Handle, false);
                foreach (var thisControl in GetAllChildren(this).OfType<TextBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<RichTextBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<ComboBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<CheckedListBox>())
                {
                    thisControl.BackColor = Color.White;
                    thisControl.ForeColor = Color.Black;
                }
                labelCharCount.BackColor = Color.White;
                ttsTrash.BackColor = Color.White;
                logTrash.BackColor = Color.White;
                iconButton22.BackColor = Color.White;

                iconButton36.BackColor = Color.White;
                iconButton37.BackColor = Color.White;



                ShowAmazonKeyPassword.BackColor = Color.White;
                ShowDeepLPassword.BackColor = Color.White;
                ShowAmazonSecretPassword.BackColor = Color.White;
                ShowAzurePassword.BackColor = Color.White;
                ShowElevenLabsPassword.BackColor = Color.White;
                ShowSpotifyPassword.BackColor = Color.White;
                ProShowKey.BackColor = Color.White;
                UberDuckShowPassword.BackColor = Color.White;
                UberDuckShowSecretPassword.BackColor = Color.White;



                labelCharCount.ForeColor = LightModeColor;
                ttsTrash.IconColor = LightModeColor;
                logTrash.IconColor = LightModeColor;
                iconButton22.IconColor = LightModeColor;

                iconButton36.IconColor = LightModeColor;
                iconButton37.IconColor = LightModeColor;
                iconButton36.ForeColor = LightModeColor;
                iconButton37.ForeColor = LightModeColor;

                richTextBox4.BackColor = LightModeColor;
                richTextBox4.ForeColor = Color.White;


                ShowAmazonKeyPassword.IconColor = LightModeColor;
                ShowDeepLPassword.IconColor = LightModeColor;
                ShowAmazonSecretPassword.IconColor = LightModeColor;
                ShowAzurePassword.IconColor = LightModeColor;
                ShowElevenLabsPassword.IconColor = LightModeColor;
                ShowSpotifyPassword.IconColor = LightModeColor;
                ProShowKey.IconColor = LightModeColor;
                UberDuckShowPassword.IconColor = LightModeColor;
                UberDuckShowSecretPassword.IconColor = LightModeColor;



            }
        }
        public static IEnumerable<Control> GetAllChildren(Control root)//Dark Mode Helper
        {
            var stack = new Stack<Control>();
            stack.Push(root);

            while (stack.Any())
            {
                var next = stack.Pop();
                foreach (Control child in next.Controls)
                    stack.Push(child);
                yield return next;
            }
        }
        #endregion


        #endregion

        #region Audio Section

        private void iconButton41_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide#speech-to-text-and-tts");
        }

        private void iconButton16_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Virtual-Cable");
        }

        private void button43_Click(object sender, EventArgs e)
        {
            try
            {
                AudioDevices.NAudioSetupInputDevices();
                AudioDevices.NAudioSetupOutputDevices();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Audio Device Startup Error: " + ex.Message);
            }

        }
        private void comboBoxInput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            AudioDevices.currentInputDevice = AudioDevices.micIDs[comboBoxInput.SelectedIndex];
            AudioDevices.currentInputDeviceName = comboBoxInput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("mic changed", AudioDevices.currentInputDevice);
        }

        private void comboBoxOutput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            AudioDevices.currentOutputDevice = AudioDevices.speakerIDs[comboBoxOutput.SelectedIndex];
            AudioDevices.currentOutputDeviceName = comboBoxOutput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }

        private void comboBoxOutput2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevices.currentOutputDevice2nd = AudioDevices.speakerIDs[comboBoxOutput2.SelectedIndex];
            AudioDevices.currentOutputDeviceName2nd = comboBoxOutput2.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }

        public static void StopAllRecogntion()
        {
            Task.Run(() => SystemSpeechRecognition.AutoStopSystemSpeechRecog());
            Task.Run(() => WebCaptionerRecognition.autoStopWebCap());
            Task.Run(() => AzureRecognition.stopContinuousListeningNow());//turns of continuous if it is on
            Task.Run(() => VoskRecognition.AutoStopVoskRecog());
            Task.Run(() => WhisperRecognition.autoStopWhisper());
        }


        private void comboBoxSTT_SelectedIndexChanged(object sender, EventArgs e)// this is used to auto turn off other speech to text option if you switch
        {
            try
            {
                StopAllRecogntion();
                //  DoSpeech.speechToTextOffSound();
            }
            catch
            {
                OutputText.outputLog("Could not automatically stop your continuous recognition for previous STT Mode. Make sure to disable it manually by swapping back and pressing the 'Speech to Text to Speech' button or it will keep running in the background and give you 'double speech'!", Color.DarkOrange);
            }

            switch (comboBoxSTT.Text.ToString())
            {
                case "Whisper":
                    if (whisperModelTextBox.Text.ToString() == "no model selected")
                    {
                        OutputText.outputLog("[Whisper selected for Speech to Text (Voice Recognition). SETUP GUIDE: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Whisper ]", Color.DarkOrange);

                    }
                    iconButtonMute.Visible = true;
                    WhisperDebugLabel.Visible = true;
                    break;

                case "Web Captioner": OutputText.outputLog("[Web Captioner selected for Speech to Text (Voice Recognition). SETUP GUIDE: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Web-Captioner ]", Color.DarkOrange); break;

                case "Vosk":
                    if (modelTextBox.Text.ToString() == "no folder selected")
                    {
                        OutputText.outputLog("[Vosk selected for Speech to Text (Voice Recognition). SETUP GUIDE: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Vosk ]", Color.DarkOrange);
                    }
                    break;

                //  case "System Speech": OutputText.outputLog("[System Speech selected for Speech to Text (Voice Recognition).]", Color.DarkOrange); break;
                case "Azure":
                    if (textBoxAzureKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[Azure selected for Speech to Text (Voice Recognition). SETUP GUIDE: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                    }
                    break;
                default:

                    break;
            }

        }

        private void rjToggleButtonSounds_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonUse2ndOutput_CheckedChanged(object sender, EventArgs e)
        {

        }




        #endregion

        #region General Text Section

        private void rjToggleButtonHideDelay2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (OutputText.katRefreshTimer != null)
                {
                    if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked == true)
                    {
                        OutputText.katRefreshTimer.Change(2000, 0);
                    }
                }
            }
            catch
            {
                OutputText.outputLog("KAT Timer was not initalized yet", Color.Red);
            }

        }

        private void button46_Click(object sender, EventArgs e)
        {
            var appPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + @"\VRChat\VRChat", "OSC");
            Process.Start("explorer.exe", appPath);
            Debug.WriteLine(appPath);

        }

        private void buttonErase_Click(object sender, EventArgs e)
        {

            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.eraseDelay = Int32.Parse(textBoxErase.Text.ToString());
                Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
                Settings1.Default.Save();
            });
        }

        private void button16_Click(object sender, EventArgs e)
        {
            OSC.ChangeAddressAndPort(textBoxOSCAddress.Text.ToString(), textBoxOSCPort.Text.ToString());
            Settings1.Default.rememberPort = textBoxOSCPort.Text.ToString();
            Settings1.Default.Save();

        }

        private void button17_Click(object sender, EventArgs e)
        {
            OSC.ChangeAddressAndPort(textBoxOSCAddress.Text.ToString(), textBoxOSCPort.Text.ToString());
            Settings1.Default.rememberAddress = textBoxOSCAddress.Text.ToString();
            Settings1.Default.Save();
        }

        #endregion

        #region Chatbox Section

        private void rjToggleButtonChatBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonShowKeyboard_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleSoundNotification_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonChatBoxUseDelay_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Killfrenzy Avatar Text Section

        private void rjToggleButtonAutoRefreshKAT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (OutputText.katRefreshTimer != null)
                {
                    if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked == true)
                    {
                        OutputText.katRefreshTimer.Change(2000, 0);
                    }
                }
            }
            catch
            {
                OutputText.outputLog("KAT Timer was not initalized yet", Color.Red);
            }
        }



        private void comboBoxPara_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.numKATSyncParameters = comboBoxPara.SelectedItem.ToString();
                Settings1.Default.SyncParaValue = comboBoxPara.SelectedIndex;

                Settings1.Default.Save();
            });

        }

        private void textBoxDelay_TextChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonOSC_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void buttonDelayHere_Click(object sender, EventArgs e)//speech to text
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.debugDelayValue = Int32.Parse(textBoxDelay.Text.ToString());
                Settings1.Default.delayDebugValueSetting = textBoxDelay.Text.ToString();
                Settings1.Default.Save();


            });
        }


        private void button12_Click(object sender, EventArgs e)//clear
        {
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
            OutputText.lastKatString = "";


            OSC.OSCSender.Send(message0);
        }

        private void hideVRCTextButton_Click(object sender, EventArgs e)//hide
        {
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            OSC.OSCSender.Send(message0);
        }


        private void button2_Click_1(object sender, EventArgs e)//replay
        {
            OutputText.outputVRChat(OutputText.lastKatString, "tttAdd");
        }

        #endregion

        #region Output Section

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "Output\\TextOutput");
        }

        private void OBSLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide#obs-text-for-streaming-and-recording-videos");

        }

        private void rjToggleButtonOBSText_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonSaveToWav_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion


        #endregion

        #region Integrations Tab

        #region Tab Selection

        private void iconButton9_Click(object sender, EventArgs e) //Media Integration
        {
            mainTabControl.SelectTab(tabSpotify);


        }

        private void iconButton10_Click(object sender, EventArgs e)//OSC Listener
        {
            // richTextBox8.Text = richTextBox1.Text;
            mainTabControl.SelectTab(tabHeartBeat);

        }

        private void iconButton24_Click(object sender, EventArgs e) //VRChat Listener
        {
            mainTabControl.SelectTab(VRCOSC);
        }

        private void iconButton27_Click_1(object sender, EventArgs e) //VoiceCommands
        {
            mainTabControl.SelectTab(tabPage2);//voiceCommands
        }


        private void iconButton42_Click(object sender, EventArgs e) // Word Replacements
        {
            mainTabControl.SelectTab(Replacements);
        }

        private void iconButton25_Click(object sender, EventArgs e) // Discord OSC
        {
            mainTabControl.SelectTab(discordTab);//discord
        }


        private void iconButton11_Click(object sender, EventArgs e) //Emoji
        {
            mainTabControl.SelectTab(tabEmoji);
        }

        #endregion

        #region Media Integration

        private void button14_Click_1(object sender, EventArgs e)
        {
            WindowsMedia.addSoundPad();
        }

        private void iconButton1_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/6-zFSiRFu-A");
        }

        private void ShowSpotifyPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxSpotKey, ShowSpotifyPassword);
        }

        private void iconButton31_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Media-Setup");

        }

        private void rjToggleSpotLegacy_CheckedChanged(object sender, EventArgs e)
        {
            SpotifyAddon.legacyState = rjToggleSpotLegacy.Checked;
        }

        private void textBoxSpotKey_TextChanged(object sender, EventArgs e)
        {
            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.Save();
        }


        private void rjToggleButtonCurrentSong_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonCurrentSong.Checked == true)  //instead of disabling other toggle, when new toggle is used it turns off the other one
            {
                rjToggleButtonWindowsMedia.Checked = false;
            }
            if (rjToggleButtonCurrentSong.Checked == false)  //instead of disabling other toggle, when new toggle is used it turns off the other one
            {
                VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.White;
            }
        }

        private void buttonSpotify_Click(object sender, EventArgs e)
        {
            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.Save();
            SpotifyAddon.SpotifyConnect();

            VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Green;


        }

        private void button44_Click(object sender, EventArgs e)
        {
            var currentTime = "Current Time ⏰: " + DateTime.Now.ToString("h:mm tt");
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true)
            {
                Task.Run(() => OutputText.outputLog(currentTime));

            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked == false)
            {

                Task.Run(() => OutputText.outputVRChat(currentTime, "time"));
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
            {
                Task.Run(() => OutputText.outputVRChatSpeechBubbles(currentTime, "time")); //original

            }
        }

        private void buttonMediaPresetSaveNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetSaveButton();
        }

        private void buttonMediaPresetEditNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetEditButton();
        }

        private void buttonMediaPresetDeleteNew_Click(object sender, EventArgs e)
        {
            MediaPresets.presetDeleteButton();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SpotifyAddon.ChangeMediaUpdateInterval();
        }


        private void rjToggleButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonWindowsMedia.Checked == true)
            {
                rjToggleButtonCurrentSong.Checked = false;
            }

        }
        private void button20_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "{pause} {title} - {artist} ";
            textBoxCustomSpot.Text = currentText;
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『{progressMinutes}/{durationMinutes}』 ";
            textBoxCustomSpot.Text = currentText;

        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『🎮{averageControllerBattery}%』『🔋{averageTrackerBattery}%{TCharge}』 ";
            textBoxCustomSpot.Text = currentText;

        }

        private void button22_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『💓{bpm}{bpmStats}』 ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "📢{spotifyVolume}% ";
            textBoxCustomSpot.Text = currentText;
        }

        private void comboBoxMediaPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxMediaPreset.SelectedIndex == 0)
            {
                buttonMediaPresetEditNew.Enabled = false;
                buttonMediaPresetDeleteNew.Enabled = false;
            }
            else
            {
                buttonMediaPresetEditNew.Enabled = true;
                buttonMediaPresetDeleteNew.Enabled = true;
                Task.Run(() => MediaPresets.setPreset());
            }


        }
        private void buttonImportMedia_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                comboBoxMediaPreset.Items.Clear();
                comboBoxMediaPreset.Items.Add("- None Selected -");
                MediaPresets.mediaPresetsStored = Import_Export.importFile(openFileDialog1.FileName);
                MediaPresets.presetsLoad();
                comboBoxMediaPreset.SelectedIndex = 0;

            }
        }

        private void buttonExportMedia_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "MediaPreset", MediaPresets.mediaPresetsStored);
        }

        #endregion

        #region OSCListener

        private void rjToggleButton8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonForwardData_CheckedChanged(object sender, EventArgs e)
        {

        }


        private void iconButton39_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/OSC-Listener");
        }
        private void rjToggleButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton1.Checked == true)
            {

                OSCListener.stopBPM = false;
            }
            if (rjToggleButton1.Checked == false)
            {
                OSCListener.stopBPM = true;

            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => OSCListener.OSCRecieveHeartRate());
            }
            catch (Exception ex) { OutputText.outputLog("[OSC Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }


        }
        private void rjToggleButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton2.Checked == true)
            {
                OSCListener.OSCReceiveSpamLog = true;
            }
            if (rjToggleButton2.Checked == false)
            {
                OSCListener.OSCReceiveSpamLog = false;
            }
        }

        private void HRIntervalChange_Click(object sender, EventArgs e)
        {
            OSCListener.HRInternalValue = Int32.Parse(HRInterval.Text.ToString());
        }



        private void button8_Click(object sender, EventArgs e)
        {
            OSCListener.OSCReceiveport = Convert.ToInt32(textBoxHRPort.Text.ToString());

        }

        #endregion

        #region VRChat Listener

        private void iconButton47_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/VRChat-Listener");
        }

        private void rjToggleButtonResetButtonsCounter_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonResetButtonsCounter.Checked == true)
            {
                buttonResetCounter1.Enabled = true;
                buttonResetCounter2.Enabled = true;
                buttonResetCounter3.Enabled = true;
                buttonResetCounter4.Enabled = true;
                buttonResetCounter5.Enabled = true;
                buttonResetCounter6.Enabled = true;
                buttonResetCounterAll.Enabled = true;
            }
            else
            {
                buttonResetCounter1.Enabled = false;
                buttonResetCounter2.Enabled = false;
                buttonResetCounter3.Enabled = false;
                buttonResetCounter4.Enabled = false;
                buttonResetCounter5.Enabled = false;
                buttonResetCounter6.Enabled = false;
                buttonResetCounterAll.Enabled = false;
            }
        }

        private void buttonResetCounter1_Click(object sender, EventArgs e)
        {
            VRChatListener.counter1 = 0;
            VRChatListener.prevCounter1 = 0;
            Settings1.Default.Counter1 = VRChatListener.counter1;
            Settings1.Default.Save();
        }

        private void buttonResetCounter2_Click(object sender, EventArgs e)
        {
            VRChatListener.counter2 = 0;
            VRChatListener.prevCounter2 = 0;
            Settings1.Default.Counter2 = VRChatListener.counter2;
            Settings1.Default.Save();
        }

        private void buttonResetCounter3_Click(object sender, EventArgs e)
        {
            VRChatListener.counter3 = 0;
            VRChatListener.prevCounter3 = 0;
            Settings1.Default.Counter3 = VRChatListener.counter3;
            Settings1.Default.Save();
        }

        private void buttonResetCounter4_Click(object sender, EventArgs e)
        {
            VRChatListener.counter4 = 0;
            VRChatListener.prevCounter4 = 0;
            Settings1.Default.Counter4 = VRChatListener.counter4;
            Settings1.Default.Save();
        }

        private void buttonResetCounter5_Click(object sender, EventArgs e)
        {
            VRChatListener.counter5 = 0;
            VRChatListener.prevCounter5 = 0;
            Settings1.Default.Counter5 = VRChatListener.counter5;
            Settings1.Default.Save();
        }

        private void buttonResetCounter6_Click(object sender, EventArgs e)
        {
            VRChatListener.counter6 = 0;
            VRChatListener.prevCounter6 = 0;
            Settings1.Default.Counter6 = VRChatListener.counter6;
            Settings1.Default.Save();
        }



        private void button32_Click(object sender, EventArgs e)
        {
            VRChatListener.FromVRChatPort = textBoxVRChatOSCPort.Text.ToString();
        }

        private void button33_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => VRChatListener.OSCLegacyVRChatListener());
            }
            catch (Exception ex) { OutputText.outputLog("[VRChat OSC Listener Error: " + ex.Message + " ]", Color.Red); }

        }

        private void button36_Click(object sender, EventArgs e)
        {
            VRChatListener.counter1 = 0;
            VRChatListener.counter2 = 0;
            VRChatListener.counter3 = 0;
            VRChatListener.counter4 = 0;
            VRChatListener.counter5 = 0;
            VRChatListener.counter6 = 0;

            VRChatListener.prevCounter1 = 0;
            VRChatListener.prevCounter2 = 0;
            VRChatListener.prevCounter3 = 0;
            VRChatListener.prevCounter4 = 0;
            VRChatListener.prevCounter5 = 0;
            VRChatListener.prevCounter6 = 0;


            Settings1.Default.Counter1 = VRChatListener.counter1;
            Settings1.Default.Counter2 = VRChatListener.counter2;
            Settings1.Default.Counter3 = VRChatListener.counter3;
            Settings1.Default.Counter4 = VRChatListener.counter4;
            Settings1.Default.Counter5 = VRChatListener.counter5;
            Settings1.Default.Counter6 = VRChatListener.counter6;
            Settings1.Default.Save();
        }


        #endregion

        #region Voice Commands

        private void buttonAddVoiceCommand_Click(object sender, EventArgs e)
        {
            VoiceCommands.clearVoiceCommands();
            VoiceCommands.voiceCommandsStored += $"{textBox1Spoken.Text.ToString()}:{textBox2Address.Text.ToString()}:{comboBox3Type.SelectedItem.ToString()}:{textBox4Value.Text.ToString()};";

            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();


        }

        private void button24_Click(object sender, EventArgs e)
        {
            if (deleteCommandsToggle.Checked == true)
            {
                VoiceCommands.clearVoiceCommands();
                VoiceCommands.refreshCommandList();
            }


        }

        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (deleteCommandsToggle.Checked == true)
            {
                try
                {
                    VoiceCommands.removeVoiceCommandsAt(checkedListBox1.SelectedIndex);
                    VoiceCommands.refreshCommandList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }

        }

        private void iconButton40_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Voice-Commands");
        }

        private void buttonImportVC_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VoiceCommands.voiceCommandsStored = Import_Export.importFile(openFileDialog1.FileName);
                VoiceCommands.voiceCommands();
                VoiceCommands.refreshCommandList();

            }
        }

        private void buttonExportVC_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "VoiceCommand", VoiceCommands.voiceCommandsStored);
        }



        #endregion

        #region Word Replacements


        private void buttonImportWR_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WordReplacements.wordReplacemntsStored = Import_Export.importFile(openFileDialog1.FileName);
                WordReplacements.replacementsLoad();
                //WordReplacements.

            }
        }

        private void buttonExportWR_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "WordReplacement", WordReplacements.wordReplacemntsStored);
        }

        private void checkedListBoxReplacements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (rjToggleButton7.Checked == true)
            {
                try
                {
                    WordReplacements.removeWordReplacementAt(checkedListBoxReplacements.SelectedIndex);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }

            }
        }

        private void buttonReplaceAdd_Click(object sender, EventArgs e)
        {

            WordReplacements.addWordReplacement(textBoxOriginalWord.Text.ToString(), textBoxReplaceWord.Text.ToString());
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            if (rjToggleButton7.Checked == true)
            {
                WordReplacements.clearWordReplacement();
            }


        }

        private void iconButton45_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Word-Replacements");
        }


        #endregion

        #region Discord Integration

        private void button15_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://shadoki.booth.pm/items/4467967");
        }


        private void iconButton44_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Discord-Integration");
        }


        #endregion

        #region Emoji Tab

        private void buttonImportEmoji_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                checkedListBox2.Items.Clear();
                EmojiAddon.emojiReplacemntsStored = Import_Export.importFile(openFileDialog1.FileName);
                EmojiAddon.emojiReplacementsLoad();


            }
        }

        private void buttonExportEmoji_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "Emojis", EmojiAddon.emojiReplacemntsStored);
        }


        private void iconButton32_Click(object sender, EventArgs e)//help
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Emoji-Setup");
        }


        private void button26_Click(object sender, EventArgs e)// edit
        {
            EmojiAddon.emojiEdit(Int32.Parse(textBox7.Text.ToString()), textBox6.Text.ToString());
        }


        private void button25_Click_2(object sender, EventArgs e) // debug reset
        {
            checkedListBox2.Items.Clear();
            EmojiAddon.ReplacePhraseList.Clear();
            System.Windows.Forms.MessageBox.Show("Restart App");

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e) // selected
        {
            int index = checkedListBox2.SelectedIndex + 1;
            textBox7.Text = index.ToString();
        }

        #endregion


        #endregion

        #region Speech Provider Tab

        #region Tab Selection

        private void iconButton20_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AzureSet);//settings
        }




        private void iconButton50_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(VoiceWizPro);
        }

        private void iconButton52_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(uberduck);
        }
        private void iconButton21_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(DeepLTab);
        }

        private void iconButton19_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AmazonPolly);
        }

        private void iconButton30_Click_1(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(LocalSpeech);
        }



        private void iconButton28_Click_2(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(elevenLabs);
        }


        #endregion

        #region Voice Wizard Pro Tab

        private void ProShowKey_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxWizardProKey, ProShowKey);
        }
        private void trackBarSilence_Scroll(object sender, EventArgs e)
        {
            textBoxSilence.Text = ((int)Math.Floor((trackBarSilence.Value / .5)) * 10).ToString();
        }

        private void iconButton55_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/VoiceWizardPro");

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard/tiers#");
        }

        #endregion

        #region Azure Cognitive Services Tab

        private void buttonImportDict_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBoxAzureDict.Text = Import_Export.importFile(openFileDialog1.FileName);


            }
        }

        private void buttonExportDict_Click(object sender, EventArgs e)
        {
            Import_Export.ExportList("Output\\Exports", "AzureDictionary", richTextBoxAzureDict.Text);
        }


        private void ShowAzurePassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAzureKey, ShowAzurePassword);
        }

        private void rjToggleButton4_CheckedChanged(object sender, EventArgs e)
        {
            AzureRecognition.stopContinuousListeningNow();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBoxAzureKey.Text.ToString();
                AzureRecognition.YourSubscriptionKey = text;
                if (rjToggleButtonKeyRegion2.Checked == true)
                {
                    Settings1.Default.yourKey = text;
                    Settings1.Default.Save();
                }
            });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox3.Text.ToString();
                AzureRecognition.YourServiceRegion = text;
                if (rjToggleButtonKeyRegion2.Checked == true)
                {
                    Settings1.Default.yourRegion = text;
                    Settings1.Default.Save();

                }
            });
        }



        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = rjToggleButtonKeyRegion2.Checked;
            Settings1.Default.Save();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonProfan.Checked == true)
            {
                AzureRecognition.profanityFilter = true;
            }
            if (rjToggleButtonProfan.Checked == false)
            {
                AzureRecognition.profanityFilter = false;
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            AzureTTS.SynthesisGetAvailableVoicesAsync(comboBoxAccentSelect.Text.ToString());

        }

        private void iconButton29_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service");
        }

        #endregion

        #region Amazon Polly Tab

        private void ShowAmazonSecretPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAmazonSecret, ShowAmazonSecretPassword);
        }
        private void ShowAmazonKeyPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxAmazonKey, ShowAmazonKeyPassword);
        }

        private void iconButton18_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Amazon-Polly");
        }

        private void button31_Click(object sender, EventArgs e)//access key
        {

            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxAmazonKey.Text.ToString();
                Settings1.Default.yourAWSKey = text;
                Settings1.Default.Save();

            });
        }

        private void button29_Click(object sender, EventArgs e)//secret key
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxAmazonSecret.Text.ToString();
                Settings1.Default.yourAWSSecret = text;
                Settings1.Default.Save();

            });

        }

        private void button30_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBox8.Text.ToString();
                Settings1.Default.yourAWSRegion = text;
                Settings1.Default.Save();

            });
        }

        #endregion

        #region Translation Tab

        private void ShowDeepLPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxDeepLKey, ShowDeepLPassword);
        }

        private void iconButton43_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/DeepL-Translation-API");
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            Settings1.Default.deepLKeysave = textBoxDeepLKey.Text.ToString();
            Settings1.Default.Save();
        }

        #endregion

        #region ElevenLabs Tab

        private void ShowElevenLabsPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxElevenLabsKey, ShowElevenLabsPassword);
        }


        private void iconButton54_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://docs.elevenlabs.io/api-reference/text-to-speech");
        }

        private void trackBarStability_Scroll(object sender, EventArgs e)
        {

            labelStability.Text = trackBarStability.Value + "%";
        }

        private void trackBarSimilarity_Scroll(object sender, EventArgs e)
        {

            labelSimboost.Text = trackBarSimilarity.Value + "%";
        }

        private void button51_Click(object sender, EventArgs e)
        {
            comboBoxLabsModelID.SelectedIndex = 0;
            comboBoxLabsOptimize.SelectedIndex = 0;
            trackBarStability.Value = 75;
            trackBarSimilarity.Value = 75;

            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";
        }


        private void iconButton35_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/ElevenLabs-TTS");
        }



        private void button37_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBoxElevenLabsKey.Text.ToString();
                Settings1.Default.elevenLabsAPIKey = text;
                Settings1.Default.Save();

            });
        }


        private void button35_Click(object sender, EventArgs e)
        {
            ElevenLabsTTS.CallElevenVoices();
            if (comboBoxTTSMode.SelectedItem.ToString() == "ElevenLabs")
            {

                comboBoxVoiceSelect.Items.Clear();

                try
                {
                    if (ElevenLabsTTS.elevenFirstLoad == true)
                    {
                        ElevenLabsTTS.CallElevenVoices();
                    }

                    if (ElevenLabsTTS.voiceDict != null)
                    {
                        foreach (KeyValuePair<string, string> kvp in ElevenLabsTTS.voiceDict)
                        {
                            comboBoxVoiceSelect.Items.Add(kvp.Value);

                        }
                    }
                    else
                    {
                        comboBoxVoiceSelect.Items.Add("error");
                    }
                }
                catch (Exception ex)
                {
                    comboBoxVoiceSelect.Items.Add("error");
                    OutputText.outputLog("[ElevenLabs Load2 Error: " + ex.Message + "]", Color.Red);

                }

                comboBoxVoiceSelect.SelectedIndex = 0;

                comboBoxStyleSelect.SelectedIndex = 0;
                comboBoxStyleSelect.Enabled = false;
                comboBoxVoiceSelect.Enabled = true;
                comboBoxTranslationLanguage.Enabled = true;
                comboBoxAccentSelect.Enabled = false;
                trackBarPitch.Enabled = false;
                trackBarVolume.Enabled = false;
                trackBarSpeed.Enabled = false;
                DoSpeech.TTSModeSaved = "ElevenLabs";
            }
        }

        #endregion

        #region Uberduck Tab

        private void UberDuckShowPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxUberKey, UberDuckShowPassword);
        }

        private void UberDuckShowSecretPassword_Click(object sender, EventArgs e)
        {
            ShowHidePassword(textBoxUberSecret, UberDuckShowSecretPassword);
        }

        private void iconButton53_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Uberduck-TTS");
        }

        #endregion

        #region Local Tab

        private void button48_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxReadFromTXTFile.Text = openFileDialog1.FileName;
            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            string path = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text.ToString();

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = path;
            string absPath = Path.Combine(basePath, relativePath);

            TextFileReader.FileToTTS(absPath);
        }

        private void rjToggleButton14_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonReadFromFile.Checked == true)
            {
                TextFileReader.ReadFromFile();
            }
            else
            {
                TextFileReader.StopWatcher();
            }

        }



        private void button11_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                modelTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text = "1.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text = "8.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text = "0.25";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text = "1.0";
        }


        private void button42_Click(object sender, EventArgs e)
        {
            WhisperRecognition.downloadWhisperModel();
        }


        private void comboBoxWhisperModelDownload_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = "Assets/models/";
            switch (comboBoxWhisperModelDownload.Text.ToString())
            {
                case "ggml-tiny.bin (75 MB)":
                    path += "ggml-tiny.bin";
                    break;

                case "ggml-base.bin (142 MB)":
                    path += "ggml-base.bin";
                    break;

                case "ggml-small.bin (466 MB)":
                    path += "ggml-small.bin";
                    break;

                case "ggml-medium.bin (1.5 GB)":
                    path += "ggml-medium.bin";
                    break;

                default: break;
            }
            if (System.IO.File.Exists(path))
            {
                modelLabel.ForeColor = Color.Green;
                modelLabel.Text = "model downloaded";

            }
            else
            {
                modelLabel.ForeColor = Color.Red;
                modelLabel.Text = "model not downloaded";

            }


        }


        private void button34_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                whisperModelTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void voskLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Vosk");
        }

        private void whisperLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Whisper");

        }

        #endregion
        #endregion
    }





}
