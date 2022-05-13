//Wizard
using Microsoft.CognitiveServices.Speech;//Subcription Azure
using Microsoft.CognitiveServices.Speech.Audio; //Subcription Azure
using Microsoft.CognitiveServices.Speech.Translation;
using SharpOSC;
using System;
using System.Speech.Recognition;//free Windows
using System.Windows.Forms;



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
        //  SpeechRecognitionEngine recognizer;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);





        
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


            InitializeComponent();

            textBox2.PasswordChar = '*';
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
           
            



            int id = 0;// The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.G.GetHashCode());
            




        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                speechTTSButton.PerformClick();
   
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
            if (rjToggleButton2.Checked == true)
            {
                ot.outputLog(this, text);
            }
            if (rjToggleButtonDisableTTS.Checked == false)
            {
                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(text, emotion, rate, pitch, volume, voice));//new
            }
            if (rjToggleButton4.Checked == true)
            {



                Task.Run(() => ot.outputVRChat(this,text)); //original
               // ot.outputVRChat(this, text);//new
            }
            if(rjToggleButton3.Checked ==true)
            {
                richTextBox3.Clear();

            }
           
            //Send Text to TTS


            // Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(text, emotion, rate, pitch, volume, voice)); //original


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



        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox2.Text.ToString();
                YourSubscriptionKey = text;
                if (rjToggleButtonKeyRegion.Checked == true)
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
                if (rjToggleButtonKeyRegion.Checked == true)
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

            rjToggleButton7.Checked = Settings1.Default.recognition;
            textBoxActivationWord.Text = Settings1.Default.activationWord;
            activationWord = Settings1.Default.activationWord;
            if (Settings1.Default.recognition == true)
            {
                var va = new VoiceActivation();

                va.loadSpeechRecognition(this);
                MessageBox.Show("STTTS Voice Activation Initiated");
            }



            textBox2.Text = Settings1.Default.yourKey;
            textBox3.Text = Settings1.Default.yourRegion;


            textBoxDelay.Text = Settings1.Default.delayDebugValueSetting;
            rjToggleButton1.Checked = Settings1.Default.profanityFilterSetting;
            rjToggleButton2.Checked = Settings1.Default.logOrNotSetting;
            rjToggleButton4.Checked = Settings1.Default.sendOSCSetting;
            rjToggleButton3.Checked = Settings1.Default.clearTTSWindowSetting;


            rjToggleButtonOnTop.Checked = Settings1.Default.alwaysTopSetting;
            rjToggleButtonDisableTTS.Checked = Settings1.Default.disableTTSSetting;

            rjToggleButtonHideDelay.Checked = Settings1.Default.hideDelaySetting;
            textBoxErase.Text = Settings1.Default.hideDelayValue;
            rjToggleButtonTextAsTranslated.Checked = Settings1.Default.wordsTranslateVRCSetting;


            richTextBox6.Text = Settings1.Default.phraseListValue;
            rjTogglePhraseList.Checked = Settings1.Default.phraseListBoolSetting;






            YourSubscriptionKey = Settings1.Default.yourKey;
            YourServiceRegion = Settings1.Default.yourRegion;

            rjToggleButtonKeyRegion.Checked = Settings1.Default.remember;

            
            comboBox2.SelectedIndex = 0;//voice
            comboBox1.SelectedIndex = 0;//style (must be set after voice)
            comboBox3.SelectedIndex = 0;//language to
            comboBox4.SelectedItem = "English [en-US] (Default)";//language from [5 is english0
            comboBoxPitch.SelectedIndex = 5;
            comboBoxVolume.SelectedIndex = 4;
            comboBoxRate.SelectedIndex = 5;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            if (rjToggleButtonKeyRegion.Checked == false)
            {
                Settings1.Default.yourRegion = "";
                Settings1.Default.yourKey = "";
            }
            Settings1.Default.recognition = rjToggleButton7.Checked;


            Settings1.Default.profanityFilterSetting = rjToggleButton1.Checked;
            Settings1.Default.logOrNotSetting = rjToggleButton2.Checked;
            Settings1.Default.sendOSCSetting = rjToggleButton4.Checked;
            Settings1.Default.clearTTSWindowSetting = rjToggleButton3.Checked;


            Settings1.Default.alwaysTopSetting = rjToggleButtonOnTop.Checked;
            Settings1.Default.disableTTSSetting = rjToggleButtonDisableTTS.Checked;

            Settings1.Default.hideDelaySetting = rjToggleButtonHideDelay.Checked;
            Settings1.Default.hideDelayValue = textBoxErase.Text.ToString();
            Settings1.Default.wordsTranslateVRCSetting = rjToggleButtonTextAsTranslated.Checked;

            Settings1.Default.phraseListValue = richTextBox6.Text.ToString();
            Settings1.Default.phraseListBoolSetting = rjTogglePhraseList.Checked;



            Settings1.Default.Save();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = rjToggleButtonKeyRegion.Checked;
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
            if(rjToggleButton1.Checked == true)
            {
                profanityFilter = true;

            }
            if (rjToggleButton1.Checked == false)
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

        private void speechTTSButton_Click(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate ()
            {
                if (comboBox3.Text.ToString() == "No Translation (Default)")
                {
                    TextSynthesis.speechTTTS(this, comboBox4.Text.ToString());

                }
                else
                {
                    TextSynthesis.translationSTTTS(this,comboBox3.Text.ToString(), comboBox4.Text.ToString());

                }
            });
        }



        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (rjToggleButtonOnTop.Checked == true)
            {
                TopMost = true;

            }
            if (rjToggleButtonOnTop.Checked == false)
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
    }
}