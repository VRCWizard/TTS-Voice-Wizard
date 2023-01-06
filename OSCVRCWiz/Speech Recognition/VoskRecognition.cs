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
using OSCVRCWiz.Settings;
using OSCVRCWiz.TTS;
using TTS;

namespace OSCVRCWiz
{
    public class VoskRecognition
    {


        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;
        public static void doVosk()
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[Starting Up Vosk...]");
                model = new Model(VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString());
                    rec = new VoskRecognizer(model, 48000f);

                    //  WaveInEvent waveIn = new WaveInEvent(44100, 1);
                    waveIn = new WaveInEvent();
                    waveIn.WaveFormat = new WaveFormat(48000, 1);
                    waveIn.DataAvailable += WaveInOnDataAvailable;
                    waveIn?.StartRecording();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[Vosk Listening]");


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

                    if (rec.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {

                        // System.Diagnostics.Debug.WriteLine(rec.Result());
                        string json = rec.Result();
                        var text = JObject.Parse(json)["text"].ToString();
                        System.Diagnostics.Debug.WriteLine("Vosk: " + text);
                    if (text != "")//only does stuff if the string is nothing silence
                    {

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text));

                    }


                    }
                    else
                    {
                        //  VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, rec.PartialResult());
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
                waveIn.StopRecording();
                rec.Dispose();
                model.Dispose();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[Vosk Stopped Listening]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
    }
}
