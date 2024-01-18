using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using Whisper;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using OSCVRCWiz.Speech_Recognition;
using System.Text.RegularExpressions;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Resources.StartUp.StartUp;

namespace OSCVRCWiz.Resources.Whisper
{
    //MODIFIED FROM Const-me/Whisper/example

    public sealed class TranscribeCallbacks : Callbacks
    {
        readonly CommandLineArgs args;
        readonly eResultFlags resultFlags;
        public static bool MuteWhisper = false;


        public TranscribeCallbacks(CommandLineArgs args)
        {
            try
            {
                this.args = args;
                resultFlags = args.resultFlags();
                //Console.OutputEncoding = System.Text.Encoding.UTF8;
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper TranscribeCallbacks Error: " + ex.Message.ToString());
                OutputText.outputLog("[Whisper TranscribeCallbackst Error: " + ex.Message.ToString() + "]", Color.Red);

            }
        }

        public bool WhisperStartedListening  = false;
        protected override bool onEncoderBegin(Context sender)
        {
            WhisperStartedListening = true;
            return true;
        }
        public static string printTime(TimeSpan ts) =>
            ts.ToString("hh':'mm':'ss'.'fff", CultureInfo.InvariantCulture);


        private bool IsSegmentInVoiceActivation(TimeSpan segmentStart, TimeSpan segmentEnd, double offset)
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
            {
                OutputText.outputLog("[Whisper Transcription Time: " + segmentStart + " -> " + segmentEnd + "]");
            }
            foreach (var activationTime in WhisperRecognition.voiceActivationTimes)
            {
             


                TimeSpan start = activationTime.Item1.Subtract(TimeSpan.FromSeconds(offset));
                TimeSpan end = activationTime.Item2.Add(TimeSpan.FromSeconds(offset));



                // Check for overlap
                if (segmentStart < end && segmentEnd > start)
                {
                    return true; // Overlapping
                }
            }

            return false; // No overlap
        }


        protected override void onNewSegment(Context sender, int countNew)
        {
            try
            {
                if(MuteWhisper==true)
                { return; }

                TranscribeResult res = sender.results(resultFlags);
                var testing = res.segments.Length;
                int counter = 1;

                int s0 = res.segments.Length - countNew;

                string text = "";
                string stuff = "";
                for (int i = s0; i < res.segments.Length; i++)
                {
                    sSegment seg = res.segments[i];
                   

                    stuff = seg.text.ToString().Trim();
                    Debug.WriteLine($"segment {s0}: {stuff}");


                    if (VoiceWizardWindow.MainFormGlobal.rjToggleVAD.Checked)
                    {
                        TimeSpan segmentStart = seg.time.begin + WhisperRecognition.WhisperStartTime;
                        TimeSpan segmentEnd = seg.time.end + WhisperRecognition.WhisperStartTime;
                        Debug.WriteLine(printTime(seg.time.begin + WhisperRecognition.WhisperStartTime) + " - " + printTime(seg.time.end + WhisperRecognition.WhisperStartTime));
                        if (IsSegmentInVoiceActivation(seg.time.begin + WhisperRecognition.WhisperStartTime, seg.time.end + WhisperRecognition.WhisperStartTime, Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxWhisperVADOffset.Text.ToString(), CultureInfo.InvariantCulture)) || WhisperRecognition.isVoiceDetected)
                        {
                           // OutputText.outputLog($"segment approved by VAD", Color.Blue);
                        }
                        else
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked)
                            {
                                OutputText.outputLog($"VAD (FILTERED): {stuff}", Color.Orange);
                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                            {
                                var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);
                                OSC.OSCSender.Send(typingbubble);

                            }
                            return;
                        }
                    }



                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked == true)
                    {

                        if (!stuff.StartsWith('[') && stuff != "Audio" && !stuff.EndsWith(']') && !stuff.StartsWith('(') && !stuff.EndsWith(')') && !stuff.StartsWith('*') && !stuff.EndsWith('*'))
                        {
                            text += stuff;
                        }
                        else
                        {
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked == true)
                            {


                                OutputText.outputLog("Whisper (FILTERED): " + stuff, Color.Orange);
                            }
                        }
                    }
                    else
                    {
                        if (stuff.StartsWith('[') || stuff.EndsWith(']') || stuff.StartsWith('(') || stuff.EndsWith(')') || stuff.StartsWith('*') || stuff.EndsWith('*'))
                        {
                            string pattern = @"[*()\[\]]"; // Match any of these characters
                            stuff = Regex.Replace(stuff, pattern, "", RegexOptions.IgnoreCase);
                            stuff = "'" + stuff + "'";
                        }

                        text += stuff;

                    }

                }
                if (text != "")
                {
                    WhisperRecognition.WhisperString += text + " ";
                    WhisperRecognition.whisperTimer.Change(250, 0);

                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper NewSegment Error: " + ex.Message.ToString());
                OutputText.outputLog("[Whisper NewSegment Error: " + ex.Message.ToString() + "]", Color.Red);

            }


        }
    }
}
