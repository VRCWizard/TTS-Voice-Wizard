using NAudio.CoreAudioApi;
using OSCVRCWiz;
using System;
using System.Collections.Generic;
using System.Text;

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


        public static void setupInputDevices()
        {
            comboIn.Add("Default");
            micIDs.Add("Default");
           
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
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
        public static void setupOutputDevices()
        {
            comboOut.Add("Default");
            speakerIDs.Add("Default");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);

                comboOut.Add(endpoint.FriendlyName);
                speakerIDs.Add(endpoint.ID);
            }
            foreach (var i in comboOut)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.Items.Add(i);
            }

        }
    }
}
