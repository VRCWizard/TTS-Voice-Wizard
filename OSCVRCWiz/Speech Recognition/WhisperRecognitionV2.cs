using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DeepL.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NAudio.Wave;
using Newtonsoft.Json;
using Octokit;
using Resources;
//using Whisper.net;

//using Whisper.net.Ggml;
//using Whisper.net.Wave;



namespace OSCVRCWiz.Speech_Recognition
{

  /*  public class WhisperRecognitionV2
    {
        const int audioSampleLengthS = 1;
        const int audioSampleLengthMs = audioSampleLengthS * 1000;
        const int totalBufferLength = 30 / audioSampleLengthS;
        static List<float[]> slidingBuffer = new(totalBufferLength + 1);

     //   static string ModelName = "ggml-base.bin";
     //  static  GgmlType ModelType = GgmlType.Base;

        // static WaveInEvent waveIn;

      // static  MemoryStream stream = new MemoryStream();




        public static  async Task Demo()
        {
            // Console.OutputEncoding = Encoding.UTF8;
            ModelName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text.ToString();

            FullDetection();
     
        }

    

        public static void FullDetection()
        {
            Debug.WriteLine(ModelName);
            var factory = WhisperFactory.FromPath(ModelName);
            var builder = factory.CreateBuilder()
                   .WithLanguage("auto")
                   .WithSegmentEventHandler(OnNewSegment);

            var processor = builder.Build();

            static void OnNewSegment(SegmentData segment)
            {
                // Debug.WriteLine($"New Segment: {e.Start} ==> {e.End} : {e.Segment}");
                Debug.WriteLine($"New Segment:{segment.Text}");
            }



                FullDetectionFromInputDevice(processor);


        }



        public static void FullDetectionFromInputDevice(WhisperProcessor processor)
        {

            WaveInEvent waveIn = new()
            {
                DeviceNumber = AudioDevices.getCurrentInputDevice(), // indicates which microphone to use
                WaveFormat = new WaveFormat(rate: 16000, bits: 16, channels: 1), // must be supported by the microphone
                BufferMilliseconds = audioSampleLengthMs
            };


            

            waveIn.DataAvailable += WaveInDataAvailable;
            waveIn.StartRecording();
            Console.WriteLine("Listening for speech");

            void WaveInDataAvailable(object sender, WaveInEventArgs e)
            {
                Debug.WriteLine("audio found");
                      var values = new short[e.Buffer.Length / 2];
                      Buffer.BlockCopy(e.Buffer, 0, values, 0, e.Buffer.Length);
                      var samples = values.Select(x => x / (short.MaxValue + 1f)).ToArray();

                      var silenceCount = samples.Count(x => IsSilence(x, -40));

                      if (silenceCount < values.Length - values.Length / 12)
                      {
                          slidingBuffer.Add(samples);

                          if (slidingBuffer.Count > totalBufferLength)
                          {
                              slidingBuffer.RemoveAt(0);
                          }
                      
                try
                    {
                        //System.AccessViolationException when not silence
                        processor.Process(slidingBuffer.SelectMany(x => x).ToArray());
                        Debug.WriteLine("audio worked");
                    }
                    catch(Exception ex) { Debug.WriteLine(ex.Message); }
                    

              
                 //   processor.Process(stream);

             }

            
           }
            

            // Console.WriteLine("Press any key to stop listening");
            //  Console.ReadLine();
        }

       
        static bool IsSilence(float amplitude, sbyte threshold)=> GetDecibelsFromAmplitude(amplitude) < threshold;

        static double GetDecibelsFromAmplitude(float amplitude)  => 20 * Math.Log10(Math.Abs(amplitude));

      



    }*/


}

