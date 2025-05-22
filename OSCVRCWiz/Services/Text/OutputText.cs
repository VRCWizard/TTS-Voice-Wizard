using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CoreOSC;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Resources.StartUp.StartUp;
using Amazon.Polly.Model;
using System.Threading;
using System.Diagnostics;

namespace OSCVRCWiz.Services.Text
{
    public class OutputText
    {
        static DisplayTextType previousRequestType = DisplayTextType.None;
        bool currentlyPrinting = false;
        static DateTime lastDateTime = DateTime.Now;
        public static string lastKatString = "";
        public static string numKATSyncParameters = "4";
        public static int debugDelayValue = Convert.ToInt32(Settings1.Default.delayDebugValueSetting);// Recommended delay of 250ms //this is the first call i see now
        public static int eraseDelay = Convert.ToInt32(Settings1.Default.hideDelayValue);
        public static bool EraserRunning = false;
        public static string lastRememberedSong;
        public static int lastStringPoint = 1;
        public static int lastCounter;
       // static CancellationTokenSource cts = new();

        public enum DisplayTextType
        {
            None,
            TextToText,
            TextToSpeech,
            HeartRate,
            Counters,
            Time,
            WindowsMedia,
            Spotify,
            UpdateText,
            RepeatText

        }


        public static void loadTextDelays()
        {
            debugDelayValue = Convert.ToInt32(Settings1.Default.delayDebugValueSetting);
            eraseDelay = Convert.ToInt32(Settings1.Default.hideDelayValue);

        }
        public static async void outputLog(string textstring, Color? color = null)
        {
          //  _ = Task.Run(() => //This effectily reduces the processing time for any function using output to log since it now runs as a seperate task that the fucntion won't wait for.
          //  {
                //  MainForm.AppendTextBox("You Said: " + textstring + "\r");
                if (color == null)
                {
                    VoiceWizardWindow.MainFormGlobal.logLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "]" + ": " + textstring);
                }
                else
                {
                    VoiceWizardWindow.MainFormGlobal.logLine("[" + DateTime.Now.ToString("h:mm:ss tt") + "]" + ": " + textstring, color);
                }
         //   });


        }
        private static string SplitToLines(string value, int maximumLineLength)
        {
            int lineSize = maximumLineLength;//need to add this to UI
            try
            {
                string perfectString = "";
                var words = value.Split(' ');
                var line = new StringBuilder();

                foreach (var word in words)
                {

                    if (line.Length + word.Length >= maximumLineLength)
                    {
                        System.Diagnostics.Debug.WriteLine(line.ToString());
                        if (line.ToString().Length <= lineSize)
                        {
                            perfectString += line.ToString();
                            int spacesToAdd = lineSize - line.ToString().Length;
                            for (int i = 0; i < spacesToAdd; i++)
                            {
                                perfectString += " ";
                            }

                        }
                        line = new StringBuilder();
                    }

                    line.AppendFormat("{0}{1}", line.Length > 0 ? " " : "", word);
                }

                System.Diagnostics.Debug.WriteLine(line.ToString());
                perfectString += line.ToString();
                return perfectString;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR FOUND==========================================================wefwefefwefwfweffwefwef");
                return "error";
            }


        }
        public static async void outputTextFile(string textstring, string filepath)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = filepath;

                string fullPath = Path.Combine(basePath, relativePath);

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true)
                {
                    await File.WriteAllTextAsync(fullPath, textstring);

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked) //hide
                    {
                        hideTimer.Change(eraseDelay, 0);

                    }
                }
            }
            catch (Exception ex)
            {
                outputLog("[OBSText File Error: " + ex.Message + ". Try moving folder location.]", Color.Red);
            }
        }

        public static async void outputVRChatSpeechBubbles(string textstring, DisplayTextType type)
        {
            try
            {
                bool keyboardOff = true;
                bool notificationSound = false;


                if (type == DisplayTextType.TextToSpeech)
                {
                    keyboardOff = !VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked;
                    notificationSound = VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked;
                }

                if (keyboardOff == false)
                {
                    if (string.IsNullOrWhiteSpace(textstring)) 
                    { 
                        keyboardOff = true; 
                    }//prevent blank messages from opening keyboard if it's enabled
                }

                


                // byte[] bytes = Encoding.Default.GetBytes(textstring);
                // textstring = Encoding.UTF8.GetString(bytes);


                //  System.Diagnostics.Debug.WriteLine("Encoded UTF-8: " + textstring);


                var typingbubbleOff = new OscMessage("/chatbox/typing", false);//this is turned on as soon as you press the STTTS button and turned off here
                var messageSpeechBubble = new OscMessage("/chatbox/input", textstring, keyboardOff, notificationSound);
                //   var messageSpeechBubble = new SharpOSC.OscMessage("/chatbox/input", textstring);
                //testing if error message appears /what value is defaulted to if not specified






                if (type == DisplayTextType.TextToSpeech)// so in otherowrds if type is tts it disables typing indicator
                {
                    OSC.OSCSender.Send(typingbubbleOff);
                }
                OSC.OSCSender.Send(messageSpeechBubble);

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == false)//why is this here?
                {
                    hideTimer.Change(eraseDelay, 0);
                }

                if (type == DisplayTextType.WindowsMedia|| type == DisplayTextType.Spotify)
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == false || VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked == true)
                        //the fix, this needs to activate when ever KAT DOESNT OUTPUT, which also happens if disable spotify output for kat is enabled
                    {
                        SpotifyAddon.lastSong = SpotifyAddon.title;
                        WindowsMedia.previousTitle = WindowsMedia.mediaTitle;
                    }

                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked) //inactive hide
                {
                    if (type == DisplayTextType.HeartRate || type == DisplayTextType.Counters || type == DisplayTextType.Time)
                    {
                        SpotifyAddon.pauseSpotify = true;
                        //this is for when using counters or bpm i guess too, it makes them pause spotify(media output)
                    }

                    hideTimer.Change(eraseDelay, 0);

                }
                else
                {
                    //this else is meant as a crude fix to output breaking when hide text delay is turned off
                    //hide tet delay is recommened with media output
                    OSCListener.pauseBPM = false;
                    SpotifyAddon.pauseSpotify = false;
                }
            }
            catch (Exception ex)
            {
                outputLog("[VRC Chatbox OSC Error: " + ex.Message + "]", Color.Red);
                outputLog("[Error usually caused by VPN.]", Color.DarkOrange);

            }

        }

        public static async void outputVRChat(string textstringbefore, DisplayTextType type, int counterNum = 0)
        {
            try
            {
              //  cts.Cancel();
              //  cts = new();
                float letter = 0.0F;
                int charCounter = 0;
                int stringPoint = 1;


                float letterFloat0 = 0;
                float letterFloat1 = 0;
                float letterFloat2 = 0;
                float letterFloat3 = 0;
                float letterFloat4 = 0;
                float letterFloat5 = 0;
                float letterFloat6 = 0;
                float letterFloat7 = 0;

                float letterFloat8 = 0; //16 mode
                float letterFloat9 = 0;//16 mode
                float letterFloat10 = 0;//16 mode
                float letterFloat11 = 0;//16 mode
                float letterFloat12 = 0;//16 mode
                float letterFloat13 = 0;//16 mode
                float letterFloat14 = 0;//16 mode
                float letterFloat15 = 0;//16 mode

                var OSCMakeKatVisible = new OscMessage("/avatar/parameters/KAT_Visible", true);
                var OSCClearKatEraseAll = new OscMessage("/avatar/parameters/KAT_Pointer", 255);
                var OSCCurrentkatPointer = new OscMessage("/avatar/parameters/KAT_Pointer", 1);
                var Char0 = new OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                var Char1 = new OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                var Char2 = new OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                var Char3 = new OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);

                var Char4 = new OscMessage("/avatar/parameters/KAT_CharSync4", letterFloat4);
                var Char5 = new OscMessage("/avatar/parameters/KAT_CharSync5", letterFloat5);
                var Char6 = new OscMessage("/avatar/parameters/KAT_CharSync6", letterFloat6);
                var Char7 = new OscMessage("/avatar/parameters/KAT_CharSync7", letterFloat7);

                var Char8 = new OscMessage("/avatar/parameters/KAT_CharSync8", letterFloat8);
                var Char9 = new OscMessage("/avatar/parameters/KAT_CharSync9", letterFloat9);
                var Char10 = new OscMessage("/avatar/parameters/KAT_CharSync10", letterFloat10);
                var Char11 = new OscMessage("/avatar/parameters/KAT_CharSync11", letterFloat11);
                var Char12 = new OscMessage("/avatar/parameters/KAT_CharSync12", letterFloat12);
                var Char13 = new OscMessage("/avatar/parameters/KAT_CharSync13", letterFloat13);
                var Char14 = new OscMessage("/avatar/parameters/KAT_CharSync14", letterFloat14);
                var Char15 = new OscMessage("/avatar/parameters/KAT_CharSync15", letterFloat15);



                if(VoiceWizardWindow.MainFormGlobal.rjTogglePartialResults.Checked == true) 
                {
                    type = DisplayTextType.TextToText;
                }

              //  if (type == DisplayTextType.TextToSpeech || type == DisplayTextType.UpdateText || type == DisplayTextType.WindowsMedia || type == DisplayTextType.Spotify)//remember last message
              //  {
                    lastKatString = textstringbefore;
               // }

               

                if (type != DisplayTextType.UpdateText)// if adding then tttadd should already be visible
                {
                    OSC.OSCSender.Send(OSCMakeKatVisible);
                }

                //if emojis
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked == true)
                {
                    textstringbefore = EmojiAddon.DoEmojiReplacement(textstringbefore);

                }





                System.Diagnostics.Debug.WriteLine("*KAT String Splitting*");
                int lineSplitLength = 32;
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    lineSplitLength = Int32.Parse(VoiceWizardWindow.MainFormGlobal.KATLineLengthTextBox.Text);

                });

                //pre-process input string
                string textstring = SplitToLines(textstringbefore, lineSplitLength);//prevent line cut off
                textstring = ParameterSyncSpacing(textstring);

               






               /* if ((DateTime.Now - lastDateTime).Seconds <= 1)//collision prevention
                {
                    Task.Delay(1555).Wait();
                }
                lastDateTime = DateTime.Now;*/


                switch (type)
                {
                    case DisplayTextType.HeartRate:
                        if (previousRequestType == DisplayTextType.HeartRate)
                        {
                            //Task.Delay(50).Wait();
                        }
                        else
                        {
                            OSC.OSCSender.Send(OSCClearKatEraseAll);
                        }
                        break;

                    case DisplayTextType.Counters:
                        if (previousRequestType == DisplayTextType.Counters)
                        {
                           // Task.Delay(50).Wait();
                           if(lastCounter != counterNum)
                            {
                                OSC.OSCSender.Send(OSCClearKatEraseAll);
                            }
                        }
                        else
                        {
                            if ((DateTime.Now - lastDateTime).Seconds <= 1)//collision prevention
                            {
                                OutputText.outputLog("[Debug: KAT Collision Prevented]");
                                Task.Delay(debugDelayValue * lastStringPoint).Wait();
                            }  
                            OSC.OSCSender.Send(OSCClearKatEraseAll);
                        }
                        break;
                    case DisplayTextType.WindowsMedia:

                        if (previousRequestType == DisplayTextType.WindowsMedia)
                        {
                            if (WindowsMedia.mediaTitle != lastRememberedSong)
                            {
                             //   Task.Delay(500).Wait();
                                OSC.OSCSender.Send(OSCClearKatEraseAll);
                            }

                        }
                        lastRememberedSong = WindowsMedia.mediaTitle;
                        break;
                    case DisplayTextType.Spotify:

                        if (previousRequestType == DisplayTextType.Spotify)
                        {
                            if (SpotifyAddon.title != lastRememberedSong)
                            {
                             //   Task.Delay(500).Wait();
                                OSC.OSCSender.Send(OSCClearKatEraseAll);
                            }

                        }
                        lastRememberedSong = SpotifyAddon.title;
                        break;
                    case DisplayTextType.TextToSpeech://dont clear all
                        if ((DateTime.Now - lastDateTime).Seconds <= 1)//collision prevention
                        {
                            OutputText.outputLog("[Debug: KAT Collision Prevented]");
                            Task.Delay(debugDelayValue * (lastStringPoint+1)).Wait();
                            
                        }
                        
                        OSC.OSCSender.Send(OSCClearKatEraseAll);

                        break;
                    case DisplayTextType.TextToText://dont clear all
                        break;
                    case DisplayTextType.UpdateText://dont clear all
                                                    break;
                    case DisplayTextType.RepeatText: //dont clear all
                        break;
                    default:
                        OSC.OSCSender.Send(OSCClearKatEraseAll);

                        break;

                }
                lastDateTime = DateTime.Now;
                previousRequestType = type;
   

                foreach (char c in textstring)
                {
                  //  if (!cts.Token.IsCancellationRequested)
                  //  {
                        letter = GetLetterID(c);


                    switch (charCounter)
                    {
                    
                   
                        case 0:
                            letterFloat0 = letter;
                            break;
                        case 1:
                            letterFloat1 = letter;
                            break;
                        case 2:
                            letterFloat2 = letter;
                            break;
                        case 3:
                            if (numKATSyncParameters == "4")
                            {
                                Task.Delay(debugDelayValue).Wait();
                                letterFloat3 = letter;


                                OSCCurrentkatPointer = new OscMessage("/avatar/parameters/KAT_Pointer", stringPoint);
                                Char0 = new OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                                Char1 = new OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                                Char2 = new OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                                Char3 = new OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);
                                OSCMakeKatVisible = new OscMessage("/avatar/parameters/KAT_Visible", true);


                                OSC.OSCSender.Send(OSCCurrentkatPointer);
                                OSC.OSCSender.Send(Char0);
                                OSC.OSCSender.Send(Char1);
                                OSC.OSCSender.Send(Char2);
                                OSC.OSCSender.Send(Char3);
                                OSC.OSCSender.Send(OSCMakeKatVisible);


                                stringPoint += 1;
                                charCounter = -1;
                                letterFloat0 = 0;
                                letterFloat1 = 0;
                                letterFloat2 = 0;
                                letterFloat3 = 0;


                            }
                            if (numKATSyncParameters == "8" || numKATSyncParameters == "16")
                            {
                                letterFloat3 = letter;

                            }
                            break;
                        case 4:
                            letterFloat4 = letter;
                            break;
                        case 5:
                            letterFloat5 = letter;
                            break;
                        case 6:
                            letterFloat6 = letter;
                            break;
                        case 7:
                            if (numKATSyncParameters == "8")
                            {
                                Task.Delay(debugDelayValue).Wait();
                                letterFloat7 = letter;



                                OSCCurrentkatPointer = new OscMessage("/avatar/parameters/KAT_Pointer", stringPoint);
                                Char0 = new OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                                Char1 = new OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                                Char2 = new OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                                Char3 = new OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);

                                Char4 = new OscMessage("/avatar/parameters/KAT_CharSync4", letterFloat4);
                                Char5 = new OscMessage("/avatar/parameters/KAT_CharSync5", letterFloat5);
                                Char6 = new OscMessage("/avatar/parameters/KAT_CharSync6", letterFloat6);
                                Char7 = new OscMessage("/avatar/parameters/KAT_CharSync7", letterFloat7);
                                OSCMakeKatVisible = new OscMessage("/avatar/parameters/KAT_Visible", true);



                                OSC.OSCSender.Send(OSCCurrentkatPointer);
                                OSC.OSCSender.Send(Char0);
                                OSC.OSCSender.Send(Char1);
                                OSC.OSCSender.Send(Char2);
                                OSC.OSCSender.Send(Char3);

                                OSC.OSCSender.Send(Char4);
                                OSC.OSCSender.Send(Char5);
                                OSC.OSCSender.Send(Char6);
                                OSC.OSCSender.Send(Char7);

                                OSC.OSCSender.Send(OSCMakeKatVisible);


                                stringPoint += 1;
                                charCounter = -1;
                                letterFloat0 = 0;
                                letterFloat1 = 0;
                                letterFloat2 = 0;
                                letterFloat3 = 0;

                                letterFloat4 = 0;
                                letterFloat5 = 0;
                                letterFloat6 = 0;
                                letterFloat7 = 0;
                            }
                            if (numKATSyncParameters == "16")
                            {
                                letterFloat7 = letter;

                            }
                            break;

                        case 8:
                            letterFloat8 = letter;
                            break;
                        case 9:
                            letterFloat9 = letter;
                            break;
                        case 10:
                            letterFloat10 = letter;
                            break;
                        case 11:
                            letterFloat11 = letter;
                            break;
                        case 12:
                            letterFloat12 = letter;
                            break;
                        case 13:
                            letterFloat13 = letter;
                            break;
                        case 14:
                            letterFloat14 = letter;
                            break;
                        case 15:
                            letterFloat15 = letter;
                            Task.Delay(debugDelayValue).Wait();

                            OSCCurrentkatPointer = new OscMessage("/avatar/parameters/KAT_Pointer", stringPoint);

                            Char0 = new OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                            Char1 = new OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                            Char2 = new OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                            Char3 = new OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);

                            Char4 = new OscMessage("/avatar/parameters/KAT_CharSync4", letterFloat4);
                            Char5 = new OscMessage("/avatar/parameters/KAT_CharSync5", letterFloat5);
                            Char6 = new OscMessage("/avatar/parameters/KAT_CharSync6", letterFloat6);
                            Char7 = new OscMessage("/avatar/parameters/KAT_CharSync7", letterFloat7);

                            Char8 = new OscMessage("/avatar/parameters/KAT_CharSync8", letterFloat8);
                            Char9 = new OscMessage("/avatar/parameters/KAT_CharSync9", letterFloat9);
                            Char10 = new OscMessage("/avatar/parameters/KAT_CharSync10", letterFloat10);
                            Char11 = new OscMessage("/avatar/parameters/KAT_CharSync11", letterFloat11);

                            Char12 = new OscMessage("/avatar/parameters/KAT_CharSync12", letterFloat12);
                            Char13 = new OscMessage("/avatar/parameters/KAT_CharSync13", letterFloat13);
                            Char14 = new OscMessage("/avatar/parameters/KAT_CharSync14", letterFloat14);
                            Char15 = new OscMessage("/avatar/parameters/KAT_CharSync15", letterFloat15);
                            OSCMakeKatVisible = new OscMessage("/avatar/parameters/KAT_Visible", true);





                            OSC.OSCSender.Send(OSCCurrentkatPointer);

                            OSC.OSCSender.Send(Char0);
                            OSC.OSCSender.Send(Char1);
                            OSC.OSCSender.Send(Char2);
                            OSC.OSCSender.Send(Char3);

                            OSC.OSCSender.Send(Char4);
                            OSC.OSCSender.Send(Char5);
                            OSC.OSCSender.Send(Char6);
                            OSC.OSCSender.Send(Char7);

                            OSC.OSCSender.Send(Char8);
                            OSC.OSCSender.Send(Char9);
                            OSC.OSCSender.Send(Char10);
                            OSC.OSCSender.Send(Char11);
                            OSC.OSCSender.Send(Char12);
                            OSC.OSCSender.Send(Char13);
                            OSC.OSCSender.Send(Char14);
                            OSC.OSCSender.Send(Char15);

                            OSC.OSCSender.Send(OSCMakeKatVisible);


                            stringPoint += 1;
                            charCounter = -1;
                            letterFloat0 = 0;
                            letterFloat1 = 0;
                            letterFloat2 = 0;
                            letterFloat3 = 0;

                            letterFloat4 = 0;
                            letterFloat5 = 0;
                            letterFloat6 = 0;
                            letterFloat7 = 0;


                            letterFloat8 = 0;
                            letterFloat9 = 0;
                            letterFloat10 = 0;
                            letterFloat11 = 0;
                            letterFloat12 = 0;
                            letterFloat13 = 0;
                            letterFloat14 = 0;
                            letterFloat15 = 0;
                            break;

                        default: break;
                        }


                        charCounter += 1;
                        //  currentlyPrinting = true;


                        if (stringPoint >= 33)
                        {
                            break;

                        }
                 //   }

                }
                lastStringPoint = stringPoint;

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked && type != DisplayTextType.RepeatText) //inactive hide
                {
                    //make timer function start here or be reset here
                    System.Diagnostics.Debug.WriteLine("Outputing text to vrchat finished. Begun scheduled hide text timer");
                    if (type == DisplayTextType.HeartRate || type == DisplayTextType.Counters || type == DisplayTextType.Time)
                    {
                        SpotifyAddon.pauseSpotify = true;
                        //this is for when using counters or bpm i guess too, it makes them pause spotify(media output)
                    }


                    hideTimer.Change(eraseDelay, 0);
                    EraserRunning = true;
                }
                else
                {
                    //this else is meant as a crude fix to output breaking when hide text delay is turned off
                    //hide tet delay is recommened with media output
                    OSCListener.pauseBPM = false;
                    SpotifyAddon.pauseSpotify = false;
                }
                if (EraserRunning == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked == true)
                {
                    if((type == DisplayTextType.WindowsMedia || type == DisplayTextType.Spotify) && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked)
                    {
                        return;
                    }
                    if(type == DisplayTextType.Counters || type== DisplayTextType.HeartRate || type == DisplayTextType.TextToText) 
                    {
                        return;
                    }


                    Task.Delay(2000).Wait();
                    outputVRChat(lastKatString, DisplayTextType.RepeatText);
                    System.Diagnostics.Debug.WriteLine("--Repeating Kat Text");


                }
            }
            catch (Exception ex)
            {
                outputLog("[VRC KAT OSC Error: " + ex.Message + "]", Color.Red);
                outputLog("[Error usually caused by VPN.]", Color.DarkOrange);
            }




        }


        public static System.Threading.Timer hideTimer;
        public static System.Threading.Timer typeTimer;
        public static System.Threading.Timer katRefreshTimer;
        public static bool typingBox = false;

        public static void initiateTextTimers()
        {
            hideTimer = new System.Threading.Timer(hidetimertick);
            hideTimer.Change(Timeout.Infinite, Timeout.Infinite);

            typeTimer = new System.Threading.Timer(typetimertick);
            typeTimer.Change(1500, 0);

            katRefreshTimer = new System.Threading.Timer(katRefreshtimertick);
            katRefreshTimer.Change(2000, 0);
        }

        private static void hidetimertick(object sender)
        {

            Thread t = new Thread(doHideTimerTick);
            t.IsBackground = true; // Set the thread as a background thread
            t.Start();
        }
        private static void typetimertick(object sender)
        {

            Thread t = new Thread(doTypeTimerTick);
            t.IsBackground = true;
            t.Start();
        }

        public static void katRefreshtimertick(object sender)
        {

            Thread t = new Thread(doKatRefreshTimerTick);
            t.IsBackground = true;
            t.Start();
        }


        private static void doHideTimerTick() //When to Hide Text
        {

            OSCListener.pauseBPM = false;
            SpotifyAddon.pauseSpotify = false;
            OutputText.EraserRunning = false;

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked == true)
                {
                    Task.Delay(2500).Wait();

                }

                var message0 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Visible", false);
                OSC.OSCSender.Send(message0);
                var message1 = new CoreOSC.OscMessage("/avatar/parameters/KAT_Pointer", 255);
                OSC.OSCSender.Send(message1);
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked)
            {
                var message1 = new CoreOSC.OscMessage("/chatbox/input", "", true, false);
                OSC.OSCSender.Send(message1);
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked)
            {
                OutputText.outputTextFile("", @"Output\TextOutput\OBSText.txt");
                OutputText.outputTextFile("", @"Output\TextOutput\OBSTextTranslated.txt");
                OutputText.outputTextFile("", @"Output\TextOutput\TranscriptionOnly.txt");
            }

            System.Diagnostics.Debug.WriteLine("****-------*****--------Tick");



        }




        private static void doTypeTimerTick()//Send Typing Parameter to VRChat
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    if (typingBox == false && VoiceWizardWindow.MainFormGlobal.mainTabControl.SelectedTab == VoiceWizardWindow.MainFormGlobal.tabPage3)
                    {
                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);//this is what spams osc
                        OSC.OSCSender.Send(typingbubble);
                    }
                    if (typingBox == true && VoiceWizardWindow.MainFormGlobal.mainTabControl.SelectedTab == VoiceWizardWindow.MainFormGlobal.tabPage3)
                    {
                        var theString = "";

                        theString = VoiceWizardWindow.MainFormGlobal.richTextBox9.Text.ToString();




                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked == false)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, DisplayTextType.TextToText)); //original


                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked == false)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChat(theString, DisplayTextType.TextToText)); //original     
                        }
                    }
                    typingBox = false;

                    typeTimer.Change(2000, 0);
                });
            }
            catch
            {

                //stop errors on close
            }

        }

        private static void doKatRefreshTimerTick()
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked == false && VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked == true)
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                {
                    if (AzureRecognition.AzureTyping == "")
                    {
                        OutputText.outputVRChat(OutputText.lastKatString, DisplayTextType.UpdateText);
                       // outputLog("doing AutoRefresh KAT");
                    }
                }


                katRefreshTimer.Change(2000, 0);
            }
        }

       public static void EmptyTextOutput()
        {
            try
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true)
                {
                    // System.IO.File.WriteAllTextAsync(@"TextOut\OBSText.txt", String.Empty);
                    OutputText.outputTextFile(String.Empty, @"Output\TextOutput\OBSText.txt");
                    OutputText.outputTextFile(String.Empty, @"Output\TextOutput\OBSTextTranslated.txt");
                    OutputText.outputTextFile(String.Empty, @"Output\TextOutput\TranscriptionOnly.txt");
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[OBSText File Error: " + ex.Message + ". Try moving folder location.]", Color.Red);
            }
        }

        public static string ParameterSyncSpacing(string textstring)
        {
            int stringleng = 0;
            foreach (char h in textstring)
            {
                stringleng += 1;
            }

            int sentenceLength = stringleng % 16;

            switch (sentenceLength)
            {
                case 1:
                    textstring += "   ";
                    if (numKATSyncParameters == "8" || numKATSyncParameters == "16")
                    {
                        textstring += "    ";
                    };
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    };
                    break;

                case 2:
                    textstring += "  ";
                    if (numKATSyncParameters == "8" || numKATSyncParameters == "16")
                    {
                        textstring += "    ";
                    }
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    };
                    break;
                case 3:
                    textstring += " ";
                    if (numKATSyncParameters == "8" || numKATSyncParameters == "16")
                    {
                        textstring += "    ";
                    }
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    }
                    break;
                case 4:
                    textstring += "";
                    if (numKATSyncParameters == "8" || numKATSyncParameters == "16")
                    {
                        textstring += "    ";
                    }
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    }
                    break;
                case 5:
                    textstring += "   ";
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    }; break;
                case 6:
                    textstring += "  ";
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    }; break;
                case 7:
                    textstring += " ";
                    if (numKATSyncParameters == "16")
                    {
                        textstring += "        ";
                    }; break;
                case 8: textstring += "        "; break; //16 mode
                case 9: textstring += "       "; break; //16 mode
                case 10: textstring += "      "; break; //16 mode
                case 11: textstring += "     "; break; //16 mode
                case 12: textstring += "    "; break; //16 mode
                case 13: textstring += "   "; break; //16 mode
                case 14: textstring += "  "; break; //16 mode
                case 15: textstring += " "; break; //16 mode
                default:; break;
            }
            return textstring;
        }

        public static float GetLetterID(char c)
        {
            float letter = 0.0f;
            switch (c)
            {
                case ' ': letter = 0; break;
                case '!': letter = 1; break;
                case '\"': letter = 2; break;
                case '#': letter = 3; break;
                case '$': letter = 4; break;
                case '%': letter = 5; break;
                case '&': letter = 6; break;
                case '\'': letter = 7; break;
                case '(': letter = 8; break;
                case ')': letter = 9; break;

                case '*': letter = 10; break;
                case '+': letter = 11; break;
                case ',': letter = 12; break;
                case '-': letter = 13; break;
                case '.': letter = 14; break;
                case '/': letter = 15; break;
                case '0': letter = 16; break;
                case '1': letter = 17; break;
                case '2': letter = 18; break;
                case '3': letter = 19; break;

                case '4': letter = 20; break;
                case '5': letter = 21; break;
                case '6': letter = 22; break;
                case '7': letter = 23; break;
                case '8': letter = 24; break;
                case '9': letter = 25; break;
                case ':': letter = 26; break;
                case ';': letter = 27; break;
                case '<': letter = 28; break;
                case '=': letter = 29; break;

                case '>': letter = 30; break;
                case '?': letter = 31; break;
                case '@': letter = 32; break;
                case 'A': letter = 33; break;
                case 'B': letter = 34; break;
                case 'C': letter = 35; break;
                case 'D': letter = 36; break;
                case 'E': letter = 37; break;
                case 'F': letter = 38; break;
                case 'G': letter = 39; break;

                case 'H': letter = 40; break;
                case 'I': letter = 41; break;
                case 'J': letter = 42; break;
                case 'K': letter = 43; break;
                case 'L': letter = 44; break;
                case 'M': letter = 45; break;
                case 'N': letter = 46; break;
                case 'O': letter = 47; break;
                case 'P': letter = 48; break;
                case 'Q': letter = 49; break;

                case 'R': letter = 50; break;
                case 'S': letter = 51; break;
                case 'T': letter = 52; break;
                case 'U': letter = 53; break;
                case 'V': letter = 54; break;
                case 'W': letter = 55; break;
                case 'X': letter = 56; break;
                case 'Y': letter = 57; break;
                case 'Z': letter = 58; break;
                case '[': letter = 59; break;

                case '\\': letter = 60; break;
                case ']': letter = 61; break;
                case '^': letter = 62; break;
                case '_': letter = 63; break;
                case '`': letter = 64; break;
                case 'a': letter = 65; break;
                case 'b': letter = 66; break;
                case 'c': letter = 67; break;
                case 'd': letter = 68; break;
                case 'e': letter = 69; break;

                case 'f': letter = 70; break;
                case 'g': letter = 71; break;
                case 'h': letter = 72; break;
                case 'i': letter = 73; break;
                case 'j': letter = 74; break;
                case 'k': letter = 75; break;
                case 'l': letter = 76; break;
                case 'm': letter = 77; break;
                case 'n': letter = 78; break;
                case 'o': letter = 79; break;

                case 'p': letter = 80; break;
                case 'q': letter = 81; break;
                case 'r': letter = 82; break;
                case 's': letter = 83; break;
                case 't': letter = 84; break;
                case 'u': letter = 85; break;
                case 'v': letter = 86; break;
                case 'w': letter = 87; break;
                case 'x': letter = 88; break;
                case 'y': letter = 89; break;

                case 'z': letter = 90; break;
                case '{': letter = 91; break;
                case '|': letter = 92; break;
                case '}': letter = 93; break;
                case '~': letter = 94; break;
                case '€': letter = 95; break;

                /// case '非': letter = 96; break;//trying to map
                // case '常': letter = 97; break;//trying to map
                // case '': letter = 98; break;
                //case '': letter = 99; break;

                //case '': letter = 100; break;
                //case '': letter = 101; break;
                //case '': letter = 102; break;
                //case '': letter = 103; break;
                //case '': letter = 104; break;
                //case '': letter = 105; break;
                //case '': letter = 106; break;
                //case '': letter = 107; break;
                //case '': letter = 108; break;
                // case '': letter = 109; break;

                // case '': letter = 110; break;
                // case '': letter = 111; break;
                //case '': letter = 112; break;
                // case '': letter = 113; break;
                // case '': letter = 114; break;
                // case '': letter = 115; break;
                // case '': letter = 116; break;
                //case '': letter = 117; break;
                //case '': letter = 118; break;
                // case '': letter = 119; break;

                // case '': letter = 120; break;
                // case '': letter = 121; break;
                //case '': letter = 122; break;
                //case '': letter = 123; break;
                // case '': letter = 124; break;
                // case '': letter = 125; break;
                // case '': letter = 126; break;
                case 'ぬ': letter = 127; break; //used as heart emoji
                                               //case '': letter = 128; break;
                case 'ふ': letter = 129; break; //used as spotify emoji

                case 'あ': letter = 130; break;//emoji 1
                case 'う': letter = 131; break;//emoji 1
                case 'え': letter = 132; break;//emoji 2
                case 'お': letter = 133; break;//emoji 2
                case 'や': letter = 134; break;//emoji 3
                case 'ゆ': letter = 135; break;//emoji 3
                case 'よ': letter = 136; break;//emoji 4
                case 'わ': letter = 137; break;//emoji 4
                case 'を': letter = 138; break;//emoji 5
                case 'ほ': letter = 139; break;//emoji 5

                case 'へ': letter = 140; break;//emoji 6
                case 'た': letter = 141; break;//emoji 6
                case 'て': letter = 142; break;//emoji 7
                case 'い': letter = 143; break;//emoji 7
                case 'す': letter = 144; break;//emoji 8
                case 'か': letter = 145; break;//emoji 8
                case 'ん': letter = 146; break;//emoji 9
                case 'な': letter = 147; break;//emoji 9
                case 'に': letter = 148; break;//emoji 10
                case 'ら': letter = 149; break;//emoji 10

                case 'せ': letter = 150; break;//11
                case 'ち': letter = 151; break;//11
                case 'と': letter = 152; break;//12
                case 'し': letter = 153; break;//12
                case 'は': letter = 154; break;//13
                case 'き': letter = 155; break;//13
                case 'く': letter = 156; break;//14
                case 'ま': letter = 157; break;//14
                case 'の': letter = 158; break;//15
                case 'り': letter = 159; break;//15

                case 'れ': letter = 160; break;//16
                case 'け': letter = 161; break;//16
                case 'む': letter = 162; break;//17
                case 'つ': letter = 163; break;//17
                case 'さ': letter = 164; break;//18
                case 'そ': letter = 165; break;//18
                case 'ひ': letter = 166; break;//19
                case 'こ': letter = 167; break;//19
                case 'み': letter = 168; break;//20
                case 'も': letter = 169; break;//20

                case 'ね': letter = 170; break;
                case 'る': letter = 171; break;
                case 'め': letter = 172; break;
                case 'ろ': letter = 173; break;
                case '。': letter = 174; break;
                case 'ぶ': letter = 175; break;
                case 'ぷ': letter = 176; break;
                case 'ぼ': letter = 177; break;
                case 'ぽ': letter = 178; break;
                case 'べ': letter = 179; break;//25

                case 'ぺ': letter = 180; break;
                case 'だ': letter = 181; break;
                case 'で': letter = 182; break;
                case 'ず': letter = 183; break;
                case 'が': letter = 184; break;
                case 'ぜ': letter = 185; break;
                case 'ぢ': letter = 186; break;
                case 'ど': letter = 187; break;
                case 'じ': letter = 188; break;
                case 'ば': letter = 189; break;//30

                case 'ぱ': letter = 190; break;
                case 'ぎ': letter = 191; break;
                case 'ぐ': letter = 192; break;
                case 'げ': letter = 193; break;
                case 'づ': letter = 194; break;
                case 'ざ': letter = 195; break;
                case 'ぞ': letter = 196; break;
                case 'び': letter = 197; break;
                case 'ぴ': letter = 198; break;
                case 'ご': letter = 199; break;//35

                case 'ぁ': letter = 200; break;
                case 'ぃ': letter = 201; break;
                case 'ぅ': letter = 202; break;
                case 'ぇ': letter = 203; break;
                case 'ぉ': letter = 204; break;
                case 'ゃ': letter = 205; break;
                case 'ゅ': letter = 206; break;
                case 'ょ': letter = 207; break;
                case 'ヌ': letter = 208; break;
                case 'フ': letter = 209; break;//40

                case 'ア': letter = 210; break;
                case 'ウ': letter = 211; break;
                case 'エ': letter = 212; break;
                case 'オ': letter = 213; break;
                case 'ヤ': letter = 214; break;
                case 'ユ': letter = 215; break;
                case 'ヨ': letter = 216; break;
                case 'ワ': letter = 217; break;
                case 'ヲ': letter = 218; break;
                case 'ホ': letter = 219; break;//45

                case 'ヘ': letter = 220; break;
                case 'タ': letter = 221; break;
                case 'テ': letter = 222; break;
                case 'イ': letter = 223; break;
                case 'ス': letter = 224; break;
                case 'カ': letter = 225; break;
                case 'ン': letter = 226; break;
                case 'ナ': letter = 227; break;
                case 'ニ': letter = 228; break;
                case 'ラ': letter = 229; break;//50

                case 'セ': letter = 230; break;
                case 'チ': letter = 231; break;
                case 'ト': letter = 232; break;
                case 'シ': letter = 233; break;
                case 'ハ': letter = 234; break;
                case 'キ': letter = 235; break;
                case 'ク': letter = 236; break;
                case 'マ': letter = 237; break;
                case 'ノ': letter = 238; break;
                case 'リ': letter = 239; break;//55

                case 'レ': letter = 240; break;
                case 'ケ': letter = 241; break;
                case 'ム': letter = 242; break;
                case 'ツ': letter = 243; break;
                case 'サ': letter = 244; break;
                case 'ソ': letter = 245; break;
                case 'ヒ': letter = 246; break;
                case 'コ': letter = 247; break;
                case 'ミ': letter = 248; break;
                case 'モ': letter = 249; break;//60

                case 'ネ': letter = 250; break;
                case 'ル': letter = 251; break;
                case 'メ': letter = 252; break;
                case 'ロ': letter = 253; break;
                case '〝': letter = 254; break;
                case '°': letter = 255; break;//63


                case '¿': letter = 31; break;

                case 'À': letter = 33; break;
                case 'Á': letter = 33; break;
                case 'Â': letter = 33; break;
                case 'Ã': letter = 33; break;
                case 'Ä': letter = 33; break;
                case 'Å': letter = 33; break;
                case 'Æ': letter = 33; break;

                case 'à': letter = 65; break;
                case 'á': letter = 65; break;
                case 'â': letter = 65; break;
                case 'ã': letter = 65; break;
                case 'ä': letter = 65; break;
                case 'å': letter = 65; break;
                case 'æ': letter = 65; break;

                case 'È': letter = 37; break;
                case 'É': letter = 37; break;
                case 'Ê': letter = 37; break;
                case 'Ë': letter = 37; break;

                case 'è': letter = 69; break;
                case 'é': letter = 69; break;
                case 'ê': letter = 69; break;
                case 'ë': letter = 69; break;


                case 'Ì': letter = 41; break;
                case 'Í': letter = 41; break;
                case 'Î': letter = 41; break;
                case 'Ï': letter = 41; break;

                case 'ì': letter = 73; break;
                case 'í': letter = 73; break;
                case 'î': letter = 73; break;
                case 'ï': letter = 73; break;

                case 'Ñ': letter = 46; break;
                case 'ñ': letter = 78; break;


                case 'Ò': letter = 47; break;
                case 'Ó': letter = 47; break;
                case 'Ô': letter = 47; break;
                case 'Õ': letter = 47; break;
                case 'Ö': letter = 47; break;

                case 'ò': letter = 79; break;
                case 'ó': letter = 79; break;
                case 'ô': letter = 79; break;
                case 'õ': letter = 79; break;
                case 'ö': letter = 79; break;


                default: letter = 31; break;






            }


            if (letter > 127.5)
            {
                letter = letter - 256;

            }
            letter = letter / 127;

            return letter;
        }






    }
}
