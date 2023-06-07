using Deepgram;
using Deepgram.Keys;
using Deepgram.Transcription;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using Octokit;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Windows.Devices.Radios;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.DataFormats;

namespace OSCVRCWiz.Speech_Recognition
{
    public class VoiceWizardProRecognition
    {

       

        public static async Task doRecognition(string apiKey, int minDuration, int maxDuration, int howQuiet, string language)
        {
            try
            {
                OutputText.outputLog("[DeepGram Listening]");

                using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet))
                {
                    // await PerformTranscription(language, audioStream, howQuiet);
                    string transcribedText = await Task.Run(() => CallVoiceProAPIAsync(apiKey, audioStream, language, howQuiet));
                    // OutputText.outputLog(text);


                    TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        TTSMessageQueued.text = transcribedText;
                        TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                        TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                        TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBox5.Text.ToString();
                        TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBox1.Text.ToString();
                        TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                        TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                        TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                        TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBox4.Text.ToString();
                        TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString();
                        TTSMessageQueued.STTMode = "DeepGram (Pro Only)";
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
            catch(Exception ex)
            {
                OutputText.outputLog("[DeepGram Stopped Listening]");
                OutputText.outputLog("[VoiceWizardPro Reognition Error: " + ex.Message + "]", Color.Red);
            }
        }


        private static async Task<string> CallVoiceProAPIAsync(string apiKey, MemoryStream memoryStream,string lang, int silenceThreshold)
        {




             var url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?" +

          //  var url = $"http://localhost:54029/api/transcribe?"+
            $"apiKey={apiKey}" +
               $"&fromLang={lang}" +
               $"&silenceThreshold={silenceThreshold}";
  


            var request = new HttpRequestMessage(HttpMethod.Post, url);


            HttpClient client = new HttpClient();

            // Set the content of the request as the MemoryStream
            request.Content = new StreamContent(memoryStream);

            HttpResponseMessage response = await client.SendAsync(request);

      

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                OutputText.outputLog("VoiceWizardPro API Error: " + response.StatusCode + ": " + errorMessage, Color.Red);
                return null;
            }


            var json = response.Content.ReadAsStringAsync().Result.ToString();
            System.Diagnostics.Debug.WriteLine("VoiceWizardPro API: " + json);

            var dataHere = JObject.Parse(json).SelectToken("text").ToString();

            //  var TTSModeUsed = JObject.Parse(json).SelectToken("TTSMethod").ToString();

            var hoursUsed = JObject.Parse(json).SelectToken("hoursUsed").ToString();
            var hoursLimit = JObject.Parse(json).SelectToken("hoursLimit").ToString();
         


            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {




                VoiceWizardWindow.MainFormGlobal.SpeechHoursUsed.Text = $"Speech Hours Used: {Math.Round(decimal.Parse(hoursUsed), 4)}/{hoursLimit}";
               
              
                Settings1.Default.hoursUsed = VoiceWizardWindow.MainFormGlobal.SpeechHoursUsed.Text.ToString();
                Settings1.Default.Save();
            });


            string transcribedText = JObject.Parse(json).SelectToken("text").ToString();
      
         
            return transcribedText;



        }


        public static async Task<MemoryStream> RecordAudio(int minDuration, int maxDuration,int howQuiet)
        {
            // Create a MemoryStream to store the recorded audio
            MemoryStream outputStream = new MemoryStream();

            // Set up the audio recording settings
            WaveFormat waveFormat = new WaveFormat(16000, 16, 1); // Sample rate: 16000 Hz, Bit depth: 16-bit, Channels: Mono
            WaveInEvent waveSource = new WaveInEvent();
            waveSource.WaveFormat = waveFormat;
            waveSource.DeviceNumber = AudioDevices.getCurrentInputDevice();

            // Set up the silence detection
            int silenceThreshold = 1000; // bigger number = less sensative to noise // if set to 2000 it will end earlier because it can't hear me talking
                                        // , small number, even the smallest sound will reset silence, if set to 500 it will never end because it still hears sound

            int silenceDuration = 50000; //make a big number 
            int recordingDuration = 25000; // Adjust this value to define the maximum recording duration (in milliseconds) //ACCURATE NOW
            int initialDelay = 5000; // Adjust this value to define the initial delay before considering audio as silence (in milliseconds)

            silenceThreshold = howQuiet;
            recordingDuration = maxDuration * 1000;
            initialDelay = minDuration * 1000;



            bool isSilence = false;
            int silenceCounter = 0;
            int recordingCounter = 0;

            // Event handler for audio data received
            waveSource.DataAvailable += (sender, e) =>
            {
                // Check for silence
                if (e.BytesRecorded > 0)
                {
                    byte[] buffer = e.Buffer;
                    int bytesRecorded = e.BytesRecorded;

                    // Analyze audio data for silence
                    for (int i = 0; i < bytesRecorded; i += 2)
                    {
                        short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                        if (sample < silenceThreshold && sample > -silenceThreshold)
                        {

                            silenceCounter += waveFormat.BlockAlign;
                        }

                        else
                        {

                            silenceCounter = 0;
                        }
                    }

                    // Write audio data to the output stream
                    outputStream.Write(buffer, 0, bytesRecorded);
                    Debug.WriteLine(silenceCounter); //very helpful for debugging silence
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        int checker = silenceCounter;
                        if(checker >=50000)
                        {
                            checker = 50000;
                        }
                     /*   if(checker > 30000)
                        {
                            VoiceWizardWindow.MainFormGlobal.pot1.ForeColor = Color.Red;
                        }
                        if (checker < 10000)
                        {
                            VoiceWizardWindow.MainFormGlobal.pot1.ForeColor = Color.Green;
                        }
                        if (checker > 10000 && checker < 30000)
                        {
                            VoiceWizardWindow.MainFormGlobal.pot1.ForeColor = Color.Orange;
                        } */
                        VoiceWizardWindow.MainFormGlobal.pot1.Value =checker;
                        
                    });

                    // Update the recording counter
                    recordingCounter += (bytesRecorded / waveFormat.BlockAlign) * 1000 / waveFormat.SampleRate;
                  //  Debug.WriteLine(recordingCounter.ToString());

                    // Check if silence duration or recording duration exceeded
                    if (recordingCounter >= recordingDuration)
                    {
                        Debug.WriteLine("Max Record Length");
                        waveSource.StopRecording();
                    }
                    
                   else  if(silenceCounter >= silenceDuration && recordingCounter > initialDelay)
                    {
                        Debug.WriteLine("Ended by silence");
                        waveSource.StopRecording();
                    } 

                }
            };

            // Create a TaskCompletionSource to signal the completion of the recording
            TaskCompletionSource<bool> recordingTaskCompletionSource = new TaskCompletionSource<bool>();

            // Event handler for recording stopped
            waveSource.RecordingStopped += (sender, e) =>
            {
                waveSource.Dispose();
                outputStream.Position = 0;
                recordingTaskCompletionSource.SetResult(true);
            };

            // Start recording
            waveSource.StartRecording();

            // Wait for the recording to complete
            await recordingTaskCompletionSource.Task;

            return outputStream;
        }

      
    }

}
