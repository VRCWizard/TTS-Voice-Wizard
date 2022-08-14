//Wizard
using Microsoft.CognitiveServices.Speech;//Subcription Azure
using Microsoft.CognitiveServices.Speech.Audio; //Subcription Azure
using Microsoft.CognitiveServices.Speech.Translation;
using SharpOSC;
using System;
using System.Speech.Recognition;//free Windows
using System.Speech;//free windows
using System.Speech.Synthesis;//free windows
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;



namespace OSCVRCWiz
{
    public partial class VoiceWizardWindow : Form
    {

        /// public static string YourSubscriptionKey = Settings1.Default.yourKey;
        /// public static string YourServiceRegion = Settings1.Default.yourRegion;
        public static string YourSubscriptionKey;
        public static string YourServiceRegion;
        public string dictationString = "";
        public string activationWord = Settings1.Default.activationWord;
        public int debugDelayValue = Convert.ToInt32(Settings1.Default.delayDebugValueSetting);// Recommended delay of 250ms 
        public int eraseDelay = Convert.ToInt32(Settings1.Default.hideDelayValue);
        int audioOutputIndex = -1;
        public bool profanityFilter = true;
        public string numKATSyncParameters = "4";
        //  SpeechRecognitionEngine recognizer;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
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
        public SharpOSC.UDPSender sender3;
        public bool justShowTheSong = false;
        public static int heartRatePort = 4026;
        public static bool pauseBPM = false;
        public static bool pauseSpotify = false;
        public static bool stopBPM = false; // fix later should be set to setting value
        public static bool BPMSpamLog = true;
        public static int HRInternalValue = 5;
        public static string TTSLiteText = "";
        

        public string CultureSelected = "en-US";//free
        public System.Speech.Synthesis.SpeechSynthesizer synthesizerLite;//free
        public MemoryStream stream;


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
            InitializeComponent();
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
            sender3 = new SharpOSC.UDPSender("127.0.0.1", 9000);//9000
            testtimer = new System.Threading.Timer(testtimertick);
            testtimer.Change(Timeout.Infinite, Timeout.Infinite);

            foreach (var voice in synthesizerLite.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                System.Diagnostics.Debug.WriteLine($"Id: {info.Id} | Name: {info.Name} | Age: { info.Age} | Gender: { info.Gender} | Culture: { info.Culture}");
                comboBoxLite.Items.Add(info.Name + "|" + info.Culture);
            }
            TTSLiteText = richTextBox3.Text.ToString();

           // webView21.AutoScrollOffset(0, 109);
            int id = 0;// The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.G.GetHashCode());

        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                // speechTTSButton.PerformClick();
                Task.Run(() => doSpeechTTS());

            }
        }
        private void hideVRCTextButton_Click(object sender, EventArgs e)//speech to text
        {
            var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            sender2.Send(message0);
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
            comboBox1.Items.Clear();
            comboBox1.Items.Add("normal");
            switch (comboBox2.Text.ToString())
            {
                case "Aria":
                    comboBox1.Items.Add("angry");
                    comboBox1.Items.Add("chat");
                    comboBox1.Items.Add("cheerful");//replace happy
                    comboBox1.Items.Add("customerservice");
                    comboBox1.Items.Add("empathetic");
                    comboBox1.Items.Add("excited");//new
                    comboBox1.Items.Add("friendly");//new
                    comboBox1.Items.Add("hopeful");//new
                    comboBox1.Items.Add("narration-professional");
                    comboBox1.Items.Add("newscast-casual");
                    comboBox1.Items.Add("newscast-formal");
                    comboBox1.Items.Add("sad");//new for this voice
                    comboBox1.Items.Add("shouting");//new
                    comboBox1.Items.Add("terrified");//new
                    comboBox1.Items.Add("unfriendly");//new
                    comboBox1.Items.Add("whispering");//new
                    break;
                case "Davis <Preview>":
                    comboBox1.Items.Add("angry");
                    comboBox1.Items.Add("chat");
                    comboBox1.Items.Add("cheerful");//replace happy
                    comboBox1.Items.Add("excited");//new
                    comboBox1.Items.Add("friendly");//new
                    comboBox1.Items.Add("hopeful");//new
                    comboBox1.Items.Add("sad");//new for this voice
                    comboBox1.Items.Add("shouting");//new
                    comboBox1.Items.Add("terrified");//new
                    comboBox1.Items.Add("unfriendly");//new
                    comboBox1.Items.Add("whispering");//new
                    break;
                case "Guy":
                    comboBox1.Items.Add("angry");//new
                    comboBox1.Items.Add("cheerful");//new
                    comboBox1.Items.Add("excited");//new
                    comboBox1.Items.Add("friendly");//new
                    comboBox1.Items.Add("hopeful");//new
                    comboBox1.Items.Add("newscast");
                    comboBox1.Items.Add("sad");//new 
                    comboBox1.Items.Add("shouting");//new
                    comboBox1.Items.Add("terrified");//new
                    comboBox1.Items.Add("unfriendly");//new
                    comboBox1.Items.Add("whispering");//new
                    break;
                case "Jenny":
                    comboBox1.Items.Add("angry");//new
                    comboBox1.Items.Add("assistant");
                    comboBox1.Items.Add("chat");
                    comboBox1.Items.Add("cheerful");//new
                    comboBox1.Items.Add("customerservice");
                    comboBox1.Items.Add("excited");//new
                    comboBox1.Items.Add("friendly");//new
                    comboBox1.Items.Add("hopeful");//new
                    comboBox1.Items.Add("newscast");
                    comboBox1.Items.Add("sad");//new 
                    comboBox1.Items.Add("shouting");//new
                    comboBox1.Items.Add("terrified");//new
                    comboBox1.Items.Add("unfriendly");//new
                    comboBox1.Items.Add("whispering");//new
                    break;
                default:
                    if (comboBox2.Text.ToString() == "Jason <Preview>" || comboBox2.Text.ToString() == "Nancy <Preview>" || comboBox2.Text.ToString() == "Jane <Preview>" || comboBox2.Text.ToString() == "Sara" || comboBox2.Text.ToString() == "Tony <Preview>")
                    {
                        comboBox1.Items.Add("angry");//new
                        comboBox1.Items.Add("cheerful");//new
                        comboBox1.Items.Add("excited");//new
                        comboBox1.Items.Add("friendly");//new
                        comboBox1.Items.Add("hopeful");//new
                        comboBox1.Items.Add("sad");//new 
                        comboBox1.Items.Add("shouting");//new
                        comboBox1.Items.Add("terrified");//new
                        comboBox1.Items.Add("unfriendly");//new
                        comboBox1.Items.Add("whispering");//new
                    }
                    break;


            }



            comboBox1.SelectedIndex = 0;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText("logfile.txt", String.Empty);
            }
            catch (Exception ex) { }


            rjToggleButtonActivation.Checked = Settings1.Default.recognition; //activation phrase off
            textBoxActivationWord.Text = Settings1.Default.activationWord;
            activationWord = Settings1.Default.activationWord;
            if (Settings1.Default.recognition == true)
            {
                var va = new VoiceActivation();

                va.loadSpeechRecognition(this);
                MessageBox.Show("[STTTS Voice Activation Initiated]");
            }
            textBox2.Text = Settings1.Default.yourKey;
            textBox3.Text = Settings1.Default.yourRegion;
            YourSubscriptionKey = Settings1.Default.yourKey;
            YourServiceRegion = Settings1.Default.yourRegion;

            textBoxDelay.Text = Settings1.Default.delayDebugValueSetting;
            rjToggleButtonProfan.Checked = Settings1.Default.profanityFilterSetting;//on
            rjToggleButtonLog.Checked = Settings1.Default.logOrNotSetting;//on
            rjToggleButtonOSC.Checked = Settings1.Default.sendOSCSetting;//on
            rjToggleButtonClear.Checked = Settings1.Default.clearTTSWindowSetting;//off
            rjToggleButtonOnTop2.Checked = Settings1.Default.alwaysTopSetting;//off
            rjToggleButtonDisableTTS2.Checked = Settings1.Default.disableTTSSetting; //off
            rjToggleButtonAsTranslated2.Checked = Settings1.Default.wordsTranslateVRCSetting;
            rjToggleButtonHideDelay2.Checked = Settings1.Default.hideDelaySetting;//off
            textBoxErase.Text = Settings1.Default.hideDelayValue;
            richTextBox6.Text = Settings1.Default.phraseListValue;
            rjToggleButtonPhraseList2.Checked = Settings1.Default.phraseListBoolSetting;
            YourSubscriptionKey = Settings1.Default.yourKey;
            YourServiceRegion = Settings1.Default.yourRegion;
            rjToggleButtonKeyRegion2.Checked = Settings1.Default.remember;
            rjToggleButton3.Checked = Settings1.Default.EmojiSetting;
            rjToggleButtonCurrentSong.Checked = Settings1.Default.SpotifyOutputSetting;
            HRInternalValue = Convert.ToInt32(Settings1.Default.HRIntervalSetting);
            heartRatePort = Convert.ToInt32(Settings1.Default.HRPortSetting);
            rjToggleButton2.Checked = Settings1.Default.BPMSpamSetting;
            comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
            comboBox1.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
            comboBox3.SelectedIndex = Settings1.Default.langToBoxSetting;//language to
            comboBox4.SelectedIndex = Settings1.Default.langSpokenSetting;//language from [5 is english0
            comboBoxPitch.SelectedIndex = Settings1.Default.pitchSetting;
            comboBoxVolume.SelectedIndex = Settings1.Default.volumeSetting;
            comboBoxRate.SelectedIndex = Settings1.Default.rateSetting;
            rjToggleButton4.Checked = Settings1.Default.STTTSContinuous;
            comboBoxInput.SelectedItem = Settings1.Default.MicName;
            comboBoxOutput.SelectedItem = Settings1.Default.SpeakerName;
            rjToggleButtonLiteMode.Checked = Settings1.Default.useBuiltInSetting;
            comboLiteInput.SelectedIndex = 0;

            comboBoxLite.SelectedIndex = Settings1.Default.BuiltInVoiceSetting;
            comboLiteOutput.SelectedIndex = 0;

            rjToggleButton5.Checked = Settings1.Default.bannerSetting;
            if(rjToggleButton5.Checked==true)
            {
                webView21.Dispose();
                button10.Dispose();
                button9.Dispose();
            }

            try
            {
                comboLiteOutput.SelectedItem = Settings1.Default.BuiltInOutputSetting;

            }
            catch (Exception ex)
            {
                comboLiteOutput.SelectedIndex = 0;

            }
            
           // comboLiteOutput.SelectedIndex = Settings1.Default.BuiltInOutputSetting;

            rjToggleButtonPeriodic.Checked = Settings1.Default.SpotifyPeriodicallySetting;
            rjToggleButtonSpotifySpam.Checked = Settings1.Default.SpotifySpamSetting;

            textBoxSpotifyTime.Text = Settings1.Default.SpotifyTimerIntervalSetting;
            timer1.Interval = Int32.Parse(textBoxSpotifyTime.Text.ToString());

            rjToggleButtonCancelAudio.Checked = Settings1.Default.AudioCancelSetting;

            textBoxCultureInfo.Text = Settings1.Default.cultureInfoSetting;

            textBoxSpotKey.Text = Settings1.Default.SpotifyKey;
            rjToggleSpotLegacy.Checked = Settings1.Default.SpotifyLegacySetting;
            SpotifyAddon.legacyState = rjToggleSpotLegacy.Checked;

            rjToggleButtonChatBox.Checked = Settings1.Default.SendVRCChatBoxSetting;
            rjToggleButtonShowKeyboard.Checked = Settings1.Default.ChatBoxKeyboardSetting;



            this.Invoke((MethodInvoker)delegate ()
            {
                comboBoxPara.SelectedIndex = Settings1.Default.SyncParaValue;

            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            if (rjToggleButtonKeyRegion2.Checked == false)
            {
                Settings1.Default.yourRegion = "";
                Settings1.Default.yourKey = "";
            }
            Settings1.Default.recognition = rjToggleButtonActivation.Checked;
            Settings1.Default.profanityFilterSetting = rjToggleButtonProfan.Checked;
            Settings1.Default.logOrNotSetting = rjToggleButtonLog.Checked;
            Settings1.Default.sendOSCSetting = rjToggleButtonOSC.Checked;
            Settings1.Default.clearTTSWindowSetting = rjToggleButtonClear.Checked;
            Settings1.Default.alwaysTopSetting = rjToggleButtonOnTop2.Checked;
            Settings1.Default.disableTTSSetting = rjToggleButtonDisableTTS2.Checked;
            Settings1.Default.wordsTranslateVRCSetting = rjToggleButtonAsTranslated2.Checked;
            Settings1.Default.hideDelaySetting = rjToggleButtonHideDelay2.Checked;
            Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
            Settings1.Default.phraseListValue = richTextBox6.Text.ToString();
            Settings1.Default.phraseListBoolSetting = rjToggleButtonPhraseList2.Checked;
            Settings1.Default.MicName = comboBoxInput.SelectedItem.ToString();
            Settings1.Default.SpeakerName = comboBoxOutput.SelectedItem.ToString();
            Settings1.Default.EmojiSetting = rjToggleButton3.Checked;
            Settings1.Default.SpotifyOutputSetting = rjToggleButtonCurrentSong.Checked;
            Settings1.Default.HRIntervalSetting = HRInternalValue.ToString();
            Settings1.Default.HRPortSetting = heartRatePort.ToString();
            Settings1.Default.BPMSpamSetting = rjToggleButton2.Checked;
            Settings1.Default.voiceBoxSetting = comboBox2.SelectedIndex;
            Settings1.Default.styleBoxSetting = comboBox1.SelectedIndex;
            Settings1.Default.langToBoxSetting = comboBox3.SelectedIndex;
            Settings1.Default.langSpokenSetting = comboBox4.SelectedIndex;
            Settings1.Default.pitchSetting = comboBoxPitch.SelectedIndex;
            Settings1.Default.volumeSetting = comboBoxVolume.SelectedIndex;
            Settings1.Default.rateSetting = comboBoxRate.SelectedIndex;
            Settings1.Default.STTTSContinuous = rjToggleButton4.Checked;
            Settings1.Default.useBuiltInSetting = rjToggleButtonLiteMode.Checked;
            Settings1.Default.BuiltInVoiceSetting = comboBoxLite.SelectedIndex;
            Settings1.Default.BuiltInOutputSetting = comboLiteOutput.SelectedItem.ToString();

            Settings1.Default.SpotifyPeriodicallySetting = rjToggleButtonPeriodic.Checked;
            Settings1.Default.SpotifySpamSetting = rjToggleButtonSpotifySpam.Checked;
            Settings1.Default.SpotifyTimerIntervalSetting = textBoxSpotifyTime.Text.ToString();

            Settings1.Default.AudioCancelSetting = rjToggleButtonCancelAudio.Checked;
            Settings1.Default.cultureInfoSetting= textBoxCultureInfo.Text.ToString();

            Settings1.Default.bannerSetting = rjToggleButton5.Checked;

            Settings1.Default.SpotifyKey = textBoxSpotKey.Text.ToString();
            Settings1.Default.SpotifyLegacySetting = rjToggleSpotLegacy.Checked;

            Settings1.Default.SendVRCChatBoxSetting= rjToggleButtonChatBox.Checked;
            Settings1.Default.ChatBoxKeyboardSetting= rjToggleButtonShowKeyboard.Checked;

            Settings1.Default.Save();
            webView21.Dispose();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = rjToggleButtonKeyRegion2.Checked;
            Settings1.Default.Save();
        }


        private void buttonActivationWord_Click(object sender, EventArgs e)
        {
            activationWord = textBoxActivationWord.Text.ToString();
            Settings1.Default.activationWord = textBoxActivationWord.Text.ToString();
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
            if (YourSubscriptionKey == "" && rjToggleButtonLiteMode.Checked == false)
            {
                var ot = new OutputText();
                ot.outputLog(this, "[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech. You can also change the Windows Built-In System Speech 'Output Device' and 'Voice' in the 'Settings > System Speech' tab]");
            }
            if (rjToggleButtonLiteMode.Checked == true || YourSubscriptionKey == "")
            {
                var lite = new WindowsBuiltInSTTTS();
                Task.Run(() => lite.TTSButtonLiteClick(this));
            }
            else
            {
                Task.Run(() => doTTSOnly());
            }
        }
        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            if(rjToggleButtonChatBox.Checked==true)
            {
                var typingbubble = new SharpOSC.OscMessage("/chatbox/typing", true);
                sender3.Send(typingbubble);

            }
           

            if (YourSubscriptionKey == "" && rjToggleButtonLiteMode.Checked == false)
            {
                var ot = new OutputText();
                ot.outputLog(this, "[No Azure Key detected, defaulting to Windows Built-In System Speech. Add you Azure Key in the 'Settings > Microsoft Azure Cognative Service' tab or enable Windows Built-In System Speech. You can also change the Windows Built-In System Speech 'Output Device' and 'Voice' in the 'Settings > System Speech' tab]");
            }
            if (rjToggleButtonLiteMode.Checked == true || YourSubscriptionKey == "")
            {
                var lite = new WindowsBuiltInSTTTS();
                Task.Run(() => lite.speechTTSButtonLiteClick(this));
            }
            else
            {
                Task.Run(() => doSpeechTTS());
            }
        }
        private void doSpeechTTS()
        {
            var ts = new TextSynthesis();
            this.Invoke((MethodInvoker)delegate ()
            {
                if (comboBox3.Text.ToString() == "No Translation (Default)")
                {
                    ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                    System.Diagnostics.Debug.WriteLine("<speechSetup change>");

                    ts.speechTTTS(this, comboBox4.Text.ToString());

                }
                else
                {
                    ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString()); //only speechSetup needed
                    System.Diagnostics.Debug.WriteLine("<speechSetup change>");


                    ts.translationSTTTS(this, comboBox3.Text.ToString(), comboBox4.Text.ToString());

                }
            });

        }
        private void doTTSOnly()
        {
            string emotion = "Normal";
            string rate = "default";
            string pitch = "default";
            string volume = "default";
            string voice = "Sara";

            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = richTextBox3.Text.ToString();


                if (string.IsNullOrWhiteSpace(comboBox1.Text.ToString()))
                {
                    emotion = "Normal";

                }
                else
                {
                    emotion = comboBox1.Text.ToString();
                }
                ////////////

                if (string.IsNullOrWhiteSpace(comboBoxRate.Text.ToString()))
                {
                    rate = "default";

                }
                else
                {
                    rate = comboBoxRate.Text.ToString();
                }
                //////////
                if (string.IsNullOrWhiteSpace(comboBoxPitch.Text.ToString()))
                {
                    pitch = "default";

                }
                else
                {
                    pitch = comboBoxPitch.Text.ToString();
                }
                //////////
                if (string.IsNullOrWhiteSpace(comboBoxVolume.Text.ToString()))
                {
                    volume = "default";

                }
                else
                {

                    volume = comboBoxVolume.Text.ToString();
                }
                if (string.IsNullOrWhiteSpace(comboBox2.Text.ToString()))
                {
                    voice = "Sara";

                }
                else
                {

                    voice = comboBox2.Text.ToString();
                }


            });
            var ot = new OutputText();
            //Send Text to Vrchat
            if (rjToggleButtonLog.Checked == true)
            {
                ot.outputLog(this, text);
            }
            if (rjToggleButtonDisableTTS2.Checked == false)
            {
               if( rjToggleButtonCancelAudio.Checked == true)
                {
                    TextSynthesis.SpeechCt.Cancel();
                }
                TextSynthesis.SpeechCt = new();
                _ = Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(this, text, emotion, rate, pitch, volume, voice, TextSynthesis.SpeechCt.Token));//new
            }
            if (rjToggleButtonOSC.Checked == true)
            {


                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChat(this, text, "tts")); //original
                                                                    // ot.outputVRChat(this, text);//new
            }
            if (rjToggleButtonChatBox.Checked == true)
            {
                VoiceWizardWindow.pauseBPM = true;
                VoiceWizardWindow.pauseSpotify = true;
                Task.Run(() => ot.outputVRChatSpeechBubbles(this, text, "tts")); //original

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
        private void iconButton2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage1);//sttts

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage3);//provider

        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(SettingsNew);//settings
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(tabPage4);//Dashboard


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

        private void metroTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            // System.Diagnostics.Debug.WriteLine(metroTrackBar1.Value);
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
        private void comboBoxInput_SelectedIndexChanged(object sender, EventArgs e)
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
        }
        private void buttonSpotify_Click(object sender, EventArgs e)
        {
            var sa = new SpotifyAddon();
            sa.SpotifyConnect(this);

        }

        private void timerSpotify_Tick(object sender, EventArgs e)
        {
            Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));
        }

        private void rjToggleButtonCurrentSong_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonCurrentSong.Checked == true)

                timer1.Start();


            if (rjToggleButtonCurrentSong.Checked == false)
            {
                timer1.Stop();

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
        private void doTimerTick()
        {
            // var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255); // causes glitch if enabled
            
            pauseBPM = false;
            pauseSpotify = false;
            if(rjToggleButtonOSC.Checked==true)
            {
                var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
                sender3.Send(message0);
            }
            
            System.Diagnostics.Debug.WriteLine("****-------*****--------Tick");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            justShowTheSong = true;
            Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));

        }

        private void button3_Click(object sender, EventArgs e)
        {
            PopupForm test = new PopupForm();
            test.BackColor = Color.LimeGreen;
           // test.TransparencyKey = Color.Black;

            test.Show(this);
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
            tabControl1.SelectTab(tabAddons);

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

        private void tabHeartBeat_Click(object sender, EventArgs e)
        {

        }
        public void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)//lite version, WindowsBuiltInSTTTS Help
        {
            System.Diagnostics.Debug.WriteLine("Recognized text: " + e.Result.Text);
            string text = e.Result.Text.ToString();
            var ot = new OutputText();
            //Send Text to Vrchat
            if (rjToggleButtonLog.Checked == true)
            {
                ot.outputLog(this, text);

            }
            if (rjToggleButtonDisableTTS2.Checked == false)
            {
                stream = new MemoryStream();//must make a new stream every time for it to work properly (reusing streams is difficult)
                synthesizerLite.SetOutputToWaveStream(stream);
                synthesizerLite.Speak(text);
                var waveOut = new WaveOut { Device = new WaveOutDevice(currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
                var waveSource = new MediaFoundationDecoder(stream);
                waveOut.Initialize(waveSource);
                waveOut.Play();
                waveOut.WaitForStopped();

            }

            if (rjToggleButtonOSC.Checked == true)
            {
                Task.Run(() => ot.outputVRChat(this, text, "tts"));
            }
            if (rjToggleButtonChatBox.Checked == true)
            {
                Task.Run(() => ot.outputVRChatSpeechBubbles(this, text, "tts")); //original

            }
        }

        private void comboBoxLite_SelectedIndexChanged(object sender, EventArgs e)
        {

            string phrase = comboBoxLite.Text.ToString();
            string[] words = phrase.Split('|');
            int counter = 1;
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
        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {
            TTSLiteText = richTextBox3.Text.ToString();
        }

        private void comboLiteOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentOutputDeviceLite = comboLiteOutput.SelectedIndex;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            timer1.Interval = Int32.Parse(textBoxSpotifyTime.Text.ToString());
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
          // {
                webView21.Dispose();
                button10.Dispose();
                button9.Dispose();

           // }


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
    }
}
