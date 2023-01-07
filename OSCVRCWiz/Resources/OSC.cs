using Json.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OSCVRCWiz.Resources
{
    public class OSC
    {
        public static CoreOSC.UDPSender OSCSender;
        public static string OSCAddress;
        public static string OSCPort;

        public static void Start()
        {
            OSCSender = new CoreOSC.UDPSender("127.0.0.1", 9000);//9000

        }
        public static void ChangeAddressAndPort(string address, string port)
        {
            try
            {
                OSCAddress = address;
                OSCPort = port;
                OSCSender = new CoreOSC.UDPSender(OSCAddress, Convert.ToInt32(OSCPort));//9000
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

    }
}
