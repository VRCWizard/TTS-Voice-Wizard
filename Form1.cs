using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using SharpOSC;
using System;
using System.Windows.Forms;

namespace OSCVRCWiz
{
    public partial class Form1 : Form
    {
        
            static string YourSubscriptionKey = "";
            static string YourServiceRegion = "eastus";
            string dictationString = "";
            int debugDelayValue = 250;//KillFrenzy Recommends a delay of 250ms (look into 8 characters at once)

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);



        static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
            {
                switch (speechRecognitionResult.Reason)
                {
                    case ResultReason.RecognizedSpeech:
                    System.Diagnostics.Debug.WriteLine($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                        break;
                    case ResultReason.NoMatch:
                    System.Diagnostics.Debug.WriteLine($"NOMATCH: Speech could not be recognized.");
                        break;
                    case ResultReason.Canceled:
                        var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                    System.Diagnostics.Debug.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                        System.Diagnostics.Debug.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        System.Diagnostics.Debug.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        System.Diagnostics.Debug.WriteLine($"CANCELED: Double check the speech resource key and region.");
                        }
                        break;
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
        public Form1()
        {


            InitializeComponent();
            textBox2.Text = "";
            textBox2.PasswordChar = '*';
            int id = 0;// The id of the hotkey. 
            RegisterHotKey(this.Handle, id, (int)KeyModifier.Control, Keys.G.GetHashCode());

        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                //  Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                //  KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
                //   int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                button3.PerformClick();
              //  MessageBox.Show("Hotkey has been pressed!");
                // do something
            }
        }
        private async void button1_Click(object sender, EventArgs e)//speech to text
        {

            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox1.Text.ToString();


            });
            //Send Text to Vrchat
            if (checkBox2.Checked == true)
            {
                outputLog(text);
            }
                if (checkBox1.Checked == true)
            {



                Task.Run(() => outputVRChat(text));
            }
            //Send Text to TTS
            Task.Run(() => SynthesizeAudioAsync(text, "Normal","default", "default", "default", "Sara"));
        }
        private void button2_Click(object sender, EventArgs e)//speech to text
        {
            var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);
            var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
            sender2.Send(message0);



        }
        private void button4_Click(object sender, EventArgs e)//speech to text
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
    
      

        private async void button3_Click(object sender, EventArgs e)//speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            //To recognize speech from an audio file, use `FromWavFileInput` instead of `FromDefaultMicrophoneInput`:
            //using var audioConfig = AudioConfig.FromWavFileInput("YourAudioFile.wav");
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
            
            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            OutputSpeechRecognitionResult(speechRecognitionResult);


            dictationString = speechRecognitionResult.Text; //Dictation string
            string emotion = "Normal";
            string rate = "default";
            string pitch = "default";
            string volume = "default";
            string voice = "Sara";
            this.Invoke((MethodInvoker)delegate ()
            {
                if (comboBox1.Text.ToString() == null || comboBox1.Text.ToString() == "" || comboBox1.Text.ToString() == " ")
                {
                    emotion = "Normal";

                }   
                else
                {
                    emotion = comboBox1.Text.ToString();
                }
                ////////////
                if (comboBoxRate.Text.ToString() == null || comboBoxRate.Text.ToString() == "" || comboBoxRate.Text.ToString() == " ")
                {
                    rate = "default";

                }
                else
                {
                    rate = comboBoxRate.Text.ToString();
                }
                //////////
                if (comboBoxPitch.Text.ToString() == null || comboBoxPitch.Text.ToString() == "" || comboBoxPitch.Text.ToString() == " ")
                {
                    pitch = "default";

                }
                else
                {
                    pitch = comboBoxPitch.Text.ToString();
                }
                //////////
                if (comboBoxVolume.Text.ToString() == null || comboBoxVolume.Text.ToString() == "" || comboBoxVolume.Text.ToString() == " ")
                {
                    volume = "default";

                }
                else
                {
                    volume = comboBoxVolume.Text.ToString();
                }
                if (comboBox2.Text.ToString() == null || comboBox2.Text.ToString() == "" || comboBox2.Text.ToString() == " ")
                {
                    voice = "Sara";

                }
                else
                {
                    voice = comboBox2.Text.ToString();
                }


            });
            if (checkBox2.Checked == true)
            {
                outputLog(dictationString);
            }
            //Send Text to Vrchat
            if (checkBox1.Checked == true)
            {
                Task.Run(() => outputVRChat(dictationString));
            }
            //Send Text to TTS

            Task.Run(() => SynthesizeAudioAsync(dictationString,emotion, rate, pitch, volume, voice));
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

        static async Task SynthesizeAudioAsync(string text, string style,string rate, string pitch, string volume,string voice) //TTS Outputs through speakers //can not change voice style
            {
                var config = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
                // Note: if only language is set, the default voice of that language is chosen.
                //  config.SpeechSynthesisLanguage = "<your-synthesis-language>"; // For example, "de-DE"
                // The voice setting will overwrite the language setting.
                // The voice setting will not overwrite the voice element in input SSML.
                //config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
                // config.SpeechSynthesisVoiceName = "en-US-SaraNeural";


                // https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support#speaker-recognition
                string angry = "<mstts:express-as style=\"angry\">"; //1
                string happy = "<mstts:express-as style=\"cheerful\">"; //2
                string sad = "<mstts:express-as style=\"sad\">";//3

            string ratexslow = "<prosody rate=\"x-slow\">"; //1
            string rateslow = "<prosody rate=\"slow\">"; //2
            string ratemedium= "<prosody rate=\"medium\">"; //3
            string ratefast = "<prosody rate=\"fast\">"; //4
            string ratexfast = "<prosody rate=\"x-fast\">"; //5

            string pitchxlow = "<prosody pitch=\"x-low\">"; //1
            string pitchlow = "<prosody pitch=\"low\">"; //2
            string pitchmedium = "<prosody pitch=\"medium\">"; //3
            string pitchhigh = "<prosody pitch=\"high\">"; //4
            string pitchxhigh = "<prosody pitch=\"x-high\">"; //5

            string volumexlow = "<prosody volume=\"x-soft\">"; //1
            string volumelow = "<prosody volume=\"soft\">"; //2
            string volumemedium = "<prosody volume=\"medium\">"; //3
            string volumehigh = "<prosody volume=\"loud\">"; //4
            string volumexhigh = "<prosody volume=\"x-loud\">"; //5


            string sara = "<voice name=\"en-US-SaraNeural\">"; ;//1
            string jenny = "<voice name=\"en-US-JennyNeural\">"; ;//2
            string guy = "<voice name=\"en-US-GuyaNeural\">"; ;//3
            string amber = "<voice name=\"en-US-AmberNeural\">"; ;//4
            string ana = "<voice name=\"en-US-AnaNeural\">"; ;//5
            string aria = "<voice name=\"en-US-AriaNeural\">"; ;//6
            string ashley = "<voice name=\"en-US-AshleyNeural\">"; ;//7
            string brandon = "<voice name=\"en-US-BrandonNeural\">"; ;//8
            string christopher = "<voice name=\"en-US-ChristopherNeural\">"; ;//9
            string cora = "<voice name=\"en-US-CoraNeural\">"; ;//10
            string elizabeth = "<voice name=\"en-US-ElizabethNeural\">"; ;//11
            string eric = "<voice name=\"en-US-EricNeural\">"; ;//12
            string jacob = "<voice name=\"en-US-JacobNeural\">"; ;//13
            string michelle = "<voice name=\"en-US-MichelleNeural\">"; ;//14
            string monica = "<voice name=\"en-US-MonicaNeural\">"; ;//15
            string davis = "<voice name=\"en-US-DavisNeural\">"; ;//16
            

            System.Diagnostics.Debug.WriteLine(rate);
            System.Diagnostics.Debug.WriteLine(pitch);
            System.Diagnostics.Debug.WriteLine(volume);








            var synthesizer = new Microsoft.CognitiveServices.Speech.SpeechSynthesizer(config);
             
                    string ssml0 = "<speak version=\"1.0\"";
                    ssml0 += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
                if (style != "Normal") { ssml0 += " xmlns:mstts=\"https://www.w3.org/2001/mstts\"";}
                    ssml0 += " xml:lang=\"en-US\">";
          
            switch (voice)
            {
                case "Sara": ssml0 += sara ; break;
                case "Jenny": ssml0 += jenny; ; break;
                case "Guy": ssml0 += guy; ; break;
                case "Amber": ssml0 += amber; ; break;
                case "Ana": ssml0 += ana; ; break;
                case "Aria": ssml0 += aria; ; break;
                case "Ashley": ssml0 += ashley; ; break;
                case "Brandon": ssml0 += brandon; ; break;
                case "Christopher": ssml0 += christopher; ; break;
                case "Cora": ssml0 += cora; ; break;
                case "Elizabeth": ssml0 += elizabeth; ; break;
                case "Eric": ssml0 += eric; ; break;
                case "Jacob": ssml0 += jacob; ; break;
                case "Michelle": ssml0 += michelle; ; break;
                case "Monica": ssml0 += monica; ; break;
                case "Davis": ssml0 += davis; ; break;


                default: ssml0 += "<voice name=\"en-US-SaraNeural\">"; break;
            }

            if (style != "Normal"){
                    if (style == "Angry") { ssml0 += angry; }
                    if (style == "Happy") { ssml0 += happy; }
                    if (style == "Sad") { ssml0 += sad; }
                }
            if (rate != "default") {
                if (rate == "x-slow") { ssml0 += ratexslow; }
                if (rate == "slow") { ssml0 += rateslow; }
                if (rate == "medium") { ssml0 += ratemedium; }
                if (rate == "fast") { ssml0 += ratefast; }
                if (rate == "x-fast") { ssml0 += ratexfast; }

            }
            if (pitch != "default")
            {
                if (pitch == "x-low") { ssml0 += pitchxlow; }
                if (pitch == "low") { ssml0 += pitchlow; }
                if (pitch == "medium") { ssml0 += pitchmedium; }
                if (pitch == "high") { ssml0 += pitchhigh; }
                if (pitch == "x-high") { ssml0 += pitchxhigh; }

            }
            if (volume != "default")
            {
                if (volume == "x-soft") { ssml0 += volumexlow; }
                if (volume == "soft") { ssml0 += volumelow; }
                if (volume == "medium") { ssml0 += volumemedium; }
                if (volume == "loud") { ssml0 += volumehigh; }
                if (volume == "x-loud") { ssml0 += volumexhigh; }

            }


            ssml0 += text;
            if (rate != "default") { ssml0 += "</prosody>"; }
            if (pitch != "default") { ssml0 += "</prosody>"; }
            if (volume != "default") { ssml0 += "</prosody>"; }

            if (style != "Normal") { ssml0 += "</mstts:express-as>"; }
                ssml0 += "</voice>";
                    ssml0 += "</speak>";

            System.Diagnostics.Debug.WriteLine(ssml0);
            await synthesizer.SpeakSsmlAsync(ssml0);
                
        
            }
        public async void outputLog(string textstring)
        {
            AppendTextBox("You said: " + textstring + "\r");
        }
            public async void outputVRChat(string textstring)
            {
            
            var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);


              
                textstring = textstring.ToLower();
            int stringleng = 0;
            foreach (char h in textstring)
            {
                stringleng += 1;
            }
            System.Diagnostics.Debug.WriteLine("textstring length ="+ textstring.Length);
           

            if (stringleng % 4 ==1)
            {
                textstring += "   ";

            }
            if (stringleng % 4 == 2)
            {
                textstring += "  ";

            }
            if (stringleng % 4 == 3)
            {
                textstring += " ";

            }
            System.Diagnostics.Debug.WriteLine("textstring length ="+ textstring.Length);

            float letter = 0.0F;
                int charCounter = 0;
                int stringPoint = 1;


                float letterFloat0 = 0;
                float letterFloat1 = 0;
                float letterFloat2 = 0;
                float letterFloat3 = 0;

                var message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 1);
                var message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                var message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                var message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                var message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);

 
                var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", true);
                sender2.Send(message0);

                foreach (char c in textstring)
                {
                    switch (c)
                    {
                        case 'a': letter = 0.511811f; break;
                        case 'b': letter = 0.51968503f; break;
                        case 'c': letter = 0.52755904f; break;
                        case 'd': letter = 0.53543305f; break;
                        case 'e': letter = 0.54330707f; break;
                        case 'f': letter = 0.5511811f; break;
                        case 'g': letter = 0.5590551f; break;
                        case 'h': letter = 0.56692916f; break;
                        case 'i': letter = 0.5748032f; break;
                        case 'j': letter = 0.5826772f; break;
                        case 'k': letter = 0.5905512f; break;
                        case 'l': letter = 0.5984252f; break;
                        case 'm': letter = 0.6062992f; break;
                        case 'n': letter = 0.61417323f; break;
                        case 'o': letter = 0.62204725f; break;
                        case 'p': letter = 0.62992126f; break;
                        case 'q': letter = 0.63779527f; break;
                        case 'r': letter = 0.6456693f; break;
                        case 's': letter = 0.6535433f; break;
                        case 't': letter = 0.6614173f; break;
                        case 'u': letter = 0.6692913f; break;
                        case 'v': letter = 0.6771653f; break;
                        case 'w': letter = 0.68503934f; break;
                        case 'x': letter = 0.6929134f; break;
                        case 'y': letter = 0.7007874f; break;
                        case 'z': letter = 0.70866144f; break;
                        case '1': letter = 0.13385826f; break;
                        case '2': letter = 0.14173229f; break;
                        case '3': letter = 0.1496063f; break;
                        case '4': letter = 0.15748031f; break;
                        case '5': letter = 0.16535433f; break;
                        case '6': letter = 0.17322835f; break;
                        case '7': letter = 0.18110237f; break;
                        case '8': letter = 0.18897638f; break;
                        case '9': letter = 0.19685039f; break;
                        case '0': letter = 0.12598425f; break;

                        case '$': letter = 0.031496063f; break;
                        case '\'': letter = 0.05511811f; break;
                        case '.': letter = 0.11023622f; break;
                        case ' ': letter = 0.0f; break;
                        case '?': letter = 0.24409449f; break;



                        default: letter = 0.0f; break;
                    }
                    switch (charCounter)
                    {
                        case 0:
                            letterFloat0 = letter;
                            break;
                        case 1:
                            letterFloat1 = letter;
                            break;
                        case 2:
                            letterFloat2 = letter;
                            break;
                        case 3:

                        Task.Delay(debugDelayValue).Wait();
                        letterFloat3 = letter;
                            message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", stringPoint);
                            message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                            message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                            message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                            message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);

                            sender2.Send(message1);
                            sender2.Send(message2);
                            sender2.Send(message3);
                            sender2.Send(message4);
                            sender2.Send(message5);

                        System.Diagnostics.Debug.WriteLine("Sending working 3");
                            stringPoint += 1;
                            charCounter = -1;
                            letterFloat0 = 0;
                            letterFloat1 = 0;
                            letterFloat2 = 0;
                            letterFloat3 = 0;
                            break;
                        default: break;
                    }


                    charCounter += 1;




                }
                int startingPoint = stringPoint;
            int frenzyDisplayLimit = 128;
            int frenzyDisplayMaxChar = 4;
            


            for (int z = startingPoint; z <= (frenzyDisplayLimit/frenzyDisplayMaxChar); z++) //INCREASE VALUE TO Killfrenzy limit!!! (old limit 128/4=33 chracters)
                {
                Task.Delay(debugDelayValue).Wait();
                message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", z);
                    message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", 0.0f);
                    message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", 0.0f);
                    message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", 0.0f);
                    message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", 0.0f);

                    sender2.Send(message1);
                    sender2.Send(message2);
                    sender2.Send(message3);
                    sender2.Send(message4);
                    sender2.Send(message5);


            }




            }

       

        private void button5_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox2.Text.ToString();



            });
            YourSubscriptionKey = text;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string text = "";
            this.Invoke((MethodInvoker)delegate ()
            {
                text = textBox3.Text.ToString();

            });
            YourServiceRegion = text;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, 0);
        }

        
    }
}