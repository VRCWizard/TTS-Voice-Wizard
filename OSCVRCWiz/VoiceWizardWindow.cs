//Wizard
using Microsoft.CognitiveServices.Speech;//Subcription Azure
using Microsoft.CognitiveServices.Speech.Audio; //Subcription Azure
using Microsoft.CognitiveServices.Speech.Translation;
using CoreOSC; //Replace SharpOSC with CoreOSC eventually
using System;
using System.Speech.Recognition;//free Windows
using System.Speech;//free windows
using System.Speech.Synthesis;//free windows
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using System.Media;
using NAudio;
using NAudio.Wave;
using WaveFormat = NAudio.Wave.WaveFormat;
using StoppedEventArgs = NAudio.Wave.StoppedEventArgs;
using System.Net;
using Resources; //for darktitle
using OSCVRCWiz.Settings;
using Octokit;
using System.Linq;
using System.Diagnostics;
//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public partial class VoiceWizardWindow : Form
    {
        string currentVersion = "0.9.0";
        string releaseDate = "December 21, 2022";
        public static string YourSubscriptionKey;
        public static string YourServiceRegion;
        public string dictationString = "";
        public string activationWord = Settings1.Default.activationWord;
        public int debugDelayValue = Convert.ToInt32(Settings1.Default.delayDebugValueSetting);// Recommended delay of 250ms 
        public int eraseDelay = Convert.ToInt32(Settings1.Default.hideDelayValue);
        int audioOutputIndex = -1;
        public bool profanityFilter = true;
        public string numKATSyncParameters = "4";
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        List<string> comboIn = new List<string>();
        List<string> comboOut = new List<string>();
        List<string> micIDs = new List<string>();
        List<string> speakerIDs = new List<string>();
        public string currentInputDevice = "";
        public string currentOutputDevice = "";
        public string currentInputDeviceName = "Default";
        public string currentOutputDeviceName = "Default";
        public int currentOutputDeviceLite = 0;
        public bool getSpotify = false;
        public System.Threading.Timer testtimer;
        public System.Threading.Timer typetimer;
        public CoreOSC.UDPSender sender3;
        public bool justShowTheSong = false;
        public static int heartRatePort = 4026;
        public static bool pauseBPM = false;
        public static bool pauseSpotify = false;
        public static bool stopBPM = false;
        public static bool BPMSpamLog = true;
        public static int HRInternalValue = 5;
        public static string TTSLiteText = "";
        public static bool typingBox = false;
        public static bool firstVoiceLoad = true;
        public WaveIn waveSource = null;
        public WaveFileWriter waveFile = null;
        public string CultureSelected = "en-US";//free
        public System.Speech.Synthesis.SpeechSynthesizer synthesizerLite;//free
        public MemoryStream stream;
        public static string emotion = "Normal";
        public static string rate = "default";
        public static string pitch = "default";
        public static string volume = "default";
        public static string voice = "Sara";
        public static string TTSModeSaved = "System Speech";
        static bool richboxsmall = false;
        List<string> systemSpeechVoiceList = new List<string>();
        public static List<string> approvedMediaSourceList = new List<string>();
        public static List<string> VCPhrase = new List<string>();
        public static List<string> VCAddress = new List<string>();
        public static List<string> VCType = new List<string>();
        public static List<string> VCValue = new List<string>();
        public static string voiceCommandsStored = "";
        HttpServer httpServer;
        public OutputText ot;
        public PopupForm pf;
        public static VoiceWizardWindow MainFormGlobal;
       
       

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }
        public VoiceWizardWindow()
        {
            try
            {
                
                InitializeComponent(); //initialize happens before voices load to help with error catching on app start                            
            //    cpuCounter = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
            //    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                MainFormGlobal = this;
                httpServer = new HttpServer();
                ot = new OutputText();
                //Azure Audio Devices----------------------------
                comboIn.Add("Default");
                micIDs.Add("Default");
                comboOut.Add("Default");
                speakerIDs.Add("Default");
                var enumerator1 = new MMDeviceEnumerator();
                foreach (var endpoint in
                         enumerator1.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
                {
                    System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);
                    comboIn.Add(endpoint.FriendlyName);
                    micIDs.Add(endpoint.ID);

                }
                var enumerator2 = new MMDeviceEnumerator();
                foreach (var endpoint in
                         enumerator2.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);

                    comboOut.Add(endpoint.FriendlyName);
                    speakerIDs.Add(endpoint.ID);
                }

                foreach (var i in comboIn)
                {
                    comboBoxInput.Items.Add(i);
                }
                foreach (var i in comboOut)
                {
                    comboBoxOutput.Items.Add(i);
                }
                //-------------------------------------------


                //Windows System Speech Audio Devices------------------
                stream = new MemoryStream();
                synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
                System.Diagnostics.Debug.WriteLine("Available output devices:");
                foreach (var device in WaveOutDevice.EnumerateDevices())
                {
                    System.Diagnostics.Debug.WriteLine("{0}: {1}", device.DeviceId, device.Name);
                    comboLiteOutput.Items.Add(device.Name);
                }
                synthesizerLite.SetOutputToWaveStream(stream);
                comboLiteInput.Items.Add("Default");
                //--------------------------------------------

                textBox2.PasswordChar = '*';
                tabControl1.Dock = DockStyle.Fill;
                tabControl1.Appearance = TabAppearance.FlatButtons;
                tabControl1.ItemSize = new Size(0, 1);
                tabControl1.SizeMode = TabSizeMode.Fixed;
                sender3 = new CoreOSC.UDPSender("127.0.0.1", 9000);//9000
                testtimer = new System.Threading.Timer(testtimertick);
                testtimer.Change(Timeout.Infinite, Timeout.Infinite);

                typetimer = new System.Threading.Timer(typetimertick);
                //  typetimer.Change(Timeout.Infinite, Timeout.Infinite);
                typetimer.Change(1500, 0);


                
                foreach (var voice in synthesizerLite.GetInstalledVoices())
                {
                    var info = voice.VoiceInfo;
                    System.Diagnostics.Debug.WriteLine($"Id: {info.Id} | Name: {info.Name} | Age: { info.Age} | Gender: { info.Gender} | Culture: { info.Culture}");
                    systemSpeechVoiceList.Add(info.Name + "|" + info.Culture);
                   // comboBoxLite.Items.Add(info.Name + "|" + info.Culture);
                }
                TTSLiteText = richTextBox3.Text.ToString();

                int id = 0;// The id of the hotkey. 
                RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.G.GetHashCode());


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

                Task.Run(() => doSpeechTTS());

            }

        }

        private void hideVRCTextButton_Click(object sender, EventArgs e)//speech to text
        {
          //  var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            sender3.Send(message0);
        }
        private void logClearButton_Click(object sender, EventArgs e)//speech to text
        {
            ClearTextBox();
        }
        private void buttonDelayHere_Click(object sender, EventArgs e)//speech to text
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                debugDelayValue = Int32.Parse(textBoxDelay.Text.ToString());
                Settings1.Default.delayDebugValueSetting = textBoxDelay.Text.ToString();
                Settings1.Default.Save();


            });
        }

        public void logLine(string line)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(logLine), new object[] { line });
                return;
            }
            richTextBox1.Select(0, 0);
            richTextBox1.SelectedText = line + "\r\n";
        }


        public void AppendTextBox(string value)
        {

            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }

            richTextBox1.Text += value;

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
                sender3.Send(message0);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox2.Text.ToString();
                YourSubscriptionKey = text;
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
                YourServiceRegion = text;
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
            System.Diagnostics.Debug.WriteLine("audio device index: " + audioOutputIndex);

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//add styles here
        {
            switch (comboBoxTTSMode.Text.ToString())
            {
                case "FonixTalk": break;
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
                                synthesizerLite.SelectVoice(word);
                                System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");

                            }
                            if (counter == 2)
                            {
                                CultureSelected = word;
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
                    foreach (string style in AudioSynthesis.AllVoices4Language[comboBox2.Text.ToString()])
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
                    ot.outputLog(this, "[The version on GitHub (" + releaseText + ") is more up to date than the current version (" + currentVersion + "). Grab the new release from the Github https://github.com/VRCWizard/TTS-Voice-Wizard/releases ]");
                    iconButton8.Visible = true;
                    //  ot.outputLog(this, "[After downloading the updated version you may want to copy your config settings over by replacing the new config file with the old one. Config files are stored in AppData/Local/TTSVoiceWizard]");
                }
                else if (versionComparison > 0)
                {
                    //This local version is greater than the release version on GitHub.
                    ot.outputLog(this, "[The current version (" + currentVersion + ") is greater than the release version on GitHub (" + releaseText + "). You are on a pre-release/development build]");
                }
                else
                {
                    //This local Version and the Version on GitHub are equal.
                    ot.outputLog(this, "[The current version (" + currentVersion + ") and the version on GitHub (" + releaseText + ") are equal. Your program is up to date]");
                }
                richTextBox5.Text = "Current Version: v"+currentVersion+" - "+releaseDate+" \nChangelog: (full changelogs visible at https://github.com/VRCWizard/TTS-Voice-Wizard/releases )";
            }
            catch (Exception ex)
            {
       
                MessageBox.Show("Error with Github info: "+ex.Message);
            }

        
             
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // DarkTitleBarClass.UseImmersiveDarkMode(Handle, true); //activates dark mode for title bar
            iconButton1.BackColor = Color.FromArgb(68, 72, 111);
            LoadSettings.LoadingSettings();
            getGithubInfo();

            if(rjToggleButton8.Checked == true)//turn on osc listener on start
            {
                Task.Run(() => HeartbeatAddon.OSCRecieveHeartRate(this));
                button7.Enabled = false;

            }
            if (rjToggleButton7.Checked == true)
            {
                webCapOn();
            }
            WindowsMedia.getWindowsMedia();
            var vc = new VoiceCommands();
            vc.voiceCommands();
            vc.refreshCommandList();

            //Construct a new OSCQuery service with new OSCQueryService(), optionally passing in the name, TCP port to use for serving HTTP, UDP port that you're using for OSC, and an ILogger if you want logs.

            //  httpServer.VRChatTesting(); //used for testing VRC OSCQuery lib


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
                profanityFilter = true;
            }
            if (rjToggleButtonProfan.Checked == false)
            {
                profanityFilter = false;
            }
        }
        private void buttonErase_Click(object sender, EventArgs e)
        {

            this.Invoke((MethodInvoker)delegate ()
            {
                eraseDelay = Int32.Parse(textBoxErase.Text.ToString());
                Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
                Settings1.Default.Save();
            });
        }
        private async void TTSButton_Click(object sender, EventArgs e)//TTS
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
            if (YourSubscriptionKey == "" && rjToggleButtonLiteMode.Checked == false && comboBoxTTSMode.Text.ToString()=="Azure")
            {
              //  var ot = new OutputText();
                ot.outputLog(this, "[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech from 'Settings > Audio Settings'.]");
            }
            switch(comboBoxTTSMode.Text.ToString())
            {
                case "FonixTalk":
                    var ftts = new FonixTalkTTS();
                    var text = richTextBox3.Text.ToString();
                    ftts.FonixTTS(text);
                    if (rjToggleButtonLog.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.ot.outputLog(this, text);
                        VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(this, text);
                    }

                    if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
                    {

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, text, "tts"));

                    }
                    if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
                    {

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, text, "tts"));
                    }
                    if (rjToggleButtonGreenScreen.Checked == true)
                    {
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(VoiceWizardWindow.MainFormGlobal, text, "tts"));

                    }
                    break;

                case "System Speech":
                    var lite = new WindowsBuiltInSTTTS();
                    Task.Run(() => lite.TTSButtonLiteClick(this)); 
                    break;
                case "Azure":
                    Task.Run(() => doTTSOnly());
                    break;
                case "TikTok":
                   
                    var text2 = richTextBox3.Text.ToString();
                    Task.Run(() => TikTok.TikTokTextAsSpeech(text2));
                    if (rjToggleButtonLog.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.ot.outputLog(this, text2);
                        VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(this, text2);
                    }

                    if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
                    {

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, text2, "tts"));

                    }
                    if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
                    {

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, text2, "tts"));
                    }
                    if (rjToggleButtonGreenScreen.Checked == true)
                    {
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(VoiceWizardWindow.MainFormGlobal, text2, "tts"));

                    }
                    break;

                default: 
                       break;
            }
 
          
        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => doSpeechTTS());
         
        }
        private void doSpeechTTS()
        {
            if (rjToggleButtonChatBox.Checked == true)
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                sender3.Send(typingbubble);

            }
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

            if (YourSubscriptionKey == "" && rjToggleButtonLiteMode.Checked == false)
            {
                ot.outputLog(this, "[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech. You can also change the Windows Built-In System Speech 'Output Device' and 'Voice' in the 'Settings > System Speech' tab]");
            }


            if (rjToggleButtonLiteMode.Checked == true || YourSubscriptionKey == "")
            {
                
                var lite = new WindowsBuiltInSTTTS();
                Task.Run(() => lite.speechTTSButtonLiteClick(this));
               // WindowsBuiltInSTTTS._stream.

            }
            else
            {
                var ts = new TextSynthesis();
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (comboBox3.Text.ToString() == "No Translation (Default)")
                    {
                        ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                        System.Diagnostics.Debug.WriteLine("<speechSetup change>");
                        ot.outputLog(this, "[Azure Listening]");
                        ts.speechTTTS(this, comboBox4.Text.ToString());

                    }
                    else
                    {
                        ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                        System.Diagnostics.Debug.WriteLine("<speechSetup change>");

                        ot.outputLog(this, "[Azure Translation Listening]");
                        ts.translationSTTTS(this, comboBox3.Text.ToString(), comboBox4.Text.ToString());

                    }
                });

            }
           

        }
        private void doTTSOnly()
        {
       
            string text = "";
            SetDefaultTTS.SetVoicePresets();
          //  var ot = new OutputText();
            //Send Text to Vrchat
            this.Invoke((MethodInvoker)delegate ()
            {
                text = richTextBox3.Text.ToString();
            });
                if (rjToggleButtonLog.Checked == true)
            {
                ot.outputLog(this, text);
                VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(this, text);
            }
            if (rjToggleButtonDisableTTS2.Checked == false)
            {
                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(this, text, emotion, rate, pitch, volume, voice));
            }
            if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
            {


                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChat(this, text, "tts")); //original
                                                                    // ot.outputVRChat(this, text);//new
            }
            if (rjToggleButtonChatBox.Checked == true &&rjToggleButtonNoTTSChat.Checked==false)
            {
                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChatSpeechBubbles(this, text, "tts")); //original

            }
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                Task.Run(() => ot.outputGreenScreen(this, text, "tts")); //original

            }

            if (rjToggleButtonClear.Checked == true)
            {
                richTextBox3.Clear();
            }
        }
        public void doTTSOnlyWebCaptioner()
        {
    

            string text = "";
            SetDefaultTTS.SetVoicePresets();
           // var ot = new OutputText();
            //Send Text to Vrchat
            if (rjToggleButtonLog.Checked == true)
            {
                ot.outputLog(this, text);
            }
            if (rjToggleButtonDisableTTS2.Checked == false)
            {
                AudioSynthesis.SynthesizeAudioAsync(this, text, emotion, rate, pitch, volume, voice);
            }
            if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
            {


                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChat(this, text, "tts")); //original
                                                                    // ot.outputVRChat(this, text);//new
            }
            if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
            {
                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChatSpeechBubbles(this, text, "tts")); //original

            }
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                Task.Run(() => ot.outputGreenScreen(this, text, "tts")); //original

            }

            if (rjToggleButtonClear.Checked == true)
            {
                richTextBox3.Clear();
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
            tabControl1.SelectTab(tabPage1);//sttts
            webView21.Hide();
            

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton4.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(tabPage3);//provider
            webView21.Hide();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            allButtonColorReset();
            iconButton5.BackColor = Color.FromArgb(68, 72, 111);
            tabControl1.SelectTab(SettingsNew);//settings
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
                numKATSyncParameters = comboBoxPara.SelectedItem.ToString();
                Settings1.Default.SyncParaValue = comboBoxPara.SelectedIndex;

                Settings1.Default.Save();
            });

        }
       /* private void comboBoxInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentInputDevice = micIDs[comboBoxInput.SelectedIndex];
            currentInputDeviceName = comboBoxInput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("mic changed", currentInputDevice);

        }
        private void comboBoxOutput_SelectedIndexChanged(object sender, EventArgs e)
        {

            currentOutputDevice = speakerIDs[comboBoxOutput.SelectedIndex];
            currentOutputDeviceName = comboBoxOutput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }*/
        private void buttonSpotify_Click(object sender, EventArgs e)
        {
            var sa = new SpotifyAddon();
            sa.SpotifyConnect(this);

        }

        private void timerSpotify_Tick(object sender, EventArgs e)
        {
            if(rjToggleButtonCurrentSong.Checked == true)
            {
                Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));
            }
            if (rjToggleButton10.Checked == true && rjToggleButtonPeriodic.Checked==true)
            {
                Task.Run(() => SpotifyAddon.getCurrentDataInfo(this));
            }
            
            //httpServer.VRChatTestingUpdate();  //used for testing OSCQuery lib



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


        private void testtimertick(object sender)
        {

            Thread t = new Thread(doTimerTick);
            t.Start();
        }
        private void typetimertick(object sender)
        {

            Thread t = new Thread(doTypeTimerTick);
            t.Start();
        }
        private void doTimerTick()
        {
            // var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255); // causes glitch if enabled

            pauseBPM = false;
            pauseSpotify = false;

            if (rjToggleButtonOSC.Checked == true)
            {
                var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
                sender3.Send(message0);
                var message1 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
                sender3.Send(message1);
            }
            if (rjToggleButtonChatBox.Checked == true && rjToggleButtonChatBoxUseDelay.Checked == true && rjToggleButtonHideDelay2.Checked)
            {
                var message1 = new CoreOSC.OscMessage("/chatbox/input", "", true, false);
                sender3.Send(message1);
            }

            System.Diagnostics.Debug.WriteLine("****-------*****--------Tick");
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                Invoke((MethodInvoker)delegate ()
                {
                    pf.customrtb1.Text = "";
                });

            }

        }


        private void button2_Click(object sender, EventArgs e)
        {
            justShowTheSong = true;
            Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));

        }

        private void rjToggleButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton1.Checked == true)
            {

                stopBPM = false;
            }
            if (rjToggleButton1.Checked == false)
            {
                stopBPM = true;

            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            Task.Run(() => HeartbeatAddon.OSCRecieveHeartRate(this));
            button7.Enabled = false;

        }
        private void rjToggleButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButton2.Checked == true)
            {
                BPMSpamLog = true;
            }
            if (rjToggleButton2.Checked == false)
            {
                BPMSpamLog = false;
            }
        }

        private void HRIntervalChange_Click(object sender, EventArgs e)
        {
            HRInternalValue = Int32.Parse(HRInterval.Text.ToString());
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
            tabControl1.SelectTab(tabSpotify);


        }

        private void iconButton10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabHeartBeat);

        }

        private void iconButton11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabEmoji);

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            //copies main log text to addon logs
            richTextBox7.Text = richTextBox1.Text;
            richTextBox8.Text = richTextBox1.Text;
            richTextBox10.Text = richTextBox1.Text;
            richTextBox12.Text = richTextBox1.Text;
        }

        private void iconButton12_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://ko-fi.com/ttsvoicewizard");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            heartRatePort = Convert.ToInt32(textBoxHRPort.Text.ToString());

        }

        private void button8_Click(object sender, EventArgs e)
        {
            heartRatePort = Convert.ToInt32(textBoxHRPort.Text.ToString());

        }
        public void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)//lite version, WindowsBuiltInSTTTS Help
        {
           // this.Invoke((MethodInvoker)delegate ()
         //   {
              

           
            System.Diagnostics.Debug.WriteLine("Recognized text: " + e.Result.Text);
            string text = e.Result.Text.ToString();


            //VoiceCommand task
            Task.Run(() => doVoiceCommand(text));


            // var ot = new OutputText();
            //Send Text to Vrchat
            if (rjToggleButtonLog.Checked == true)
            {
                ot.outputLog(this, text);
                VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(this, text);

            }
            if (rjToggleButtonDisableTTS2.Checked == false)
            {
                switch (comboBoxTTSMode.Text.ToString())
                {
                    case "FonixTalk":
                        var fx = new FonixTalkTTS();
                        Task.Run(() => fx.FonixTTS(text));
                        break;

                    case "System Speech":
                        var sys = new WindowsBuiltInSTTTS();
                        Task.Run(() => sys.systemTTSAction(text));

                        break;
                    case "Azure":
                        SetDefaultTTS.SetVoicePresets();
                        Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, text, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                        break;

                    default:
                      
                break;
                }   

            }

            if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
            {
                Task.Run(() => ot.outputVRChat(this, text, "tts"));
            }
            if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
            {
                Task.Run(() => ot.outputVRChatSpeechBubbles(this, text, "tts")); //original

            }
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                Task.Run(() => ot.outputGreenScreen(this, text, "tts")); //original

            }
          //  });
        }
        
        public void doVoiceCommand(string text)
        {
            
            int index = 0;
            foreach (string x in VoiceWizardWindow.VCPhrase)
            {
                System.Diagnostics.Debug.WriteLine("checking " + x);
                if (text.Contains(x, StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine("it contains " + x);
                    var vc = new VoiceCommands();
                    vc.phraseFound(index);
                }
                index++;
            }
        }


        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonChatBox.Checked == true && (richTextBox3.Text.ToString().Length > TTSLiteText.Length))
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                sender3.Send(typingbubble);
            }
            TTSLiteText = richTextBox3.Text.ToString();
            labelCharCount.Text = richTextBox3.Text.ToString().Length.ToString();

        }

        private void comboLiteOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentOutputDeviceLite = comboLiteOutput.SelectedIndex;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {


            var mills = Int32.Parse(textBoxSpotifyTime.Text.ToString());
            if (mills < 1500)
            {
                timer1.Interval = 1500;
                textBoxSpotifyTime.Text = "1500";
            }
            else
            {
                timer1.Interval = Int32.Parse(textBoxSpotifyTime.Text.ToString());
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
            tabControl1.SelectTab(General);//settings
        }

        private void iconButton19_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(TextOut);//settings
        }

        private void iconButton18_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(AzureSet);//settings
        }

        private void iconButton21_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(SystemSet);//settings

        }

        private void rjToggleButton4_CheckedChanged(object sender, EventArgs e)
        {
            var ts = new TextSynthesis();
            ts.stopContinuousListeningNow(this);
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

            }


        }

        private void rjToggleButtonGreenScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                pf = new PopupForm();
                pf.BackColor = Color.LimeGreen;
                pf.customrtb1.SelectionAlignment = HorizontalAlignment.Center;
                pf.Show(this);
            }
            if (rjToggleButtonGreenScreen.Checked == false)
            {
                pf.Dispose();

            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (rjToggleButtonGreenScreen.Checked == true)
            {
                pf.customrtb1.Font = new Font("Calibri", Int32.Parse(textBoxFont.Text.ToString()));

            }
            Settings1.Default.fontSizeSetting = textBoxFont.Text.ToString();




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
                int id = 0;
                RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.G.GetHashCode());

            }


        }

        private void button12_Click(object sender, EventArgs e)
        {
            var sender4 = new CoreOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
            sender4.Send(message0);
        }

        private void richTextBox9_TextChanged(object sender, EventArgs e)
        {
            typingBox = true;
            var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
            sender3.Send(typingbubble);
        }
        private void doTypeTimerTick()
        {
            if (typingBox == false)
            {
                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);//this is what spams osc
                sender3.Send(typingbubble);
            }
            if (typingBox == true)
            {
                var theString = "";
                this.Invoke((MethodInvoker)delegate ()
                {
                    theString = richTextBox9.Text.ToString();

                });


                if (rjToggleButtonChatBox.Checked == true && rjToggleButtonNoTTSChat.Checked == false)
                {
                    VoiceWizardWindow.pauseBPM = true;
                    VoiceWizardWindow.pauseSpotify = true;
                    Task.Run(() => ot.outputVRChatSpeechBubbles(this, theString, "ttt")); //original


                }
                if (rjToggleButtonOSC.Checked == true && rjToggleButtonNoTTSKAT.Checked == false)
                {
                    VoiceWizardWindow.pauseBPM = true;
                    VoiceWizardWindow.pauseSpotify = true;
                    Task.Run(() => ot.outputVRChat(this, theString, "tttAdd")); //original     
                }
            }
            typingBox = false;
            
            typetimer.Change(2000, 0);

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
                AudioSynthesis.SynthesisGetAvailableVoicesAsync(this, comboBox5.Text.ToString());

            }
         

        }

        private void button13_Click(object sender, EventArgs e)
        {
            AudioSynthesis.SynthesisGetAvailableVoicesAsync(this, comboBox5.Text.ToString());

        }

        private void iconButton24_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabWebCap);//sttts
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
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/releases");
        }

        private void iconButton26_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://trello.com/b/cUhN6eF0/ttsvoicewizard-planned-features");
        }

        private void iconButton25_Click(object sender, EventArgs e)
        {
         //   System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/bGVs2ew08WY");

        }

        private void iconButton27_Click(object sender, EventArgs e)
        {
          //  System.Diagnostics.Process.Start("explorer.exe", "https://youtu.be/wBRUcx9EWes");
        
        }

       // private void iconButton28_Click(object sender, EventArgs e)
       // {
         //   System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Web-Captioner");
       // }

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

        private void button11_Click(object sender, EventArgs e)
        {
            webCapOn();
                
        }
        private void webCapOn()
        {
            System.Diagnostics.Debug.WriteLine("Starting HTTP listener...");
            var httpServer = new HttpServer();
            Task.Run(() => httpServer.Start());
            System.Diagnostics.Debug.WriteLine("Starting HTTP listener Started");
            ot.outputLog(this, "[Starting HTTP listener for Web Captioner Started. Go to https://webcaptioner.com/captioner > Settings (bottom right) > Channels > Webhook > set 'http://localhost:8080/' as the Webhook URL and experiment with different chunking values (I recommend a large value so it only sends when you finish talking). Now you're all set to click 'Start Captioning' in Web Captioner]");
            button11.Enabled = false;
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
            sender3 = new CoreOSC.UDPSender(textBoxOSCAddress.Text.ToString(), Convert.ToInt32(textBoxOSCPort.Text.ToString()));//9000
            Settings1.Default.rememberPort = textBoxOSCPort.Text.ToString();
            Settings1.Default.Save();

        }

        private void button17_Click(object sender, EventArgs e)
        {
            sender3 = new CoreOSC.UDPSender(textBoxOSCAddress.Text.ToString(), Convert.ToInt32(textBoxOSCPort.Text.ToString()));//9000
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
                    comboBox3.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "FonixTalk";
                    if (VoiceWizardWindow.firstVoiceLoad == false)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (VoiceWizardWindow.firstVoiceLoad == true)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        VoiceWizardWindow.firstVoiceLoad = false;
                    }

                    break;
                case "TikTok":
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("en_au_001");
                    comboBox2.Items.Add("en_au_002");
                    comboBox2.Items.Add("en_uk_001");
                    comboBox2.Items.Add("en_uk_003");

                    comboBox2.Items.Add("en_us_001");
                    comboBox2.Items.Add("en_us_002");
                    comboBox2.Items.Add("en_us_006");
                    comboBox2.Items.Add("en_us_007");
                    comboBox2.Items.Add("en_us_009");
                    comboBox2.Items.Add("en_us_010");

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
                    comboBox3.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "TikTok";
                    if (VoiceWizardWindow.firstVoiceLoad == false)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (VoiceWizardWindow.firstVoiceLoad == true)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        VoiceWizardWindow.firstVoiceLoad = false;
                    }

                    break;
                case "System Speech":
                    comboBox2.Items.Clear();
                    foreach (string voice in systemSpeechVoiceList)
                    {
                        comboBox2.Items.Add(voice);
                    }
                    comboBox2.SelectedIndex = 0;
                    comboBox1.Items.Clear();
                    comboBox1.Items.Add("default");
                    comboBox1.SelectedIndex = 0;
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    TTSModeSaved = "System Speech";
                    if (VoiceWizardWindow.firstVoiceLoad == false)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice]");
                        comboBox2.SelectedIndex = 0;
                    }
                    if (VoiceWizardWindow.firstVoiceLoad == true)
                    {
                        ot.outputLog(this, "[DEBUG: setting voice to saved value]");
                        comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
                        VoiceWizardWindow.firstVoiceLoad = false;
                    }
                    break;
                case "Azure":
                    AudioSynthesis.SynthesisGetAvailableVoicesAsync(this, comboBox5.Text.ToString());
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
                    comboBox3.Enabled = false;
                    comboBox5.Enabled = false;
                    comboBoxPitch.Enabled = false;
                    comboBoxVolume.Enabled = false;
                    comboBoxRate.Enabled = false;
                    break;
            }
        }



        private void button19_Click(object sender, EventArgs e)
        {
            comboLiteOutput.Items.Clear();
            foreach (var device in WaveOutDevice.EnumerateDevices())
            {
                System.Diagnostics.Debug.WriteLine("{0}: {1}", device.DeviceId, device.Name);
                comboLiteOutput.Items.Add(device.Name);
            }
            ot.outputLog(this, "[System Speech / FonixTalk Output Devices Reloaded]");
        }
        
        private void iconButton25_Click_1(object sender, EventArgs e)
        {
            switch(richboxsmall)
            {
                case true:
                    //int boxWidth = richTextBox3.Bounds.Width;
                    //int boxHeight = richTextBox3.Bounds.Width;

                    richTextBox3.SetBounds(28, 51, 471, 211);
                    ttsTrash.SetBounds(455, 226, 35, 29);
                    iconButton25.SetBounds(414, 226, 35, 29);
                    speechTTSButton.SetBounds(247, 291, 329, 60);
                    richboxsmall = false;
                    break;
                case false:
                    richTextBox3.SetBounds(28, 51, 471, 79);
                    ttsTrash.SetBounds(453, 95, 35, 29);
                    iconButton25.SetBounds(412, 95, 35, 29);
                    speechTTSButton.SetBounds(28, 148, 546, 122);
                    richboxsmall = true;

                    break;
            }
       

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            ot.outputVRChat(this, OutputText.lastKatString, "tttAdd");
        }


        private void button18_Click_1(object sender, EventArgs e)
        {
            string currentText = richTextBox11.Text.ToString();
            currentText = currentText + ", " + WindowsMedia.mediaSourceNew + ", ";
            richTextBox11.Text = currentText;
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "♫ {title} - {artist} ♫ ";
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
            currentText = currentText + "『🎮{averageControllerBattery}%』『🪫{averageTrackerBattery}%』 ";
            textBoxCustomSpot.Text = currentText;

        }

        private void button22_Click(object sender, EventArgs e)
        {
            string currentText = textBoxCustomSpot.Text.ToString();
            currentText = currentText + "『💓{bpm}』 ";
            textBoxCustomSpot.Text = currentText;
        }


        private void iconButton27_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage2);//voiceCommands
        }


        private void buttonAddVoiceCommand_Click(object sender, EventArgs e)
        {
             VCAddress.Clear();
              VCPhrase.Clear();
              VCValue.Clear();
              VCType.Clear();
            voiceCommandsStored += $"{textBox1Spoken.Text.ToString()}:{textBox2Address.Text.ToString()}:{comboBox3Type.SelectedItem.ToString()}:{textBox4Value.Text.ToString()};";
            var vc = new VoiceCommands();
            vc.voiceCommands();
            vc.refreshCommandList();


        }

        private void buttonRemoveVoiceCommand_Click(object sender, EventArgs e)
        {
            int index = Int32.Parse(textBox4Value.Text.ToString()) - 1;
            VCAddress.RemoveAt(index);
            VCPhrase.RemoveAt(index);
            VCValue.RemoveAt(index);
            VCType.RemoveAt(index);
            var vc = new VoiceCommands();
            vc.refreshCommandList();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            VCAddress.Clear();
            VCPhrase.Clear();
            VCValue.Clear();
            VCType.Clear();
            var vc = new VoiceCommands();
            vc.refreshCommandList();

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


        private void checkedListBox1_DoubleClick(object sender, EventArgs e)
        {
           
            
        }

        private void rjToggleButtonLiteMode_CheckedChanged(object sender, EventArgs e)
        {
            if(rjToggleButtonLiteMode.Checked==true)
            {
                ot.outputLog(this, "[Input device for System Speech is forced to default device, you can either change your default deivce in Control Panel > Sound or change this apps device specifically in Windows Settings > Sound]");
                comboBoxInput.SelectedIndex = 0;
                comboBoxInput.Enabled = false;

            }
            if (rjToggleButtonLiteMode.Checked == false)
            {
                comboBoxInput.Enabled = true;
                try
                {
                    comboBoxInput.SelectedItem = Settings1.Default.MicName;
                }
                catch(Exception ex)
                {

                }

                

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

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if(deleteCommandsToggle.Checked==true)
            {
                try
                {
                    VCAddress.RemoveAt(checkedListBox1.SelectedIndex);
                    VCPhrase.RemoveAt(checkedListBox1.SelectedIndex);
                    VCValue.RemoveAt(checkedListBox1.SelectedIndex);
                    VCType.RemoveAt(checkedListBox1.SelectedIndex);
                    var vc = new VoiceCommands();
                    vc.refreshCommandList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }
           
        }

        private void rjToggleButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonWebCapAzure_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonDisableTTS2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rjToggleButtonMedia_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxInput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            currentInputDevice = micIDs[comboBoxInput.SelectedIndex];
            currentInputDeviceName = comboBoxInput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("mic changed", currentInputDevice);
        }

        private void comboBoxOutput_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            currentOutputDevice = speakerIDs[comboBoxOutput.SelectedIndex];
            currentOutputDeviceName = comboBoxOutput.SelectedItem.ToString();
            System.Diagnostics.Debug.WriteLine("speaker changed");
        }

        private void iconButton28_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Web-Captioner");

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
