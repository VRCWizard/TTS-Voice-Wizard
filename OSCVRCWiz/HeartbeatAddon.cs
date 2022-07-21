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
        public static void OSCRecieveHeartRate(VoiceWizardWindow MainForm)
        {
            int skipper = 0;
            // The cabllback function
            HandleOscPacket callback = delegate (OscPacket packet)
            {
                var messageReceived = (OscMessage)packet;
                if (messageReceived != null)
                {


                    try
                    {
                        if (heartConnect == false)
                        {
                            var ot = new OutputText();
                            ot.outputLog(MainForm, "HRtoVRChat_OSC Connected");
                            heartConnect = true;

                        }



                        if (messageReceived.Address == "/avatar/parameters/HR" && VoiceWizardWindow.pauseBPM == false && VoiceWizardWindow.stopBPM == false)
                        {
                            skipper += 1;
                            if (skipper >= VoiceWizardWindow.HRInternalValue)
                            {
                                skipper = 0;

                                System.Diagnostics.Debug.WriteLine("OSC Received a message Address: " + messageReceived.Address);
                                System.Diagnostics.Debug.WriteLine("OSC Received a message Argument: " + messageReceived.Arguments[0].ToString());
                                var ot = new OutputText();
                                if (VoiceWizardWindow.BPMSpamLog == true)
                                {
                                    ot.outputLog(MainForm, "Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm");

                                }
                                if(MainForm.rjToggleButton3.Checked == true)
                                {
                                    ot.outputVRChat(MainForm, "ぬ" + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //ぬ means heart emoji

                                }
                                if(MainForm.rjToggleButton3.Checked == false)
                                {
                                   ot.outputVRChat(MainForm, "Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //add pack emoji toggle (add emoji selection page

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
