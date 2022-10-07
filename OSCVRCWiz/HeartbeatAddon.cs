using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpOSC;

namespace OSCVRCWiz
{
    public class HeartbeatAddon
    {
        static bool heartConnect = false;
        public static string globalBPM = "0";
        public static int globalAverageTrackerBattery =0;
        public static int globalLeftControllerBattery = 0;
        public static int globalRightControllerBattery =0;
        public static int globalAverageControllerBattery = 0;
        public static void OSCRecieveHeartRate(VoiceWizardWindow MainForm)
        {
            int skipper = 0;
            var ot = new OutputText();
            // The cabllback function
            ot.outputLog(MainForm, "[OSC Listener Activated]");
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {


                    try
                    {
                        if (heartConnect == false)
                        {
                            
                            ot.outputLog(MainForm, "[First OSC Recieved]");
                            heartConnect = true;

                        }


                        if (messageReceived.Address == "/avatar/parameters/averageTrackerBattery")
                        {
                           
                            decimal averageTrackerBattery = Decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageTrackerBattery * 100);
                            globalAverageTrackerBattery = battery;
                            if (VoiceWizardWindow.BPMSpamLog == true)
                            {
                              //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + messageReceived.Arguments[0].ToString() + "]");
                              //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + Convert.ToInt32(averageTrackerBattery) + "]");
                                ot.outputLog(MainForm, "[Average Tracker Battery: " + battery + "%]");
                            }
                         /*   if ((battery == 20 || battery == 15 || battery == 10) && MainForm.rjToggleButtonChatBox.Checked == true)
                            {
                                ot.outputVRChatSpeechBubbles(MainForm, "Warning: Trackers are getting low, their average battery life is " + battery + "%", "tts");
                            }*/

                        }
                        if (messageReceived.Address == "/avatar/parameters/leftControllerBattery")
                        {
                            decimal leftControllerBattery = Decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(leftControllerBattery * 100);
                            globalLeftControllerBattery = battery;
                            if (VoiceWizardWindow.BPMSpamLog == true)
                            {
                               
                                ot.outputLog(MainForm, "[Left Controller Battery: " + battery + "%]");
                            }
                          /*  if (battery <= 20 && MainForm.rjToggleButtonChatBox.Checked == true)
                            {
                             //   ot.outputVRChatSpeechBubbles(MainForm, "Warning: Left Controller is getting low, its battery life is" + battery + "%", "tts");
                            }*/
                        }
                        if (messageReceived.Address == "/avatar/parameters/rightControllerBattery")
                        {
                            decimal rightControllerBattery = Decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(rightControllerBattery * 100);
                            globalRightControllerBattery = battery;
                            if (VoiceWizardWindow.BPMSpamLog == true)
                            {
                                
                                ot.outputLog(MainForm, "[Right Controller Battery: " + battery + "%]");
                            }
                          /*  if (battery > 20 && MainForm.rjToggleButtonChatBox.Checked == true)
                            {
                               // ot.outputVRChatSpeechBubbles(MainForm, "Warning: Right Controller is getting low, its battery life is" + battery + "%", "tts");
                            }*/
                        }
                        if (messageReceived.Address == "/avatar/parameters/averageControllerBattery")
                        {
                            decimal averageControllerBattery = Decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageControllerBattery * 100);
                            globalAverageControllerBattery = battery;
                            if (VoiceWizardWindow.BPMSpamLog == true)
                            {
                                
                                ot.outputLog(MainForm, "[Average Controller Battery: " + battery + "%]");
                                
                            }
                          /*  if((battery == 20|| battery == 15|| battery == 10) && MainForm.rjToggleButtonChatBox.Checked == true)
                            {
                                ot.outputVRChatSpeechBubbles(MainForm, "Warning: Controllers are getting low, their average battery life is" + battery + "%","tts");
                            }*/
                         
                        }



                        if (messageReceived.Address == "/avatar/parameters/HR" && VoiceWizardWindow.pauseBPM == false )
                        {
                            skipper += 1;
                            if (skipper >= VoiceWizardWindow.HRInternalValue)
                            {
                                skipper = 0;


                                System.Diagnostics.Debug.WriteLine("OSC Received a message Address: " + messageReceived.Address);
                                System.Diagnostics.Debug.WriteLine("OSC Received a message Argument: " + messageReceived.Arguments[0].ToString());
                                globalBPM = messageReceived.Arguments[0].ToString();

                                if (VoiceWizardWindow.stopBPM == false)
                                {

                                   // var ot = new OutputText();
                                    if (VoiceWizardWindow.BPMSpamLog == true)
                                    {
                                        ot.outputLog(MainForm, "Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm");

                                    }
                                    if (MainForm.rjToggleButton3.Checked == true && MainForm.rjToggleButtonOSC.Checked == true)
                                    {
                                        ot.outputVRChat(MainForm, "ぬ" + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //ぬ means heart emoji

                                    }
                                    if (MainForm.rjToggleButton3.Checked == false && MainForm.rjToggleButtonOSC.Checked == true)
                                    {
                                        ot.outputVRChat(MainForm, "Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //add pack emoji toggle (add emoji selection page

                                    }
                                    if (MainForm.rjToggleButtonChatBox.Checked == true)
                                    {
                                        Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, "Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm")); //original


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

            var listener = new UDPListener(VoiceWizardWindow.heartRatePort, callback);

            //  System.Diagnostics.Debug.WriteLine("****-------*****--------Press enter to stop");
            //  Console.ReadLine();
            // listener.Close();
            //NO U can't stop the connection rn, yes you must restart app to stop listening to heatbeat



        }
    }
}
