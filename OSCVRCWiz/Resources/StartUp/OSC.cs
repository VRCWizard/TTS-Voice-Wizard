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
        public static UDPSender OSCReSender;
        //public static string OSCAddress = "127.0.0.1";
        //public static string OSCPort = "9000";





        public static void InitializeOSC()
        {
            try
            {
                OSCSender = new UDPSender(Settings1.Default.OSCAddress, Convert.ToInt32(Settings1.Default.OSCPort));//9000
                OSCReSender = new UDPSender(Settings1.Default.OSCAddress, Convert.ToInt32(OSCListener.OSCReceiveport));
                VoiceWizardWindow.MainFormGlobal.textBoxOSCAddress.Text = Settings1.Default.rememberAddress;
                VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text = Settings1.Default.OSCPort;
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

        //VRChat OSCQuery Test Code
        /*   public static void OSCQueryAdvertMyApp() //no in use yet
           {

               var OSCQueryService = new OSCQueryServiceBuilder()
                                 .WithServiceName("TTS Voice Wizard v" + VoiceWizardWindow.currentVersion)
                                .WithTcpPort(Extensions.GetAvailableTcpPort())
                                 .WithUdpPort(Convert.ToInt32(OSCPort))
                                 .Build();

               // Add endpoints for all data tts voice wizard sends out
              // OSCQueryService.AddEndpoint("/path", "T means bool, i means int, f means float, s means string", Attributes.AccessValues.WriteOnly, null, "description for people to look at");






           }*/

        /* private static async Task OSCQueryVRchatListener()//no in use yet
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

         }*/




    }
}
