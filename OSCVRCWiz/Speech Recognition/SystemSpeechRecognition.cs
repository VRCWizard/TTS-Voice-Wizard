using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;//free Windows


//using NAudio.Wave;

namespace OSCVRCWiz
{
    public class SystemSpeechRecognition
    {
        //public static SpeechSynthesizer synthesizer;
        static bool listeningCurrently = false;
        static SpeechRecognitionEngine recognizer;
        private static bool _userRequestedAbort = false;
        



      
        public static void startListeningNow()
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
                          new EventHandler<SpeechRecognizedEventArgs>(VoiceWizardWindow.MainFormGlobal.recognizer_SpeechRecognized);

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

            public static void speechTTSButtonLiteClick()
            {

                if (listeningCurrently == false)
                {
                    _userRequestedAbort = false;
                    Task.Run(() => startListeningNow());
                //  waveIn.StartRecording();
                // var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[System Speech Started Listening]");
                    listeningCurrently = true;
                }
                else
                {
                    _userRequestedAbort = true;
                // recognizer.RecognizeAsyncStop();
                ///  waveIn.StopRecording();
                //    var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[System Speech Stopped Listening]");
                    listeningCurrently = false;
                }




            }
       // public void OnDataAvailable(object? sender, WaveInEventArgs e)=> _stream.Write(e.Buffer, 0, e.BytesRecorded);




    }
}
