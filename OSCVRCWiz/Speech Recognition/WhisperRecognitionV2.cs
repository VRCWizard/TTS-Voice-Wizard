using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DeepL.Model;
using NAudio.Wave;
using Newtonsoft.Json;
using Octokit;
using Resources;
//using Whisper.net;

//using Whisper.net.Ggml;
//using Whisper.net.Wave;

//doesnt work :(

namespace OSCVRCWiz.Speech_Recognition
{

 /*   public class WhisperRecognitionV2
    {
        const int audioSampleLengthS = 1;
        const int audioSampleLengthMs = audioSampleLengthS * 1000;
        const int totalBufferLength = 30 / audioSampleLengthS;
        static List<float[]> slidingBuffer = new(totalBufferLength + 1);

        static string ModelName = "ggml-base.bin";
       static  GgmlType ModelType = GgmlType.Base;
        static string Command = "translate";
        static string Language = "auto";
        // static WaveInEvent waveIn;

      // static  MemoryStream stream = new MemoryStream();




        public static  async Task Demo()
        {
            // Console.OutputEncoding = Encoding.UTF8;
            ModelName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text.ToString();

            
            switch (Command)
            {
                case "translate":
                    FullDetection();
                    break;
                default:
                    Debug.WriteLine("Unknown command");
                    break;
            }
        }


        public static void FullDetection()
        {
            Debug.WriteLine(ModelName);
            var builder = WhisperProcessorBuilder.Create()
                .WithFileModel(ModelName)
                .WithSegmentEventHandler(OnNewSegment);
                //.WithLanguage(Language);

       //     if (Command == "translate")
         //   {
         //       builder.WithTranslate();
         //   }

            var processor = builder.Build();

            static void OnNewSegment(object sender, OnSegmentEventArgs e)
            {
                // Debug.WriteLine($"New Segment: {e.Start} ==> {e.End} : {e.Segment}");
                Debug.WriteLine($"New Segment:{e.Segment}");
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



           var  stream = new MemoryStream();
            ushort channelCount = 1; // Mono
            ushort bitDepth = 16; // 16-bit
            int sampleRate = 16000; // 48 kHz
            bool isFloatingPoint = false; // Not floating point
            int totalSampleCount = (int)(stream.Length / (bitDepth / 8));
            Debug.WriteLine(totalSampleCount);
            WriteWavHeader(stream, isFloatingPoint, channelCount, bitDepth, sampleRate, totalSampleCount);

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



                          // create a MemoryStream to hold the float array data

                         // var stream = new MemoryStream();
                          ushort channelCount = 1; // Mono
                          ushort bitDepth = 16; // 16-bit
                          int sampleRate = 16000; // 48 kHz
                          bool isFloatingPoint = false; // Not floating point
                          int totalSampleCount = (int)(stream.Length / (bitDepth / 8));
                          Debug.WriteLine(totalSampleCount);
                          WriteWavHeader(stream, isFloatingPoint, channelCount, bitDepth, sampleRate, totalSampleCount);
                          // create a BinaryWriter to write the float data to the stream
                          BinaryWriter writer = new BinaryWriter(stream);

                          // write the number of float arrays to the stream
                          writer.Write(slidingBuffer.Count);

                          // write each float array to the stream
                          foreach (float[] floatArray in slidingBuffer)
                          {
                              // write the length of the float array to the stream
                              writer.Write(floatArray.Length);

                              // write each float value to the stream
                              foreach (float value in floatArray)
                              {
                                  writer.Write(value);
                              }
                          }


                // set the position of the MemoryStream to the beginning
                //   stream.Seek(0, SeekOrigin.Begin);

                // pass the MemoryStream to the processor's Process method
                // var rs = new RawSourceWaveStream(stream, new WaveFormat(16000, 16, 1));
                //   stream.Flush();
                //      stream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                // Set the wave format parameters


                // Calculate the total number of samples

                //   stream.Flush();
               
              
            //    stream.Write(e.Buffer, 0, e.BytesRecorded);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                try
                    {
                        //System.AccessViolationException when not silence
                        processor.Process(stream);
                        Debug.WriteLine("audio worked");
                    }
                    catch(Exception ex) { Debug.WriteLine(ex.Message); }
                    

              
                 //   processor.Process(stream);

             }

            
           }

           // Console.WriteLine("Press any key to stop listening");
          //  Console.ReadLine();
        }

        static private void WriteWavHeader(MemoryStream stream, bool isFloatingPoint, ushort channelCount, ushort bitDepth, int sampleRate, int totalSampleCount)
        {
            stream.Position = 0;

            // RIFF header.
            // Chunk ID.
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);

            // Chunk size.
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * totalSampleCount) + 36), 0, 4);

            // Format.
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);



            // Sub-chunk 1.
            // Sub-chunk 1 ID.
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);

            // Sub-chunk 1 size.
            stream.Write(BitConverter.GetBytes(16), 0, 4);

            // Audio format (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);

            // Channels.
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);

            // Sample rate.
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);

            // Bytes rate.
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);

            // Block align.
            stream.Write(BitConverter.GetBytes((ushort)channelCount * (bitDepth / 8)), 0, 2);

            // Bits per sample.
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);



            // Sub-chunk 2.
            // Sub-chunk 2 ID.
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);

            // Sub-chunk 2 size.
            stream.Write(BitConverter.GetBytes((bitDepth / 8) * totalSampleCount), 0, 4);
        }
        static bool IsSilence(float amplitude, sbyte threshold)=> GetDecibelsFromAmplitude(amplitude) < threshold;

        static double GetDecibelsFromAmplitude(float amplitude)  => 20 * Math.Log10(Math.Abs(amplitude));

      



    }*/


}

