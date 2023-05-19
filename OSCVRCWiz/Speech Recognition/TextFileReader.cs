using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.System.RemoteSystems;
using static System.Net.Mime.MediaTypeNames;

namespace OSCVRCWiz.Speech_Recognition
{
    public class TextFileReader
    {
        private static FileSystemWatcher watcher;
        public static void ReadFromFile()
        {
           try
            {
                string path = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text.ToString();

                // Create a new FileSystemWatcher and set its properties
                watcher = new FileSystemWatcher();
                watcher.Path = Path.GetDirectoryName(path);
                watcher.Filter = Path.GetFileName(path);
                watcher.NotifyFilter = NotifyFilters.LastWrite;

                // Add event handlers for the Changed and Error events
                watcher.Changed += OnFileChanged;
                watcher.Error += OnError;

                // Start monitoring the file
                watcher.EnableRaisingEvents = true;
                OutputText.outputLog("[Text File Reader Enabled]");
            }
            catch (Exception ex){

               VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked= false;
               OutputText.outputLog("[Text File Reader Error: This error occured while attempting to read the text file: " + ex.Message + "]", Color.Red);
            }

        }
        static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Handle file change event
           // Debug.WriteLine($"File {e.FullPath} was modified.");
            using (FileStream stream = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string contents = reader.ReadToEnd();
                    //  Debug.WriteLine(contents);
                    TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        TTSMessageQueued.text = contents.Replace("\n", " ").Replace("\r", " ");
                        TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                        TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                        TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text.ToString();
                        TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString();
                        TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                        TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                        TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                        TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.Text.ToString();
                        TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                        TTSMessageQueued.STTMode = "Text File Reader";
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
                }
            }

        }

        static void OnError(object sender, ErrorEventArgs e)
        {
            // Handle error event
            OutputText.outputLog($"Text File Reader Error occurred: {e.GetException().Message}");
        }
        public static void StopWatcher()
        {
            // Stop the watcher and dispose of it
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            OutputText.outputLog("[Text File Reader Disabled]");
        }
    }
}
