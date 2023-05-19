using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;//free Windows
//using NAudio.Wave;
//using CSCore.SoundIn;
//using CSCore.XAudio2;
using Resources;
using System.Collections;
using System.Linq;
using OSCVRCWiz.Addons;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using OSCVRCWiz.Text;
using NAudio.Wave;
using System.Diagnostics;
using OSCVRCWiz.Resources;
using CoreOSC;


//using NAudio.Wave;

namespace OSCVRCWiz
{
    public class SystemSpeechRecognition
    {
        static bool listeningCurrently = false;
        static WaveInEvent waveIn;
        static SpeechStreamer audioStream = new(12800);
        static SpeechRecognitionEngine rec;
      //  static Dictionary<string, int> AlternateInputDevices = new Dictionary<string, int>();

       public static void getInstalledRecogs()
        {
            try
            {
                string info;
            foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxSysSpeechRecog.Items.Add(ri.Description.ToString());
                VoiceWizardWindow.MainFormGlobal.comboBoxSysSpeechRecog.SelectedIndex = 0;
            }
            }
            catch (Exception ex)
            {

                OutputText.outputLog("[System Speech Get Installed Recognizers Error: " + ex.Message + "]", Color.Red);
            }

        }

        public static void startListeningNow()
        {

            try
            {


                var installRec = SpeechRecognitionEngine.InstalledRecognizers()[0];


                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    foreach (RecognizerInfo ri in SpeechRecognitionEngine.InstalledRecognizers())
                    {
                        if (ri.Description.ToString() == VoiceWizardWindow.MainFormGlobal.comboBoxSysSpeechRecog.SelectedItem.ToString())
                        {
                            installRec = ri;
                            break;
                        }
                    }
                });

                // Create the selected recognizer.
                rec = new SpeechRecognitionEngine(installRec);
         

           
                 
            // Create and load a dictation grammar.  
            rec.LoadGrammar(new DictationGrammar());
             // Add a handler for the speech recognized event.  
            rec.SpeechRecognized +=new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);


            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();

            // Start Listening
            waveIn.WaveFormat = new WaveFormat(48000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;

            OutputText.outputLog("[System Speech Started Listening]");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }
                waveIn?.StartRecording();
            rec.SetInputToAudioStream(audioStream, new(48000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono));
            rec.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch (Exception ex)
            {

                OutputText.outputLog("[System Speech Recognizer Error: " + ex.Message + "]", Color.Red);
                listeningCurrently = false;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }

        }
        public static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)//lite version, WindowsBuiltInSTTTS Help
        {
            System.Diagnostics.Debug.WriteLine("Recognized text: " + e.Result.Text);
            string text = e.Result.Text.ToString();
           // Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "System Speech"));

            TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                TTSMessageQueued.text = text;
                TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text.ToString();
                TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString();
                TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.Text.ToString();
                TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                TTSMessageQueued.STTMode = "System Speech";
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

        private static void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
           // Debug.WriteLine("audio found");
            audioStream.Write(e.Buffer, 0, e.BytesRecorded);

        }

        public static void speechTTSButtonLiteClick()
        {

                if (listeningCurrently == false)
                {
                listeningCurrently = true;
                Task.Run(() => startListeningNow());
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }

            }
                else
                {
                OutputText.outputLog("[System Speech Stopped Listening]");
                listeningCurrently = false;
                waveIn.StopRecording();
                rec.RecognizeAsyncStop();
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }




        }
        public static void AutoStopSystemSpeechRecog()
        {
            if (listeningCurrently == true)
            {
                OutputText.outputLog("[System Speech Stopped Listening]");
                listeningCurrently = false;
                waveIn.StopRecording();
                rec.RecognizeAsyncStop();
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }
        }
    }
}
