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
//using CSCore.SoundIn;
using OSCVRCWiz.Settings;
using OSCVRCWiz.TTS;
using TTS;
using OSCVRCWiz.Text;
using Resources;

namespace OSCVRCWiz
{
    public class VoskRecognition
    {


        static Model model;
        static VoskRecognizer rec;
        static WaveInEvent waveIn;
        static Dictionary<string, int> AlternateInputDevices = new Dictionary<string, int>();
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
                MessageBox.Show("No vosk model folder selected. Please note that if the folder you select is not a valid model the program will close!");
            }
        }
        public static void doVosk()
        {
            try
            {
                OutputText.outputLog("[Starting Up Vosk...]");
                model = new Model(VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString());
                rec = new VoskRecognizer(model, 48000f);


                // Setting to Correct Input Device
                int waveInDevices = WaveIn.DeviceCount;
                AlternateInputDevices.Clear();
                for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
                {
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                    AlternateInputDevices.Add(deviceInfo.ProductName, waveInDevice);
                }
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0;
                foreach (var kvp in AlternateInputDevices)
                {
                    if (AudioDevices.currentInputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        waveIn.DeviceNumber = kvp.Value;
                        System.Diagnostics.Debug.WriteLine("Input device worked" + kvp.Key);
                    }                 
                }


                //Start Listening
                    waveIn.WaveFormat = new WaveFormat(48000, 1);
                    waveIn.DataAvailable += WaveInOnDataAvailable;
                    waveIn?.StartRecording();
                OutputText.outputLog("[Vosk Listening]");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                voskEnabled = false;
                OutputText.outputLog("[Vosk Failed to Start (only avaliable on x64 build)]");
                

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
