using CoreOSC;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Services.Speech;
using System.Diagnostics;


namespace OSCVRCWiz.Services.Integrations
{
    public class VRChatListener
    {
        public static string FromVRChatPort = "9001";
        public static bool AFKDetector = false;
        public static DateTime afkStartTime = DateTime.Now;
        public static int counter1 = 0;
        public static int counter2 = 0;
        public static int counter3 = 0;
        public static int counter4 = 0;
        public static int counter5 = 0;
        public static int counter6 = 0;
        public static int counter7 = 0;
        public static int counter8 = 0;
        public static int counter9 = 0;
        public static int counter10 = 0;

        public static int prevCounter1 = 0;
        public static int prevCounter2 = 0;
        public static int prevCounter3 = 0;
        public static int prevCounter4 = 0;
        public static int prevCounter5 = 0;
        public static int prevCounter6 = 0;
        public static int prevCounter7 = 0;
        public static int prevCounter8 = 0;
        public static int prevCounter9 = 0;
        public static int prevCounter10 = 0;

        public static string[] textBoxes;


        public static string[] messageTextBoxes;


        public static void OnStartUp()
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCActivate.Checked == true)//turn on vrchat listener on start
            {
                try
                {
                    Task.Run(() => OSCLegacyVRChatListener());
                   // Task.Run(() => OSCQueryStart());
                }
                catch (Exception ex) { OutputText.outputLog("[OSC VRChat Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }
                VoiceWizardWindow.MainFormGlobal.button33.Enabled = false;

            }
        }

        public static void setValues()
        {
            textBoxes = new string[] {
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter7.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter8.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter9.Text,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter10.Text,
                                };
            messageTextBoxes = new string[] {
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage7.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage8.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage9.Text,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage10.Text,
                        };
        }


        public static void OSCLegacyVRChatListener()//no in use remove apon release of oscquery
        {
            //  int port = 9001;//VRChats default UDP // ONLY ONE APP CAN LISTEN HERE



            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.button33.Enabled = false;
                VoiceWizardWindow.MainFormGlobal.button33.ForeColor = Color.Green;
                OutputText.outputLog($"[VRChat Listener Activated ({FromVRChatPort})]", Color.Green);
                OutputText.outputLog($"[VRChat Listener: Remember that only one program can listen on a UDP port. TTS Voice Wizard is listening on port {FromVRChatPort}.", Color.DarkOrange);
                if (FromVRChatPort == "9000")
                {
                    OutputText.outputLog($"VRChat Listener: VRChat normally listens on port 9000 so unless you know what you are doing this is probably a mistake.", Color.Red);
                }
            });

            setValues();//tesing this


            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageAddress = "";
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {
                    try
                    {
                        messageAddress = messageReceived.Address.ToString();
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true)
                        {
                            if (messageAddress == "/avatar/parameters/AFK")//AFK
                            {
                               

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

                        if (messageAddress == "/avatar/parameters/DoSpeechToText")//Activate Speech to Text from OSC
                        {

                            if (messageReceived.Arguments[0].ToString() == "True")
                            {
                                Task.Run(() => DoSpeech.MainDoSpeechTTS());
                            }


                        }

                        int[] counters = { counter1, counter2, counter3, counter4, counter5, counter6, counter7, counter8, counter9, counter10 }; // Counters
                      /*  TextBox[] textBoxes = {
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter1,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter2,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter3,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter4,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter5,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter6,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter7,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter8,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter9,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter10,
                                };
                        TextBox[] messageTextBoxes = {
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage7,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage8,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage9,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage10,
                        };*/

                        if(messageAddress.Contains("owo_suit_"))
                        {
                            messageAddress = "/OwoSuit/Counter/Debug";
                        }

                        for (int i = 0; i < counters.Length; i++)
                        {
                            
                            if (messageAddress == textBoxes[i] && messageReceived.Arguments[0].ToString() == "True")
                            {
                                counters[i]++;
                                switch (i)
                                {
                                    case 0:
                                        counter1 = counters[i];
                                        break;
                                    case 1:
                                        counter2 = counters[i];
                                        break;
                                    case 2:
                                        counter3 = counters[i];
                                        break;
                                    case 3:
                                        counter4 = counters[i];
                                        break;
                                    case 4:
                                        counter5 = counters[i];
                                        break;
                                    case 5:
                                        counter6 = counters[i];
                                        break;
                                     case 6:
                                        counter7 = counters[i];
                                        break;
                                    case 7:
                                        counter8 = counters[i];
                                        break;
                                    case 8:
                                        counter9 = counters[i];
                                        break;
                                    case 9:
                                        counter10 = counters[i];
                                        break;
                                }


                                var theString = messageTextBoxes[i];
                                theString = theString.Replace("{counter}", counters[i].ToString());

                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)
                                {
                                    Task.Run(() => OutputText.outputLog(theString));
                                }
                            }
                        }

                    }
                    catch (Exception ex) { OutputText.outputLog("[Error Receiving OSC: "+ex.Message+" ]", Color.Red); }
                }
            };



            var listener = new UDPListener(Convert.ToInt32(FromVRChatPort), callback);

        }






        public  static System.Threading.Timer VRCCounterTimer;

        public static void initiateTimer()
        {
            VRCCounterTimer = new System.Threading.Timer(VRCCountertimertick);
            VRCCounterTimer.Change(2000, 0);
        }

        public static void VRCCountertimertick(object sender)
        {

            Thread t = new Thread(doVRCCounterTimerTick);
            t.IsBackground = true; // Set the thread as a background thread
            t.Start();
        }
        private static void doVRCCounterTimerTick()
        {
           // OutputText.outputLog("timer tick");
            try
            {

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true && AFKDetector == true && OSCListener.pauseBPM != true)
                {
                    var elapsedTime = DateTime.Now - afkStartTime;
                    string elapsedMinutesSeconds = $"{elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();
                    theString = theString.Replace("{time}", elapsedMinutesSeconds);
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked == true)//////////////delete when done testing
                    {
                        Task.Run(() => OutputText.outputLog(theString));
                    }////////////////////////////////////////////////////
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, OutputText.DisplayTextType.Counters));
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                    {
                        Task.Run(() => OutputText.outputVRChat(theString, OutputText.DisplayTextType.Counters));
                    }
                }

                bool SentValue = false;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOutputVRCCountersOnContact.Checked == true && VoiceWizardWindow.MainFormGlobal.button33.Enabled == false)
                {
                    int[] counters = {
                        counter1,
                        counter2,
                        counter3,
                        counter4,
                        counter5,
                        counter6,
                        counter7,
                        counter8,
                        counter9,
                        counter10,
                    };
                    int[] prevCounters = { prevCounter1,
                        prevCounter2,
                        prevCounter3,
                        prevCounter4,
                        prevCounter5,
                        prevCounter6,
                        prevCounter7,
                        prevCounter8,
                        prevCounter9,
                        prevCounter10,
                    };
                   /* TextBox[] textBoxes = {
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage7,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage8,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage9,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage10,
                    };*/

                    for (int i = 0; i < counters.Length; i++)
                    {
                        if (counters[i] > prevCounters[i])
                        {
                            // prevCounters[i] = counters[i];

                            switch (i)
                            {
                                case 0:
                                    prevCounter1 = counters[i];
                                    break;
                                case 1:
                                    prevCounter2 = counters[i];
                                    break;
                                case 2:
                                    prevCounter3 = counters[i];
                                    break;
                                case 3:
                                    prevCounter4 = counters[i];
                                    break;
                                case 4:
                                    prevCounter5 = counters[i];
                                    break;
                                case 5:
                                    prevCounter6 = counters[i];
                                    break;
                                case 6:
                                    prevCounter7 = counters[i];
                                    break;
                                case 7:
                                    prevCounter8 = counters[i];
                                    break;
                                case 8:
                                    prevCounter9 = counters[i];
                                    break;
                                case 9:
                                    prevCounter10 = counters[i];
                                    break;
                            }

                            if (!SentValue)//so it can only happen once per tick to prevent timeout
                            {
                                SentValue = true;
                                var theString = messageTextBoxes[i];
                                theString = theString.Replace("{counter}", counters[i].ToString());

                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                                {
                                    Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, OutputText.DisplayTextType.Counters));
                                }
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
                                {
                                    Task.Run(() => OutputText.outputVRChat(theString, OutputText.DisplayTextType.Counters, i));
                                }
                            }

                           
                        }
                    }




                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked == true)
                {
                    Settings1.Default.Counter1 = counter1;
                    Settings1.Default.Counter2 = counter2;
                    Settings1.Default.Counter3 = counter3;
                    Settings1.Default.Counter4 = counter4;
                    Settings1.Default.Counter5 = counter5;
                    Settings1.Default.Counter6 = counter6;
                    Settings1.Default.Counter7 = counter7;
                    Settings1.Default.Counter8 = counter8;
                    Settings1.Default.Counter9 = counter9;
                    Settings1.Default.Counter10 = counter10;
                    Settings1.Default.Save();
                }
                VRCCounterTimer.Change(Int32.Parse(VoiceWizardWindow.MainFormGlobal.counterOutputInterval.Text), 0);
            }
            catch (Exception ex) { OutputText.outputLog($"[Error Occurred with VRC Counter: {ex.Message}]", Color.Red); }


        }

        //VRChat OSCQuery Test Code
      //  public static void OSCQueryStart() 
      //  {
           /* var tcpPort = Extensions.GetAvailableTcpPort();
            var udpPort = Int16.Parse(FromVRChatPort);//Extensions.GetAvailableUdpPort();

            var oscQueryService = new OSCQueryServiceBuilder()
                .WithDefaults()
                .WithTcpPort(tcpPort)
                .WithUdpPort(udpPort)
                .WithServiceName("TTS Voice Wizard")
                .Build();

            // Manually logging the ports to see them without a logger
            OutputText.outputLog($"Started OSCQueryService at TCP {tcpPort}, UDP {udpPort}");*/

            /* var OSCQueryService = new OSCQueryServiceBuilder()
                                  .WithServiceName("TTS Voice Wizard v" + VoiceWizardWindow.currentVersion)
                                 .WithTcpPort(Extensions.GetAvailableTcpPort())
                                  .WithUdpPort(Convert.ToInt32(OSCPort))
                                  .Build();*/

            // Add endpoints for all data tts voice wizard sends out
            // OSCQueryService.AddEndpoint("/path", "T means bool, i means int, f means float, s means string", Attributes.AccessValues.WriteOnly, null, "description for people to look at");

       // }
       /* public static void OSCQueryStop()
        {
            oscQueryService.Dispose();
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
