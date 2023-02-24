//Wizard


using System.Media;
using System.Net;
using Resources; //for darktitle
using OSCVRCWiz.Settings;
using Octokit;
using System.Linq;
using System.Diagnostics;
using OSCVRCWiz.TTS;
using OSCVRCWiz.Addons;
using TTS;
using OSCVRCWiz.Text;
using OSCVRCWiz.TranslationAPIs;
using System.Reflection;
using OSCVRCWiz.Resources;
using Addons;
using NAudio.Wave;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System;
using Settings;
using System.Windows.Forms;
using AutoUpdaterDotNET;
using OSCVRCWiz.Speech_Recognition;
using static System.Net.Mime.MediaTypeNames;


//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public partial class VoiceWizardWindow : Form
    {
        public static string currentVersion = "1.0.1.6";
        string releaseDate = "February 24, 2023";
        string versionBuild = "x64"; //update when converting to x86/x64
        //string versionBuild = "x86"; //update when converting to x86/x64
        string updateXMLName = "https://github.com/VRCWizard/TTS-Voice-Wizard/releases/latest/download/AutoUpdater-x64.xml"; //update when converting to x86/x64
      //  string updateXMLName = "https://github.com/VRCWizard/TTS-Voice-Wizard/releases/latest/download/AutoUpdater-x86.xml"; //update when converting to x86/x64
        //update build



        // public OutputText ot;
        //public GreenScreen pf;
        public static VoiceWizardWindow MainFormGlobal;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static string TTSModeSaved = "System Speech";
        public static string TTSBoxText = "";
        public static bool typingBox = false;

   
        public System.Threading.Timer hideTimer;
        public System.Threading.Timer toastTimer;
        public System.Threading.Timer typeTimer;

        public static System.Threading.Timer spotifyTimer;

        public System.Threading.Timer katRefreshTimer;

        public System.Threading.Timer VRCCounterTimer;

        public static System.Threading.Timer whisperTimer;

        public static string modifierKey = "Control";
        public static string normalKey = "G";






        public VoiceWizardWindow()
        {
            try
            {
                
                InitializeComponent();                         
            //    cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            //    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                MainFormGlobal = this;


                CUSTOMRegisterHotKey();
               AudioDevices.NAudioSetupInputDevices();
               AudioDevices.NAudioSetupOutputDevices();
              // AudioDevices.OuputDeviceGet();
               SystemSpeechTTS.getVoices();
               SystemSpeechRecognition.getInstalledRecogs();
               OSC.Start();



               // Startup Changes
               tabControl1.Dock = DockStyle.Fill;
               tabControl1.Appearance = TabAppearance.FlatButtons;
               tabControl1.ItemSize = new Size(0, 1);
               tabControl1.SizeMode = TabSizeMode.Fixed;
               hideTimer = new System.Threading.Timer(hidetimertick);
               hideTimer.Change(Timeout.Infinite, Timeout.Infinite);
               typeTimer = new System.Threading.Timer(typetimertick);
               typeTimer.Change(1500, 0);

               toastTimer = new System.Threading.Timer(toasttimertick);
               toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
               katRefreshTimer = new System.Threading.Timer(katRefreshtimertick);
               katRefreshTimer.Change(2000, 0);

                VRCCounterTimer = new System.Threading.Timer(VRCCountertimertick);
                VRCCounterTimer.Change(1600, 0);

                whisperTimer = new System.Threading.Timer(whispertimertick);
                whisperTimer.Change(Timeout.Infinite, Timeout.Infinite);


                //listView1.View = View.List;
                TTSBoxText = richTextBox3.Text.ToString();
               labelCharCount.Text = TTSBoxText.Length.ToString();

            /*   if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
               {
                   MessageBox.Show("This application is already running!");
                   System.Diagnostics.Process.GetCurrentProcess().Kill();
               } */ //this will only allow one instance of tts voice wizard to run... maybe only do this if system tray on launch is active

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
        public struct voicePreset //use then when setting up presets
        {
           public string PresetName;
            public string TTSMode;
            public string Voice;
            public string Accent;
            public string SpokenLang;
            public string TranslateLang;
            public string Style;
            public string Pitch;
            public string Volume;
            public string Speed;
        }
        public static void CUSTOMRegisterHotKey()
        {
            int id = 0;// The id of the hotkey. 
            KeyModifier modkey;
            Enum.TryParse(modifierKey, out modkey);

            Keys normKey;
            Enum.TryParse(normalKey, out normKey);

            RegisterHotKey(MainFormGlobal.Handle, id, (int)modkey, normKey.GetHashCode());
        }
        protected override void WndProc(ref Message m)
        {
            //link to implementation https://www.fluxbytes.com/csharp/how-to-register-a-global-hotkey-for-your-application-in-c/ 
            //additional links https://stackoverflow.com/questions/2450373/set-global-hotkeys-using-c-sharp
            base.WndProc(ref m);
            //  System.Diagnostics.Debug.WriteLine("-------------get key press id: " + m.Result.ToString());
            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */
               

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                System.Diagnostics.Debug.WriteLine("-------------get key press id: " + key.ToString());

                Task.Run(() => MainDoSpeechTTS());

            }

        }

        private void hideVRCTextButton_Click(object sender, EventArgs e)//speech to text
        {
          //  var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            OSC.OSCSender.Send(message0);
        }
        private void logClearButton_Click(object sender, EventArgs e)//speech to text
        {
            ClearTextBox();
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

        public void logLine(string line, Color? color = null)
        {
            
            
            if (InvokeRequired)
            {
                this.Invoke(new Action<string,Color?>(logLine), new object[] { line,color });
                return;
            }
            richTextBox1.Select(0, 0);
            if (rjToggleDarkMode.Checked == true) { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.White); }
            else { richTextBox1.SelectionColor = color.GetValueOrDefault(Color.Black); }
            
            richTextBox1.SelectedText = line + "\r\n";
        }
 
        public void ClearTextBox()
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action(ClearTextBox));
                return;
            }

            richTextBox1.Text = "";

        }

        public void ClearTextBoxTTS()
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action(ClearTextBoxTTS));
                return;
            }

            richTextBox3.Text = "";

        }
        public void ClearTypingBox()
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

        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox2.Text.ToString();
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
        private void comboBoxVirtualOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            //  audioOutputIndex = comboBoxVirtualOutput.SelectedIndex - 1; //+1 while i have mapped device in there
            System.Diagnostics.Debug.WriteLine("audio device index: " + AudioDevices.audioOutputIndex);

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//add styles here
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "FonixTalk": break;

                case "ElevenLabs": break;

                case "TikTok":break;
                 
                case "System Speech":
                    string phrase = comboBox2.Text.ToString();
                    string[] words = phrase.Split('|');
                    int counter = 1;
                    try
                    {
                        foreach (var word in words)
                        {
                            if (counter == 1)
                            {
                                //synthesizerLite.SelectVoice(word);
                                SystemSpeechTTS.currentLiteVoice = word;
                                System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");

                            }
                            if (counter == 2)
                            {
                                //CultureSelected = word;
                                System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");
                            }
                            counter++;
                        }
                    }
                    catch (Exception ex) 
                    {
                        MessageBox.Show("Unable to load System Speech Voices (use a different option instead): " + ex.Message);

                    }

                    break;
                case "Azure":
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("normal");
                    foreach (string style in AzureTTS.AllVoices4Language[comboBox2.Text.ToString()])
                    {
                        comboBox1.Items.Add(style);
                    }
                    comboBox1.SelectedIndex = 0; break;
                default:
                 
                    break;
            }
           
        }



        private async void getGithubInfo()
        {
            try
            {
                
                var githubClient = new GitHubClient(new ProductHeaderValue("TTS-Voice-Wizard"));

                //  var user = await githubClient.User.Get("VRCWizard");
                // System.Diagnostics.Debug.WriteLine(user.Followers + " follows");

                var release = githubClient.Repository.Release.GetLatest("VRCWizard", "TTS-Voice-Wizard").Result;

                //   System.Diagnostics.Debug.WriteLine(release.TagName.ToString());
                string releaseText = release.TagName.ToString();
                releaseText = releaseText.Replace("v", "");
                Version latestGitHubVersion = new Version(releaseText);
                System.Diagnostics.Debug.WriteLine(releaseText);

                Version localVersion = new Version(currentVersion);

                int versionComparison = localVersion.CompareTo(latestGitHubVersion);
               // var ot = new OutputText();
                if (versionComparison < 0)
                {
                    //The version on GitHub is more up to date than this local release.
                    OutputText.outputLog("[The version on GitHub (" + releaseText + ") is more up to date than the current version (" + currentVersion + "). Click the yellow update button to auto update or grab the new release from the Github https://github.com/VRCWizard/TTS-Voice-Wizard/releases ]");
                    iconButton8.Visible = true;
                    //  ot.outputLog(this, "[After downloading the updated version you may want to copy your config settings over by replacing the new config file with the old one. Config files are stored in AppData/Local/TTSVoiceWizard]");
                }
                else if (versionComparison > 0)
                {
                    //This local version is greater than the release version on GitHub.
                    OutputText.outputLog("[The current version (" + currentVersion + ") is greater than the release version on GitHub (" + releaseText + "). You are on a pre-release/development build]");
                }
                else
                {
                    //This local Version and the Version on GitHub are equal.
                    OutputText.outputLog("[The current version (" + currentVersion + ") and the version on GitHub (" + releaseText + ") are equal. Your program is up to date]");
                }
                richTextBox5.Text = "Current Version: v"+currentVersion+ "-"+versionBuild + " - " + releaseDate+" \nChangelog: (full changelogs visible at https://github.com/VRCWizard/TTS-Voice-Wizard/releases )";
            }
            catch (Exception ex)
            {
       
                MessageBox.Show("Error with Github info: "+ex.Message);
            }

        
             
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            iconButton1.BackColor = Color.FromArgb(68, 72, 111);
            LoadSettings.LoadingSettings();
            getGithubInfo();

            if(rjToggleButton8.Checked == true)//turn on osc listener on start
            {
                Task.Run(() => OSCListener.OSCRecieveHeartRate(this));
                button7.Enabled = false;

            }

            if (rjToggleButton11.Checked == true)//turn on osc listener on start
            {
                Task.Run(() => OSC.OSCLegacyVRChatListener());
                button33.Enabled = false;

            }


           

            WindowsMedia.getWindowsMedia();
            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();
            ToastNotification.ToastListen();
            

            if (rjToggleButtonSystemTray.Checked == true)
            {
                if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
                {
                    MessageBox.Show("TTS Voice Wizard (System Tray Launch): This application is already running!");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                
                // bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);
                this.WindowState = FormWindowState.Minimized;
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.ShowInTaskbar = false;
                    notifyIcon1.Visible = true;
                    this.Hide();
                    int id = 0;
                    CUSTOMRegisterHotKey();
                }

            }

            //CSCoreAudioDevices.CSCoreOuputDevicesGet();
            //CSCoreAudioDevices.CSCoreOuputDevicesGet();
            //CSCoreAudioDevices.CSCoreOuputDevicesGet()

            try
            {
                if (rjToggleButtonOBSText.Checked == true)
                {
                    File.WriteAllTextAsync(@"TextOut\OBSText.txt", String.Empty);
                }
            }
            catch(Exception ex)
            {
                OutputText.outputLog("[OBSText File Error: " + ex.Message + ". Try moving folder location.]", Color.Red);
            }
            








        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VoiceWizardWindow.UnregisterHotKey(this.Handle, 0);
            SaveSettings.SavingSettings();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = rjToggleButtonKeyRegion2.Checked;
            Settings1.Default.Save();
        }
        private void buttonActivationWord_Click(object sender, EventArgs e)
        {
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
        private void buttonErase_Click(object sender, EventArgs e)
        {

            this.Invoke((MethodInvoker)delegate ()
            {
                OutputText.eraseDelay = Int32.Parse(textBoxErase.Text.ToString());
                Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
                Settings1.Default.Save();
            });
        }
        private async void TTSButton_Click(object sender, EventArgs e)//TTS
        {

            var text = richTextBox3.Text.ToString();
            //AudioDevices.CSCoreOuputDevicesGet();
            Task.Run(() => MainDoTTS(text));


        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            
            Task.Run(() => MainDoSpeechTTS());
         
        }
        public async void MainDoTTS(string text, string STTMode ="Text", string AzureTranslateText ="[ERROR]")
        {
            if (STTMode == "Text")
            {
                if (rjToggleButtonMedia.Checked == true)
                {
                    try
                    {
                        var soundPlayer = new SoundPlayer(@"sounds\TTSButton.wav");
                        soundPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (rjToggleButtonClear.Checked == true)
                    {
                        richTextBox3.Clear();

                    }

                });
            }


                var language = "";
            this.Invoke((MethodInvoker)delegate () 
            {

                language = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();
               
            });

                string selectedTTSMode = VoiceWizardWindow.TTSModeSaved;
                //VoiceCommand task

                if (AzureRecognition.YourSubscriptionKey == "" && selectedTTSMode == "Azure")
                {
                //  var ot = new OutputText();
                OutputText.outputLog("[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech from 'Settings > Audio Settings'.]");
                }
                Task.Run(() => VoiceCommands.MainDoVoiceCommand(text));
                 if (rjToggleReplaceBeforeTTS.Checked == true)
                 {
                text = WordReplacements.MainDoWordReplacement(text);
                  }
                var writeText = text;
                  
            var speechText = text;
                var newText = text;
                var translationMethod = "";

                if (language != "No Translation (Default)")
                {
                   // var DL = new DeepL();
                   if (STTMode != "Azure Translate")  
                      { 
                    newText = await DeepLTranslate.translateTextDeepL(text);
                    translationMethod = "DeepL Translation";
                      }
                else { 
                    newText = AzureTranslateText;
                    translationMethod = "Azure Translation";
                     }
                    if (rjToggleButtonVoiceWhatLang.Checked == true) { 
                        speechText= newText;

                    }
                    if (rjToggleButtonAsTranslated2.Checked == true)
                    {
                        writeText = newText;

                    }

                 }
           
           

            switch (selectedTTSMode)
                    {
                        case "FonixTalk":
                            Task.Run(() => FonixTalkTTS.FonixTTS(speechText));
                            break;
                        case "ElevenLabs":
                            Task.Run(() => ElevenLabsTTS.ElevenLabsTextAsSpeech(speechText));
                            break;
                case "System Speech":
                            Task.Run(() => SystemSpeechTTS.systemTTSAction(speechText));
                            break;
                        case "Azure":
                            Task.Run(() => AzureTTS.SynthesizeAudioAsync(speechText)); //turning off TTS for now
                            break;
                        case "TikTok":
                            Task.Run(() => TikTokTTS.TikTokTextAsSpeech(speechText));
                            break;
                        case "Glados":
                            Task.Run(() => GladosTTS.GladosTextAsSpeech(speechText));
                            break;
                        case "Amazon Polly":
                            Task.Run(() => AmazonPollyTTS.PollyTTS(speechText));
                            break;


                default:

                            break;
                    }

            if (rjToggleReplaceBeforeTTS.Checked == false)
            {
                writeText = WordReplacements.MainDoWordReplacement(writeText);
            }


            if (rjToggleButtonLog.Checked == true)
            {
                if(language == "No Translation (Default)")
                {
                    OutputText.outputLog($"[{STTMode} > {selectedTTSMode}]: {writeText}");
                }
                else
                {
                    OutputText.outputLog($"[{STTMode} > {selectedTTSMode}]: {text} [{translationMethod}]: {newText}");
                }

                if (rjToggleButtonOBSText.Checked == true)
                {
                    OutputText.outputTextFile(writeText);
                }

            }
            if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
                {
                OSCListener.pauseBPM = true;
                SpotifyAddon.pauseSpotify = true;
                Task.Run(() => OutputText.outputVRChat(writeText, "tts"));
                }
                if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
                {
                OSCListener.pauseBPM = true;
                SpotifyAddon.pauseSpotify = true;
                Task.Run(() => OutputText.outputVRChatSpeechBubbles(writeText, "tts")); //original

                }
                if (rjToggleButtonGreenScreen.Checked == true)
                {
                    Task.Run(() => OutputText.outputGreenScreen(writeText, "tts")); //original

                }


            
        }
        private void MainDoSpeechTTS()
        {
            if (rjToggleButtonMedia.Checked == true)
            {
                try
                {
                    var soundPlayer = new SoundPlayer(@"sounds\speechButton.wav");
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            

            
            this.Invoke((MethodInvoker)delegate ()
            {
                switch (comboBoxSTT.SelectedItem.ToString())
            {

                case "Vosk":

                        Task.Run(() => VoskRecognition.toggleVosk());

                        break;
               case "Whisper":

                        Task.Run(() => WhisperRecognition.toggleWhisper());

                        break;
               case "Web Captioner":
                        Task.Run(() => WebCaptionerRecognition.WebCapToggle());
                        break;

                case "System Speech":
                   
                    Task.Run(() => SystemSpeechRecognition.speechTTSButtonLiteClick());

                    break;
                case "Azure":
                    if (AzureRecognition.YourSubscriptionKey == "")
                    {
                        OutputText.outputLog("[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech. You can also change the Windows Built-In System Speech 'Output Device' and 'Voice' in the 'Settings > System Speech' tab]");
                    }
                    if (AzureRecognition.YourSubscriptionKey != "")
                    {
                      //  var azureRec = new AzureRecognition();
                      
                            if (comboBox3.Text.ToString() == "No Translation (Default)")
                            {
                                AzureRecognition.speechSetup(comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                                System.Diagnostics.Debug.WriteLine("<speechSetup change>");
                               
                                    OutputText.outputLog("[Azure Listening]");
                                AzureRecognition.speechTTTS(comboBox4.Text.ToString());
                                

                            }
                            else
                            {
                                AzureRecognition.speechSetup(comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                                System.Diagnostics.Debug.WriteLine("<speechSetup change>");
                                                           
                                    OutputText.outputLog("[Azure Translation Listening]");
                                AzureRecognition.translationSTTTS(comboBox3.Text.ToString(), comboBox4.Text.ToString());
                                

                            }
                        
                    }
                    break;

                default:

                    break;
            }
            });




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
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TTSButton.PerformClick();

                e.Handled = true;
            }
        }
        private void allButtonColorReset()
        {
            iconButton2.BackColor = Color.FromArgb(31, 30, 68);
            iconButton4.BackColor = Color.FromArgb(31, 30, 68);
            iconButton5.BackColor = Color.FromArgb(31, 30, 68);
            iconButton1.BackColor = Color.FromArgb(31, 30, 68);
            iconButton3.BackColor = Color.FromArgb(31, 30, 68);
            iconButton23.BackColor = Color.FromArgb(31, 30, 68);

        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton2.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(tabPage1);//sttts
            webView21.Hide();
            

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton4.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(APIs);//provider
            webView21.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton5.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(General);//settings
            webView21.Hide();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton1.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(tabPage4);//Dashboard
            webView21.Show();


        }

        private void iconButton6_Click(object sender, EventArgs e)
        {

            //    System.Diagnostics.Process.Start(new ProcessStartInfo("https://discord.gg/YjgR9SWPnW") { UseShellExecute = true });
            System.Diagnostics.Process.Start("explorer.exe", "https://discord.gg/YjgR9SWPnW");

        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("https://github.com/VRCWizard/TTS-Voice-Wizard");
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard");
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

        private void buttonSpotify_Click(object sender, EventArgs e)
        {
            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.Save();
            SpotifyAddon.SpotifyConnect();

        }

     

        private void rjToggleButtonCurrentSong_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonCurrentSong.Checked == true)  //instead of disabling other toggle, when new toggle is used it turns off the other one
            {
                rjToggleButton10.Checked = false;
            }

        }


        private void logTrash_Click(object sender, EventArgs e)
        {
            ClearTextBox();
        }

        private void ttsTrash_Click(object sender, EventArgs e)
        {
            ClearTextBoxTTS();
        }


        private void hidetimertick(object sender)
        {

            Thread t = new Thread(doHideTimerTick);
            t.Start();
        }
        private void typetimertick(object sender)
        {

            Thread t = new Thread(doTypeTimerTick);
            t.Start();
        }
        public void spotifytimertick(object sender)
        {

            Thread t = new Thread(doSpotifyTimerTick);
            t.Start();
        }
        public void toasttimertick(object sender)
        {

            Thread t = new Thread(doToastTimerTick);
            t.Start();
        }
        public void katRefreshtimertick(object sender)
        {

            Thread t = new Thread(doKatRefreshTimerTick);
            t.Start();
        }
        public void VRCCountertimertick(object sender)
        {

            Thread t = new Thread(doVRCCounterTimerTick);
            t.Start();
        }
        public void whispertimertick(object sender)
        {

            Thread t = new Thread(doWhisperTimerTick);
            t.Start();
        }

        private void doHideTimerTick()
        {
            // var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255); // causes glitch if enabled

            OSCListener.pauseBPM = false;
            SpotifyAddon.pauseSpotify = false;
            OutputText.EraserRunning = false;

            if (rjToggleButtonOSC.Checked == true)
            {
                if(VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked == true)
                {
                    Task.Delay(2500).Wait();

                }
                
                var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
                OSC.OSCSender.Send(message0);
                var message1 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
                OSC.OSCSender.Send(message1);
            }
            if (rjToggleButtonChatBox.Checked == true && rjToggleButtonChatBoxUseDelay.Checked == true && rjToggleButtonHideDelay2.Checked)
            {
                var message1 = new CoreOSC.OscMessage("/chatbox/input", "", true, false);
                OSC.OSCSender.Send(message1);
            }

            System.Diagnostics.Debug.WriteLine("****-------*****--------Tick");
          /*  if (rjToggleButtonGreenScreen.Checked == true)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    pf.customrtb1.Text = "";
                });

            }*/

        }
        private void doToastTimerTick()
        {
            try { 
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                var message0 = new CoreOSC.OscMessage(VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString(), false);
                OSC.OSCSender.Send(message0);
            });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Discord Toast Error: " + ex.Message + "]", Color.Red);

            }

        }

       private void doWhisperTimerTick()
        {
            string text = WhisperRecognition.WhisperString;

            VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Whisper");
            WhisperRecognition.WhisperString = "";




        }


        private void doTypeTimerTick()
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                if (typingBox == false && tabControl1.SelectedTab == tabPage3)
                {
                    var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);//this is what spams osc
                    OSC.OSCSender.Send(typingbubble);
                }
                if (typingBox == true && tabControl1.SelectedTab == tabPage3)
                {
                    var theString = "";

                    theString = richTextBox9.Text.ToString();




                    if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
                    {
                        OSCListener.pauseBPM = true;
                        SpotifyAddon.pauseSpotify = true;
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "ttt")); //original


                    }
                    if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
                    {
                        OSCListener.pauseBPM = true;
                        SpotifyAddon.pauseSpotify = true;
                        Task.Run(() => OutputText.outputVRChat(theString, "tttAdd")); //original     
                    }
                }
                typingBox = false;

                typeTimer.Change(2000, 0);
            });

        }
        private void doSpotifyTimerTick()
        {


            if (rjToggleButtonCurrentSong.Checked == true)
            {
                Task.Run(() => SpotifyAddon.spotifyGetCurrentSongInfo());
            }
            if (rjToggleButton10.Checked == true && rjToggleButtonPeriodic.Checked == true)
            {
                Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
            }

            spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);


        }
        private void doKatRefreshTimerTick()
        {
            if (rjToggleButtonHideDelay2.Checked == false &&rjToggleButtonAutoRefreshKAT.Checked==true)
            {
                if (rjToggleButtonOSC.Checked == true)
                {
                    OutputText.outputVRChat(OutputText.lastKatString, "tttAdd");
                }


                katRefreshTimer.Change(2000, 0);
            }
        }

        private void doVRCCounterTimerTick()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true && OSC.AFKDetector==true)
            {
                var theString = "";
                theString = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();
                if (rjToggleButtonChatBox.Checked == true)
                {
                    Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                }
                if (rjToggleButtonOSC.Checked == true)
                {
                    Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                }

            }


                if (rjToggleButton13.Checked == true && button33.Enabled==false)
            {
                
              
                    if(OSC.counter1 > OSC.prevCounter1)
                    {
                        OSC.prevCounter1 = OSC.counter1;
                         var theString = "";
                         theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text.ToString();

                          theString = theString.Replace("{counter}", OSC.counter1.ToString());

                        if (rjToggleButtonChatBox.Checked == true)
                        {
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString,"bpm"));
                        }
                        if (rjToggleButtonOSC.Checked == true)
                        {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                        }


                    }
                else if (OSC.counter2 > OSC.prevCounter2)
                {
                    OSC.prevCounter2 = OSC.counter2;
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter2.ToString());

                    if (rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


                }
                else if (OSC.counter3 > OSC.prevCounter3)
                {
                    OSC.prevCounter3 = OSC.counter3;
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter3.ToString());

                    if (rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


                }
                else if (OSC.counter4 > OSC.prevCounter4)
                {
                    OSC.prevCounter4 = OSC.counter4;
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter4.ToString());

                    if (rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


                }
                else if (OSC.counter5 > OSC.prevCounter5)
                {
                    OSC.prevCounter5 = OSC.counter5;
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter5.ToString());

                    if (rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


                }
                else if (OSC.counter6 > OSC.prevCounter6)
                {
                    OSC.prevCounter6 = OSC.counter6;
                    
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter6.ToString());

                    if (rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


                }
                if (rjToggleButtonCounterSaver.Checked == true)
                {
                    Settings1.Default.Counter1 = OSC.counter1;
                    Settings1.Default.Counter2 = OSC.counter2;
                    Settings1.Default.Counter3 = OSC.counter3;
                    Settings1.Default.Counter4 = OSC.counter4;
                    Settings1.Default.Counter5 = OSC.counter5;
                    Settings1.Default.Counter6 = OSC.counter6;
                    Settings1.Default.Save();
                }



            }
                VRCCounterTimer.Change(1600, 0);
            
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
            Task.Run(() => OSCListener.OSCRecieveHeartRate(this));
            button7.Enabled = false;

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
        private void iconButton3_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton3.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(tabAddons); //addon
            webView21.Hide();
            

        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            richTextBox7.Text = richTextBox1.Text;
            tabControl1.SelectTab(tabSpotify);


        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            richTextBox8.Text = richTextBox1.Text;
            tabControl1.SelectTab(tabHeartBeat);

        }

        private void iconButton11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabEmoji);

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if(richTextBox1.Lines.Count()>=2000)
            {
                ClearTextBox();
                OutputText.outputLog("Log exceeded limit and was automatically cleared");
            }
            //copies main log text to addon logs
            if (tabControl1.SelectedTab.Text.ToString() == "Spotify")
            {
                richTextBox7.Text = richTextBox1.Text;
            }
            if (tabControl1.SelectedTab.Text.ToString() == "Heartbeat")
            {
                richTextBox8.Text = richTextBox1.Text;
            }
            //richTextBox10.Text = richTextBox1.Text;
            if (tabControl1.SelectedTab.Text.ToString() == "VoiceCommandsTab")
            {
                richTextBox12.Text = richTextBox1.Text;
            }
            if (tabControl1.SelectedTab.Text.ToString() == "Discord")
            {
                richTextBoxDiscord.Text = richTextBox1.Text;
            }
        }

        private void iconButton12_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OSCListener.OSCReceiveport = Convert.ToInt32(textBoxHRPort.Text.ToString());

        }

        private void button8_Click(object sender, EventArgs e)
        {
            OSCListener.OSCReceiveport = Convert.ToInt32(textBoxHRPort.Text.ToString());

        }
      

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonChatBox.Checked == true && (richTextBox3.Text.ToString().Length > TTSBoxText.Length))
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                OSC.OSCSender.Send(typingbubble);
            }
            TTSBoxText = richTextBox3.Text.ToString();
            labelCharCount.Text = TTSBoxText.Length.ToString();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {


            var mills = Int32.Parse(textBoxSpotifyTime.Text.ToString());
            if (mills < 1500)
            {
                // timer1.Interval = 1500;

                SpotifyAddon.spotifyInterval = "1500";
                textBoxSpotifyTime.Text = SpotifyAddon.spotifyInterval;
                spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);
            }
            else
            {
                // timer1.Interval = Int32.Parse(textBoxSpotifyTime.Text.ToString());
                SpotifyAddon.spotifyInterval = textBoxSpotifyTime.Text.ToString();
                spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);
            }

        }
      
     
        private void iconButton13_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/wBRUcx9EWes");

        }

        private void iconButton14_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://twitter.com/Wizard_VR");

        }

        private void iconButton15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://discord.gg/YjgR9SWPnW");
        }

        private void iconButton16_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard");
        }

        private void iconButton17_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard");

        }
        private void iconButton18_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", SpotifyAddon.spotifyurllink);

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Uri uri = new Uri("https://voicewizardsponsors.carrd.co/");
            webView21.Source = uri;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // if (button10.Text == "X")
            webView21.Dispose();
            button10.Dispose();
            button9.Dispose();


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

        private void iconButton20_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(AzureSet);//settings
        }

 
      //  private void iconButton18_Click_1(object sender, EventArgs e)
      //  {
      //      tabControl1.SelectTab(AzureSet);//settings
      //  }


        private void rjToggleButton4_CheckedChanged(object sender, EventArgs e)
        {
            //var ts = new AzureRecognition();
            AzureRecognition.stopContinuousListeningNow();
        }

        private void rjToggleButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton6.Checked == true)
            {
                panel1.SetBounds(0, 0, 65, 731);
                panel2Logo.SetBounds(0, 0, 220, 55);
                pictureBox1.SetBounds(0, 0, 55, 55);
                iconButton1.Text = "";
                iconButton2.Text = "";
                iconButton23.Text = "";
                iconButton5.Text = "";
                iconButton3.Text = "";
                iconButton6.Text = "";
                iconButton7.Text = "";
                iconButton12.Text = "";
                iconButton8.Text = "";
                iconButton4.Text = "";
            }
            if (rjToggleButton6.Checked == false)
            {
                panel1.SetBounds(0, 0, 220, 731);
                panel2Logo.SetBounds(0, 0, 220, 140);
                pictureBox1.SetBounds(38, 15, 125, 125);
                iconButton1.Text = "Dashboard";
                iconButton2.Text = "Text to Speech";
                iconButton23.Text = "Text to Text";
                //  iconButton4.Text = "Speech Provider";
                iconButton5.Text = "Settings";
                iconButton3.Text = "Addon";
                iconButton6.Text = "Discord";
                iconButton7.Text = "Github";
                iconButton12.Text = "Donate";
                iconButton8.Text = "Update";
                iconButton8.Text = "Speech Provider";

            }


        }

        private void rjToggleButtonGreenScreen_CheckedChanged(object sender, EventArgs e)
        {
          /*  if (rjToggleButtonGreenScreen.Checked == true)
            {
                pf = new GreenScreen();
                pf.BackColor = Color.LimeGreen;
                pf.customrtb1.SelectionAlignment = HorizontalAlignment.Center;
                pf.Show(this);
            }
            if (rjToggleButtonGreenScreen.Checked == false)
            {
                pf.Dispose();

            }*/
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
          /*  if (rjToggleButtonGreenScreen.Checked == true)
            {
                pf.customrtb1.Font = new Font("Calibri", Int32.Parse(textBoxFont.Text.ToString()));

            }
            Settings1.Default.fontSizeSetting = textBoxFont.Text.ToString(); */




        }

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
                    
                   
                    CUSTOMRegisterHotKey();
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
               
                
                CUSTOMRegisterHotKey();
                this.WindowState = FormWindowState.Normal;

            }


        }

        private void button12_Click(object sender, EventArgs e)
        {
           // var sender4 = new CoreOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
            OutputText.lastKatString = "";
            

            OSC.OSCSender.Send(message0);
        }

        private void richTextBox9_TextChanged(object sender, EventArgs e)
        {
            typingBox = true;
            var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
            OSC.OSCSender.Send(typingbubble);
        }
       


        private void iconButton23_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton23.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(tabPage3);//ttt

            webView21.Hide();
          
        }

        private void iconButton22_Click(object sender, EventArgs e)
        {
            ClearTypingBox();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(TTSModeSaved=="Azure")
            {
                AzureTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());

            }
         

        }

        private void button13_Click(object sender, EventArgs e)
        {
            AzureTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());

        }

        private void richTextBox5_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", e.LinkText);
        }


        private void iconButton8_Click(object sender, EventArgs e)
        {
            // System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/releases");
            AutoUpdater.Start(updateXMLName);
            AutoUpdater.InstalledVersion = new Version(currentVersion);
            AutoUpdater.DownloadPath = @"updates";

        }

        private void iconButton26_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://trello.com/b/cUhN6eF0/ttsvoicewizard-planned-features");
        }

 
 
        private void iconButton31_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Media-Setup");
        
        }

        private void iconButton29_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service");
        }

        private void iconButton30_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/System-Speech");
        }


      


        private void iconButton32_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Emoji-Setup");
        }

        private void iconButton33_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/bGVs2ew08WY");
        }

        private void iconButton34_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/wBRUcx9EWes");
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

        private void rjToggleButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton10.Checked == true)
            {
                rjToggleButtonCurrentSong.Checked = false;
            }

        }

        private void comboBoxTTSMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "FonixTalk":
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Betty");
                    comboBox2.Items.Add("Dennis");
                    comboBox2.Items.Add("Frank");
                    comboBox2.Items.Add("Harry");
                    comboBox2.Items.Add("Kit");
                    comboBox2.Items.Add("Paul");
                    comboBox2.Items.Add("Rita");
                    comboBox2.Items.Add("Ursula");
                    comboBox2.Items.Add("Wendy");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "FonixTalk";
                    if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }

                    break;
                case "TikTok":
                    comboBox2.Items.Clear();
                    

                    comboBox2.Items.Add("en_us_001");
                    comboBox2.Items.Add("en_us_002");
                    comboBox2.Items.Add("en_us_006");
                    comboBox2.Items.Add("en_us_007");
                    comboBox2.Items.Add("en_us_009");
                    comboBox2.Items.Add("en_us_010");

                    comboBox2.Items.Add("en_au_001");
                    comboBox2.Items.Add("en_au_002");
                    comboBox2.Items.Add("en_uk_001");
                    comboBox2.Items.Add("en_uk_003");

                    comboBox2.Items.Add("fr_001");
                    comboBox2.Items.Add("fr_002");
                    comboBox2.Items.Add("de_001");
                    comboBox2.Items.Add("de_002");
                    comboBox2.Items.Add("es_002");
                    comboBox2.Items.Add("es_mx_002");
                    comboBox2.Items.Add("br_001");
                    comboBox2.Items.Add("br_003");
                    comboBox2.Items.Add("br_004");
                    comboBox2.Items.Add("br_005");
                    comboBox2.Items.Add("id_001");
                    comboBox2.Items.Add("jp_001");
                    comboBox2.Items.Add("jp_003");
                    comboBox2.Items.Add("jp_005");
                    comboBox2.Items.Add("jp_006");
                    comboBox2.Items.Add("kr_002");
                    comboBox2.Items.Add("kr_003");
                    comboBox2.Items.Add("kr_004");

                    comboBox2.Items.Add("en_us_ghostface");
                    comboBox2.Items.Add("en_us_chewbacca");
                    comboBox2.Items.Add("en_us_c3po");
                    comboBox2.Items.Add("en_us_stitch");
                    comboBox2.Items.Add("en_us_stormtrooper");
                    comboBox2.Items.Add("en_us_rocket");
                    comboBox2.Items.Add("en_male_narration");
                    comboBox2.Items.Add("en_male_funny");
                    comboBox2.Items.Add("en_female_emotional");
                    comboBox2.Items.Add("en_female_f08_salut_damour");
                    comboBox2.Items.Add("en_male_m03_lobby");
                    comboBox2.Items.Add("en_male_m03_sunshine_soon");
                    comboBox2.Items.Add("en_female_f08_warmy_breeze");

                    comboBox2.SelectedIndex = 0;

                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "TikTok";
                    if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }

                    break;
                case "System Speech":
                    comboBox2.Items.Clear();
                    foreach (string voice in SystemSpeechTTS.systemSpeechVoiceList)
                    {
                        comboBox2.Items.Add(voice);
                    }
                    comboBox2.SelectedIndex = 0;
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "System Speech";
                    if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }
                    break;
                case "Azure":
                    AzureTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());
                   // comboBox2.SelectedIndex = 0;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = true;
                    comboBoxPitch.Enabled = true;
                    comboBoxVolume.Enabled = true;
                    comboBoxRate.Enabled = true;
                    TTSModeSaved = "Azure";
                    break;

                case "Glados":

                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Glados");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "Glados";

                    break;
                case "ElevenLabs":

                    comboBox2.Items.Clear();
                   
                    try
                    {
                        if(ElevenLabsTTS.elevenFirstLoad==true)
                        {
                            ElevenLabsTTS.CallElevenVoices();
                        }

                        if (ElevenLabsTTS.voiceDict != null)
                        {
                            foreach (KeyValuePair<string, string> kvp in ElevenLabsTTS.voiceDict)
                            {
                                comboBox2.Items.Add(kvp.Value);

                            }
                        }
                        else
                        {
                            comboBox2.Items.Add("error");
                        }
                    }
                    catch (Exception ex)
                    {
                        comboBox2.Items.Add("error");
                        OutputText.outputLog("[ElevenLabs Load1 Error: " + ex.Message + "]", Color.Red);

                    }

                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "ElevenLabs";

                    break;


                case "Amazon Polly":

                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Salli");
                    comboBox2.Items.Add("Kimberly");
                    comboBox2.Items.Add("Kendra");
                    comboBox2.Items.Add("Joanna");
                    comboBox2.Items.Add("Ivy");
                    comboBox2.Items.Add("Ruth ($Neural)");
                    comboBox2.Items.Add("Kevin ($Neural)");
                    comboBox2.Items.Add("Matthew");
                    comboBox2.Items.Add("Justin");
                    comboBox2.Items.Add("Joey");
                    comboBox2.Items.Add("Stephen ($Neural)");

                    comboBox2.Items.Add("Nicole");
                    comboBox2.Items.Add("Olivia ($Neural)");
                    comboBox2.Items.Add("Russell");

                    comboBox2.Items.Add("Amy");
                    comboBox2.Items.Add("Emma");
                    comboBox2.Items.Add("Brian");
                    comboBox2.Items.Add("Arthur ($Neural)");

                    comboBox2.Items.Add("Aditi");
                 //   comboBox2.Items.Add("Reveena");
                    comboBox2.Items.Add("Kajal ($Neural)");

                    comboBox2.Items.Add("Aria ($Neural)");

                    comboBox2.Items.Add("Ayanda ($Neural)");

                    comboBox2.Items.Add("Geraint");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "Amazon Polly";

                    break;



                default:
                    TTSModeSaved = "No TTS";
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("no voice");
                    comboBox2.SelectedIndex = 0;
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    break;
            }
        }


      

        private void button2_Click_1(object sender, EventArgs e)
        {
            OutputText.outputVRChat(OutputText.lastKatString, "tttAdd");
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
            currentText = currentText + "『🎮{averageControllerBattery}%』『🔋{averageTrackerBattery}%』 ";
            textBoxCustomSpot.Text = currentText;

        }

        private void button22_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『💓{bpm}』 ";
            textBoxCustomSpot.Text = currentText;
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "📢{spotifyVolume}% ";
            textBoxCustomSpot.Text = currentText;
        }


        private void iconButton27_Click_1(object sender, EventArgs e)
        {
            richTextBox12.Text = richTextBox1.Text;
            tabControl1.SelectTab(tabPage2);//voiceCommands
        }


        private void buttonAddVoiceCommand_Click(object sender, EventArgs e)
        {
            VoiceCommands.clearVoiceCommands();
            VoiceCommands.voiceCommandsStored += $"{textBox1Spoken.Text.ToString()}:{textBox2Address.Text.ToString()}:{comboBox3Type.SelectedItem.ToString()}:{textBox4Value.Text.ToString()};";
            
            VoiceCommands.voiceCommands();
            VoiceCommands.refreshCommandList();


        }

        private void buttonRemoveVoiceCommand_Click(object sender, EventArgs e)
        {
            int index = Int32.Parse(textBox4Value.Text.ToString()) - 1;
            VoiceCommands.removeVoiceCommandsAt(index);
            VoiceCommands.refreshCommandList();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            if(deleteCommandsToggle.Checked==true)
            {
                VoiceCommands.clearVoiceCommands();
                VoiceCommands.refreshCommandList();
            }
            

        }

        private void iconButton36_Click(object sender, EventArgs e)
        {
            richTextBox3.Font = new Font("Segoe UI", Int32.Parse(richTextBox3.Font.Size.ToString())+1);

        }

        private void iconButton37_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(richTextBox3.Font.Size.ToString())>=1)
            {
                richTextBox3.Font = new Font("Segoe UI", Int32.Parse(richTextBox3.Font.Size.ToString()) - 1);
            }
           

        }


        private void iconButton38_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service");


        }

        private void button25_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(AzureSet);//settings
        }

        private void iconButton39_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/OSC-Listener");
        }

        private void iconButton40_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Voice-Commands");
        }



        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(deleteCommandsToggle.Checked==true)
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

        private void iconButton28_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Web-Captioner");

        }


        private void comboBoxSTT_SelectedIndexChanged(object sender, EventArgs e)// this is used to auto turn off other speech to text option if you switch
        {
            try
            {
                Task.Run(() => SystemSpeechRecognition.AutoStopSystemSpeechRecog());
                Task.Run(() => WebCaptionerRecognition.autoStopWebCap());
                Task.Run(() => AzureRecognition.stopContinuousListeningNow());//turns of continuous if it is on
                Task.Run(() => VoskRecognition.AutoStopVoskRecog());
            }
            catch
            {
                OutputText.outputLog("Could not automatically stop your continuous recognition for previous STT Mode. Make sure to disable is manually by swapping back and pressing the 'Speech to Text to Speech' button or it will keep running in the background and give you 'double speech'!", Color.Orange);
            }



          if (comboBoxSTT.SelectedItem.ToString() == "System Speech" || comboBoxSTT.SelectedItem.ToString() == "Vosk")
            {
             //   OutputText.outputLog("[If your system's default input device changes you will have to reload this]");

            }
         
        }

        private void iconButton41_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide#speech-to-text-and-tts");
        }



        private void iconButton28_Click(object sender, EventArgs e)
        {
           
        }
        Color DarkModeColor = Color.FromArgb(31, 30, 68);
        Color LightModeColor = Color.FromArgb(68, 72, 111);

        private void rjToggleButton7_CheckedChanged_1(object sender, EventArgs e)
        {

           if (rjToggleDarkMode.Checked== true)//dark mode
            {
                DarkTitleBarClass.UseImmersiveDarkMode(Handle, true);
                foreach (var thisControl in GetAllChildren(this).OfType<TextBox>())
                {
                    thisControl.BackColor = Color.FromArgb(31, 30, 68);
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<RichTextBox>())
                {
                    thisControl.BackColor = Color.FromArgb(31, 30, 68);
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<ComboBox>())
                {
                    thisControl.BackColor = Color.FromArgb(31, 30, 68);
                    thisControl.ForeColor = Color.White;
                }
                foreach (var thisControl in GetAllChildren(this).OfType<CheckedListBox>())
                {
                    thisControl.BackColor = Color.FromArgb(31, 30, 68);
                    thisControl.ForeColor = Color.White;
                }
                labelCharCount.BackColor= Color.FromArgb(31, 30, 68);
                ttsTrash.BackColor = Color.FromArgb(31, 30, 68);
                logTrash.BackColor = Color.FromArgb(31, 30, 68);
                iconButton22.BackColor = Color.FromArgb(31, 30, 68);

               /* iconButton13.BackColor = LightModeColor;//dashboard buttons
                iconButton14.BackColor = LightModeColor;
                iconButton15.BackColor = LightModeColor;
                iconButton16.BackColor = LightModeColor;
                iconButton17.BackColor = LightModeColor;
                iconButton26.BackColor = LightModeColor;*/

              



                labelCharCount.ForeColor = Color.White;
            ttsTrash.IconColor = Color.White;
            logTrash.IconColor = Color.White;
            iconButton22.IconColor = Color.White;

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

               /* iconButton13.BackColor = Color.FromArgb(68, 72, 111);//dashboard buttons
                iconButton14.BackColor = Color.FromArgb(68, 72, 111);
                iconButton15.BackColor = Color.FromArgb(68, 72, 111);
                iconButton16.BackColor = Color.FromArgb(68, 72, 111);
                iconButton17.BackColor = Color.FromArgb(68, 72, 111);
                iconButton26.BackColor = Color.FromArgb(68, 72, 111);*/

                labelCharCount.ForeColor = Color.FromArgb(68, 72, 111);
                ttsTrash.IconColor = Color.FromArgb(68, 72, 111);
                logTrash.IconColor = Color.FromArgb(68, 72, 111);
                iconButton22.IconColor = Color.FromArgb(68, 72, 111);

                richTextBox4.BackColor = Color.FromArgb(68, 72, 111);
                richTextBox4.ForeColor = Color.White;

                richTextBox5.BackColor = Color.FromArgb(31, 30, 68);
                richTextBox5.ForeColor = Color.White;


            }
        }
        public static IEnumerable<Control> GetAllChildren(Control root)
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



        private void button11_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                modelTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }



        private void iconButton30_Click_2(object sender, EventArgs e)
        {
           
        }

        private void iconButton42_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/DeepL-Translation-API");
        }

        private void iconButton43_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/DeepL-Translation-API");
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            Settings1.Default.deepLKeysave = textBox5.Text.ToString();
            Settings1.Default.Save();
        }

        private void button15_Click(object sender, EventArgs e)
        {

            VoicePresets.presetSaveButton();

        }

        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)//preset selected
        {
            if(comboBoxPreset.SelectedIndex==0)
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
            
           
            
            
        }
      

        private void button19_Click(object sender, EventArgs e)
        {
            VoicePresets.presetEditButton();
            
        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            VoicePresets.presetDeleteButton();
           
        }
      
      

        private void iconButton25_Click(object sender, EventArgs e)
        {
            richTextBoxDiscord.Text = richTextBox1.Text;
            tabControl1.SelectTab(discordTab);//discord
           

        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://shadoki.booth.pm/items/4467967");
        }

        private void buttonReplaceAdd_Click(object sender, EventArgs e)
        {

            WordReplacements.addWordReplacement(textBoxOriginalWord.Text.ToString(),textBoxReplaceWord.Text.ToString());
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            if (rjToggleButton7.Checked==true)
            {
              WordReplacements.clearWordReplacement();
            }
               
            
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
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void iconButton42_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(Replacements);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            EmojiAddon.emojiEdit(Int32.Parse(textBox7.Text.ToString()), textBox6.Text.ToString());
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = checkedListBox2.SelectedIndex + 1;
            textBox7.Text = index.ToString();
        }

        private void iconButton44_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Discord-Integration");
        }

        private void iconButton45_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Word-Replacements");
        }

        private void button25_Click_2(object sender, EventArgs e)
        {
            checkedListBox2.Items.Clear();
            EmojiAddon.ReplacePhraseList.Clear();
            MessageBox.Show("Restart App");

        }

        private void rjToggleButton9_CheckedChanged(object sender, EventArgs e)
        {
            if(rjToggleButton9.Checked==true)
            {
                CUSTOMRegisterHotKey();
            }
            if (rjToggleButton9.Checked == false)
            {
                UnregisterHotKey(this.Handle, 0);
            }

        }

        private void rjToggleButtonHideDelay2_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked == true)
            {
                katRefreshTimer.Change(2000, 0);
            }
        }

        private void rjToggleButtonAutoRefreshKAT_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked == true)
            {
                katRefreshTimer.Change(2000, 0);
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox4.Enabled == true)
            {
                Keys modifierKeys = e.Modifiers;

                //  Keys pressedKey = e.KeyData ^ modifierKeys; //remove modifier keys

                //do stuff with pressed and modifier keys
                //  var converter = new KeysConverter();

                textBox4.Text = modifierKeys.ToString();


                /*   Keys key;
                   Enum.TryParse(modifierKeys.ToString(), out key);
                   Debug.WriteLine(key.ToString());*/
            }
        }

        private void button28_Click(object sender, EventArgs e)//edit key
        {
            textBox4.Clear();
            textBox1.Clear();
            button27.Enabled = true;
            button28.Enabled = false;
            textBox1.Enabled= true;
            textBox4.Enabled = true;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
           /* string text = e.KeyChar.ToString();
            textBox4.Text += text;*/
        }

        private void button27_Click(object sender, EventArgs e)//save key
        {
            textBox1.Enabled = false;
            textBox4.Enabled = false;
            button27.Enabled = false;
            button28.Enabled = true;
            modifierKey = textBox4.Text.ToString();
            normalKey = textBox1.Text.ToString();
            UnregisterHotKey(this.Handle, 0);
            CUSTOMRegisterHotKey();
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



                /*Keys key;
                Enum.TryParse(pressedKey.ToString(), out key);
                Debug.WriteLine(key.ToString());*/
            }

        }

        private void iconButton21_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(DeepLTab);
        }

        private void iconButton19_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(AmazonPolly);
        }

        private void button31_Click(object sender, EventArgs e)//access key
        {
           
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBox9.Text.ToString();
                Settings1.Default.yourAWSKey = text;
                Settings1.Default.Save();
                
            });
        }

        private void button29_Click(object sender, EventArgs e)//secret key
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBox10.Text.ToString();
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

        private void iconButton18_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Amazon-Polly");
        }

        private void iconButton24_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(VRCOSC);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                whisperModelTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void iconButton30_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(LocalSpeech);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            Task.Run(() => OSC.OSCLegacyVRChatListener());
            button33.Enabled = false;
        }

        private void button32_Click(object sender, EventArgs e)
        {
            OSC.FromVRChatPort = textBoxVRChatOSCPort.Text.ToString();
        }

        private void iconButton28_Click_2(object sender, EventArgs e)
        {
            tabControl1.SelectTab(elevenLabs);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                var text = textBox12.Text.ToString();
                Settings1.Default.elevenLabsAPIKey = text;
                Settings1.Default.Save();

            });
        }

        private void button35_Click(object sender, EventArgs e)
        {
            ElevenLabsTTS.CallElevenVoices();
            if (comboBoxTTSMode.SelectedItem.ToString() == "ElevenLabs")
            {

                comboBox2.Items.Clear();

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
                            comboBox2.Items.Add(kvp.Value);

                        }
                    }
                    else
                    {
                        comboBox2.Items.Add("error");
                    }
                }
                catch (Exception ex)
                {
                    comboBox2.Items.Add("error");
                    OutputText.outputLog("[ElevenLabs Load2 Error: " + ex.Message + "]", Color.Red);

                }

                comboBox2.SelectedIndex = 0;

                comboBox1.SelectedIndex = 0;
                comboBox1.Enabled = false;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox5.Enabled = false;
                comboBoxPitch.Enabled = false;
                comboBoxVolume.Enabled = false;
                comboBoxRate.Enabled = false;
                TTSModeSaved = "ElevenLabs";
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

        private void OBSLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide#obs-text-for-streaming-and-recording-videos");
        
        }

        private void iconButton35_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/ElevenLabs-TTS");
        }

        private void iconButton38_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/n5nLnacVGu4");
        }

        private void iconButton46_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide");
        }

        private void button36_Click(object sender, EventArgs e)
        {
            OSC.counter1 = 0;
            OSC.counter2 = 0;
            OSC.counter3 = 0;
            OSC.counter4 = 0;
            OSC.counter5 = 0;
            OSC.counter6 = 0;

            OSC.prevCounter1 = 0;
            OSC.prevCounter2 = 0;
            OSC.prevCounter3 = 0;
            OSC.prevCounter4 = 0;
            OSC.prevCounter5 = 0;
            OSC.prevCounter6 = 0;


            Settings1.Default.Counter1 = OSC.counter1;
            Settings1.Default.Counter2 = OSC.counter2;
            Settings1.Default.Counter3 = OSC.counter3;
            Settings1.Default.Counter4 = OSC.counter4;
            Settings1.Default.Counter5 = OSC.counter5;
            Settings1.Default.Counter6 = OSC.counter6;
            Settings1.Default.Save();
        }
    }


    public static class StringExtensions//method to make .contains case insensitive
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
    }


}
