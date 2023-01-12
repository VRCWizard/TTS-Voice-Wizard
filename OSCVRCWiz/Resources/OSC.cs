using CoreOSC;
using Json.Net;
using Newtonsoft.Json;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VRC.OSCQuery;

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

        //VRChat OSCQuery Test Code
        public static void OSCQueryAdvertMyApp() //no in use yet
        {

            var OSCQueryService = new OSCQueryServiceBuilder()
                              .WithServiceName("TTS Voice Wizard v" + VoiceWizardWindow.currentVersion)
                             .WithTcpPort(Extensions.GetAvailableTcpPort())
                              .WithUdpPort(Convert.ToInt32(OSCPort))
                              .Build();

            // Add endpoints for all data tts voice wizard sends out
           // OSCQueryService.AddEndpoint("/path", "T means bool, i means int, f means float, s means string", Attributes.AccessValues.WriteOnly, null, "description for people to look at");






        }
        private static void OSCLegacyVRChatListener()//no in use remove apon release of oscquery
        {
            int port = 9001;//VRChats default UDP // ONLY ONE APP CAN LISTEN HERE
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {
                    //OSC recieved
                    // OutputText.outputLog(messageReceived.ToString());
                    System.Diagnostics.Debug.WriteLine("address: " + messageReceived.Address.ToString() + "argument: " + messageReceived.Arguments[0].ToString());

                }
            };



            var listener = new UDPListener(port, callback);

        }
        private static async Task OSCQueryVRchatListener()//no in use yet
        {
            var _tcpPort = 9001;//VRChats default TCP // MANY APPS CAN LISTEN HERE
            var response = await new HttpClient().GetAsync($"http://localhost:{_tcpPort}/avatar/parameters");
            //  var response = await new HttpClient().GetAsync($"http://localhost:{_tcpPort}/input");
            //  var response = await new HttpClient().GetAsync($"http://localhost:{_tcpPort}/tracking");
            //   var response = await new HttpClient().GetAsync($"http://localhost:{_tcpPort}/chatbox");

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OSCQueryRootNode>(responseString);

                var sb = new StringBuilder();
                foreach (var pair in result.Contents)
                {

                    sb.AppendLine($"{pair.Key}: {pair.Value}");

                }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                // OutputText.outputLog(sb.ToString());
                // OutputText.outputLog("hi");
            }

            await Task.Delay(500); // poll every half second

            OSCQueryVRchatListener();

        }


       

    }
}
