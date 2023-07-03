using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
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
                FileToTTS(e.FullPath);
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


        public static void FileToTTS(string path)
        {
            try
            {
               
                using (FileStream stream = new FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string contents = reader.ReadToEnd();
                        TTSMessageQueue.QueueMessage(contents.Replace("\n", " ").Replace("\r", " "), "Text File Reader");
                    }
                }
            }
            catch (Exception ex)
            {

                VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked = false;
                OutputText.outputLog("[Text File Reader Error: This error occured while attempting to read the text file: " + ex.Message + "]", Color.Red);
            }
        }
    }
}
