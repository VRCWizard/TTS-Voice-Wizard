using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
//using CSCore;
//using CSCore.MediaFoundation;
using System.Media;
using System.Net;
//using CSCore.CoreAudioAPI;
using OSCVRCWiz.Text;
using Resources;
using NAudio.Wave;
using Microsoft.VisualBasic;
using Windows.Storage.Streams;
using System.Collections;
using Microsoft.Extensions.Primitives;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;
using System.Diagnostics;

namespace OSCVRCWiz.TTS
{
    public class TikTokTTS
    {
       // public static WaveOut TikTokOutput=null;

        public static async Task TikTokTextAsSpeech(string text, CancellationToken ct = default)
        {

            // if ("tiktokvoice.mp3" == null)
            //   throw new NullReferenceException("Output path is null");
            //text = FormatInputText(text);
            string voice = "";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                voice = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
            });
            System.Diagnostics.Debug.WriteLine("tiktok speech ran " + voice);
            byte[] result = null;
            try
            {
                result = await CallTikTokAPIAsync(text, voice);
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TikTok TTS Error: " + ex.Message + "]", Color.Red);
             

            }


            try
            {
                //  File.WriteAllBytes("TikTokTTS.mp3", result);          
                //  Task.Run(() => PlayAudioHelper());

                MemoryStream memoryStream = new MemoryStream(result);

             




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


                var volume = 5;
                int pitch = 5;
                int rate = 5;
                var volumeFloat = 1f;
                var pitchFloat = 1f;
                var rateFloat = 1f;
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                    pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                    rate = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                });

                volumeFloat = 0.5f + volume * 0.1f;
                pitchFloat = 0.5f + pitch * 0.1f;
                rateFloat = 0.5f + rate * 0.1f;

                bool useTempo = false;
                if (rate != 5)//if rate is changed will use only rate, else use pitch which also changes rate.
                {
                    useTempo = true;
                    pitchFloat = rateFloat;
                }


                var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
                VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(useTempo, false));
                speedControl.PlaybackRate = pitchFloat;
                var AnyOutput = new WaveOut();
                AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
                AnyOutput.Init(speedControl);
                AnyOutput.Play();
                ct.Register(async () => AnyOutput.Stop());

                WaveOut AnyOutput2 = null;
                VarispeedSampleProvider speedControl_2 = null;
                WaveChannel32 wave32_2 = null;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
                {
                    wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                    wave32_2.PadWithZeroes = false;
                    speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                    speedControl_2.PlaybackRate = pitchFloat;//output 2
                    AnyOutput2 = new WaveOut();
                    AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                    AnyOutput2.Init(speedControl_2);
                    AnyOutput2.Play();
                    ct.Register(async () => AnyOutput2.Stop());
                }
              
                


                Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds * 2);// VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
                Thread.Sleep(500);



                AnyOutput.Stop();
                AnyOutput.Dispose();
                AnyOutput = null;
                speedControl.Dispose();
                speedControl = null;
                wave32.Dispose();
                wave32 = null;
                wav.Dispose();
                wav = null;
                memoryStream.Dispose();
               // synthesizerLite.Dispose();
                memoryStream = null;
               // synthesizerLite = null;

                if (AnyOutput2 != null)
                {
                    AnyOutput2.Stop();
                    AnyOutput2.Dispose();
                    AnyOutput2 = null;
                    speedControl_2.Dispose();
                    speedControl_2 = null;
                    wave32_2.Dispose();
                    wave32_2 = null;
                    wav2.Dispose();
                    wav2 = null;
                }
                ct = new();


                Debug.WriteLine("disposed of all");



            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TikTok TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }

            }
            //System.Diagnostics.Debug.WriteLine("tiktok speech ran"+result.ToString());
        }

        public static async Task<byte[]> CallTikTokAPIAsync(string text, string voice)
        {


            var url = "https://tiktok-tts.weilnet.workers.dev/api/generation";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            var data = "{\"text\":\"" + text + "\",\"voice\":\"" + voice + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            string audioInBase64 = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var dataHere = JObject.Parse(result.ToString()).SelectToken("data").ToString();
                audioInBase64 = dataHere.ToString();

                System.Diagnostics.Debug.WriteLine(result);
            }

            System.Diagnostics.Debug.WriteLine(httpResponse.StatusCode);



            System.Diagnostics.Debug.WriteLine(audioInBase64);
            return Convert.FromBase64String(audioInBase64);

        }



        
    }



}
