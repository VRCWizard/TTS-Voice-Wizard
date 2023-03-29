using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DeepL.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NAudio.Wave;
using Newtonsoft.Json;
using Octokit;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;
using Resources;
//using Whisper.net;
//using Whisper.net.Ggml;
using Windows.UI.Core;
using static System.Net.Mime.MediaTypeNames;
//using Whisper.net;

//using Whisper.net.Ggml;
//using Whisper.net.Wave;

//doesnt work :(

namespace OSCVRCWiz.Speech_Recognition
{

    /*  public class WhisperRecognitionV3
      {
          public static async Task RunDetection()
          {
              //  var modelName = "ggml-base.bin";
              var modelName = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.ToString();
              var modelType = GgmlType.Base;
           //   if (!File.Exists(modelName)) //auto install model
           //   {
            //      Console.WriteLine($"Downloading Model {modelName}");
           //       using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(modelType);
           //       using var fileWriter = File.OpenWrite(modelName);
          //        await modelStream.CopyToAsync(fileWriter);
           //   }

              await StreamDetection(modelName);
             // FullDetectionFromInputDevice(modelName);
          }


          public static void FullDetectionFromInputDevice(string modelName)
          {

              WaveInEvent waveIn = new()
              {
                  DeviceNumber = AudioDevices.getCurrentInputDevice(), // indicates which microphone to use
                  WaveFormat = new WaveFormat(rate: 16000, bits: 16, channels: 1), // must be supported by the microphone

              };

              void OnNewSegment(SegmentData segment)
              {
                  Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(segment.Text, "WhisperV2"));
                  // Debug.WriteLine(segment.Text);
                  //storage.TranscriptList.Add(new Transcript() { SegmentStart = segment.Start, SegmentEnd = segment.End, Text = segment.Text });
              }
              using var factory = WhisperFactory.FromPath(modelName);
              var builder = factory.CreateBuilder()
                  .WithLanguage("auto")
                  .WithSegmentEventHandler(OnNewSegment);

              var processor = builder.Build();

              Console.WriteLine("Listening for speech");
              MemoryStream memstream = new MemoryStream();



              waveIn.DataAvailable += WaveInDataAvailable;
              waveIn.StartRecording();

              void WaveInDataAvailable(object sender, WaveInEventArgs e)
              {

                  OutputText.outputLog("[data received]");
                  //writer.Write(a.Buffer, 0, a.BytesRecorded);
                  memstream.Write(e.Buffer, 0, e.BytesRecorded);
                  if (memstream.Length > waveIn.WaveFormat.AverageBytesPerSecond * 5)
                  {
                      using var reader = new BinaryReader(memstream);
                      memstream.Seek(0, SeekOrigin.Begin);

                      var samplesCount = memstream.Length / 2; // 16bit
                      float[] samples = new float[samplesCount];
                      Debug.WriteLine($"Start process with {samplesCount} samples");
                      for (var i = 0; i < samplesCount; i++)
                      {
                          samples[i] = reader.ReadInt16() / 32768.0f; // 16bit
                      }
                      //  var tempproc = builder.Build();   // FIXME : keep one processor to avoid loosing context
                      try
                      {
                          processor.Process(samples);
                      memstream = new MemoryStream();
                      }
                      catch (Exception ex) { Debug.WriteLine(ex.Message); }
                  }


              }

              // Console.WriteLine("Press any key to stop listening");
              //  Console.ReadLine();
          }

          private static Task StreamDetection(string modelName)
          {

              return Task.Run(() =>
              {
                  OutputText.outputLog("[Whisper Listening]");
                  void OnNewSegment(SegmentData segment)
                  {
                      Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(segment.Text, "WhisperV2"));
                     // Debug.WriteLine(segment.Text);
                      //storage.TranscriptList.Add(new Transcript() { SegmentStart = segment.Start, SegmentEnd = segment.End, Text = segment.Text });
                  }

                  using var factory = WhisperFactory.FromPath(modelName);
                  var builder = factory.CreateBuilder()
                      .WithLanguage("auto")
                      .WithSegmentEventHandler(OnNewSegment);

                  using var processor = builder.Build();

                  using var capture = new WasapiLoopbackCapture();
                  capture.WaveFormat = new WaveFormat(16000, 16, 1);

                  //using var ringbufferstream = new RingBufferStream(1024 * 1024, false);
                  //using var writer = new WaveFileWriter(ringbufferstream, capture.WaveFormat);
                  MemoryStream memstream = new MemoryStream();
                  {
                      capture.DataAvailable += (s, a) =>
                      {
                         // OutputText.outputLog("[data received]");
                          //writer.Write(a.Buffer, 0, a.BytesRecorded);
                          memstream.Write(a.Buffer, 0, a.BytesRecorded);
                          if (memstream.Length > capture.WaveFormat.AverageBytesPerSecond * 5)
                          {
                              using var reader = new BinaryReader(memstream);
                              memstream.Flush();
                              memstream.Seek(0, SeekOrigin.Begin);

                              var samplesCount = memstream.Length / 2; // 16bit
                              float[] samples = new float[samplesCount];
                              Debug.WriteLine($"Start process with {samplesCount} samples");
                              for (var i = 0; i < samplesCount; i++)
                              {
                                  samples[i] = reader.ReadInt16() / 32768.0f; // 16bit
                              }
                              try
                              {
                              using var tempproc = builder.Build();   // FIXME : keep one processor to avoid loosing context
                              tempproc.Process(samples);
                              memstream = new MemoryStream();
                              }
                              catch (Exception ex) { Debug.WriteLine(ex.Message); }
                          }
                      };
                      capture.RecordingStopped += (s, a) =>
                      {
                          capture.Dispose();
                      };
                      capture.StartRecording();
                      while (capture.CaptureState != NAudio.CoreAudioApi.CaptureState.Stopped)
                      {
                          Thread.Sleep(500);
                      }
                  }
              });

          }
    } */


}

