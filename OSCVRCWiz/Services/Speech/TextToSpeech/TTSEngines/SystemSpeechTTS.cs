using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class SystemSpeechTTS
    {

        public static List<string> systemSpeechVoiceList = new List<string>();
        //public static string currentLiteVoice = "";

        public static void InitializeSystemSpeech()
        {
            try
            {
                getVoices();
                SystemSpeechRecognition.getInstalledRecogs();
            }
            catch (Exception ex) { MessageBox.Show("System Speech Startup Error: " + ex.Message); }
        }


        public static void getVoices()
        {
            System.Speech.Synthesis.SpeechSynthesizer synthesizerVoices = new System.Speech.Synthesis.SpeechSynthesizer();

            // synthesizerLite.c += new EventHandler<SpeakCompletedEventArgs>(reader_SpeakCompleted);
            foreach (var voice in synthesizerVoices.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                // System.Diagnostics.Debug.WriteLine($"Id: {info.Id} | Name: {info.Name} | Age: {info.Age} | Gender: {info.Gender} | Culture: {info.Culture}");
                systemSpeechVoiceList.Add(info.Name + "|" + info.Culture);
                // comboBoxLite.Items.Add(info.Name + "|" + info.Culture);
            }

        }

        public static async void systemTTSAction(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            //  var semitone = Math.Pow(2, 1.0/24);
            //   var upOneTone = semitone;
            // var downOneTone = 1.0 / upOneTone;


            try
            {
                string phrase = TTSMessageQueued.Voice;
                string[] words = phrase.Split('|');
                int counter = 1;
                var voice = "none";

                foreach (var word in words)
                {
                    if (counter == 1)
                    {
                        //synthesizerLite.SelectVoice(word);
                        voice = word;
                        // System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");

                    }
                    if (counter == 2)
                    {
                        //CultureSelected = word;
                        //  System.Diagnostics.Debug.WriteLine(counter + ": " + word + "///////////////////////////////////////////");
                    }
                    counter++;
                }


                System.Speech.Synthesis.SpeechSynthesizer synthesizerLite = new System.Speech.Synthesis.SpeechSynthesizer();
                synthesizerLite.SelectVoice(voice);

                MemoryStream memoryStream = new MemoryStream();
                synthesizerLite.SetOutputToWaveStream(memoryStream);
                synthesizerLite.Speak(TTSMessageQueued.text);

                //  AudioDevices.playWaveStream(memoryStream, TTSMessageQueued, ct);
                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, true, AudioFormat.Wav);
                memoryStream.Dispose();

                synthesizerLite.Dispose();
                synthesizerLite = null;
            }
            catch (Exception ex)
            {
                OutputText.outputLog("System Speech TTS Error: " + ex.Message + "]", Color.Red);
                Task.Run(() => TTSMessageQueue.PlayNextInQueue());


            }






        }

        public static void SetVoices(ComboBox voices, ComboBox styles)
        {
            voices.Items.Clear();
            foreach (string voice in SystemSpeechTTS.systemSpeechVoiceList)
            {
                voices.Items.Add(voice);
            }
            voices.SelectedIndex = 0;
            styles.Items.Clear();
            styles.Items.Add("default");
            styles.SelectedIndex = 0;
            styles.Enabled = false;
            voices.Enabled = true;
        }


        }
}
