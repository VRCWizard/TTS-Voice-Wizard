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

namespace OSCVRCWiz.Resources.Whisper
{
    //MODIFIED FROM Const-me/Whisper/example

    public sealed class TranscribeCallbacks : Callbacks
    {
        readonly CommandLineArgs args;
        readonly eResultFlags resultFlags;


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


        protected override void onNewSegment(Context sender, int countNew)
        {
            try
            {

                TranscribeResult res = sender.results(resultFlags);
                var testing = res.segments.Length;
                int counter = 1;

                int s0 = res.segments.Length - countNew;
                if (s0 == 0)
                    Debug.WriteLine("");
                string text = "";
                string stuff = "";
                for (int i = s0; i < res.segments.Length; i++)
                {
                    sSegment seg = res.segments[i];

                    stuff = seg.text.ToString().Trim();
                    Debug.WriteLine($"segment {s0}: {stuff}");





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


                                OutputText.outputLog("Whisper (FILTERED): " + stuff);
                            }
                        }
                        // continue;
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
