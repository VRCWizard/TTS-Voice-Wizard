
using NAudio.CoreAudioApi;
using NAudio.Wave;
using OSCVRCWiz;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Text;
using VarispeedDemo.SoundTouch;
//using NAudio.CoreAudioApi;




namespace Resources
{
    public class AudioDevices
    {
        public static int audioOutputIndex = -1;
        public static List<string> comboIn = new List<string>();
        public static List<string> comboOut = new List<string>();
        public static List<string> micIDs = new List<string>();
        public static List<string> speakerIDs = new List<string>();
        public static string currentInputDevice = "";
        public static string currentOutputDevice = "";
        public static string currentInputDeviceName = "Default";
        public static string currentOutputDeviceName = "Default";
        // public static int currentOutputDeviceLite = 0;

        public static string currentOutputDevice2nd = "";
        public static string currentOutputDeviceName2nd = "Default";




        public static void NAudioSetupInputDevices()
        {
            comboIn.Add("Default");
            micIDs.Add("Default");
           
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.DeviceState.Active))
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);
                comboIn.Add(endpoint.FriendlyName);
                micIDs.Add(endpoint.ID);

            }
          

            foreach (var i in comboIn)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxInput.Items.Add(i);
            }
           

        }
        public static void NAudioSetupOutputDevices()
        {
            comboOut.Add("Default");
            speakerIDs.Add("Default");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);

                comboOut.Add(endpoint.FriendlyName);
                speakerIDs.Add(endpoint.ID);
            }
            foreach (var i in comboOut)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.Items.Add(i);
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.Items.Add(i);
            }

           

        }
        public static int getCurrentInputDevice() {

            // Setting to Correct Input Device
            int waveInDevices = WaveIn.DeviceCount;
            //InputDevicesDict.Clear();
           Dictionary<string, int> DevicesDict = new Dictionary<string, int>();
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                DevicesDict.Add(deviceInfo.ProductName, waveInDevice);
            }
           
            foreach (var kvp in DevicesDict)
            {
                if (AudioDevices.currentInputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                   
                }
            }
            return 0;

        }

       





        public static int getCurrentOutputDevice()
        {

            // Setting to Correct Input Device
            int waveDevices = WaveOut.DeviceCount;
            //InputDevicesDict.Clear();
            Dictionary<string, int> DevicesDict = new Dictionary<string, int>();
            for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveDevice);
                DevicesDict.Add(deviceInfo.ProductName, waveDevice);
            }

            foreach (var kvp in DevicesDict)
            {
                if (AudioDevices.currentOutputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;

                }
            }
            return 0;

        }
        public static int getCurrentOutputDevice2()
        {

            // Setting to Correct Input Device
            int waveDevices = WaveOut.DeviceCount;
            //InputDevicesDict.Clear();
            Dictionary<string, int> DevicesDict = new Dictionary<string, int>();
            for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveDevice);
                DevicesDict.Add(deviceInfo.ProductName, waveDevice);
            }

            foreach (var kvp in DevicesDict)
            {
                if (AudioDevices.currentOutputDeviceName2nd.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;

                }
            }
            return 0;

        }
     

    }
}
