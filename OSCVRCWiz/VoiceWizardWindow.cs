//Wizard
using Microsoft.CognitiveServices.Speech;//Subcription Azure
using Microsoft.CognitiveServices.Speech.Audio; //Subcription Azure
using Microsoft.CognitiveServices.Speech.Translation;
using SharpOSC;
using System;
using System.Speech.Recognition;//free Windows
using System.Windows.Forms;
using NAudio.CoreAudioApi;



namespace OSCVRCWiz
{
    public partial class VoiceWizardWindow : Form
    {

        public static string YourSubscriptionKey = Settings1.Default.yourKey;
        public static string YourServiceRegion = Settings1.Default.yourRegion;
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
        public string currentInputDevice ="";
        public string currentOutputDevice = "";
        public string currentInputDeviceName = "Default";
        public string currentOutputDeviceName = "Default";
        public bool getSpotify = false;
        public System.Threading.Timer testtimer;
        public SharpOSC.UDPSender sender3;
        public bool justShowTheSong = false;
        public static int heartRatePort = 4026;
        public static bool pauseBPM = false;
        public static bool stopBPM = false; // fix later should be set to setting value
        public static bool BPMSpamLog = true;
        public static int HRInternalValue= 5;

        //public bool STTTSContinuous = false;


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

            textBox2.PasswordChar = '*';
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;


            sender3 = new SharpOSC.UDPSender("127.0.0.1", 9000);//9000
            testtimer = new System.Threading.Timer(testtimertick);
           // testtimer.Change(5000, 0);
            testtimer.Change(Timeout.Infinite, Timeout.Infinite);










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
        private async void TTSButton_Click(object sender, EventArgs e)//TTS
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
                
                if (string.IsNullOrWhiteSpace(comboBoxRate.Text.ToString()) )
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
                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(this, text, emotion, rate, pitch, volume, voice));//new
            }
            if (rjToggleButtonOSC.Checked == true)
            {


                VoiceWizardWindow.pauseBPM = true;
                Task.Run(() => ot.outputVRChat(this,text, "tts")); //original
               // ot.outputVRChat(this, text);//new
            }
            if(rjToggleButtonClear.Checked ==true)
            {
                richTextBox3.Clear();

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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//voices
        {

            comboBox1.Items.Clear();
            comboBox1.Items.Add("Normal");
            if (comboBox2.Text.ToString() == "Sara")
            {
                comboBox1.Items.Add("Angry");
                comboBox1.Items.Add("Happy");
                comboBox1.Items.Add("Sad");
            }
            if (comboBox2.Text.ToString() == "Jenny")
            {
                comboBox1.Items.Add("Assistant");
                comboBox1.Items.Add("Chat");
                comboBox1.Items.Add("Customer Service");
                comboBox1.Items.Add("Newscast");
            }
            if (comboBox2.Text.ToString() == "Guy")
            {
                comboBox1.Items.Add("Newscast");
            }
            if (comboBox2.Text.ToString() == "Aria")
            {
                comboBox1.Items.Add("Chat");
                comboBox1.Items.Add("Happy");
                comboBox1.Items.Add("Customer Service");
                comboBox1.Items.Add("Empathetic");
                comboBox1.Items.Add("Narration (Professional)");
                comboBox1.Items.Add("Newscast (Casual)");
                comboBox1.Items.Add("Newscast (Formal)");
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
          //  rjToggleButtonConnectSpotify.Checked = Settings1.Default.ConnectSpot;
            textBoxActivationWord.Text = Settings1.Default.activationWord;
            activationWord = Settings1.Default.activationWord;
            if (Settings1.Default.recognition == true)
            {
                var va = new VoiceActivation();

                va.loadSpeechRecognition(this);
                MessageBox.Show("STTTS Voice Activation Initiated");
            }
         //   if (Settings1.Default.ConnectSpot == true)
          //  {
         //       var sa = new SpotifyAddon();
         //       sa.SpotifyConnect(this);

        //    }




                textBox2.Text = Settings1.Default.yourKey;
            textBox3.Text = Settings1.Default.yourRegion;


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
            rjToggleButtonCurrentSong.Checked= Settings1.Default.SpotifyOutputSetting;

            
            HRInternalValue = Convert.ToInt32(Settings1.Default.HRIntervalSetting);
             heartRatePort = Convert.ToInt32(Settings1.Default.HRPortSetting);
            rjToggleButton2.Checked = Settings1.Default.BPMSpamSetting;




            // comboBox2.SelectedIndex = 0;//voice
            //   comboBox1.SelectedIndex = 0;//style (must be set after voice)
            //  comboBox3.SelectedIndex = 0;//language to
            //  comboBox4.SelectedItem = "English [en-US] (Default)";//language from [5 is english0
            //  comboBoxPitch.SelectedIndex = 5;
            //  comboBoxVolume.SelectedIndex = 4;
            //  comboBoxRate.SelectedIndex = 5;
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




            this.Invoke((MethodInvoker)delegate ()
            {
                comboBoxPara.SelectedIndex = Settings1.Default.SyncParaValue;

            });




           /// var ts = new TextSynthesis();
          //  this.Invoke((MethodInvoker)delegate ()
          //  {
              //// ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString());
           // });
          ////  System.Diagnostics.Debug.WriteLine("<speechSetup Form Load>");
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
           // Settings1.Default.ConnectSpot = rjToggleButtonConnectSpotify.Checked;


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

            Settings1.Default.MicName =comboBoxInput.SelectedItem.ToString();
            Settings1.Default.SpeakerName = comboBoxOutput.SelectedItem.ToString();


            Settings1.Default.EmojiSetting = rjToggleButton3.Checked;
            Settings1.Default.SpotifyOutputSetting = rjToggleButtonCurrentSong.Checked;
            Settings1.Default.HRIntervalSetting = HRInternalValue.ToString();
            Settings1.Default.HRPortSetting = heartRatePort.ToString();
            Settings1.Default.BPMSpamSetting = rjToggleButton2.Checked;




             Settings1.Default.voiceBoxSetting= comboBox2.SelectedIndex;
            Settings1.Default.styleBoxSetting = comboBox1.SelectedIndex;
             Settings1.Default.langToBoxSetting = comboBox3.SelectedIndex;
            Settings1.Default.langSpokenSetting = comboBox4.SelectedIndex;
              Settings1.Default.pitchSetting= comboBoxPitch.SelectedIndex;
             Settings1.Default.volumeSetting= comboBoxVolume.SelectedIndex;
             Settings1.Default.rateSetting= comboBoxRate.SelectedIndex;



            Settings1.Default.STTTSContinuous = rjToggleButton4.Checked;






            Settings1.Default.Save();
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
            if(rjToggleButtonProfan.Checked == true)
            {
                profanityFilter = true;

            }
            if (rjToggleButtonProfan.Checked == false)
            {
                profanityFilter = false;

            }

          //  var ts = new TextSynthesis();
          //  this.Invoke((MethodInvoker)delegate ()
           // {
             //   ts.speechSetup(this, comboBox3.Text.ToString(), comboBox4.Text.ToString());
           // });
         //   System.Diagnostics.Debug.WriteLine("<speechSetup Profanity Filter Changed>");

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

        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            Task.Run(() => doSpeechTTS());
            
           
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
            tabControl1.SelectTab(tabPage2);//settings
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
            currentInputDevice= micIDs[comboBoxInput.SelectedIndex];
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            //var sa = new SpotifyAddon();
          // if(getSpotify==true)
         //   {
                Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));

          //  }
       
           //////////////////////////// SpotifyAddon.getCurrentSongInfo(this); //Use Task.Run to prevent application from freezing while completeing this action

           
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

            Thread t = new Thread(dostuff);
            t.Start();
        }
        private void dostuff()
        {
           // var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255); // causes glitch if enabled
            var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            pauseBPM = false;

            sender3.Send(message0);
            
            //Executes some code
            //Thread.Sleep(100);
            System.Diagnostics.Debug.WriteLine("****-------*****--------Tick");
           // testtimer.Change(5000, 0);
          //  testtimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            justShowTheSong = true;
            Task.Run(() => SpotifyAddon.getCurrentSongInfo(this));

        }

        private void button3_Click(object sender, EventArgs e)
        {
            PopupForm test  = new PopupForm();
            test.BackColor = Color.Black;
            test.TransparencyKey = Color.Black;

            test.Show(this);
        }
       

        private void rjToggleButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(rjToggleButton1.Checked == true)
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
            if(rjToggleButton2.Checked == true)
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

        private void iconButton8_Click(object sender, EventArgs e)
        {
           // tabControl1.SelectTab(tabPage5);//provider

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

        private void rjToggleButton3_CheckedChanged(object sender, EventArgs e)
        {
            //delete

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

        private void rjToggleButton4_CheckedChanged(object sender, EventArgs e)
        {
         //   if (rjToggleButton4.Checked == true)
         //   {

         //   }
          //  if (rjToggleButton4.Checked == false)
          //  {


          //  }
        }
    }
}