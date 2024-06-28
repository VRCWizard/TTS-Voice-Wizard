using CoreOSC;
using Json.Net;
using Newtonsoft.Json;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
//using VRC.OSCQuery;
using static System.Net.Mime.MediaTypeNames;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class OSC
    {
        public static UDPSender OSCSender;
      //  public static UDPSender OSCReSender;
        //public static string OSCAddress = "127.0.0.1";
        //public static string OSCPort = "9000";





        public static void InitializeOSC()
        {
            try
            {
                OSCSender = new UDPSender(Settings1.Default.OSCAddress, Convert.ToInt32(Settings1.Default.OSCPort));//9000
             //   OSCReSender = new UDPSender(Settings1.Default.OSCAddress, Convert.ToInt32(OSCListener.OSCReceiveport));
                VoiceWizardWindow.MainFormGlobal.textBoxOSCAddress.Text = Settings1.Default.OSCAddress;
                VoiceWizardWindow.MainFormGlobal.textBoxOSCPort.Text = Settings1.Default.OSCPort;
            }
            catch (Exception ex) { MessageBox.Show("OSC Startup Error: " + ex.Message); }

        }
        public static void ChangeAddressAndPort(string address, string port)
        {
            try
            {
                Settings1.Default.OSCAddress = address;
                Settings1.Default.OSCPort = port;
                Settings1.Default.Save();
                OSCSender = new UDPSender(address, Convert.ToInt32(port));//9000
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        




    }
}
