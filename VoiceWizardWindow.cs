//Wizard
using Microsoft.CognitiveServices.Speech;//Subcription Azure
using Microsoft.CognitiveServices.Speech.Audio; //Subcription Azure
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
        string dictationString = "";
        public string activationWord = Settings1.Default.activationWord;
        public int debugDelayValue = 250;// Recommended delay of 250ms 
        int audioOutputIndex = -1;
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
                text = textBox1.Text.ToString();


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
            if (checkBox2.Checked == true)
            {
                ot.outputLog(this,text);
            }
            if (checkBox1.Checked == true)
            {



                Task.Run(() => ot.outputVRChat(this,text));
            }
            //Send Text to TTS
            
            
            Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(text, emotion, rate, pitch, volume, voice, audioOutputIndex));

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


            });
        }



        private async void speechTTSButton_Click(object sender, EventArgs e)//speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {
                var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);


                speechConfig.SpeechRecognitionLanguage = "en-US";

                //To recognize speech from an audio file, use `FromWavFileInput` instead of `FromDefaultMicrophoneInput`:
                //using var audioConfig = AudioConfig.FromWavFileInput("YourAudioFile.wav");
                var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
                var speechRecognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);

                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
                //OutputSpeechRecognitionResult(speechRecognitionResult);


                dictationString = speechRecognitionResult.Text; //Dictation string
                string emotion = "Normal";
                string rate = "default";
                string pitch = "default";
                string volume = "default";
                string voice = "Sara";
                this.Invoke((MethodInvoker)delegate ()
                {
                    if (string.IsNullOrWhiteSpace(comboBox1.Text.ToString()))
                    {
                        emotion = "Normal";

                    }
                    else
                    {
                        emotion = comboBox1.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(comboBoxRate.Text.ToString()))
                    {
                        rate = "default";

                    }
                    else
                    {
                        rate = comboBoxRate.Text.ToString();
                    }
                    if (string.IsNullOrWhiteSpace(comboBoxPitch.Text.ToString()))
                    {
                        pitch = "default";

                    }
                    else
                    {
                        pitch = comboBoxPitch.Text.ToString();
                    }
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
                if (checkBox2.Checked == true)
                {
                    ot.outputLog(this,dictationString);
                }
                //Send Text to Vrchat
                if (checkBox1.Checked == true)
                {
                    Task.Run(() => ot.outputVRChat(this,dictationString));
                }
                //Send Text to TTS

                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(dictationString, emotion, rate, pitch, volume, voice, audioOutputIndex));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No valid subscription key given or speech service has been disabled");

            }
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
                if (checkBox3.Checked == true)
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
                if (checkBox3.Checked == true)
                {
                    Settings1.Default.yourRegion = text;
                    Settings1.Default.Save();

                }


            });


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
            if (checkBox3.Checked == false)
            {
                Settings1.Default.yourRegion = "";
                Settings1.Default.yourKey = "";
            }
            Settings1.Default.recognition = checkBox4.Checked;
            Settings1.Default.Save();
        }

        private void comboBoxVirtualOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioOutputIndex = comboBoxVirtualOutput.SelectedIndex - 1; //+1 while i have mapped device in there
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            checkBox4.Checked = Settings1.Default.recognition;
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

            YourSubscriptionKey = Settings1.Default.yourKey;
            YourServiceRegion = Settings1.Default.yourRegion;

            checkBox3.Checked = Settings1.Default.remember;

        }
     
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Settings1.Default.remember = checkBox3.Checked;
            Settings1.Default.Save();
        }


        private void buttonActivationWord_Click(object sender, EventArgs e)
        {
            activationWord = textBoxActivationWord.Text.ToString();
            Settings1.Default.activationWord = textBoxActivationWord.Text.ToString();
            Settings1.Default.Save();
        }
    }
}