using System;
using System.Collections.Generic;
using System.Text;
using System.Media;
using Resources;
using OSCVRCWiz.Text;
using NAudio.Wave;
using System.Diagnostics;
using NAudio.Wave.SampleProviders;
using VarispeedDemo.SoundTouch;


namespace OSCVRCWiz.TTS
{
    public class FonixTalkTTS
    {
        private static readonly HttpClient client = new HttpClient();
        // private static bool Moonbase = false;
        public static Process pro;



        public static void FonixTTS(string text, CancellationToken ct = default)
        {
            Process[] pname = Process.GetProcessesByName("MoonbaseVoices");
            if (pname.Length == 0)
            {
                ProcessStartInfo psi = new ProcessStartInfo("MoonbaseVoices.exe");
                psi.WindowStyle = ProcessWindowStyle.Minimized;
                try
                {
                    pro = Process.Start(psi);
                }
                catch (Exception ex){

                    OutputText.outputLog("[Moonbase TTS Startup Error: " + ex.Message + "]", Color.Red);

                    OutputText.outputLog("[Something prevented the program from running the MoonbaseVoice.exe console app included inside the TTSVoiceWizard download folder. Make sure that 'MoonbaseVoices.exe' exists in the download folder and has not been renamed. Try running TTS Voice Wizard as Administrator]", Color.DarkOrange);

                 

                }

                

                
               // Moonbase = true;
               // Task.Delay(2000).Wait();
            }
            try
            {

                Task<string> stringTask = MoonBase(text);
                string audio = stringTask.Result;
                var audiobytes = Convert.FromBase64String(audio);
                MemoryStream memoryStream = new MemoryStream(audiobytes);



                MemoryStream memoryStream2 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream2);


                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
               var  wav = new RawSourceWaveStream(memoryStream, new WaveFormat(11000, 16, 1));

                memoryStream2.Flush();
                memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
               var wav2 = new RawSourceWaveStream(memoryStream2, new WaveFormat(11000, 16, 1));







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
             //   synthesizerLite.Dispose();
                memoryStream = null;
            //    synthesizerLite = null;

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
                OutputText.outputLog("[Moonbase TTS *AUDIO* Error: " + ex.Message + "]", Color.Red);

                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    OutputText.outputLog("[Looks like you may have 2 audio devices with the same name which causes an error in TTS Voice Wizard. To fix this go to Control Panel > Sound > right click on one of the devices > properties > rename the device.]", Color.DarkOrange);
                }
            }

         }
        public static async Task<string> MoonBase(string textIn)
        {
            try
            {
                string name = "";
               VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    name = VoiceWizardWindow.MainFormGlobal.comboBox2.Text.ToString();
                   
                });
           


                var url = $"http://localhost:54027/audio?voice={name}&text={textIn}";
              

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                  // request.Content = JsonContent.Create(new { voice = name, text = textIn });
              //  request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "voice", name }, { "text", textIn } });



                HttpResponseMessage response = await client.SendAsync(request);

                //   System.Diagnostics.Debug.WriteLine("Fonix:" + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine("Moonbase: " + response.StatusCode);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Moonbase: " + response.StatusCode + " " + await response.Content.ReadAsStringAsync());
                    return "";
                }




              //  return await response.Content.ReadAsStringAsync();
            }
             catch (Exception ex)
            {
                OutputText.outputLog("[Moonbase Error: "+ex.Message+"]", Color.Red);
                OutputText.outputLog("[Make sure you have downloaded the Moonbase Voice dependencies: https://github.com/VRCWizard/TTS-Voice-Wizard/wiki/Moonbase-TTS ]", Color.DarkOrange);
                //  MessageBox.Show("FonixTalk Error: "+ex.Message);
                return "";
                
            }



}




    }
}
