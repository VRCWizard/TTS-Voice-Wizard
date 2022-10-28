using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;//free Windows
using System.Speech;//free windows

using System.Speech.Synthesis;//free windows
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;

namespace OSCVRCWiz
{
    public class WindowsBuiltInSTTTS
    {
        //public static SpeechSynthesizer synthesizer;
        static bool listeningCurrently = false;
        SpeechRecognitionEngine recognizer;
        private static bool _userRequestedAbort = false;



            public void TTSButtonLiteClick(VoiceWizardWindow MainForm)//TTS
            {
                // synthesizer = new SpeechSynthesizer();
                //synthesizer.SelectVoice("Microsoft Zira Desktop");
                // synthesizer.SetOutputToDefaultAudioDevice();


                string text = "";

               text = VoiceWizardWindow.TTSLiteText;

             //   var ot = new OutputText();
                //Send Text to Vrchat
                if (MainForm.rjToggleButtonLog.Checked == true)
                {
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, text);
                }
                if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                {

                Task.Run(() => systemTTSAction(text));


            }
               

                if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                {

                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, text,"tts")); //original
                                                                 // ot.outputVRChat(this, text);//new
                }
                if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                {

                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, text, "tts")); //original
                                                                            // ot.outputVRChat(this, text);//new
                }



        }
        public void systemTTSAction(string text)
        {
            VoiceWizardWindow.MainFormGlobal.stream = new MemoryStream();
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.SetOutputToWaveStream(VoiceWizardWindow.MainFormGlobal.stream);
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.Speak(text);
            var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
            var waveSource = new MediaFoundationDecoder(VoiceWizardWindow.MainFormGlobal.stream);
            waveOut.Initialize(waveSource);
            waveOut.Play();
            waveOut.WaitForStopped();
            //MainForm.stream.SetLength(0);
            // sw.Flush(); //Site tip
            //Site tip
            // MainForm.stream.Flush();
            // MainForm.stream.Dispose();

        }
        public void startListeningNow(VoiceWizardWindow MainForm)
            {
                string cultureHere = "en-US";

              //  cultureHere = MainForm.CultureSelected;
            

                try
                {
                    using (recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(cultureHere)))
                    {
                        // Create and load a dictation grammar.  
                        recognizer.LoadGrammar(new DictationGrammar());

                        // Add a handler for the speech recognized event.  
                        recognizer.SpeechRecognized +=
                          new EventHandler<SpeechRecognizedEventArgs>(MainForm.recognizer_SpeechRecognized);

                        // Configure input to the speech recognizer.  
                        recognizer.SetInputToDefaultAudioDevice();

                        bool completed = false;

                        // Attach event handlers.
                        recognizer.RecognizeCompleted += (o, e) =>
                        {
                            if (e.Error != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Error occurred during recognition: {0}", e.Error);
                            }
                            else if (e.InitialSilenceTimeout)
                            {
                                System.Diagnostics.Debug.WriteLine("Detected silence");
                            }
                            else if (e.BabbleTimeout)
                            {
                                System.Diagnostics.Debug.WriteLine("Detected babbling");
                            }
                            else if (e.InputStreamEnded)
                            {
                                System.Diagnostics.Debug.WriteLine("Input stream ended early");
                            }
                            else if (e.Result != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Grammar = {0}; Text = {1}; Confidence = {2}", e.Result.Grammar.Name, e.Result.Text, e.Result.Confidence);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("No result");
                            }

                            completed = true;
                        };
                        // Start asynchronous, continuous speech recognition.  
                        recognizer.RecognizeAsync(RecognizeMode.Multiple);


                        while (!completed)
                        {
                            if (_userRequestedAbort)
                            {
                                recognizer.RecognizeAsyncCancel();
                                break;
                            }

                            Thread.Sleep(333);
                        }

                        Console.WriteLine("Done.");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System Speech STT error:" + ex.Message.ToString()+" | (Most likely you are trying to use a language that is not installed on your PC, therefore you must add that language for more info check the FAQ in the discord)");

                }

            }

            public void speechTTSButtonLiteClick(VoiceWizardWindow MainForm)
            {

                if (listeningCurrently == false)
                {
                    _userRequestedAbort = false;
                    Task.Run(() => startListeningNow(MainForm));
                //  waveIn.StartRecording();
                // var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[System Speech Started Listening]");
                    listeningCurrently = true;
                }
                else
                {
                    _userRequestedAbort = true;
                // recognizer.RecognizeAsyncStop();
                ///  waveIn.StopRecording();
                //    var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[System Speech Stop Listening]");
                    listeningCurrently = false;
                }




            }

        

    }
}
