using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;
using Resources;
using Polly.Caching;
//using SharpTalk;
using System.Xml.Linq;
using Amazon.Internal;
using Amazon;
using System.Diagnostics;
using OSCVRCWiz.Text;
using OSCVRCWiz.Settings;
using static OSCVRCWiz.TTS.ElevenLabsTTS;
using System.Windows;
using NAudio.Wave.SampleProviders;


namespace OSCVRCWiz.TTS
{
    public class AmazonPollyTTS
    {
       
        public static async Task PollyTTS(string text, CancellationToken ct = default)
        {
            try
            {
                var AWSaccessKeyId = Settings1.Default.yourAWSKey;
                var AWSsecretKey = Settings1.Default.yourAWSSecret;              
                var AWSRegion = RegionEndpoint.GetBySystemName(Settings1.Default.yourAWSRegion);



                var client = new AmazonPollyClient(AWSaccessKeyId, AWSsecretKey, AWSRegion);


                var response = await PollySynthesizeSpeech(client, text);

            MemoryStream memoryStream = new MemoryStream();
            WriteSpeechToStream(response.AudioStream, memoryStream);




                MemoryStream memoryStream2 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream2);


                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                Mp3FileReader wav = new Mp3FileReader(memoryStream);


                memoryStream2.Flush();
                memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
                Mp3FileReader wav2 = new Mp3FileReader(memoryStream2);



                var AnyOutput = new WaveOut();
                AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                AnyOutput.Init(wav);
                AnyOutput.Play();
                ct.Register(async () => AnyOutput.Stop());
                WaveOut AnyOutput2 = null;

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
                {
                    AnyOutput2 = new WaveOut();
                    AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                    AnyOutput2.Init(wav2);
                    AnyOutput2.Play();
                    ct.Register(async () => AnyOutput2.Stop());
                    while (AnyOutput2.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(2000);
                    }
                }
                while (AnyOutput.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(2000);
                }
                if (AnyOutput.PlaybackState == PlaybackState.Stopped)
                {
                    
                    AnyOutput.Stop();
                    AnyOutput.Dispose();
                    AnyOutput = null;
                    if (AnyOutput2 != null)
                    {
                        AnyOutput2.Stop();
                        AnyOutput2.Dispose();
                        
                        AnyOutput2 = null;
                    }
                    memoryStream.Dispose();
                    memoryStream = null;
                    //  memoryStream2.Dispose();
                    wav.Dispose();
                   
                    wav = null;
                    wav2 = null;
                    client.Dispose();
                    client= null;
                    response.Dispose();
                    response= null;
                    ct = new();

                    Debug.WriteLine("azure dispose successful");
                }



            }
            catch(Exception ex)
            {
               OutputText.outputLog("[Amazon Polly TTS Error: " + ex.Message + "]", Color.Red);
           }
        }

        /// <summary>
        /// Calls the Amazon Polly SynthesizeSpeechAsync method to convert text
        /// to speech.
        /// </summary>
        /// <param name="client">The Amazon Polly client object used to connect
        /// to the Amazon Polly service.</param>
        /// <param name="text">The text to convert to speech.</param>
        /// <returns>A SynthesizeSpeechResponse object that includes an AudioStream
        /// object with the converted text.</returns>
        private static async Task<SynthesizeSpeechResponse> PollySynthesizeSpeech(IAmazonPolly client,string text)
        {
            var tts = new SynthesizeSpeechRequest();


                tts.OutputFormat = OutputFormat.Mp3;
                tts.VoiceId = VoiceId.Joanna;
                
                tts.Engine = Engine.Standard;
      
            tts.TextType = TextType.Ssml;
            
               


              
              int rate = 5;
              int pitch = 5;
              int volume =5;
              string voice = "blank";
              VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
              {

                  rate = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                  pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                  volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                  if (!string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString())) { voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString(); }


              });
            /*   string ratexslow = "<prosody rate=\"x-slow\">"; //1
               string rateslow = "<prosody rate=\"slow\">"; //2
               string ratemedium = "<prosody rate=\"medium\">"; //3
               string ratefast = "<prosody rate=\"fast\">"; //4
               string ratexfast = "<prosody rate=\"x-fast\">"; //5

               string pitchxlow = "<prosody pitch=\"x-low\">"; //1
               string pitchlow = "<prosody pitch=\"low\">"; //2
               string pitchmedium = "<prosody pitch=\"medium\">"; //3
               string pitchhigh = "<prosody pitch=\"high\">"; //4
               string pitchxhigh = "<prosody pitch=\"x-high\">"; //5

               string volumexlow = "<prosody volume=\"x-soft\">"; //1
               string volumelow = "<prosody volume=\"soft\">"; //2
               string volumemedium = "<prosody volume=\"medium\">"; //3
               string volumehigh = "<prosody volume=\"loud\">"; //4
               string volumexhigh = "<prosody volume=\"x-loud\">"; //5*/

            var ratePercent = (int)Math.Floor(((0.5f + rate * 0.1f) - 1) * 100);
            var pitchPercent = (int)Math.Floor(((0.5f + pitch * 0.1f) - 1) * 100);
            var volumePercent = (int)Math.Floor(((0.5f + volume * 0.1f) - 1) * 10);

            string rateString = "<prosody rate=\"" + ratePercent + "%\">"; //1
            string pitchString = "<prosody pitch=\"" + pitchPercent + "%\">"; //1
            string volumeString = "<prosody volume=\"" + volumePercent + "dB\">"; //1

            Debug.WriteLine("rate: " + ratePercent);
            Debug.WriteLine("pitch: " + pitchPercent);

            Debug.WriteLine("volume: " + volumePercent);
            Debug.WriteLine("volume: " + volumeString);


            //  string ssml0 = "<speak>";
            string ssml0 = "<speak version=\"1.0\"";
            ssml0 += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
            ssml0 += " xml:lang=\"en-US\">";


            if (rate != 5)//5 = default /middle of track bar
              {
                  ssml0 += rateString; 
                 

              }
              if (pitch != 5)
              {
                 ssml0 += pitchString; 
                

              }
              if (volume != 5)
              {
                  ssml0 += volumeString;
                 

              }
              ssml0 += text;
              if (rate != 5) { ssml0 += "</prosody>"; }
              if (pitch != 5) { ssml0 += "</prosody>"; }
              if (volume != 5) { ssml0 += "</prosody>"; }

              ssml0 += "</speak>";




            tts.Text = ssml0;








            switch (voice)
            {
                case "Salli": tts.VoiceId = VoiceId.Salli; break;
                case "Kimberly": tts.VoiceId = VoiceId.Kimberly; break;
                case "Kendra": tts.VoiceId = VoiceId.Kendra; break;
                case "Joanna": tts.VoiceId = VoiceId.Joanna; break;
                case "Ivy": tts.VoiceId = VoiceId.Ivy; break;
                case "Ruth ($Neural)": tts.VoiceId = VoiceId.Ruth;
                    tts.Engine = Engine.Neural;
                    break;
                case "Kevin ($Neural)": tts.VoiceId = VoiceId.Kevin;
                    tts.Engine = Engine.Neural;
                    break;
                case "Matthew": tts.VoiceId = VoiceId.Matthew; break;
                case "Justin": tts.VoiceId = VoiceId.Justin; break;
                case "Joey": tts.VoiceId = VoiceId.Joey; break;
                case "Stephen ($Neural)": tts.VoiceId = VoiceId.Stephen;
                    tts.Engine = Engine.Neural;break;

                case "Nicole": tts.VoiceId = VoiceId.Nicole; break;
                case "Olivia ($Neural)":
                    tts.VoiceId = VoiceId.Olivia;
                    tts.Engine = Engine.Neural;  break;
                case "Russell": tts.VoiceId = VoiceId.Russell; break;

                case "Amy": tts.VoiceId = VoiceId.Amy; break;
                case "Emma": tts.VoiceId = VoiceId.Emma; break;
                case "Brian": tts.VoiceId = VoiceId.Brian; break;
                case "Arthur ($Neural)":
                    tts.VoiceId = VoiceId.Arthur;
                    tts.Engine = Engine.Neural; break;


                case "Aditi": tts.VoiceId = VoiceId.Aditi; break;

            //    case "Reveena": tts.VoiceId = VoiceId.rev; break;

                case "Kajal ($Neural)":
                    tts.VoiceId = VoiceId.Kajal;
                    tts.Engine = Engine.Neural; break;

                case "Aria ($Neural)":
                    tts.VoiceId = VoiceId.Aria;
                    tts.Engine = Engine.Neural; break;

                case "Ayanda ($Neural)":
                    tts.VoiceId = VoiceId.Ayanda;
                    tts.Engine = Engine.Neural; break;



                case "Geraint": tts.VoiceId = VoiceId.Geraint; break;



                default: tts.VoiceId = VoiceId.Joanna; break;

            }

            var synthesizeSpeechResponse = await client.SynthesizeSpeechAsync(tts);

            return synthesizeSpeechResponse;

            /*
             *  comboBox2.Items.Add("Nicole");
                    comboBox2.Items.Add("Olivia ($Neural)");
                    comboBox2.Items.Add("Russell");

                    comboBox2.Items.Add("Amy");
                    comboBox2.Items.Add("Emma");
                    comboBox2.Items.Add("Brian");
                    comboBox2.Items.Add("Arthur ($Neural)");

                    comboBox2.Items.Add("Aditi");
                    comboBox2.Items.Add("Reveena");
                    comboBox2.Items.Add("Kajal ($Neural)");

                    comboBox2.Items.Add("Aria ($Neural)");

                    comboBox2.Items.Add("Ayanda ($Neural)");

                    comboBox2.Items.Add("Geraint");*/
        }

        /// <summary>
        /// Writes the AudioStream returned from the call to
        /// SynthesizeSpeechAsync to a file in MP3 format.
        /// </summary>
        /// <param name="audioStream">The AudioStream returned from the
        /// call to the SynthesizeSpeechAsync method.</param>
        /// <param name="outputFileName">The full path to the file in which to
        /// save the audio stream.</param>
        public static void WriteSpeechToStream(Stream audioStream, MemoryStream output)
        {
            
            byte[] buffer = new byte[2 * 1024];
            int readBytes;

            while ((readBytes = audioStream.Read(buffer, 0, 2 * 1024)) > 0)
            {
                output.Write(buffer, 0, readBytes);
            }

            // Flushes the buffer to avoid losing the last second or so of
            // the synthesized text.
            output.Flush();
            
        }

    }
}
