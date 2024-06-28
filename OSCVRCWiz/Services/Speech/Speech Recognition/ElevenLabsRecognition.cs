
using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using Swan.Formatters;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http.Json;
using WebRtcVadSharp;
using static System.Windows.Forms.DataFormats;

namespace OSCVRCWiz.Speech_Recognition
{
    public class ElevenLabsRecognition
    {

        private static bool ElevenEnabled = false;

        public static CancellationTokenSource elevenCt = new();
        private static HttpClient client = new HttpClient();//reusing client save so much time!!! around 100ms
        static string lastKey = "";



        public static async Task doRecognition(bool calibrating)
        {
            try
            {
                elevenCt = new();
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
                        OutputText.outputLog("[ElevenLabs Listening]");
                        DoSpeech.speechToTextOnSound();

                        using (MemoryStream audioStream = await VoiceWizardProRecognition.RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false, elevenCt))
                        {

                            if (audioStream != null)
                            {
                                Stream stream = await CallElevenLabsRecognition(audioStream);
                                MemoryStream memoryStream = new MemoryStream();
                                AmazonPollyTTS.WriteSpeechToStream(stream, memoryStream);

                                TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                {
                                    TTSMessageQueued.text = "";

                                    TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString();
                                    TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.Text.ToString();
                                    TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.Text.ToString();
                                    TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                                    TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                                    TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                                    TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString();
                                    TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString();
                                    TTSMessageQueued.STTMode = "ElevenLabs STS";
                                });

                                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, elevenCt.Token, true, AudioFormat.Mp3);
                                memoryStream.Dispose();
                                // string transcribedText = await Task.Run(() => CallElevenLabsRecognition(audioStream));
                                //  TTSMessageQueue.QueueMessage(transcribedText, "ElevenLabs");

                            }
                            else
                            {
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                {
                                    OutputText.outputLog("[ElevenLabs: No voice detected]");
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked == true)
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
                          if (!ElevenEnabled)
                          {
                              OutputText.outputLog("[ElevenLabs Listening (Continuous)]");
                              DoSpeech.speechToTextOnSound();
                              ElevenEnabled = true;


                              while (ElevenEnabled)
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
                                      OutputText.outputLog($"[ElevenLabs Settings Error: {ex.Message}", Color.Red);
                                  }
                                    using (MemoryStream audioStream = await VoiceWizardProRecognition.RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false,elevenCt))
                                    {


                                      if (ElevenEnabled)
                                      {
                                     // MemoryStream audioStream = await RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, false);
                                      if (audioStream != null)
                                          {

                                            Stream stream = await CallElevenLabsRecognition(audioStream);
                                            MemoryStream memoryStream = new MemoryStream();
                                            AmazonPollyTTS.WriteSpeechToStream(stream, memoryStream);

                                            TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                            {
                                                TTSMessageQueued.text = "";

                                                TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString();
                                                TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.Text.ToString();
                                                TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.Text.ToString();
                                                TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                                                TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                                                TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                                                TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString();
                                                TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString();
                                                TTSMessageQueued.STTMode = "ElevenLabs STS";
                                            });

                                            AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, elevenCt.Token, true, AudioFormat.Mp3);
                                            memoryStream.Dispose();

                                        }
                                          else
                                          {
                                          //audioStream.Dispose();
                                          if (VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked)
                                              {
                                                  OutputText.outputLog("[ElevenLabs: No voice detected]");
                                                  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true )
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
                              OutputText.outputLog("[ElevenLabs Stopped Listening]");
                              DoSpeech.speechToTextOffSound();
                              ElevenEnabled = false;
                              elevenCt.Cancel();
                          }
                      } 
                }
                
                else
                {
                    OutputText.outputLog("[DeepGram Calibrating]");
                    OutputText.outputLog("[Deepgram is being calibrated to ignore your background noise, do not speak. Speaking will ruin the calibration]",Color.Orange);
                    DoSpeech.speechToTextOnSound();

                    using (MemoryStream audioStream = await VoiceWizardProRecognition.RecordAudio(minDuration, maxDuration, howQuiet, silenceScale, minValidDuration, VADMode, true, elevenCt))
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
        public static void SaveMemoryStreamToFile(MemoryStream memoryStream, string filePath)
        {
            // Ensure memory stream is positioned at the beginning
            memoryStream.Position = 0;

            // Create a new file stream to write the audio data
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                // Copy the contents of the memory stream to the file stream
                memoryStream.CopyTo(fileStream);
            }
        }

        public static async Task<Stream> CallElevenLabsRecognition(MemoryStream memoryStream)
        {
            // Stopwatch stopwatch = new Stopwatch();
            string apiUrl = "https://api.elevenlabs.io/v1/speech-to-speech/{voice_id}";
            //  string audioFilePath = "path/to/audio/file.wav"; // Path to your audio file
            OutputText.outputLog("ElevenLabs API");

            var voiceID = "";

            int optimize = 0;
            int stabilities = 0;
            int similarities = 0;
            int styles = 0;
            bool boost = true;
            string modelID = "eleven_english_sts_v2";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {

                voiceID = Services.Speech.TextToSpeech.TTSEngines.ElevenLabsTTS.voiceDict.FirstOrDefault(x => x.Value == VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString()).Key;
                optimize = int.Parse(VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedItem.ToString());
                stabilities = VoiceWizardWindow.MainFormGlobal.trackBarStability.Value;
                similarities = VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value;
                styles = VoiceWizardWindow.MainFormGlobal.trackBarStyleExaggeration.Value;
                boost = VoiceWizardWindow.MainFormGlobal.rjToggleSpeakerBoost.Checked;

                Debug.WriteLine(optimize);
                Debug.WriteLine(stabilities);
                Debug.WriteLine(similarities);
                Debug.WriteLine(modelID);


            });
         


         //   var bytes = memoryStream.ToArray();
          //  var audioBase64 = ByteArrayContent(bytes);
           // OutputText.outputLog("Eleven Bytes: "+audioBase64);

            var similarityFloat = similarities * 0.01f;
            var stabilityFloat = stabilities * 0.01f;
            var styleFloat = styles * 0.01f;

            //  var url = $"https://api.elevenlabs.io/v1/speech-to-speech/{voiceID}?optimize_streaming_latency={optimize}";
            var url = $"https://api.elevenlabs.io/v1/speech-to-speech/{voiceID}";
            var apiKey = Settings1.Default.elevenLabsAPIKey;

            /* var request = new HttpRequestMessage(HttpMethod.Post, url);

             request.Content = JsonContent.Create(new
             {
                 audio = memoryStream,
                /* model_id = modelID,
                 voice_settings = new
                 {
                     stability = stabilityFloat,
                     similarity_boost = similarityFloat,
                     style = styleFloat,
                     use_speaker_boost = boost
                 }*/
            // });*/

            // var rs = new RawSourceWaveStream(memoryStream, new WaveFormat(16000, 16, 1));

            
            
                using (var audio = new MemoryStream())
                {
                    WaveFormat waveFormat = new WaveFormat(16000, 16, 1);
                    using (WaveFileWriter waveFileWriter = new WaveFileWriter(audio, waveFormat))
                    {
                        // Assuming 'memoryStream' contains raw audio data
                        memoryStream.Position = 0; // Ensure memoryStream is at the beginning
                        byte[] buffer = new byte[1024]; // Example buffer size
                        int bytesRead;

                        // Read from 'memoryStream' and write to 'waveFileWriter' in chunks
                        while ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            waveFileWriter.Write(buffer, 0, bytesRead);
                        }

                        audio.Position = 0;// never forget to set position of stream back to begining, took me way to long to figure that out
                        if (apiKey != lastKey)
                        {
                            OutputText.outputLog("[ElevenLabs Debug: Key Changed]");
                            client = new HttpClient();
                            client.DefaultRequestHeaders.Add("xi-api-key", apiKey);//can only call once per client, or make new client each time
                       
                        }
                        lastKey = apiKey;

                        MultipartFormDataContent formData = new MultipartFormDataContent();
                        formData.Add(new StreamContent(audio), "audio", "PogAudio.wav");

                        HttpResponseMessage response = await client.PostAsync(url, formData);


                        //request.Headers.Add("xi-api-key", apiKey);
                        //request.Headers.Add("Accept", "audio/mpeg");

                        //  HttpResponseMessage response = await client.SendAsync(request);

                        if (!response.IsSuccessStatusCode)
                        {

                            string json = response.Content.ReadAsStringAsync().Result.ToString();


                            OutputText.outputLog("[ElevenLabs TTS Error: " + response.StatusCode + ": " + json + "]", Color.Red);





                        }

                        OutputText.outputLog("Finished API call");



                        return await response.Content.ReadAsStreamAsync();
                    }
                }
            



        }




    }

}
