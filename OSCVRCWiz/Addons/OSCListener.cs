using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreOSC;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;



namespace OSCVRCWiz.Addons
{
    public class OSCListener
    {
        static bool heartConnect = false;
        public static string globalBPM = "0";

        public static int currentHR = 0;
        public static int HRPrevious = 0;
        public static string HREleveated = "";

        public static string trackerCharge = "";
        public static string controllerChargeAVG = "";
        public static string controllerChargeR = "";
        public static string controllerChargeL = "";
        public static string controllerChargeHMD = "";


        public static int globalAverageTrackerBattery = 0;
        public static int globalLeftControllerBattery = 0;
        public static int globalRightControllerBattery = 0;
        public static int globalHMDBattery = 0;
        public static int globalAverageControllerBattery = 0;

        public static int AverageTrackerPrevious= 0;
        public static int AVGControllerPrevious = 0;
        public static int RControllerPrevious = 0;
        public static int LControllerPrevious = 0;
        public static int HMDControllerPrevious = 0;
        public static int OSCReceiveport = 4026;
        public static bool OSCReceiveSpamLog = true;
        public static int HRInternalValue = 5;
        public static bool pauseBPM = false;
        public static bool stopBPM = false;


        public static void OSCRecieveHeartRate()
        {
            
            int skipper = 0;
            // var ot = new OutputText();
            // The cabllback function
            OutputText.outputLog("[OSC Listener Activated]",Color.Green);

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.button7.Enabled = false;
                VoiceWizardWindow.MainFormGlobal.button7.ForeColor = Color.Green;
            });

            HandleOscPacket callback = delegate (OscPacket packet)
            {
                OscMessage messageReceived = null;
                OscBundle messageBundle = null;
                try { messageReceived = (OscMessage)packet; }
                catch(Exception exx) //this is just the quickest why I thought of to deal with bundles without puting a loop around everything. (that implementation would probably be better)
                {
                 //  OutputText.outputLog("TESTING: " + exx.Message, Color.Red);
                    try
                    {

                        messageBundle = (OscBundle)packet;
                        for (int i = 0; i < messageBundle.Messages.Count; i++)
                        {
                            var resend = messageBundle.Messages[i];
                            OSC.OSCReSender.Send(resend);
                        }
                        
                    }
                    catch (Exception ex){
                        OutputText.outputLog("Issue reading osc bundle: "+ ex.Message, Color.Red);
                    }

                }

                
              
                
                
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

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/averageTrackerBattery", (float)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }

                            decimal averageTrackerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageTrackerBattery * 100);
                            globalAverageTrackerBattery = battery;

                            var labelBattery = $"🔋 {battery}%";
                            if (globalAverageTrackerBattery>AverageTrackerPrevious)
                            {
                                trackerCharge = "⚡";
                                labelBattery += " " + trackerCharge;
                            }
                            else { trackerCharge = ""; }
                            AverageTrackerPrevious = globalAverageTrackerBattery;

                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {



                                if (VoiceWizardWindow.MainFormGlobal.groupBoxTrackers.ForeColor != Color.Green)
                                {
                                    VoiceWizardWindow.MainFormGlobal.groupBoxTrackers.ForeColor = Color.Green;
                                    VoiceWizardWindow.MainFormGlobal.TrackerLabel.ForeColor = Color.Green;
                                }
                            VoiceWizardWindow.MainFormGlobal.TrackerLabel.Text= labelBattery;
                            });



                            if (OSCReceiveSpamLog == true)
                            {
                                //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + messageReceived.Arguments[0].ToString() + "]");
                                //  ot.outputLog(MainForm, "[Average Tracker Battery Debug: " + Convert.ToInt32(averageTrackerBattery) + "]");
                                OutputText.outputLog("[Average Tracker Battery: " + battery + "%]");
                            }



                        }
                        if (messageReceived.Address == "/avatar/parameters/leftControllerBattery")
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/leftControllerBattery", (float)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }


                            decimal leftControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(leftControllerBattery * 100);
                            globalLeftControllerBattery = battery;

                            var labelBattery = $"🔋 {battery}%";
                            if (globalLeftControllerBattery > LControllerPrevious)
                            {
                                controllerChargeL = "⚡";
                                labelBattery += " " + controllerChargeL;
                            }
                            else { controllerChargeL = ""; }
                            LControllerPrevious = globalLeftControllerBattery;

                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {


                                if (VoiceWizardWindow.MainFormGlobal.groupBoxControllers.ForeColor != Color.Green)
                                {

                                    VoiceWizardWindow.MainFormGlobal.groupBoxControllers.ForeColor = Color.Green;
                                    VoiceWizardWindow.MainFormGlobal.ControllerLabel.ForeColor = Color.Green;
                                }
                                VoiceWizardWindow.MainFormGlobal.ControllerLabel.Text = labelBattery;
                            });

                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Left Controller Battery: " + battery + "%]");
                            }

                        }
                        if (messageReceived.Address == "/avatar/parameters/rightControllerBattery")
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/rightControllerBattery", (float)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }


                            decimal rightControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(rightControllerBattery * 100);
                            globalRightControllerBattery = battery;

                            var labelBattery = $"🔋 {battery}%";
                            if (globalRightControllerBattery > RControllerPrevious)
                            {
                                controllerChargeR = "⚡";
                                labelBattery += " " + controllerChargeR;
                            }
                            else { controllerChargeR = ""; }
                            RControllerPrevious = globalRightControllerBattery;

                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {


                                if (VoiceWizardWindow.MainFormGlobal.groupBoxRight.ForeColor != Color.Green)
                                {

                                    VoiceWizardWindow.MainFormGlobal.groupBoxRight.ForeColor = Color.Green;
                                    VoiceWizardWindow.MainFormGlobal.labelRight.ForeColor = Color.Green;
                                }
                                VoiceWizardWindow.MainFormGlobal.labelRight.Text = labelBattery;
                            });

                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Right Controller Battery: " + battery + "%]");
                            }

                        }
                        if (messageReceived.Address == "/avatar/parameters/averageControllerBattery")
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/averageControllerBattery", (float)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }


                            decimal averageControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(averageControllerBattery * 100);
                            globalAverageControllerBattery = battery;

                            var labelBattery = $"🔋 {battery}%";
                            if (globalAverageControllerBattery > AVGControllerPrevious)
                            {
                                controllerChargeAVG = "⚡";
                                labelBattery += " " + controllerChargeAVG;
                            }
                            else { controllerChargeL = ""; }
                            AVGControllerPrevious = globalAverageControllerBattery;

                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[Average Controller Battery: " + battery + "%]");

                            }


                        }
                        if (messageReceived.Address == "/avatar/parameters/HMDBat")
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/HMDBat", (float)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }


                            decimal HMDControllerBattery = decimal.Parse(messageReceived.Arguments[0].ToString());
                            int battery = Convert.ToInt32(HMDControllerBattery * 100);
                            globalHMDBattery = battery;

                            var labelBattery = $"🔋 {battery}%";
                            if (globalHMDBattery > HMDControllerPrevious)
                            {
                                controllerChargeHMD = "⚡";
                                labelBattery += " " + controllerChargeHMD;
                            }
                            else { controllerChargeHMD = ""; }
                            HMDControllerPrevious = globalHMDBattery;

                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {


                                if (VoiceWizardWindow.MainFormGlobal.groupBoxHead.ForeColor != Color.Green)
                                {

                                    VoiceWizardWindow.MainFormGlobal.groupBoxHead.ForeColor = Color.Green;
                                    VoiceWizardWindow.MainFormGlobal.labelHead.ForeColor = Color.Green;
                                }
                                VoiceWizardWindow.MainFormGlobal.labelHead.Text = labelBattery;
                            });

                            if (OSCReceiveSpamLog == true)
                            {

                                OutputText.outputLog("[HMD Battery: " + battery + "%]");
                            }

                        }

                        if (messageReceived.Address == "/avatar/parameters/HR" && pauseBPM == false)
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/avatar/parameters/HR", (int)messageReceived.Arguments[0]);
                                OSC.OSCSender.Send(forwardData);
                            }

                            skipper += 1;
                            if (skipper >= HRInternalValue)
                            {
                                skipper = 0;


                                System.Diagnostics.Debug.WriteLine("OSC Received a message Address: " + messageReceived.Address);
                                System.Diagnostics.Debug.WriteLine("OSC Received a message Argument: " + messageReceived.Arguments[0].ToString());
                                globalBPM = messageReceived.Arguments[0].ToString();

                                currentHR = Convert.ToInt32(messageReceived.Arguments[0].ToString());
                                var labelBattery = $"❤️ {globalBPM}";

                              

                               if (currentHR > HRPrevious)
                                {
                                    HREleveated = "🔺";
                                    labelBattery += " "+HREleveated;
                                }
                                else if(currentHR < HRPrevious)
                                {
                                    HREleveated = "🔻";
                                    labelBattery += " " + HREleveated;

                                }
                                else if (currentHR == HRPrevious)
                                {
                                    HREleveated = "";
                                    labelBattery += " " + HREleveated;

                                }
                              
                                HRPrevious = currentHR;

                                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                {



                                    if (VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor != Color.Green)
                                    {
                                        VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor = Color.Green;
                                        VoiceWizardWindow.MainFormGlobal.HeartrateLabel.ForeColor = Color.Green;
                                    }
                                VoiceWizardWindow.MainFormGlobal.HeartrateLabel.Text = labelBattery;
                                });



                                if (stopBPM == false)
                                {

                                    // var ot = new OutputText();
                                    if (OSCReceiveSpamLog == true)
                                    {
                                        OutputText.outputLog("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm");

                                    }
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                                    {
                                        OutputText.outputVRChat("ぬ" + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //ぬ means heart emoji

                                    }
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked == false && VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                                    {
                                        OutputText.outputVRChat("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm");  //add pack emoji toggle (add emoji selection page

                                    }
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                                    {
                                        Task.Run(() => OutputText.outputVRChatSpeechBubbles("Heartbeat: " + messageReceived.Arguments[0].ToString() + " bpm", "bpm")); //original


                                    }
                                }



                            }

                        }
                        if (messageReceived.Address == "/TTSVoiceWizard/TextToSpeech")//OSCListener TTS
                        {
                            var text = messageReceived.Arguments[0].ToString();
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked == true)
                            {
                                var forwardData = new OscMessage("/TTSVoiceWizard/TextToSpeech", text);
                                OSC.OSCSender.Send(forwardData);
                            }
                            //Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "OSCListener"));
                           
                            TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {
                            TTSMessageQueued.text = text;
                            TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                            TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                            TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text.ToString();
                            TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString();
                            TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                            TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                            TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                            TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                            TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                            TTSMessageQueued.STTMode = "OSCListener";
                            TTSMessageQueued.AzureTranslateText = "[ERROR]";
                            });


                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true)
                            {
                                TTSMessageQueue.Enqueue(TTSMessageQueued);
                            }
                            else
                            {
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(TTSMessageQueued));
                            }
                            


                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                            {
                                if (VoiceWizardWindow.MainFormGlobal.groupBoxOSCtoTTS.ForeColor != Color.Green)
                            {
                                VoiceWizardWindow.MainFormGlobal.groupBoxOSCtoTTS.ForeColor = Color.Green;
                                VoiceWizardWindow.MainFormGlobal.labelOSCtoTTS.ForeColor = Color.Green;
                            }
                            });





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
