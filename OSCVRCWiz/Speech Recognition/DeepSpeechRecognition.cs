using System;
using System.Collections.Generic;
using System.Text;
//using DeepSpeechClient;
//using DeepSpeechClient.Interfaces;
//using DeepSpeechClient.Models;
//using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using NAudio;

namespace OSCVRCWiz
{
    
    
    public class DeepSpeechRecognition
    {/*

        /// <summary>
        /// Get the value of an argurment.
        /// </summary>
        /// <param name="args">Argument list.</param>
        /// <param name="option">Key of the argument.</param>
        /// <returns>Value of the argument.</returns>
        static string GetArgument(IEnumerable<string> args, string option)=> args.SkipWhile(i => i != option).Skip(1).Take(1).FirstOrDefault();

      

        static string MetadataToString(CandidateTranscript transcript)
        {
            var nl = Environment.NewLine;
            string retval =
             Environment.NewLine + $"Recognized text: {string.Join("", transcript?.Tokens?.Select(x => x.Text))} {nl}"
             + $"Confidence: {transcript?.Confidence} {nl}"
             + $"Item count: {transcript?.Tokens?.Length} {nl}"
             + string.Join(nl, transcript?.Tokens?.Select(x => $"Timestep : {x.Timestep} TimeOffset: {x.StartTime} Char: {x.Text}"));
            return retval;
        }

        public static void runDeepSpeech()
        {
            //string[] args;
            string model = "deepspeech-0.9.3-models.pbmm";
          //  string model = "deepspeech-0.9.3-models.tflite";
            string scorer = "deepspeech-0.9.3-models.scorer";
            string audio = "Test0001.wav";
           // bool extended = false;
            // if (args.Length > 0)
            // {
            //    model = GetArgument(args, "--model");
            //    scorer = GetArgument(args, "--scorer");
            //    audio = GetArgument(args, "--audio");
            //    extended = !string.IsNullOrWhiteSpace(GetArgument(args, "--extended"));
            // }
            //Task.Delay(5000).Wait();// testing if file has enough time to close
            Stopwatch stopwatch = new Stopwatch();
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading model...");
                stopwatch.Start();
                // sphinx-doc: csharp_ref_model_start
                using (IDeepSpeech sttClient = new DeepSpeech(model))
                {
                    // sphinx-doc: csharp_ref_model_stop
                    stopwatch.Stop();

                    System.Diagnostics.Debug.WriteLine($"Model loaded - {stopwatch.Elapsed.Milliseconds} ms");
                    stopwatch.Reset();
                    if (scorer != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Loading scorer...");
                        sttClient.EnableExternalScorer(scorer);
                        System.Diagnostics.Debug.WriteLine("Loading scorer...2");
                    }
                    System.Diagnostics.Debug.WriteLine("Loading scorer...3");
                  //  string audioFile = audio;
                    System.Diagnostics.Debug.WriteLine("Loading scorer...4");
                    var waveBuffer = new WaveBuffer(File.ReadAllBytes(audio));
                    System.Diagnostics.Debug.WriteLine("Loading scorer...5");
                    using (var waveInfo = new WaveFileReader(audio))
                    {
                        System.Diagnostics.Debug.WriteLine("Running inference....");

                        stopwatch.Start();

                        string speechResult;
                        speechResult = sttClient.SpeechToText(waveBuffer.ShortBuffer, Convert.ToUInt32(waveBuffer.MaxSize / 2));
                        stopwatch.Stop();

                        System.Diagnostics.Debug.WriteLine($"Audio duration: {waveInfo.TotalTime.ToString()}");
                        System.Diagnostics.Debug.WriteLine($"Inference took: {stopwatch.Elapsed.ToString()}");
                        System.Diagnostics.Debug.WriteLine("Recognized text: " + speechResult);
                    }
                    waveBuffer.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
      }
     */

    }
}
