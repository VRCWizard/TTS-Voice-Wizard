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
using System.ComponentModel;
using static System.Net.WebRequestMethods;
using System.IO;
using Windows.Media.AppBroadcasting;
using System.Collections;
using CoreOSC;
using System.Runtime.InteropServices;



//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public partial class VoiceWizardWindow : Form
    {
        public static string currentVersion = "1.3.3";
        // string releaseDate = "May 7, 2023";
        //   string versionBuild = "x64"; //update when converting to x86/x64
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

        public static string modifierKeySTTTS = "Control";
        public static string normalKeySTTTS = "G";

        public static string modifierKeyStopTTS = "";
        public static string normalKeyStopTTS = "";

        public static string modifierKeyQuickType = "Control";
        public static string normalKeyQuickType = "J";

        // public static bool StopAnyTTS = false;
        public static WaveOut AnyOutput = new();
        public static WaveOut AnyOutput2 = new();

        CancellationTokenSource speechCt = new();
        public bool logPanelExtended = true;
        public bool logPanelExtended2 = true;
        public static int fontSize = 20;
        public static bool stt_listening = false;










        public VoiceWizardWindow()
        {

            try
            {

                InitializeComponent();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Initalization Error: " + ex.Message);
            }



            //    cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            //    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            MainFormGlobal = this;

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
            catch (Exception ex)
            {
                MessageBox.Show("Hotkey Startup Error: " + ex.Message + "\n\nYour config file (where settings are stored) may have been corrupted.\nNavigate to C:\\Users\\<user>\\AppData\\Local\\TTSVoiceWizard and delete the files in this directory to reset your settings.");
            }

            try
            {
                AudioDevices.NAudioSetupInputDevices();
                AudioDevices.NAudioSetupOutputDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio Device Startup Error: " + ex.Message);
            }
            // AudioDevices.OuputDeviceGet();

            try
            {
                SystemSpeechTTS.getVoices();
                SystemSpeechRecognition.getInstalledRecogs();

            }
            catch (Exception ex)
            {
                MessageBox.Show("System Speech Startup Error: " + ex.Message);
            }

            try
            {
                OSC.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("OSC Startup Error: " + ex.Message);
            }

            try
            {

                // Startup Changes
                // tabControl1.Dock = DockStyle.Fill;
                mainTabControl.Appearance = TabAppearance.FlatButtons;
                mainTabControl.ItemSize = new Size(0, 1);
                mainTabControl.SizeMode = TabSizeMode.Fixed;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Startup Error: " + ex.Message);
            }
            /*   if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
               {
                   MessageBox.Show("This application is already running!");
                   System.Diagnostics.Process.GetCurrentProcess().Kill();
               } */ //this will only allow one instance of tts voice wizard to run... maybe only do this if system tray on launch is active



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
            public int PitchNew;
            public int VolumeNew;
            public int SpeedNew;
        }

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

            RegisterHotKey(MainFormGlobal.Handle, id, (int)modkey, normKey.GetHashCode());
        }
        private IntPtr prevFocusedWindow = IntPtr.Zero;
        private bool captureEnabled = false;
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

                if (id == 0)//sttts
                {
                    Task.Run(() => MainDoSpeechTTS());
                }
                if (id == 1)//stop
                {
                    Task.Run(() => MainDoStopTTS());

                }
                if (id == 2)//game keyboard
                {
                    if (captureEnabled == false) // not capturing so turn it on
                    {
                        // Save the handle of the previously focused window
                        prevFocusedWindow = GetForegroundWindow();

                        // Activate and bring the form to the front
                        this.Activate();
                        this.BringToFront();
                        richTextBox3.Text = "";
                        mainTabControl.SelectTab(tabPage1);//sttts
                        richTextBox3.Select();


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
        /* private const int WM_HOTKEY = 0x0312;
         private const int KEY_UP = 0x0101;
         private const int KEY_DOWN = 0x0100;

         private IntPtr hookId = IntPtr.Zero;
         private bool captureEnabled = false;
       //  private bool capturingNow = false;

         private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
         {
             if (nCode >= 0)
             {
                 int vkCode = Marshal.ReadInt32(lParam);



                 if (captureEnabled)
                 {
                     // Capture the input by storing it in a buffer or sending it to the game window
                     Debug.WriteLine($"Captured input: {(Keys)vkCode}");

                     // Prevent the input from being passed on to the game
                     return new IntPtr(1);
                 }
             }

             return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
         }

         // Constants and native function declarations
         private const int WH_KEYBOARD_LL = 13;
         private const int MOD_ALT = 0x0001;
         private const int MOD_CONTROL = 0x0002;
         private const int MOD_SHIFT = 0x0004;
         private const int MOD_WIN = 0x0008;

         private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);



         [DllImport("user32.dll", SetLastError = true)]
         private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

         [DllImport("user32.dll")]
         private static extern int UnhookWindowsHookEx(IntPtr idHook);

         [DllImport("user32.dll")]
         private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);*/


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

                richTextBox3.Text = "";
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
                case "Moonbase": break;

                case "ElevenLabs": break;

                case "TikTok": break;

                case "NovelAI": break;

                case "System Speech":


                    break;
                case "Azure":
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("normal");
                    foreach (string style in AzureTTS.AllVoices4Language[comboBox2.Text.ToString()])
                    {
                        comboBox1.Items.Add(style);
                    }
                    comboBox1.SelectedIndex = 0; break;

                case "Google (Pro Only)": break;



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
                //richTextBox5.Text = "Current Version: v"+currentVersion+ "-"+versionBuild + " - " + releaseDate+" \nChangelog: (full changelogs visible at https://github.com/VRCWizard/TTS-Voice-Wizard/releases )";
                versionLabel.Text = "v" + currentVersion;
            }
            catch (Exception ex)
            {
                //  OutputText.outputLog("[Error with Github info: " + ex.Message + ".]", Color.Red);
                MessageBox.Show("Error with Github info: " + ex.Message + ". Check your Internet Connection.");
            }



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            iconButton1.BackColor = Color.FromArgb(68, 72, 111);
            LoadSettings.LoadingSettings();
            getGithubInfo();


            if (fontSize >= 1)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox3.Font = new Font("Segoe UI", fontSize);
                });
            }



            if (rjToggleButton8.Checked == true)//turn on osc listener on start
            {
                try
                {
                    Task.Run(() => OSCListener.OSCRecieveHeartRate());

                }
                catch (Exception ex) { OutputText.outputLog("[OSC Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }
                button7.Enabled = false;

            }

            if (rjToggleButton11.Checked == true)//turn on osc listener on start
            {
                try
                {
                    Task.Run(() => OSC.OSCLegacyVRChatListener());
                }
                catch (Exception ex) { OutputText.outputLog("[OSC VRChat Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }
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
                    // int id = 0;
                    CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);
                    CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);
                    CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);
                }

            }

            //CSCoreAudioDevices.CSCoreOuputDevicesGet();
            //CSCoreAudioDevices.CSCoreOuputDevicesGet();
            //CSCoreAudioDevices.CSCoreOuputDevicesGet()

            try
            {
                if (rjToggleButtonOBSText.Checked == true)
                {
                    System.IO.File.WriteAllTextAsync(@"TextOut\OBSText.txt", String.Empty);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[OBSText File Error: " + ex.Message + ". Try moving folder location.]", Color.Red);
            }




            OutputText.outputLog("[QuickStart Guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Quickstart-Guide ]");




        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VoiceWizardWindow.UnregisterHotKey(this.Handle, 0);
            VoiceWizardWindow.UnregisterHotKey(this.Handle, 1);
            VoiceWizardWindow.UnregisterHotKey(this.Handle, 2);
            SaveSettings.SavingSettings();
            try
            {
                if (FonixTalkTTS.pro != null)
                {
                    FonixTalkTTS.pro.Kill();
                }

            }
            catch (Exception ex) { }
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
            if (captureEnabled == true && rjToggleButtonRefocus.Checked == true)//is capturing so turn it off
            {

                // Activate and bring the previous window to the front
                if (prevFocusedWindow != IntPtr.Zero)
                {
                    SetForegroundWindow(prevFocusedWindow);
                }
                captureEnabled = false;
            }
            TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
            this.Invoke((MethodInvoker)delegate ()
            {
                TTSMessageQueued.text = richTextBox3.Text.ToString();
                TTSMessageQueued.TTSMode = comboBoxTTSMode.Text.ToString();
                TTSMessageQueued.Voice = comboBox2.Text.ToString();
                TTSMessageQueued.Accent = comboBox5.Text.ToString();
                TTSMessageQueued.Style = comboBox1.Text.ToString();
                TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                TTSMessageQueued.SpokenLang = comboBox4.Text.ToString();
                TTSMessageQueued.TranslateLang = comboBox3.Text.ToString();
                TTSMessageQueued.STTMode = "Text";
                TTSMessageQueued.AzureTranslateText = "[ERROR]";
            });

            if (rjToggleButtonQueueSystem.Checked == true && rjToggleButtonQueueTypedText.Checked == true)
            {
                TTSMessageQueue.Enqueue(TTSMessageQueued);
            }
            else
            {
                Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(TTSMessageQueued));
            }

            //  if (TTSMessageQueued.STTMode == "Text")
            // {
            if (rjToggleButtonMedia.Checked == true)
            {
                try
                {


                    Task.Run(() =>
                     {
                         string sound = @"sounds\TTSButton.wav";

                         var soundPlayer = new SoundPlayer(sound);
                         soundPlayer.Play();
                     });
                }
                catch (Exception ex)
                {
                    OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                    OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
                }
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                if (rjToggleButtonClear.Checked == true)
                {
                    richTextBox3.Clear();

                }

            });
            //  }



        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {

            Task.Run(() => MainDoSpeechTTS());

        }
        public async void MainDoTTS(TTSMessageQueue.TTSMessage TTSMessageQueued)
        {
            try
            {
                if (IsHandleCreated)
                {




                    var language = "";
                    this.Invoke((MethodInvoker)delegate ()
                     {

                         language = TTSMessageQueued.TranslateLang;

                     });

                    string selectedTTSMode = VoiceWizardWindow.TTSModeSaved;
                    //VoiceCommand task

                    if (AzureRecognition.YourSubscriptionKey == "" && selectedTTSMode == "Azure")
                    {
                        //  var ot = new OutputText();
                        OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                    }
                    VoiceCommands.MainDoVoiceCommand(TTSMessageQueued.text);
                    if (selectedTTSMode == "Azure" && rjToggleButtonStyle.Checked == true)
                    {
                        TTSMessageQueued.Style = comboBox1.Text.ToString();// fix for speaking style not changing
                    }
                    if (rjToggleReplaceBeforeTTS.Checked == true)
                    {
                        TTSMessageQueued.text = WordReplacements.MainDoWordReplacement(TTSMessageQueued.text);
                    }
                    var originalText = TTSMessageQueued.text;
                    var writeText = TTSMessageQueued.text;//send to osc

                    var speechText = TTSMessageQueued.text;//send to tts
                    var newText = TTSMessageQueued.text; //translated text
                    var translationMethod = "";

                        if (!String.IsNullOrEmpty(TTSMessageQueued.text))
                    {
                        if (language != "No Translation (Default)")
                        {
                            // var DL = new DeepL();

                            /*     if (STTMode != "Azure Translate"&& STTMode != "Whisper")
                                 {
                                     newText = await DeepLTranslate.translateTextDeepL(text);
                                     translationMethod = "DeepL Translation";
                                 }
                                 else if (STTMode == "Azure Translate")
                                 {
                                     newText = AzureTranslateText;
                                     translationMethod = "Azure Translation";
                                 }
                                 else if (STTMode == "Whisper"&& language == "English[en]")
                                 {
                                     newText = text;
                                     translationMethod = "Whisper English Translation";
                                 }*/
                            if ((rjToggleButtonUsePro.Checked == true && rjToggleButtonProTranslation.Checked != true) || rjToggleButtonUsePro.Checked != true)
                            {
                                if (TTSMessageQueued.STTMode != "Azure Translate")
                                {
                                    newText = await DeepLTranslate.translateTextDeepL(TTSMessageQueued.text.ToString());
                                    translationMethod = "DeepL Translation";
                                }
                                else
                                {

                                    newText = TTSMessageQueued.AzureTranslateText;
                                    translationMethod = "Azure Translation";
                                }



                                if (rjToggleButtonVoiceWhatLang.Checked == true)
                                {
                                    speechText = newText;
                                    TTSMessageQueued.text = speechText;

                                }
                                if (rjToggleButtonAsTranslated2.Checked == true)
                                {
                                    writeText = newText;

                                }
                            }

                        }
                        if (rjToggleButtonStopCurrentTTS.Checked == true)
                        {
                            MainDoStopTTS();
                        }


                        var voiceWizardAPITranslationString = "";
                        speechCt = new();
                        switch (selectedTTSMode)
                        {
                            case "Moonbase":
                                /* if (rjToggleButtonUsePro.Checked == true && rjToggleButtonProMoonbase.Checked == true)
                                 {
                                     voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                     selectedTTSMode = "Moonbase (Pro)";
                                 }
                                 else
                                 {*/
                                Task.Run(() => FonixTalkTTS.FonixTTS(TTSMessageQueued, speechCt.Token));
                                //  }
                                // Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(TTSMessageQueued, speechCt.Token)); //turning off TTS for now
                                break;
                            case "ElevenLabs":
                                Task.Run(() => ElevenLabsTTS.ElevenLabsTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "System Speech":
                                Task.Run(() => SystemSpeechTTS.systemTTSAction(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Azure":
                                if (rjToggleButtonUsePro.Checked == true && rjToggleButtonProAzure.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    selectedTTSMode = "Azure (Pro)";
                                }
                                else
                                {
                                    Task.Run(() => AzureTTS.SynthesizeAudioAsync(TTSMessageQueued, speechCt.Token)); //turning off TTS for now
                                }
                                // Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "TikTok":
                                Task.Run(() => TikTokTTS.TikTokTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;

                            case "NovelAI":
                                Task.Run(() => NovelAITTS.NovelAITextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Glados":
                                Task.Run(() => GladosTTS.GladosTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Amazon Polly":
                                if (rjToggleButtonUsePro.Checked == true && rjToggleButtonProAmazon.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    selectedTTSMode = "Amazon Polly (Pro)";
                                }
                                else
                                {
                                    Task.Run(() => AmazonPollyTTS.PollyTTS(TTSMessageQueued, speechCt.Token));
                                }
                                break;
                            case "Google (Pro Only)":
                                if (rjToggleButtonUsePro.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                }
                                else
                                {
                                    Task.Run(() => OutputText.outputLog("[You do not have the VoiceWizardPro API enabled, consider becoming a member: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange));
                                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                                    return;
                                }
                                break;

                            case "Uberduck":

                                TTSMessageQueued.Voice = UberDuckTTS.UberVoiceNameAndID[TTSMessageQueued.Voice];
                                Task.Run(() => UberDuckTTS.uberduckTTS(TTSMessageQueued, speechCt.Token));




                                break;
                            case "Miku":
                                // Task.Run(() => MikuTTS.MikuTextAsSpeech(speechText, speechCt.Token));
                                break;

                            case "Fart to Speech":
                                // Task.Run(() => FartTTS.FartTextAsSpeech(speechText, speechCt.Token)); //april fools
                                break;


                            default:

                                break;
                        }

                        if (language != "No Translation (Default)")
                        {

                            if (rjToggleButtonUsePro.Checked == true && rjToggleButtonProTranslation.Checked == true)
                            {

                                newText = voiceWizardAPITranslationString;
                                /* if (rjToggleButtonVoiceWhatLang.Checked == true)
                                 {
                                     speechText = VoiceWizardProTTS.voiceWizardAPITranslationString;


                                 }*/
                                if (rjToggleButtonAsTranslated2.Checked == true)
                                {
                                    writeText = voiceWizardAPITranslationString;

                                }
                                translationMethod = "VoiceWizardPro Translation";
                                voiceWizardAPITranslationString = "";
                            }

                        }
                    }
                    else
                    {
                        TTSMessageQueue.PlayNextInQueue();
                    }



                    if (rjToggleReplaceBeforeTTS.Checked == false)
                    {
                        
                        writeText = WordReplacements.MainDoWordReplacement(writeText);
                     
                       
                    }


                    if (rjToggleButtonLog.Checked == true)
                    {
                        if (language == "No Translation (Default)")
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > {selectedTTSMode}]: {writeText}");
                        }
                        else
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > {selectedTTSMode}]: {originalText} [{translationMethod}]: {newText}");
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
                    if (rjToggleButtonQueueSystem.Checked == true && TTSMessageQueued.TTSMode == "No TTS")
                    {
                        Task.Run(() => NoTTSQueue());
                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }


                    //   if (rjToggleButtonGreenScreen.Checked == true)
                    //   {
                    //       Task.Run(() => OutputText.outputGreenScreen(writeText, "tts")); //original

                    //    }

                }
                else
                {
                    OutputText.outputLog("[DoTTS Handle was not created]", Color.Red);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[DoTTS Error: " + ex.Message + "]", Color.Red);

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }




        }
        private static void NoTTSQueue()
        {

            Task.Delay(Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxDelayAfterNoTTS.Text.ToString())).Wait();

            Task.Run(() => TTSMessageQueue.PlayNextInQueue());


        }
        private void MainDoSpeechTTS()
        {
            if (rjToggleButtonMedia.Checked == true)
            {
                try
                {

                    Task.Run(() =>
                    {
                        string sound = @"sounds\speechButton.wav";
                        //   if(rjToggleButtonAprilFools.Checked == true)
                        //    {
                        //       sound = @"sounds\metalPipe.wav";
                        //     }
                        var soundPlayer = new SoundPlayer(sound);

                        soundPlayer.Play();
                    });

                }
                catch (Exception ex)
                {
                    OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                    OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
                }

            }
            try
            {

                this.Invoke((MethodInvoker)delegate ()
                {
                    switch (comboBoxSTT.SelectedItem.ToString())
                    {

                        case "Vosk":

                            Task.Run(() => VoskRecognition.toggleVosk());

                            break;
                        case "Whisper":
                            if (whisperModelTextBox.Text.ToString() == "no model selected")
                            {
                                downloadWhisperModel();
                                OutputText.outputLog("[Auto installing default Whisper model for you, please wait. To download higher accuracy Whisper model navigate to Speech Provider > Local > Whisper.cpp Model and download/select a bigger model]", Color.DarkOrange);
                            }
                            else
                            {
                                Task.Run(() => WhisperRecognition.toggleWhisper());
                            }


                            //   Task.Run(() => WhisperRecognitionV2.Demo());

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
                                OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                            }
                            if (AzureRecognition.YourSubscriptionKey != "")
                            {
                                //  var azureRec = new AzureRecognition();

                                if (comboBox3.Text.ToString() == "No Translation (Default)" || (rjToggleButtonUsePro.Checked == true && rjToggleButtonProTranslation.Checked == true))
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
            catch (Exception ex)
            {
                //  MessageBox.Show("[STTTS Error: " + ex.Message.ToString());
                OutputText.outputLog("[STTTS Error: " + ex.Message.ToString() + "]", Color.Red);

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
            mainTabControl.SelectTab(tabPage1);//sttts
            webView21.Hide();


        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton4.BackColor = Color.FromArgb(68, 72, 111);
            mainTabControl.SelectTab(APIs);//provider
            webView21.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton5.BackColor = Color.FromArgb(68, 72, 111);
            mainTabControl.SelectTab(General);//settings
            webView21.Hide();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton1.BackColor = Color.FromArgb(68, 72, 111);
            mainTabControl.SelectTab(tabPage4);//Dashboard
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

            VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Green;


        }



        private void rjToggleButtonCurrentSong_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonCurrentSong.Checked == true)  //instead of disabling other toggle, when new toggle is used it turns off the other one
            {
                rjToggleButton10.Checked = false;
            }
            if (rjToggleButtonCurrentSong.Checked == false)  //instead of disabling other toggle, when new toggle is used it turns off the other one
            {
                VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.White;
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
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked == true)
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
            if (rjToggleButtonOBSText.Checked == true && rjToggleButtonHideDelay2.Checked)
            {
                OutputText.outputTextFile("");
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
            try
            {
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



            TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
            this.Invoke((MethodInvoker)delegate ()
            {
                TTSMessageQueued.text = text;
                TTSMessageQueued.TTSMode = comboBoxTTSMode.Text.ToString();
                TTSMessageQueued.Voice = comboBox2.Text.ToString();
                TTSMessageQueued.Accent = comboBox5.Text.ToString();
                TTSMessageQueued.Style = comboBox1.Text.ToString();
                TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                TTSMessageQueued.SpokenLang = comboBox4.Text.ToString();
                TTSMessageQueued.TranslateLang = comboBox3.Text.ToString();
                TTSMessageQueued.STTMode = "Whisper";
                TTSMessageQueued.AzureTranslateText = "[ERROR]";
            });

            if (rjToggleButtonQueueSystem.Checked == true)
            {
                TTSMessageQueue.Enqueue(TTSMessageQueued);
            }
            else
            {
                Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(TTSMessageQueued));
            }



            //  VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Whisper");

            WhisperRecognition.WhisperPrevText = WhisperRecognition.WhisperString;
            WhisperRecognition.WhisperString = "";





        }


        private void doTypeTimerTick()
        {
            try
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (typingBox == false && mainTabControl.SelectedTab == tabPage3)
                    {
                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);//this is what spams osc
                        OSC.OSCSender.Send(typingbubble);
                    }
                    if (typingBox == true && mainTabControl.SelectedTab == tabPage3)
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
            catch
            {

                //stop errors on close
            }

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
            if (rjToggleButton10.Checked == true && rjToggleButtonForceMedia.Checked == true)
            {
                if (WindowsMedia.mediaManager != null)
                {
                    WindowsMedia.mediaManager.ForceUpdate();//windows media will be forced to update on this interval, this is for debug
                    Debug.WriteLine("forced media");
                }
            }

            spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);


        }
        private void doKatRefreshTimerTick()
        {
            if (rjToggleButtonHideDelay2.Checked == false && rjToggleButtonAutoRefreshKAT.Checked == true)
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
            //  SpotifyAddon.pauseSpotify = true;

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true && OSC.AFKDetector == true && OSCListener.pauseBPM != true)
            {
                var elapsedTime = DateTime.Now - OSC.afkStartTime;
                string elapsedMinutesSeconds = $"{elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
                var theString = "";
                theString = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();
                theString = theString.Replace("{time}", elapsedMinutesSeconds);
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)//////////////delete when done testing
                {
                    Task.Run(() => OutputText.outputLog(theString));
                }////////////////////////////////////////////////////
                if (rjToggleButtonChatBox.Checked == true)
                {
                    Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                }
                if (rjToggleButtonOSC.Checked == true)
                {
                    Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                }

            }


            if (rjToggleButton13.Checked == true && button33.Enabled == false)
            {


                if (OSC.counter1 > OSC.prevCounter1)
                {
                    OSC.prevCounter1 = OSC.counter1;
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text.ToString();

                    theString = theString.Replace("{counter}", OSC.counter1.ToString());

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
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

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
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

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
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

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
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

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
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

                    if (rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "bpm"));
                    }
                    if (rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, "bpm"));
                    }


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
        private void iconButton3_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton3.BackColor = Color.FromArgb(68, 72, 111);
            mainTabControl.SelectTab(tabAddons); //addon
            webView21.Hide();


        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            //richTextBox7.Text = richTextBox1.Text;
            mainTabControl.SelectTab(tabSpotify);


        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            // richTextBox8.Text = richTextBox1.Text;
            mainTabControl.SelectTab(tabHeartBeat);

        }

        private void iconButton11_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(tabEmoji);

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Count() >= 2000)
            {
                ClearTextBox();
                OutputText.outputLog("Log exceeded limit and was automatically cleared");
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
            mainTabControl.SelectTab(AzureSet);//settings
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
                panel1.SetBounds(0, 0, 159, 731);
                panel2Logo.SetBounds(0, 0, 159, 105);
                pictureBox1.SetBounds(12, -8, 129, 113);
                iconButton1.Text = "Dashboard";
                iconButton2.Text = "Text to Speech";
                iconButton23.Text = "Text to Text";
                iconButton4.Text = "Speech Provider";
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


                    CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);
                    CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);
                    CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);
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


                CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);
                CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);
                CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);
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
            mainTabControl.SelectTab(tabPage3);//ttt

            webView21.Hide();

        }

        private void iconButton22_Click(object sender, EventArgs e)
        {
            ClearTypingBox();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TTSModeSaved == "Azure")
            {
                AzureTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());

            }
            if (TTSModeSaved == "Amazon Polly")
            {
                AmazonPollyTTS.SynthesisGetAvailableVoices(comboBox5.Text.ToString());

            }
            if (TTSModeSaved == "Google (Pro Only)")
            {
                GoogleTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());

            }
            if (TTSModeSaved == "Uberduck")
            {
                UberDuckTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString(), false);

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
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;

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
                case "Moonbase":
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
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Moonbase";
                  /*  if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }*/


                    OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Moonbase-TTS ]", Color.DarkOrange);


                    break;
                case "TikTok":
                    comboBox2.Items.Clear();

                    var tiktokVoices = new List<string>()
                    {
                    "English US Female",
             "English US Male 1",
              "English US Male 2",
              "English US Male 3",
              "English US Male 4",
             "English UK Male 1",
              "English UK Male 2",
               "English AU Female",
               "English AU Male",
                "French Male 1",
               "French Male 2",
               "German Female",
               "German Male",
               "Spanish Male",
             "Spanish MX Male",
                "Portuguese BR Female 1",
               "Portuguese BR Female 2",
                 "Portuguese BR Male",
                "Indonesian Female",
                "Japanese Female 1",
                 "Japanese Female 2",
                "Japanese Female 3",
                "Japanese Male",
                "Korean Male 1",
               "Korean Male 2",
                 "Korean Female",
                 "Ghostface (Scream)",
                "Chewbacca (Star Wars)",
               "C3PO (Star Wars)",
                "Stitch (Lilo & Stitch)",
      "Stormtrooper (Star Wars)",
     "Rocket (Guardians of the Galaxy)",
                       "Alto",
                       "Tenor",
                 "Sunshine Soon",
                     "Warmy Breeze",
                "Glorious",
            "It Goes Up",
             "Chipmunk",
             "Dramatic",
            "Funny",
            "Emotional",
            "Narrator",
                    };
                    foreach (var voice in tiktokVoices)
                    {
                        comboBox2.Items.Add(voice);
                    }

                    comboBox2.SelectedIndex = 0;

                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "TikTok";
             /*       if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }*/

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
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "System Speech";
                /*    if (AzureTTS.firstVoiceLoad == false)
                    {
                        OutputText.outputLog("[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (AzureTTS.firstVoiceLoad == true)
                    {
                        OutputText.outputLog("[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        AzureTTS.firstVoiceLoad = false;
                    }*/
                    break;
                case "Azure":
                    comboBox5.Items.Clear();
                    // comboBox2.Items.Add("");

                    var voiceAccents = new List<string>()
                    {
                        "Arabic [ar]",
                        "Chinese [zh]",
                        "Czech [cs]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                        "Estonian [et]",
                        "Filipino [fil]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Hungarian [hu]",
                        "Indonesian [id]",
                        "Irish [ga]",
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Thai [th]",
                        "Ukrainian [uk]",
                        "Vietnamese [vi]"
                    };
                    foreach (var accent in voiceAccents)
                    {
                        comboBox5.Items.Add(accent);
                    }
                    comboBox5.SelectedIndex = 5;


                    AzureTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());
                    // comboBox2.SelectedIndex = 0;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Azure";

                    if (textBox2.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


                    break;

                case "Google (Pro Only)":
                    comboBox5.Items.Clear();
                    // comboBox2.Items.Add("");

                    var voiceAccentsGoogle = new List<string>()
                    {
                        "Arabic [ar]",
                        "Chinese [zh]",
                        "Czech [cs]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                       // "Estonian [et]",
                        "Filipino [fil]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Hungarian [hu]",
                        "Indonesian [id]",
                       // "Irish [ga]",
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Thai [th]",
                        "Ukrainian [uk]",
                        "Vietnamese [vi]"
                    };
                    foreach (var accent in voiceAccentsGoogle)
                    {
                        comboBox5.Items.Add(accent);
                    }
                    comboBox5.SelectedIndex = 5;


                    GoogleTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString());
                    // comboBox2.SelectedIndex = 0;
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Google (Pro Only)";

                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


                    break;

                case "Uberduck":


                    comboBox5.Items.Clear();
                    UberDuckTTS.SynthesisGetAvailableVoicesAsync(comboBox5.Text.ToString(), true);
                    // comboBox2.SelectedIndex = 0;
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Uberduck";

                    if (textBoxWizardProKey.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }


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
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Glados";

                    OutputText.outputLog("[Glados Voice setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS ]", Color.DarkOrange);

                    break;
                case "NovelAI":

                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("NovelAI");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "NovelAI";

                    //  OutputText.outputLog("[Glados Voice setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS ]", Color.DarkOrange);

                    break;
                case "ElevenLabs":

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
                        OutputText.outputLog("[ElevenLabs Load1 Error: " + ex.Message + "]", Color.Red);

                    }

                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "ElevenLabs";

                    if (textBox12.Text.ToString() == "")
                    {
                        OutputText.outputLog("[You appear to be missing an ElevenLabs Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/ElevenLabs-TTS ]", Color.DarkOrange);
                    }

                    break;


                case "Amazon Polly":

                    comboBox5.Items.Clear();
                    var voiceAccentsAmazon = new List<string>()
                    {
                        "Arabic [ar]",
                        "Catalan [ca]",
                        "Chinese [zh]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Icelandic [is]",//
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                         "Romanian [ro]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Welsh [cy]"
                    };
                    foreach (var accent in voiceAccentsAmazon)
                    {
                        comboBox5.Items.Add(accent);
                    }
                    comboBox5.SelectedIndex = 5;

                    comboBox2.Items.Clear();
                    AmazonPollyTTS.SynthesisGetAvailableVoices(comboBox5.Text.ToString());







                    comboBox2.SelectedIndex = 0;
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = true;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Amazon Polly";

                    if (textBox9.Text.ToString() == "" && rjToggleButtonUsePro.Checked == false)
                    {
                        OutputText.outputLog("[You appear to be missing an Amazon Polly Key, make sure to follow the setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Amazon-Polly ]", Color.DarkOrange);
                        OutputText.outputLog("[You appear to be missing an VoiceWizardPro Key, consider becoming a memeber: https://ko-fi.com/ttsvoicewizard/tiers ]", Color.DarkOrange);
                    }

                    break;
                case "Miku":

                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Miku");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Miku";

                    //   OutputText.outputLog("[Miku Voice setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS ]", Color.DarkOrange);

                    break;
                case "Fart to Speech":

                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("Squeaky Cheeks");
                    comboBox2.Items.Add("Stinky Symphony");
                    comboBox2.Items.Add("Thunder Down Under");
                    comboBox2.Items.Add("Silent But Deadly");
                    comboBox2.Items.Add("Pooting Plunger");
                    comboBox2.Items.Add("Air Biscuit");
                    comboBox2.Items.Add("Ripping Rector");
                    comboBox2.Items.Add("Flatulent Flute");
                    comboBox2.SelectedIndex = 0;

                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox5.Enabled = false;
                    trackBarPitch.Enabled = true;
                    trackBarVolume.Enabled = true;
                    trackBarSpeed.Enabled = true;
                    TTSModeSaved = "Fart to Speech";

                    // OutputText.outputLog("[Miku Voice setup guide: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Glados-TTS ]", Color.DarkOrange);

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
                    trackBarPitch.Enabled = false;
                    trackBarVolume.Enabled = false;
                    trackBarSpeed.Enabled = false;
                    break;
            }
            updateAllTrackBarLabels();
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
            currentText = currentText + "『🎮{averageControllerBattery}%{cCharge}』『🔋{averageTrackerBattery}%{tCharge}』 ";
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


        private void iconButton27_Click_1(object sender, EventArgs e)
        {
            // richTextBox12.Text = richTextBox1.Text;
            mainTabControl.SelectTab(tabPage2);//voiceCommands
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
            if (deleteCommandsToggle.Checked == true)
            {
                VoiceCommands.clearVoiceCommands();
                VoiceCommands.refreshCommandList();
            }


        }

        private void iconButton36_Click(object sender, EventArgs e)
        {
            fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) + 1;
            richTextBox3.Font = new Font("Segoe UI", fontSize);

        }

        private void iconButton37_Click(object sender, EventArgs e)
        {
            if (Int32.Parse(richTextBox3.Font.Size.ToString()) >= 1)
            {
                fontSize = Int32.Parse(richTextBox3.Font.Size.ToString()) - 1;
                richTextBox3.Font = new Font("Segoe UI", fontSize);
            }


        }


        private void iconButton38_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service");


        }

        private void button25_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AzureSet);//settings
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
                Task.Run(() => WhisperRecognition.autoStopWhisper());
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
                    if (textBox2.Text.ToString() == "")
                    {
                        OutputText.outputLog("[Azure selected for Speech to Text (Voice Recognition). SETUP GUIDE: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Azure-Speech-Service ]", Color.DarkOrange);
                    }
                    break;
                default:

                    break;
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

            if (rjToggleDarkMode.Checked == true)//dark mode
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
                labelCharCount.BackColor = Color.FromArgb(31, 30, 68);
                ttsTrash.BackColor = Color.FromArgb(31, 30, 68);
                logTrash.BackColor = Color.FromArgb(31, 30, 68);
                iconButton22.BackColor = Color.FromArgb(31, 30, 68);
                iconButton36.BackColor = Color.FromArgb(31, 30, 68);

                iconButton37.BackColor = Color.FromArgb(31, 30, 68);


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

                iconButton36.IconColor = Color.White;
                iconButton37.IconColor = Color.White;
                iconButton36.ForeColor = Color.White;
                iconButton37.ForeColor = Color.White;

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

                iconButton36.IconColor = Color.FromArgb(68, 72, 111);
                iconButton37.IconColor = Color.FromArgb(68, 72, 111);
                iconButton36.ForeColor = Color.FromArgb(68, 72, 111);
                iconButton37.ForeColor = Color.FromArgb(68, 72, 111);

                richTextBox4.BackColor = Color.FromArgb(68, 72, 111);
                richTextBox4.ForeColor = Color.White;

                //richTextBox5.BackColor = Color.FromArgb(31, 30, 68);
                // richTextBox5.ForeColor = Color.White;


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
            // richTextBoxDiscord.Text = richTextBox1.Text;
            mainTabControl.SelectTab(discordTab);//discord


        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://shadoki.booth.pm/items/4467967");
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
            mainTabControl.SelectTab(Replacements);
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
            if (rjToggleButton9.Checked == true)
            {
                CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);
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
            textBox1.Enabled = true;
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
            modifierKeySTTTS = textBox4.Text.ToString();
            normalKeySTTTS = textBox1.Text.ToString();
            UnregisterHotKey(this.Handle, 0);
            CUSTOMRegisterHotKey(0, modifierKeySTTTS, normalKeySTTTS);
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
            mainTabControl.SelectTab(DeepLTab);
        }

        private void iconButton19_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(AmazonPolly);
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
            mainTabControl.SelectTab(VRCOSC);
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
            mainTabControl.SelectTab(LocalSpeech);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() => OSC.OSCLegacyVRChatListener());
            }
            catch (Exception ex) { OutputText.outputLog("[OSC VRChat Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }

        }

        private void button32_Click(object sender, EventArgs e)
        {
            OSC.FromVRChatPort = textBoxVRChatOSCPort.Text.ToString();
        }

        private void iconButton28_Click_2(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(elevenLabs);
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
                trackBarPitch.Enabled = false;
                trackBarVolume.Enabled = false;
                trackBarSpeed.Enabled = false;
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

        private void button38_Click(object sender, EventArgs e)
        {
            //Stops any tts from playing
            MainDoStopTTS();


        }
        private void MainDoStopTTS()
        {

            // StopAnyTTS = true;
            try
            {
                speechCt.Cancel();

                //speechCt.Dispose();
                // speechCt = new();
                //AnyOutput.Stop();

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Stop TTS Error: " + ex.Message + "]", Color.Red);
            }


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
            modifierKeyStopTTS = textBoxStopTTS1.Text.ToString();
            normalKeyStopTTS = textBoxStopTTS2.Text.ToString();
            UnregisterHotKey(this.Handle, 1);
            CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);
        }

        private void rjToggleButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton12.Checked == true)
            {
                CUSTOMRegisterHotKey(1, modifierKeyStopTTS, normalKeyStopTTS);
            }
            if (rjToggleButton12.Checked == false)
            {
                UnregisterHotKey(this.Handle, 1);
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

            OutputText.outputLog("Restart required for changes to take effect (Disabling Windows Media may solve 'random' crashing). If you just restarted, Windows Media mode is already disabled so disregard this message.", Color.Red);


        }

        private void comboBoxOutput2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDevices.currentOutputDevice2nd = AudioDevices.speakerIDs[comboBoxOutput2.SelectedIndex];
            AudioDevices.currentOutputDeviceName2nd = comboBoxOutput2.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }

        private void trackBarPitch_Scroll(object sender, EventArgs e)
        {
            //   float value = 0.5f + trackBarPitch.Value * 0.1f;
            //   labelPitchNum.Text = "x" + Math.Round(value, 1).ToString();
            if (TTSModeSaved != "Azure" && TTSModeSaved != "Amazon Polly" && trackBarSpeed.Value != 5 && trackBarPitch.Value != 5)
            {
                OutputText.outputLog("Speed can not be used with pitch for this TTS method. Although changing pitch will alter both pitch and speed.", Color.DarkOrange);
            }
            if (TTSModeSaved != "Azure" && TTSModeSaved != "Amazon Polly")
            {
                if (trackBarSpeed.Value == 5)
                {
                    //  trackBarPitch.Enabled = true;

                }
                else
                {
                    trackBarSpeed.Value = 5;
                    //  trackBarPitch.Enabled = false;
                }

            }
            updateAllTrackBarLabels();
        }

        private void trackBarSpeed_Scroll(object sender, EventArgs e)
        {

            //  float value = 0.5f + trackBarSpeed.Value * 0.1f;
            // labelSpeedNum.Text = "x" + Math.Round(value,1).ToString();
            if (TTSModeSaved != "Azure" && TTSModeSaved != "Amazon Polly" && trackBarSpeed.Value != 5 && trackBarPitch.Value != 5)
            {
                OutputText.outputLog("Speed can not be used with pitch for this TTS method. Although changing pitch will alter both pitch and speed.", Color.DarkOrange);
            }
            if (TTSModeSaved != "Azure" && TTSModeSaved != "Amazon Polly")
            {
                if (trackBarSpeed.Value == 5)
                {
                    //  trackBarPitch.Enabled = true;

                }
                else
                {
                    trackBarPitch.Value = 5;
                    //  trackBarPitch.Enabled = false;
                }

            }
            updateAllTrackBarLabels();


        }

        private void trackBarVolume_Scroll(object sender, EventArgs e)
        {
            // float value = 0.5f + trackBarVolume.Value * 0.1f;
            //  labelVolumeNum.Text = "x" + Math.Round(value, 1).ToString();
            updateAllTrackBarLabels();
        }
        public void updateAllTrackBarLabels()
        {
            float value1 = 0.5f + trackBarPitch.Value * 0.1f;
            labelPitchNum.Text = "x" + Math.Round(value1, 1).ToString();

            float value2 = 0.5f + trackBarSpeed.Value * 0.1f;
            labelSpeedNum.Text = "x" + Math.Round(value2, 1).ToString();

            float value3 = trackBarVolume.Value * 0.1f;
            labelVolumeNum.Text = (Math.Round(value3, 1)*100).ToString("0.#") + "%";



            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";



        }

        private void label142_Click(object sender, EventArgs e)
        {

        }

        private void groupBox38_Enter(object sender, EventArgs e)
        {

        }

        private void button41_Click(object sender, EventArgs e)
        {
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text = "1.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text = "8.0";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text = "0.25";
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text = "1.0";
        }

        private void rjToggleButtonAprilFools_CheckedChanged(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {



                //   if (rjToggleButtonAprilFools.Checked == true)
                //    {
                //    rjToggleButtonMedia.Checked = true;
                //    TTSButton.BackColor = Color.DeepPink;
                //    speechTTSButton.BackColor = Color.DeepPink;
                //   }
                //   if (rjToggleButtonAprilFools.Checked == false)
                //   {
                //    TTSButton.BackColor = DarkModeColor;
                //     speechTTSButton.BackColor = DarkModeColor;

                //    }
            });
        }

        private void button42_Click(object sender, EventArgs e)
        {
            downloadWhisperModel();
        }
        private static void downloadWhisperModel()
        {
            string address = "https://huggingface.co/datasets/ggerganov/whisper.cpp/resolve/main/";
            string path = "models/";


            switch (VoiceWizardWindow.MainFormGlobal.comboBoxWhisperModelDownload.Text.ToString())
            {
                case "ggml-tiny.bin (75 MB)":
                    path += "ggml-tiny.bin";
                    address += "ggml-tiny.bin";

                    break;

                case "ggml-base.bin (142 MB)":
                    path += "ggml-base.bin";
                    address += "ggml-base.bin";

                    break;

                case "ggml-small.bin (466 MB)":
                    path += "ggml-small.bin";
                    address += "ggml-small.bin";

                    break;

                case "ggml-medium.bin (1.5 GB)":
                    path += "ggml-medium.bin";
                    address += "ggml-medium.bin";

                    break;

                default: break;
            }

            if (!System.IO.File.Exists(path))
            {
                VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = Color.DarkOrange;
                VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model downloading... PLEASE WAIT";


                WebClient client = new WebClient();
                Uri uri = new Uri(address);

                // Call DownloadFileCallback2 when the download completes.
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback2);

                // Specify a progress notification handler here ...

                client.DownloadFileAsync(uri, path);
            }
            VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text = path;
        }
        private static void DownloadFileCallback2(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // Console.WriteLine("File download cancelled.");
                MessageBox.Show("File download cancelled");
                OutputText.outputLog("[Whisper Model Download Cancelled: Model Download was cancelled, If this was un-intentional try manually downloading the model from here]", Color.Red);
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = Color.Red;
                    VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model error";
                });
                return;
            }

            if (e.Error != null)
            {
                MessageBox.Show("Error while downloading file.");
                OutputText.outputLog("[Whisper Model Download Error: " + e.Error.Message + "]", Color.Red);
                //Console.WriteLine(e.Error.ToString());
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = Color.Red;
                    VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model error";
                });
                return;
            }
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.modelLabel.ForeColor = Color.Green;
                VoiceWizardWindow.MainFormGlobal.modelLabel.Text = "model downloaded";
            });
            OutputText.outputLog("[Your Whisper Model has completed downloading]", Color.Green);
            MessageBox.Show("Your Whisper Model has completed downloading");



        }

        private void comboBoxWhisperModelDownload_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = "models/";
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

        private void button43_Click(object sender, EventArgs e)
        {
            try
            {
                AudioDevices.NAudioSetupInputDevices();
                AudioDevices.NAudioSetupOutputDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio Device Startup Error: " + ex.Message);
            }

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

        private void button45_Click(object sender, EventArgs e)
        {
            if (logPanelExtended == true)
            {
                logPanel.Size = new Size(20, logPanel.Height);
                button45.Text = "🢀🢀🢀";
                logPanelExtended = false;
            }
            else
            {
                logPanel.Size = new Size(300, logPanel.Height);
                button45.Text = "🢂🢂🢂";
                logPanelExtended = true;
            }

        }

        private void translucentPanel3_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void button47_Click(object sender, EventArgs e)
        {
            var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TTSVoiceWizard");
            Process.Start("explorer.exe", appPath);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow") + @"\VRChat\VRChat", "OSC");
            Process.Start("explorer.exe", appPath);
            Debug.WriteLine(appPath);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", "TextOut");
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rjToggleButtonQueueSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonQueueSystem.Checked == true)
            {
                label81.Visible = true;
                labelQueueSize.Visible = true;
                buttonQueueClear.Visible = true;
                //  label150.Visible = true;
                // label151.Visible = true;
                //  textBoxQueueDelayBeforeNext.Visible = true;
                //  textBoxDelayAfterNoTTS.Visible = true;
            }
            else
            {
                label81.Visible = false;
                labelQueueSize.Visible = false;
                buttonQueueClear.Visible = false;
                //  label150.Visible = false;
                //  label151.Visible = false;
                //textBoxQueueDelayBeforeNext.Visible = false;
                //  textBoxDelayAfterNoTTS.Visible = false;
            }

        }

        private void buttonQueueClear_Click(object sender, EventArgs e)
        {
            TTSMessageQueue.queueTTS.Clear();
            labelQueueSize.Text = TTSMessageQueue.queueTTS.Count.ToString();
        }

        private void button48_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxReadFromTXTFile.Text = openFileDialog1.FileName;
            }
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

        private void button49_Click(object sender, EventArgs e)
        {
            try
            {
                string path = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text.ToString();
                using (FileStream stream = new FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string contents = reader.ReadToEnd();
                        //  Debug.WriteLine(contents);
                        TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            TTSMessageQueued.text = contents.Replace("\n", " ").Replace("\r", " ");
                            TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                            TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                            TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text.ToString();
                            TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString();
                            TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                            TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                            TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                            TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.Text.ToString();
                            TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                            TTSMessageQueued.STTMode = "Text File Reader";
                            TTSMessageQueued.AzureTranslateText = "[ERROR]";
                        });


                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true)
                        {
                            TTSMessageQueue.Enqueue(TTSMessageQueued);
                        }
                        else
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(TTSMessageQueued));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked = false;
                OutputText.outputLog("[Text File Reader Error: This error occured while attempting to read the text file: " + ex.Message + "]", Color.Red);
            }
        }

        private void iconButton15_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string path = "";
                var data = e.Data.GetData(DataFormats.FileDrop);
                if (data != null)
                {
                    var fileNames = data as string[];
                    if (fileNames.Length > 0)
                    {
                        path = fileNames[0];

                    }
                }


                using (FileStream stream = new FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string contents = reader.ReadToEnd();
                        richTextBox3.Text = contents.Replace("\n", " ").Replace("\r", "");

                    }
                }
            }
            catch (Exception ex)
            {

                VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked = false;
                OutputText.outputLog("[Text File Import Error: This error occured while attempting to read the text file: " + ex.Message + "]", Color.Red);
            }

        }

        private void iconButton15_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void iconButton47_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/VRChat-Listener");
        }

        private void iconButton16_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Virtual-Cable");
        }

        private void button50_Click(object sender, EventArgs e)
        {

            if (logPanelExtended2 == true)
            {
                panelCustomize.Size = new Size(20, logPanel.Height);
                button50.Text = "🢀🢀🢀";
                logPanelExtended2 = false;
            }
            else
            {
                panelCustomize.Size = new Size(315, logPanel.Height);
                button50.Text = "🢂🢂🢂";
                logPanelExtended2 = true;
            }


        }

        private void rjToggleButtonQuickTypeEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonQuickTypeEnabled.Checked == true)
            {
                CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);
            }
            if (rjToggleButtonQuickTypeEnabled.Checked == false)
            {
                UnregisterHotKey(this.Handle, 2);
            }
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
            modifierKeyQuickType = textBoxQuickType1.Text.ToString();
            normalKeyQuickType = textBoxQuickType2.Text.ToString();
            UnregisterHotKey(this.Handle, 2);
            CUSTOMRegisterHotKey(2, modifierKeyQuickType, normalKeyQuickType);
        }

        private void button38_Click_1(object sender, EventArgs e)
        {
            SaveSettings.SavingSettings();
        }

        private void textBoxStopTTS1_TextChanged(object sender, EventArgs e)
        {

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

        private void iconButton50_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(VoiceWizPro);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard/tiers#");
        }

        private void iconButton52_Click(object sender, EventArgs e)
        {
            mainTabControl.SelectTab(uberduck);
        }

        private void iconButton53_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Uberduck-TTS");
        }

        private void iconButton54_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://docs.elevenlabs.io/api-reference/text-to-speech");
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null && comboBox4.SelectedItem != null)
            {
                // Get the language code from the selected spoken language
                string spokenLanguageCode = comboBox4.SelectedItem.ToString().Substring(0, comboBox4.SelectedItem.ToString().IndexOf(' '));

                // Get the language code from the selected translation language
                string translationLanguageCode = comboBox3.SelectedItem.ToString().Substring(0, comboBox3.SelectedItem.ToString().IndexOf(' '));

                // Check if the selected spoken language is the same as the selected translation language
                if (spokenLanguageCode == translationLanguageCode)
                {
                    // Set the translation language to position 0 (no translation)
                    comboBox3.SelectedIndex = 0;
                }
            }


        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null && comboBox4.SelectedItem != null)
            {
                // Get the language code from the selected spoken language
                string spokenLanguageCode = comboBox4.SelectedItem.ToString().Substring(0, comboBox4.SelectedItem.ToString().IndexOf(' '));

                // Get the language code from the selected translation language
                string translationLanguageCode = comboBox3.SelectedItem.ToString().Substring(0, comboBox3.SelectedItem.ToString().IndexOf(' '));

                // Check if the selected spoken language is the same as the selected translation language
                if (spokenLanguageCode == translationLanguageCode)
                {
                    // Set the translation language to position 0 (no translation)
                    comboBox3.SelectedIndex = 0;
                }
            }
        }

        private void iconButton55_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/VoiceWizardPro");
        
        }

        private void rjToggleButtonProAmazon_CheckedChanged(object sender, EventArgs e)
        {
            if(rjToggleButtonUsePro.Checked==true &&rjToggleButtonProAmazon.Checked==true)
            {

            }
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
            comboBoxLabsModelID.SelectedIndex= 0;
            comboBoxLabsOptimize.SelectedIndex= 0;
            trackBarStability.Value = 75;
            trackBarSimilarity.Value = 75;

            labelStability.Text = trackBarStability.Value + "%";
            labelSimboost.Text = trackBarSimilarity.Value + "%";
        }

        private void rjToggleButtonResetButtonsCounter_CheckedChanged(object sender, EventArgs e)
        {
            if(rjToggleButtonResetButtonsCounter.Checked==true)
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
            OSC.counter1 = 0;
            OSC.prevCounter1 = 0;
            Settings1.Default.Counter1 = OSC.counter1;
            Settings1.Default.Save();
        }

        private void buttonResetCounter2_Click(object sender, EventArgs e)
        {
            OSC.counter2 = 0;
            OSC.prevCounter2 = 0;
            Settings1.Default.Counter2 = OSC.counter2;
            Settings1.Default.Save();
        }

        private void buttonResetCounter3_Click(object sender, EventArgs e)
        {
            OSC.counter3 = 0;
            OSC.prevCounter3 = 0;
            Settings1.Default.Counter3 = OSC.counter3;
            Settings1.Default.Save();
        }

        private void buttonResetCounter4_Click(object sender, EventArgs e)
        {
            OSC.counter4 = 0;
            OSC.prevCounter4 = 0;
            Settings1.Default.Counter4 = OSC.counter4;
            Settings1.Default.Save();
        }

        private void buttonResetCounter5_Click(object sender, EventArgs e)
        {
            OSC.counter5 = 0;
            OSC.prevCounter5 = 0;
            Settings1.Default.Counter5 = OSC.counter5;
            Settings1.Default.Save();
        }

        private void buttonResetCounter6_Click(object sender, EventArgs e)
        {
            OSC.counter6 = 0;
            OSC.prevCounter6 = 0;
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
