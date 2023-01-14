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

namespace OSCVRCWiz
{
    public class VoskRecognition
    {


        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;
      //  static Dictionary<string, int> AlternateInputDevices = new Dictionary<string, int>();
        static bool voskEnabled = false;


        public static void toggleVosk()
        {
            if (VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString() != "no folder selected")
            {
                if (voskEnabled == false)
                {
                    Task.Run(() => VoskRecognition.doVosk());
                    voskEnabled = true;
                }
                else
                {
                    Task.Run(() => VoskRecognition.stopVosk());
                    voskEnabled = false;

                }
            }
            else
            {
                OutputText.outputLog("[No vosk model folder selected. When selecting you model foler make sure that the folder you select DIRECTLY contains the model files or the program will close!]", Color.Red);
                MessageBox.Show("No vosk model folder selected. When selecting you model foler make sure that the folder you select DIRECTLY contains the model files or the program will close!");
                
            }
        }
        public static void AutoStopVoskRecog()
        {
            if (voskEnabled == true)
            {
                Task.Run(() => VoskRecognition.stopVosk());
                voskEnabled = false;

            }

        }
            public static void doVosk()
        {
            try
            {
                OutputText.outputLog("[Starting Up Vosk...]");
                model = new Model(VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString());
                rec = new VoskRecognizer(model, 48000f);


              
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = AudioDevices.getCurrentInputDevice();


                //Start Listening
                    waveIn.WaveFormat = new WaveFormat(48000, 1);
                    waveIn.DataAvailable += WaveInOnDataAvailable;
                    waveIn?.StartRecording();
                OutputText.outputLog("[Vosk Listening]");


            }
            catch (Exception ex)
            {
                voskEnabled = false;
                OutputText.outputLog("[Vosk Failed to Start]", Color.Red);
                OutputText.outputLog("[Reminder that Vosk only works on the x64 build of TTS Voice Wizard]", Color.Red);
                MessageBox.Show("Vosk Error: " + ex.Message);
                


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
                OutputText.outputLog("[Vosk Stopped Listening]");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }
    }
}
