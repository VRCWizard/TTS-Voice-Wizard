using CoreOSC;
using Json.Net;
using Newtonsoft.Json;
using OSCVRCWiz.Addons;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
//using VRC.OSCQuery;
using static System.Net.Mime.MediaTypeNames;

namespace OSCVRCWiz.Resources
{
    public class OSC
    {
        public static CoreOSC.UDPSender OSCSender;
        public static CoreOSC.UDPSender OSCReSender;
        public static string OSCAddress= "127.0.0.1";
        public static string OSCPort = "9000";

        public static string FromVRChatPort = "9001";

        public static bool AFKDetector = false;
        public static DateTime afkStartTime = DateTime.Now;
        public static int counter1 = 0;
        public static int counter2 = 0;
        public static int counter3 = 0;
        public static int counter4 = 0;
        public static int counter5 = 0;
        public static int counter6= 0;

        public static int prevCounter1 = 0;
        public static int prevCounter2 = 0;
        public static int prevCounter3 = 0;
        public static int prevCounter4 = 0;
        public static int prevCounter5 = 0;
        public static int prevCounter6 = 0;

        public static void Start()
        {
            OSCSender = new CoreOSC.UDPSender(OSCAddress, Convert.ToInt32(OSCPort));//9000
            OSCReSender = new CoreOSC.UDPSender(OSCAddress, Convert.ToInt32(OSCListener.OSCReceiveport));

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
       public static void OSCLegacyVRChatListener()//no in use remove apon release of oscquery
        {
            //  int port = 9001;//VRChats default UDP // ONLY ONE APP CAN LISTEN HERE

           

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.button33.Enabled = false;
                VoiceWizardWindow.MainFormGlobal.button33.ForeColor = Color.Green;
                OutputText.outputLog("[VRChat OSC Listener Activated]", Color.Green);
                OutputText.outputLog($"[VRChat OSC Listener: Remember that only one program can listen on a UDP port. TTS Voice Wizard is listening on port {FromVRChatPort}.", Color.Orange);
            });

            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {
                    try
                    {
                        //OSC recieved
                        // OutputText.outputLog(messageReceived.ToString());
                        //  System.Diagnostics.Debug.WriteLine("address: " + messageReceived.Address.ToString() + "argument: " + messageReceived.Arguments[0].ToString());
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true)
                        {
                            if (messageReceived.Address.ToString() == "/avatar/parameters/AFK")
                            {

                              //  var theString = "";
                              //  theString = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();


                                if (messageReceived.Arguments[0].ToString() == "True")
                                {

                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                                    {
                                        Task.Run(() => OutputText.outputLog("Now AFK"));
                                    }
                                    AFKDetector = true;
                                    afkStartTime = DateTime.Now;


                                }
                                else
                                {
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                                    {
                                        Task.Run(() => OutputText.outputLog("No Longer AFK"));
                                    }
                                    AFKDetector = false;
                                }



                            }
                        }

                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text.ToString() && messageReceived.Arguments[0].ToString() =="True")
                        {
                            counter1++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text.ToString();

                            theString = theString.Replace("{counter}", counter1.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                              Task.Run(() => OutputText.outputLog(theString));
                            }      

                        }
                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
                        {
                            counter2++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text.ToString();

                            theString = theString.Replace("{counter}", counter2.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(theString));
                            }

                        }
                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
                        {
                            counter3++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text.ToString();

                            theString = theString.Replace("{counter}", counter3.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(theString));
                            }

                        }
                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
                        {
                            counter4++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text.ToString();

                            theString = theString.Replace("{counter}", counter4.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(theString));
                            }

                        }
                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
                        {
                            counter5++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text.ToString();

                            theString = theString.Replace("{counter}", counter5.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(theString));
                            }

                        }
                        if (messageReceived.Address.ToString() == VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
                        {
                            counter6++;
                            var theString = "";
                            theString = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text.ToString();

                            theString = theString.Replace("{counter}", counter6.ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(theString));
                            }

                        }

                    }
                    catch (Exception ex) { }
                }
            };



            var listener = new UDPListener(Convert.ToInt32(FromVRChatPort), callback);

        }
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
