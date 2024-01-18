
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using System.Diagnostics;
using System.Globalization;
using WebRtcVadSharp;

namespace OSCVRCWiz.Speech_Recognition
{
    public class VoiceWizardProRecognition
    {

        private static bool DeepGramEnabled = false;

        private static WebRtcVad vad;
        private static FrameLength frameLength = FrameLength.Is30ms;
        private static int frameSize;
        public static CancellationTokenSource deepgramCt = new();

        public static async Task doRecognition(string apiKey,bool calibrating)
        {
            try
            {
                deepgramCt = new();
                int minDuration = 2;
                int maxDuration = 10;
                int howQuiet = 1000;
                string language = "en";
                int silenceScale = 30000;
                double minValidDuration = 0.5;
                OperatingMode VADMode = OperatingMode.HighQuality;

                //  DoSpeech.speechToTextOnSound();

                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    minDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.minimumAudio.Text);
                    maxDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.maximumAudio.Text);
                    howQuiet = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text);
                    language = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                    silenceScale = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text);
                    minValidDuration = Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text.ToString(), CultureInfo.InvariantCulture);
                    VADMode = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;

                });


                if (!calibrating)
                {
                    if (!VoiceWizardWindow.MainFormGlobal.rjToggleDeepGramContinuous.Checked)
                    {
                        OutputText.outputLog("[DeepGram Listening]");
                        DoSpeech.speechToTextOnSound();

                        using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false))
                        {

                            if (audioStream != null)
                            {

                                string transcribedText = await Task.Run(() => CallVoiceProAPIAsync(apiKey, audioStream, language, howQuiet));
                                TTSMessageQueue.QueueMessage(transcribedText, "DeepGram (Pro Only)");

                            }
                            else
                            {
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                {
                                    OutputText.outputLog("[DeepGram: No voice detected]");
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                                    {
                                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);
                                        OSC.OSCSender.Send(typingbubble);

                                    }
                                }

                            }
                            DoSpeech.speechToTextButtonOff();
                        }
                    }
                    else
                    {
                        if (!DeepGramEnabled)
                        {
                            OutputText.outputLog("[DeepGram Listening (Continuous)]");
                            DoSpeech.speechToTextOnSound();
                            DeepGramEnabled = true;


                            while (DeepGramEnabled)
                            {
                                try
                                {
                                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                    {
                                        minDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.minimumAudio.Text);
                                        maxDuration = Int32.Parse(VoiceWizardWindow.MainFormGlobal.maximumAudio.Text);
                                        howQuiet = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text);
                                        language = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();
                                        silenceScale = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text);
                                        minValidDuration = Convert.ToDouble(VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text.ToString(), CultureInfo.InvariantCulture);
                                        VADMode = (OperatingMode)VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;

                                    });
                                }
                                catch (Exception ex)
                                {
                                    OutputText.outputLog($"[Deepgram Settings Error: {ex.Message}", Color.Red);
                                }
                                using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false))
                                {
                                    if (DeepGramEnabled)
                                    {
                                        if (audioStream != null)
                                        {

                                            string transcribedText = await Task.Run(() => CallVoiceProAPIAsync(apiKey, audioStream, language, howQuiet));
                                            TTSMessageQueue.QueueMessage(transcribedText, "DeepGram (Pro Only)");

                                        }
                                        else
                                        {
                                            if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                            {
                                                OutputText.outputLog("[DeepGram: No voice detected]");
                                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                                                {
                                                    var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", false);
                                                    OSC.OSCSender.Send(typingbubble);

                                                }
                                            }

                                        }
                                    }
                                    // DoSpeech.speechToTextButtonOff();
                                }
                            }
                        }
                        else
                        {
                            // must allow canceling of recognition
                            OutputText.outputLog("[DeepGram Stopped Listening]");
                            DoSpeech.speechToTextOffSound();
                            DeepGramEnabled = false;
                            deepgramCt.Cancel();
                        }
                    }
                }
                else
                {
                    OutputText.outputLog("[DeepGram Calibrating]");
                    OutputText.outputLog("[Deepgram is being calibrated to ignore your background noise, do not speak. Speaking will ruin the calibration]",Color.Orange);
                    DoSpeech.speechToTextOnSound();

                    using (MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, true))
                    {
          
                        OutputText.outputLog("[DeepGram Calibration Complete]");
                        OutputText.outputLog("[You may now activate Deepgram recognition]",Color.Green);
                        DoSpeech.speechToTextButtonOff();
                    }

                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[DeepGram Stopped Listening]");


                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;

                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }
                OutputText.outputLog("[VoiceWizardPro Reognition Error: " + errorMsg + "]", Color.Red);

                DoSpeech.speechToTextButtonOff();
            }
        }

        private static async Task<string> CallVoiceProAPIAsync(string apiKey, MemoryStream memoryStream, string lang, int silenceThreshold)
        {


            var branch = "eastus";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                branch = VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.Text.ToString();
            });

            var url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?";
            switch (branch)
            {
                case "eastus": url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?"; break;
                case "dev": url = $"https://ttsvoicewizard-playground.herokuapp.com/api/transcribe?"; break;
                case "local": url = $"http://localhost:54029/api/transcribe?"; break;
                default: break;
            }

            url +=

            // var url = $"https://ttsvoicewizard.herokuapp.com/api/transcribe?" +

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

            // var dataHere = JObject.Parse(json).SelectToken("text").ToString();

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
            string duration = JObject.Parse(json).SelectToken("duration").ToString();


            if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
            {
                OutputText.outputLog($"Audio Duration: {duration}");
            }

            var roundedDuration = JObject.Parse(json).SelectToken("roundedDuration");
            if (roundedDuration != null)
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                {
                    OutputText.outputLog($"Audio Rounded Duration: {roundedDuration.ToString()}");
                }
            }

            return transcribedText;



        }







        public static async Task<MemoryStream> RecordAudio(int minDuration, int maxDuration, int howQuiet, int silenceDuration, double minValidDuration, OperatingMode VADMode,bool calibration)
        {
            // Create a MemoryStream to store the recorded audio
            MemoryStream outputStream = new MemoryStream();

            // Set up the audio recording settings
            WaveFormat waveFormat = new WaveFormat(16000, 16, 1); // Sample rate: 16000 Hz, Bit depth: 16-bit, Channels: Mono
            WaveInEvent waveSource = new WaveInEvent();
            waveSource.WaveFormat = waveFormat;
            waveSource.DeviceNumber = AudioDevices.getCurrentInputDevice();

            //vad
            bool isVoiceDetected = false;
            bool validAudioClip = false;
            TimeSpan startTime = DateTime.MinValue.TimeOfDay;
            TimeSpan endTime = DateTime.MinValue.TimeOfDay;
            vad = new WebRtcVad()
            {
                OperatingMode = VADMode,
                FrameLength = frameLength,
                SampleRate = SampleRate.Is16kHz,
            };
            frameSize = (int)vad.SampleRate / 1000 * 2 * (int)frameLength;

            // Set up the silence detection
            int silenceThreshold = 1000; // bigger number = less sensative to noise // if set to 2000 it will end earlier because it can't hear me talking
                                         // , small number, even the smallest sound will reset silence, if set to 500 it will never end because it still hears sound

            // int silenceDuration = silenceScale; //make a big number //WAS 50000
            int recordingDuration = 25000; // Adjust this value to define the maximum recording duration (in milliseconds) //ACCURATE NOW
            int initialDelay = 5000; // Adjust this value to define the initial delay before considering audio as silence (in milliseconds)

            silenceThreshold = howQuiet;
            recordingDuration = maxDuration * 1000;
            initialDelay = minDuration * 1000;



            bool isSilence = false;
            int silenceCounter = 0;
            int recordingCounter = 0;
            int soundVolume = 0;
            int calibrationMax = 0;




            // Event handler for audio data received
            waveSource.DataAvailable += (sender, e) =>
            {
                if (deepgramCt.Token.IsCancellationRequested)
                {
                    validAudioClip = false;
                    waveSource.StopRecording();
                }





                var bufferVAD = e.Buffer.Take(frameSize).ToArray();

                if (vad.HasSpeech(bufferVAD))
                {
                    if (!isVoiceDetected)
                    {
                        // Voice has just started
                        startTime = DateTime.Now.TimeOfDay;
                        isVoiceDetected = true;

                        //VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text += "🎙️";
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.Green;
                        });
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                        {
                            OutputText.outputLog("VAD Start Time: " + startTime);
                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {
                            var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                            OSC.OSCSender.Send(typingbubble);
                        }
                    }
                    // Update the end time while voice is detected
                    endTime = DateTime.Now.TimeOfDay;
                }
                else
                {
                    if (isVoiceDetected)
                    {
                        // Voice has stopped
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            VoiceWizardWindow.MainFormGlobal.labelVADIndicator.ForeColor = Color.White;
                        });
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                        {
                            OutputText.outputLog("VAD End Time: " + endTime);
                        }

                        isVoiceDetected = false;

                        if ((endTime - startTime).TotalSeconds >= minValidDuration)
                        {
                            validAudioClip = true;
                        }

                        startTime = DateTime.MinValue.TimeOfDay;
                        endTime = DateTime.MinValue.TimeOfDay;
                    }
                }


                

                // Check for silence
                if (e.BytesRecorded > 0)
                {
                    byte[] buffer = e.Buffer;
                    int bytesRecorded = e.BytesRecorded;

                    // Analyze audio data for silence
                    for (int i = 0; i < bytesRecorded; i += 2)
                    {
                       
                       

                        short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                          soundVolume = Math.Abs((int)sample);//has to be int

                        if (calibration && (soundVolume > calibrationMax))
                        {
                            calibrationMax = soundVolume;
                        }
 
                        // int absSample = ab(sample);

                        //  if (sample < silenceThreshold && sample > -silenceThreshold)
                        if (soundVolume < silenceThreshold)
                        {
                            
                            silenceCounter += waveFormat.BlockAlign;
                        }

                        else
                        {

                            silenceCounter = 0;
                        }
                    }
                    //rms = Math.Sqrt(rms / (bytesRecorded / 2)); // Calculate RMS
                  //  double dbValue = 20 * Math.Log10(rms);

                    //Debug.WriteLine($"dB: {testValue}");

                 //  OutputText.outputLog(AudioDevices.currentInputDevice);
                   /* var device = AudioDevices.GetDeviceById(AudioDevices.currentInputDevice);
                    if (device != null)
                    {
                        device.AudioEndpointVolume.Mute = false;
                        OutputText.outputLog(device.AudioMeterInformation.MasterPeakValue.ToString());
                    }*/
                   //use to show volume in audio tab

                    //// device.AudioEndpointVolume.Mute = false;
                    //richTextBox1.Text = device.AudioMeterInformation.MasterPeakValue.ToString();

                    // Write audio data to the output stream
                    outputStream.Write(buffer, 0, bytesRecorded);
                  //  Debug.WriteLine(silenceCounter); //very helpful for debugging silence
                  //  Debug.WriteLine(noiseCounter);




                    if (!VoiceWizardWindow.MainFormGlobal.IsDisposed)
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                       {
                      
                        int checker = silenceCounter;
                        if (checker > silenceDuration)
                        {
                            checker = silenceDuration;
                        }
                        if (soundVolume > 2000) {soundVolume = 2000;}
                        if (soundVolume < 0){soundVolume = 0;}


                           if (calibration) 
                         {
                               VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text = (calibrationMax+100).ToString();
                          }

                        VoiceWizardWindow.MainFormGlobal.progressBar1.Value = soundVolume;
                        VoiceWizardWindow.MainFormGlobal.pot1.Value = checker;
                        VoiceWizardWindow.MainFormGlobal.pot1.Maximum = silenceDuration;
                       });
                    }
                    else
                    {
                        return;
                    }


                    // Update the recording counter
                    recordingCounter += (bytesRecorded / waveFormat.BlockAlign) * 1000 / waveFormat.SampleRate;


                    // Check if silence duration or recording duration exceeded
                    if(calibration && (recordingCounter>=3000))
                    {
                        waveSource.StopRecording();
                    }
                    if (recordingCounter >= recordingDuration)
                    {
                        Debug.WriteLine("Max Record Length");
                        waveSource.StopRecording();
                    }

                    else if (silenceCounter >= silenceDuration && recordingCounter > initialDelay)
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

            if(calibration)
            {
                //VoiceWizardWindow.
                return null;
            }

            if (!isVoiceDetected && !validAudioClip)
            {
                //OutputText.outputLog("invalid clip");
                return null;
            }

            return outputStream;
        }



    }

}
