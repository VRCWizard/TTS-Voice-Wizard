using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;
using PortAudioSharp;
using Vosk;
using Swan.Formatters;
using Newtonsoft.Json.Linq;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Windows.Threading;
using CSCore.SoundIn;
using DeepL_Translation;
using OSCVRCWiz.Settings;

namespace OSCVRCWiz
{
    public class Vosk
    {


        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;
        public static void doVosk()
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Starting Up Vosk...]");
                model = new Model(VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString());
                    rec = new VoskRecognizer(model, 48000f);

                    //  WaveInEvent waveIn = new WaveInEvent(44100, 1);
                    waveIn = new WaveInEvent();
                    waveIn.WaveFormat = new WaveFormat(48000, 1);
                    waveIn.DataAvailable += WaveInOnDataAvailable;
                    waveIn?.StartRecording();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk Listening]");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }
        private static async void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {

               // try
              //  {
                    if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {

                        // System.Diagnostics.Debug.WriteLine(rec.Result());
                        string json = rec.Result();
                        var s = JObject.Parse(json)["text"].ToString();
                        System.Diagnostics.Debug.WriteLine("Vosk: " + s);
                        if (s != "")//only does stuff if the string is nothing silence
                        {



                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(s));

                        var translation = "";
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            translation = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedItem.ToString();
                        });





                        if (translation != "No Translation (Default)")
                            {
                                var DL = new DeepLC();
                                s = await DL.translateTextDeepL(s);
                                // text = VoiceWizardWindow.MainFormGlobal.deepLString;
                            }

                       


                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                        {
                            VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk]: " + s);
                            VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(VoiceWizardWindow.MainFormGlobal, s);
                        }

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, s, "tts"));


                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {//
                         // VoiceWizardWindow.pauseBPM = true;                                          
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, s, "tts"));
                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(VoiceWizardWindow.MainFormGlobal, s, "tts")); //original

                        }
                        //Send Text to TTS
                        //  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked == true)//azure new is incorrect now means any tts
                        //  {
                        string ttsModeNow = VoiceWizardWindow.TTSModeSaved;
                        


                            switch (ttsModeNow)
                            {
                                case "FonixTalk":
                                    var fx = new FonixTalkTTS();
                                    Task.Run(() => fx.FonixTTS(s));
                                    break;

                                case "TikTok":

                                    Task.Run(() => TikTok.TikTokTextAsSpeech(s));
                                    break;


                                case "System Speech":
                                    var sys = new WindowsBuiltInSTTTS();
                                    Task.Run(() => sys.systemTTSAction(s));

                                    break;
                                case "Azure":
                                    SetDefaultTTS.SetVoicePresets();
                                    Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, s, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                    break;
                                default:

                                    break;
                            }
                        }


                    }
                    else
                    {
                        //  VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, rec.PartialResult());
                    }

              //  }
             //   catch (Exception exception)
             //   {
             //   }

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
                waveIn.StopRecording();
                rec.Dispose();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk Stopped Listening]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
    }
}
