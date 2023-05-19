using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;
//using PortAudioSharp;
using Vosk;
using Swan.Formatters;
using Newtonsoft.Json.Linq;
//using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Windows.Threading;
//using CSCore.SoundIn;
using OSCVRCWiz.Settings;
using OSCVRCWiz.TTS;
using TTS;
using OSCVRCWiz.Text;
using Resources;
using NAudio.Wave;
using System.Windows.Shapes;
using System.Diagnostics;
using OSCVRCWiz.Resources;
using CoreOSC;

namespace OSCVRCWiz
{
    public class VoskRecognition
    {


        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;
      //  static Dictionary<string, int> AlternateInputDevices = new Dictionary<string, int>();
        static bool voskEnabled = false;

        static bool voskPause = false;

       // static bool voskMute = false;



        public static void toggleVosk()
        {
            if (VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString() != "no folder selected")
            {
                if (voskEnabled == false)
                {
                   

                    if(voskPause==false)
                    {
                        Task.Run(() => VoskRecognition.doVosk());
                        voskPause= true;
                    }
                    else
                    {
                        Task.Run(() => VoskRecognition.unpauseVosk());
                    }
                       
                        voskEnabled = true;
                       // voskPaused= true;
                 
                }
                else
                {
                    Task.Run(() => VoskRecognition.pauseVosk());


                    voskEnabled = false;

                }
            }
            else
            {
                OutputText.outputLog("[No vosk model folder selected. When selecting you model foler make sure that the folder you select DIRECTLY contains the model files or the program will crash!]", Color.Red);
                MessageBox.Show("No vosk model folder selected. When selecting your model folder make sure that the folder you select DIRECTLY contains the model files or the program will crash!");
                
            }
        }
        public static void AutoStopVoskRecog()
        {
            if (voskEnabled == true || voskPause==true)
            {
                VoskRecognition.stopVosk();
                voskEnabled = false;
                voskPause= false;

            }

        }
            public static void doVosk()
        {

            var path = VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString();
            try
            {
                if (!Directory.Exists(path + "\\graph"))//simiple check before crashing to let user know why.
                {
                    // MessageBox.Show("It seems that the folder you have selected may not be a valid vosk model. The most common mistake is picking the outer folder when you should select the folder that contains the 'readme' and 'graph' folder etc.");
                    if (MessageBox.Show("Are you sure this is a valid vosk model? If the selected folder is not valid TTS Voice Wizard will crash. (The most common mistake is picking the outer folder when you should select the folder that contains the 'readme' and 'graph' folder etc.) ", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // user clicked yes
                        Task.Run(() => runVoskNow(path));
                    }
                    else
                    {
                        // user clicked no
                        OutputText.outputLog("[Vosk Failed to Start]", Color.Red);
                        voskEnabled = false;
                    }
                }
                else
                {
                    Task.Run(() => runVoskNow(path));
                }


            }
            catch (Exception ex)
            {
                voskEnabled = false;
                OutputText.outputLog("[Vosk Failed to Start]", Color.Red);
              //  OutputText.outputLog("[Reminder that Vosk only works on the x64 build of TTS Voice Wizard]", Color.Red);
                MessageBox.Show("Vosk Error: " + ex.Message);
                


            }
        }
        private static void runVoskNow(string path)
        {
            OutputText.outputLog("[Starting Up Vosk...(don't click anything)]");
            model = new Model(path);
            rec = new VoskRecognizer(model, 48000f);



            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();


            //Start Listening
            waveIn.WaveFormat = new WaveFormat(48000, 1);
            waveIn.DataAvailable += WaveInOnDataAvailable;
            waveIn?.StartRecording();
            OutputText.outputLog("[Vosk Listening]");

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                OSC.OSCSender.Send(sttListening);
            }
        }
        private static async void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                if (rec != null)
                {
                    if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {

                        // System.Diagnostics.Debug.WriteLine(rec.Result());
                        string json = rec.Result();
                        var text = JObject.Parse(json)["text"].ToString();
                        System.Diagnostics.Debug.WriteLine("Vosk: " + text);
                        if (text != "")//only does stuff if the string is nothing silence
                        {
                           // Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Vosk"));

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
                                TTSMessageQueued.STTMode = "Vosk";
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
                    else
                    {
                        //  VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, rec.PartialResult());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void stopVosk()
        {
            try
            {
                if (voskEnabled == true)
                {
                    pauseVosk();
                    Debug.WriteLine("wavein stopped");
                    waveIn = null;
                    Debug.WriteLine("wavein nulled");
                }

                // model = null;
             //  rec = null;
            //    Debug.WriteLine("rec nulled");
               // model = null;
                // Debug.WriteLine("model nulled");

               
                model?.Dispose();
                Debug.WriteLine("model disposed");
                
                rec?.Dispose();
                Debug.WriteLine("rec disposed");

                //  model = null;
                // Debug.WriteLine("model nulled");
                OutputText.outputLog("[Vosk Stopped Listening (resources freed)]");

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
                // OutputText.outputLog("[Vosk Resources may not have properly been disposed (memory use still high in Task Manager). Consider restarting TTS Voice Wizard.]",Color.Red);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }



        }
        public static void pauseVosk()
        {
            try
            {
                waveIn?.StopRecording();
                
                // model = null;
                //  rec = null;
                //   rec?.Dispose();
                //   model?.Dispose();
              //  voskPaused = true;
                OutputText.outputLog("[Vosk Muted, to free resources switch speech to text mode]");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }



        }
        public static void unpauseVosk()
        {
            try
            {

                waveIn?.StartRecording();
                // model = null;
                //  rec = null;
                //   rec?.Dispose();
                //   model?.Dispose();
               // voskPaused = false;
                OutputText.outputLog("[Vosk Unmuted]");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
    }
}
