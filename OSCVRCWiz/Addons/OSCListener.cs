using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreOSC;
using OSCVRCWiz.Text;



namespace OSCVRCWiz.Addons
{
    public class OSCListener
    {
        static bool heartConnect = false;
        public static string globalBPM = "0";
        public static int globalAverageTrackerBattery = 0;
        public static int globalLeftControllerBattery = 0;
        public static int globalRightControllerBattery = 0;
        public static int globalAverageControllerBattery = 0;
        public static int OSCReceiveport = 4026;
        public static bool OSCReceiveSpamLog = true;
        public static int HRInternalValue = 5;
        public static bool pauseBPM = false;
        public static bool stopBPM = false;


        public static void OSCRecieveHeartRate(VoiceWizardWindow MainForm)
        {

            int skipper = 0;
            // var ot = new OutputText();
            // The cabllback function
            OutputText.outputLog("[OSC Listener Activated]");
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {
                    //refactor using a switch case
                    /*   switch(messageReceived.Address)
                          {
                              case "/avatar/parameters/averageTrackerBattery": break;
                                  //etc
                          }*/

                    try
                    {
                        //VRCHAT BETA TESTING

                        System.Diagnostics.Debug.WriteLine("address: " + messageReceived.Address.ToString() + "argument: " + messageReceived.Arguments[0].ToString());


                        if (heartConnect == false)
                        {

                            OutputText.outputLog("[First OSC Received]");
                            heartConnect = true;

                        }


                        if (messageReceived.Address == "/avatar/parameters/averageTrackerBattery")
                        {

                            decimal averageTrackerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageTrackerBattery * 100);
                            globalAverageTrackerBattery = battery;
                            if (OSCReceiveSpamLog == true)
                            {
                                //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + messageReceived.Arguments[0].ToString() + "]");
                                //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + Convert.ToInt32(averageTrackerBattery) + "]");
                                OutputText.outputLog("[Average Tracker Battery: " + battery + "%]");
                            }



                        }
                        if (messageReceived.Address == "/avatar/parameters/leftControllerBattery")
                        {
                            decimal leftControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(leftControllerBattery * 100);
                            globalLeftControllerBattery = battery;
                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Left Controller Battery: " + battery + "%]");
                            }

                        }
                        if (messageReceived.Address == "/avatar/parameters/rightControllerBattery")
                        {
                            decimal rightControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(rightControllerBattery * 100);
                            globalRightControllerBattery = battery;
                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Right Controller Battery: " + battery + "%]");
                            }

                        }
                        if (messageReceived.Address == "/avatar/parameters/averageControllerBattery")
                        {
                            decimal averageControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageControllerBattery * 100);
                            globalAverageControllerBattery = battery;
                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Average Controller Battery: " + battery + "%]");

                            }


                        }

                        if (messageReceived.Address == "/avatar/parameters/HR" && pauseBPM == false)
                        {


                            skipper += 1;
                            if (skipper >= HRInternalValue)
                            {
                                skipper = 0;


                                System.Diagnostics.Debug.WriteLine("OSC Received a message Address: " + messageReceived.Address);
                                System.Diagnostics.Debug.WriteLine("OSC Received a message Argument: " + messageReceived.Arguments[0].ToString());
                                globalBPM = messageReceived.Arguments[0].ToString();

                                if (stopBPM == false)
                                {

                                    // var ot = new OutputText();
                                    if (OSCReceiveSpamLog == true)
                                    {
                                        OutputText.outputLog("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm");

                                    }
                                    if (MainForm.rjToggleButton3.Checked == true && MainForm.rjToggleButtonOSC.Checked == true)
                                    {
                                        OutputText.outputVRChat("ぬ" + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //ぬ means heart emoji

                                    }
                                    if (MainForm.rjToggleButton3.Checked == false && MainForm.rjToggleButtonOSC.Checked == true)
                                    {
                                        OutputText.outputVRChat("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //add pack emoji toggle (add emoji selection page

                                    }
                                    if (MainForm.rjToggleButtonChatBox.Checked == true)
                                    {
                                        Task.Run(() => OutputText.outputVRChatSpeechBubbles("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm")); //original


                                    }
                                }



                            }

                        }
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("****-------*****--------Received a message! null address");

                    }
                }

            };

            var listener = new UDPListener(OSCReceiveport, callback);

        }
    }
}
