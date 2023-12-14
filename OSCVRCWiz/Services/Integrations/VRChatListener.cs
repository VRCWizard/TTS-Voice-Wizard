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

        public static int prevCounter1 = 0;
        public static int prevCounter2 = 0;
        public static int prevCounter3 = 0;
        public static int prevCounter4 = 0;
        public static int prevCounter5 = 0;
        public static int prevCounter6 = 0;




        public static void OnStartUp()
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCActivate.Checked == true)//turn on vrchat listener on start
            {
                try
                {
                    Task.Run(() => OSCLegacyVRChatListener());
                }
                catch (Exception ex) { OutputText.outputLog("[OSC VRChat Listener Error: Another Application is already listening on this port, please close that application and restart TTS Voice Wizard.]", Color.Red); }
                VoiceWizardWindow.MainFormGlobal.button33.Enabled = false;

            }
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

            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {
                    try
                    {

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked == true)
                        {
                            if (messageReceived.Address.ToString() == "/avatar/parameters/AFK")//AFK
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

                        if (messageReceived.Address.ToString() == "/avatar/parameters/DoSpeechToText")//Activate Speech to Text from OSC
                        {

                            if (messageReceived.Arguments[0].ToString() == "True")
                            {
                                Task.Run(() => DoSpeech.MainDoSpeechTTS());
                            }


                        }

                        int[] counters = { counter1, counter2, counter3, counter4, counter5, counter6 }; // Counters
                        TextBox[] textBoxes = {
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter1,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter2,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter3,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter4,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter5,
                                    VoiceWizardWindow.MainFormGlobal.textBoxCounter6
                                };
                        TextBox[] messageTextBoxes = {
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5,
                            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6
                        };

                        for (int i = 0; i < counters.Length; i++)
                        {
                            if (messageReceived.Address.ToString() == textBoxes[i].Text.ToString() && messageReceived.Arguments[0].ToString() == "True")
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
                                }


                                var theString = messageTextBoxes[i].Text.ToString();
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


                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOutputVRCCountersOnContact.Checked == true && VoiceWizardWindow.MainFormGlobal.button33.Enabled == false)
                {


                    int[] counters = {
                        counter1,
                        counter2,
                        counter3,
                        counter4,
                        counter5,
                        counter6 };
                    int[] prevCounters = { prevCounter1,
                        prevCounter2,
                        prevCounter3,
                        prevCounter4,
                        prevCounter5,
                        prevCounter6 };
                    TextBox[] textBoxes = {
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5,
                        VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6
                    };

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
                            }

                            var theString = textBoxes[i].Text.ToString();
                            theString = theString.Replace("{counter}", counters[i].ToString());

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && OSCListener.pauseBPM != true)
                            {
                                Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, OutputText.DisplayTextType.Counters));
                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && OSCListener.pauseBPM != true)
                            {
                                Task.Run(() => OutputText.outputVRChat(theString, OutputText.DisplayTextType.Counters,i));
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
                    Settings1.Default.Save();
                }
                VRCCounterTimer.Change(Int32.Parse(VoiceWizardWindow.MainFormGlobal.counterOutputInterval.Text), 0);
            }
            catch (Exception ex) { OutputText.outputLog($"[Error Occurred with VRC Counter: {ex.Message}]", Color.Red); }


        }

    }
}
