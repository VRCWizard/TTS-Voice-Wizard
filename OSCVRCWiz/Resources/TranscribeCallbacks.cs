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
using OSCVRCWiz.Text;
using System.Text.RegularExpressions;

namespace OSCVRCWiz.Resources
{
    //MODIFIED FROM Const-me/Whisper/ example

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

        // Terminal color map. 10 colors grouped in ranges [0.0, 0.1, ..., 0.9]
        // Lowest is red, middle is yellow, highest is green.
        readonly string[] k_colors = new string[]
        {
            "\x1B[38;5;196m", "\x1B[38;5;202m", "\x1B[38;5;208m", "\x1B[38;5;214m", "\x1B[38;5;220m",
            "\x1B[38;5;226m", "\x1B[38;5;190m", "\x1B[38;5;154m", "\x1B[38;5;118m", "\x1B[38;5;82m"
        };

        int colorIndex(in sToken tok)
        {
            float p = tok.probability;
            float p3 = p * p * p;
            int col = (int)(p3 * k_colors.Length);
            col = Math.Clamp(col, 0, k_colors.Length - 1);
            return col;
        }

      //  public static string printTime(TimeSpan ts) =>
     //       ts.ToString("hh':'mm':'ss'.'fff", CultureInfo.InvariantCulture);
     //   public static string printTimeWithComma(TimeSpan ts) =>
      //      ts.ToString("hh':'mm':'ss','fff", CultureInfo.InvariantCulture);

        protected override void onNewSegment(Context sender, int countNew)
        {
            try
            {

                TranscribeResult res = sender.results(resultFlags);
                ReadOnlySpan<sToken> tokens = res.tokens;
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
                    //   Debug.WriteLine("countNew : " + countNew);
                    //   Debug.WriteLine("s0 : " + s0);
                    //   Debug.WriteLine("testing : " + testing);

                    //counter++;

                    //Char ch = '[';
                    //i am also want to ignore stuff starting with * ???? ill have to see after further testing/ when i release it.
                    //also since whisper can recognize laughter, i could add that as a feature.
                    //the better way may be to whitelist certain things because there seem to be too many variations to blacklist... 




                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked == true)
                    {



                        if (!stuff.StartsWith('[') && stuff != "Audio" && !stuff.EndsWith(']') && !stuff.StartsWith('(') && !stuff.EndsWith(')') && !stuff.StartsWith('*') && !stuff.EndsWith('*'))
                        {

                            //   WhisperRecognition.WhisperString += text + " ";
                            //  VoiceWizardWindow.whisperTimer.Change(250, 0);
                            //  Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Whisper"));
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
                        if (!stuff.StartsWith('[') || !stuff.EndsWith(']') || !stuff.StartsWith('(') || !stuff.EndsWith(')') || !stuff.StartsWith('*') || !stuff.EndsWith('*'))
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
                    VoiceWizardWindow.whisperTimer.Change(250, 0);
                    // Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Whisper"));


                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("[Whisper NewSegment Error: " + ex.Message.ToString());
                OutputText.outputLog("[Whisper NewSegment Error: " + ex.Message.ToString() + "]", Color.Red);
                
            }


        }
    }
}
