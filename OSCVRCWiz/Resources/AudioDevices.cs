
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using OSCVRCWiz;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media.Media3D;
using VarispeedDemo.SoundTouch;
//using NAudio.CoreAudioApi;




namespace Resources
{
    public class AudioDevices
    {
        public static int audioOutputIndex = -1;
        public static List<string> comboIn = new List<string>();
        public static List<string> comboOut = new List<string>();
        public static List<string> micIDs = new List<string>();
        public static List<string> speakerIDs = new List<string>();
        public static string currentInputDevice = "";
        public static string currentOutputDevice = "";
        public static string currentInputDeviceName = "Default";
        public static string currentOutputDeviceName = "Default";
        // public static int currentOutputDeviceLite = 0;

        public static string currentOutputDevice2nd = "";
        public static string currentOutputDeviceName2nd = "Default";




        public static void NAudioSetupInputDevices()
        {
            VoiceWizardWindow.MainFormGlobal.comboBoxInput.Items.Clear();
            comboIn.Clear();
            micIDs.Clear();


            comboIn.Add("Default");
            micIDs.Add("Default");
           
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Capture, NAudio.CoreAudioApi.DeviceState.Active))
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);
                comboIn.Add(endpoint.FriendlyName);
                micIDs.Add(endpoint.ID);

            }
          

            foreach (var i in comboIn)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxInput.Items.Add(i);
            }

            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxInput.SelectedItem = Settings1.Default.MicName;
            }
            catch
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxInput.SelectedItem = "Default";
            }
        }
        public static void NAudioSetupOutputDevices()
        {
            VoiceWizardWindow.MainFormGlobal.comboBoxOutput.Items.Clear();
            VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.Items.Clear();//forgor this :p
            comboOut.Clear();
            speakerIDs.Clear();


            comboOut.Add("Default");
            speakerIDs.Add("Default");
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.DeviceState.Active))
            {
                System.Diagnostics.Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);

                comboOut.Add(endpoint.FriendlyName);
                speakerIDs.Add(endpoint.ID);
            }
            foreach (var i in comboOut)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.Items.Add(i);
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.Items.Add(i);
            }

  
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem = Settings1.Default.SpeakerName;
            }
            catch
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem = "Default";
            }
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem = Settings1.Default.SpeakerName2;
            }
            catch
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem = "Default";
            }



        }
        public static int getCurrentInputDevice() {

       /*     // Setting to Correct Input Device
            int waveInDevices = WaveIn.DeviceCount;
            //InputDevicesDict.Clear();
           Dictionary<string, int> DevicesDict = new Dictionary<string, int>();
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                DevicesDict.Add(deviceInfo.ProductName, waveInDevice);
            }
           
            foreach (var kvp in DevicesDict)
            {
                if (AudioDevices.currentInputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                   
                }
            }
            return 0;*/






            try
            {
                int waveDevices = WaveIn.DeviceCount;
                List<Tuple<string, int>> devicesList = new List<Tuple<string, int>>();

                for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
                {
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveDevice);
                    devicesList.Add(new Tuple<string, int>(deviceInfo.ProductName, waveDevice));
                }

               // List<int> matchingDevices = new List<int>();

                foreach (var device in devicesList)
                {
                    if (AudioDevices.currentInputDeviceName.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        //  matchingDevices.Add(device.Item2);
                        return device.Item2;
                    }
                }

               /* if (matchingDevices.Count > 0)
                {
                    // Handle multiple matching devices, if needed
                    // For example, select the first matching device
                    OutputText.outputLog("[ Input device with the same name as another audio device selected, selecting first instance ]", Color.DarkOrange);
                    return matchingDevices[0];
                }*/

                // Handle the case when no matching device is found

                // ...
            }
            catch (Exception ex)
            {

                // Handle the exception
                // ...
            }

            return 0;

        }

       





        public static int getCurrentOutputDevice()
        {
            /*  try
              {
                  // Setting to Correct Input Device
                  int waveDevices = WaveOut.DeviceCount;
                  //InputDevicesDict.Clear();
                  Dictionary<string, int> DevicesDict = new Dictionary<string, int>();
                  for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
                  {
                      WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveDevice);
                      DevicesDict.Add(deviceInfo.ProductName, waveDevice);
                  }

                  foreach (var kvp in DevicesDict)
                  {
                      if (AudioDevices.currentOutputDeviceName.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                      {
                          return kvp.Value;

                      }
                  }
              }
              catch (Exception ex)
              {
                  OutputText.outputLog("[" +ex.Message+"]", Color.Red);
                  OutputText.outputLog("[ For the 'An item with the same key has already been added' error go to Control Panel > Sound > right click on one of the devices with the same name > properties > rename the device.]", Color.DarkOrange);
                  OutputText.outputLog("[Defaulting to output device 0]", Color.DarkOrange);


                  return 0;
              }
              return 0;*/

            try
            {
                int waveDevices = WaveOut.DeviceCount;
                List<Tuple<string, int>> devicesList = new List<Tuple<string, int>>();

                for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
                {
                    WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveDevice);
                    devicesList.Add(new Tuple<string, int>(deviceInfo.ProductName, waveDevice));
                }

             //   List<int> matchingDevices = new List<int>();

                foreach (var device in devicesList)
                {
                    if (AudioDevices.currentOutputDeviceName.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        // matchingDevices.Add(device.Item2);
                        return device.Item2;
                    }
                }

              /*  if (matchingDevices.Count > 0)
                {
                    // Handle multiple matching devices, if needed
                    // For example, select the first matching device
                    OutputText.outputLog("[ Output device with the same name as another audio device selected, selecting first instance ]", Color.DarkOrange);
                    return matchingDevices[0];
                }*/

                // Handle the case when no matching device is found

                // ...
            }
            catch (Exception ex)
            {

                // Handle the exception
                // ...
            }

            return 0;

        }
        public static int getCurrentOutputDevice2()
        {
            try
            {
                int waveDevices = WaveOut.DeviceCount;
                List<Tuple<string, int>> devicesList = new List<Tuple<string, int>>();

                for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
                {
                    WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveDevice);
                    devicesList.Add(new Tuple<string, int>(deviceInfo.ProductName, waveDevice));
                }

              //  List<int> matchingDevices = new List<int>();

                foreach (var device in devicesList)
                {
                    if (AudioDevices.currentOutputDeviceName2nd.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        return device.Item2;
                    }
                }

               /* if (matchingDevices.Count > 0)
                {
                    // Handle multiple matching devices, if needed
                    // For example, select the first matching device
                    OutputText.outputLog("[ Output device 2 with the same name as another audio device selected, selecting first instance ]", Color.DarkOrange);
                    return matchingDevices[0];
                }*/

                // Handle the case when no matching device is found

                // ...
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[" + ex.Message + "]", Color.Red);
                // Handle the exception
                // ...
            }

            return 0;
        }

        public static void playSSMLWaveStream(MemoryStream memoryStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            MemoryStream memoryStream2 = new MemoryStream();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            memoryStream.CopyTo(memoryStream2);


            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
            {
                MemoryStream memoryStream3 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream3);

                memoryStream3.Flush();
                memoryStream3.Seek(0, SeekOrigin.Begin);// go to begining before copying
                audioFiles.writeAudioToOutputWav(memoryStream3);
            }


            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            WaveFileReader wav = new WaveFileReader(memoryStream);


            memoryStream2.Flush();
            memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
            WaveFileReader wav2 = new WaveFileReader(memoryStream2);



            var AnyOutput = new WaveOut();
            AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
            AnyOutput.Init(wav);
            AnyOutput.Play();
            ct.Register(async () => AnyOutput.Stop());
            var AnyOutput2 = new WaveOut();
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {
                //  AnyOutput2 = new WaveOut();
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

                // AnyOutput = null;
                //  if (AnyOutput2 != null)
                //  {
                AnyOutput2.Stop();
                AnyOutput2.Dispose();
                //    AnyOutput2 = null;
                //   }
                memoryStream.Dispose();
                memoryStream = null;
                //  memoryStream2.Dispose();
                wav.Dispose();
                wav2.Dispose();
                wav = null;
                wav2 = null;


                ct = new();
                Debug.WriteLine("azure dispose successful");
                TTSMessageQueue.PlayNextInQueue();
            }
        }

        public static void playSSMLMP3Stream(MemoryStream memoryStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
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
            var AnyOutput2 = new WaveOut();

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {

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
                //  AnyOutput = null;
                // if (AnyOutput2 != null)
                //  {
                AnyOutput2.Stop();
                AnyOutput2.Dispose();

                //     AnyOutput2 = null;
                //  }
                memoryStream.Dispose();
                memoryStream = null;
                //  memoryStream2.Dispose();
                wav.Dispose();

                wav = null;
                wav2 = null;

                ct = new();

                Debug.WriteLine("amazon dispose successful");
                TTSMessageQueue.PlayNextInQueue();
            }

        }
        public static void playMoonbaseStream(MemoryStream memoryStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            MemoryStream memoryStream2 = new MemoryStream();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            memoryStream.CopyTo(memoryStream2);

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
            {
                MemoryStream memoryStream3 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream3);

                memoryStream3.Flush();
                memoryStream3.Seek(0, SeekOrigin.Begin);// go to begining before copying
                audioFiles.writeAudioToOutputRaw(memoryStream3);
            }


            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            var wav = new RawSourceWaveStream(memoryStream, new WaveFormat(11000, 16, 1));

            memoryStream2.Flush();
            memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
            var wav2 = new RawSourceWaveStream(memoryStream2, new WaveFormat(11000, 16, 1));







            var volume = 10;
            int pitch = 5;
            int rate = 5;
            var volumeFloat = 1f;
            var pitchFloat = 1f;
            var rateFloat = 1f;

            volume = TTSMessageQueued.Volume;
            pitch = TTSMessageQueued.Pitch;
            rate = TTSMessageQueued.Speed;

            volumeFloat = volume * 0.1f;
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

            WaveOut AnyOutput2 = new WaveOut();
            VarispeedSampleProvider speedControl_2 = null;
            WaveChannel32 wave32_2 = null;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {
                wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                wave32_2.PadWithZeroes = false;
                speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                speedControl_2.PlaybackRate = pitchFloat;//output 2
                                                         // AnyOutput2 = new WaveOut();
                AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                AnyOutput2.Init(speedControl_2);
                AnyOutput2.Play();
                ct.Register(async () => AnyOutput2.Stop());
            }

            ct.Register(async () => TTSMessageQueue.PlayNextInQueue());
            float delayTime = pitchFloat;
            if (rate != 5) { delayTime = rateFloat; }

            int delayInt = (int)Math.Ceiling((int)wave32.TotalTime.TotalMilliseconds / delayTime);
            Thread.Sleep(delayInt);
            //  Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds * 2);// VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
            Thread.Sleep(100);

            //   WaveFileWriter.CreateWaveFile(@"TextOut\file.wav", speedControl.ToWaveProvider());

            AnyOutput.Stop();
            AnyOutput.Dispose();
            //  AnyOutput = null;
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


            AnyOutput2.Stop();
            AnyOutput2.Dispose();
            //  AnyOutput2 = null;
            if (wave32_2 != null)
            {
                speedControl_2.Dispose();
                speedControl_2 = null;
                wave32_2.Dispose();
                wave32_2 = null;
                wav2.Dispose();
                wav2 = null;
            }
            if (!ct.IsCancellationRequested)
            {
                TTSMessageQueue.PlayNextInQueue();
            }
        }
        public static void playWaveStream(MemoryStream memoryStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            MemoryStream memoryStream2 = new MemoryStream();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            memoryStream.CopyTo(memoryStream2);

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
            {
                MemoryStream memoryStream3 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream3);

                memoryStream3.Flush();
                memoryStream3.Seek(0, SeekOrigin.Begin);// go to begining before copying
                audioFiles.writeAudioToOutputWav(memoryStream3);
            }


            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            WaveFileReader wav = new WaveFileReader(memoryStream);


            memoryStream2.Flush();
            memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
            WaveFileReader wav2 = new WaveFileReader(memoryStream2);



            var volume = 10;
            int pitch = 5;
            int rate = 5;
            var volumeFloat = 1f;
            var pitchFloat = 1f;
            var rateFloat = 1f;


            volume = TTSMessageQueued.Volume;
            pitch = TTSMessageQueued.Pitch;
            rate = TTSMessageQueued.Speed;

            volumeFloat = volume * 0.1f;
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

            WaveOut AnyOutput2 = new WaveOut();
            VarispeedSampleProvider speedControl_2 = null;
            WaveChannel32 wave32_2 = null;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {
                wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                wave32_2.PadWithZeroes = false;
                speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                speedControl_2.PlaybackRate = pitchFloat;//output 2
                                                         // AnyOutput2 = new WaveOut();
                AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                AnyOutput2.Init(speedControl_2);
                AnyOutput2.Play();
                ct.Register(async () => AnyOutput2.Stop());

            }


            ct.Register(async () => TTSMessageQueue.PlayNextInQueue());
            float delayTime = pitchFloat;
            if (rate != 5) { delayTime = rateFloat; }
            int delayInt = (int)Math.Ceiling((int)wave32.TotalTime.TotalMilliseconds / delayTime);
            Thread.Sleep(delayInt);

            //  Thread.Sleep((int)wave32.TotalTime.TotalMilliseconds * 2);// VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
            Thread.Sleep(100);



            AnyOutput.Stop();
            AnyOutput.Dispose();
            //  AnyOutput = null;
            speedControl.Dispose();
            speedControl = null;
            wave32.Dispose();
            wave32 = null;
            wav.Dispose();
            wav = null;
            memoryStream.Dispose();
            //  synthesizerLite.Dispose();
            memoryStream = null;
            //     synthesizerLite = null;


            AnyOutput2.Stop();
            AnyOutput2.Dispose();
            // AnyOutput2 = null;
            if (wave32_2 != null)
            {
                speedControl_2.Dispose();
                speedControl_2 = null;
                wave32_2.Dispose();
                wave32_2 = null;
                wav2.Dispose();
                wav2 = null;
            }
            if (!ct.IsCancellationRequested)
            {
                TTSMessageQueue.PlayNextInQueue();
            }

        }
        public static void playMp3Stream(MemoryStream memoryStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            MemoryStream memoryStream2 = new MemoryStream();
            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            memoryStream.CopyTo(memoryStream2);

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
            {
                MemoryStream memoryStream3 = new MemoryStream();
                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                memoryStream.CopyTo(memoryStream3);

                memoryStream3.Flush();
                memoryStream3.Seek(0, SeekOrigin.Begin);// go to begining before copying
                audioFiles.writeAudioToOutputMp3(memoryStream3);
            }


            memoryStream.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
            Mp3FileReader wav = new Mp3FileReader(memoryStream);


            memoryStream2.Flush();
            memoryStream2.Seek(0, SeekOrigin.Begin);// go to begining before copying
            Mp3FileReader wav2 = new Mp3FileReader(memoryStream2);


            var volume = 10;
            int pitch = 5;
            int rate = 5;
            var volumeFloat = 1f;
            var pitchFloat = 1f;
            var rateFloat = 1f;

            volume = TTSMessageQueued.Volume;
            pitch = TTSMessageQueued.Pitch;
            rate = TTSMessageQueued.Speed;


            volumeFloat =  volume * 0.1f;
            pitchFloat = 0.5f + pitch * 0.1f;
            rateFloat = 0.5f + rate * 0.1f;


            bool useTempo = false;
            if (rate != 5)//if rate is changed will use only rate, else use pitch which also changes rate.
            {
                useTempo = true;
                pitchFloat = rateFloat;
                // delayTime = rateFloat;
            }


            var wave32 = new WaveChannel32(wav, volumeFloat, 0f);  //1f volume is normal, keep pan at 0 for audio through both ears
            VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(useTempo, false));
            speedControl.PlaybackRate = pitchFloat;
            var AnyOutput = new WaveOut();
            AnyOutput.DeviceNumber = AudioDevices.getCurrentOutputDevice();
            AnyOutput.Init(speedControl);
            AnyOutput.Play();
            ct.Register(async () => AnyOutput.Stop());

            WaveOut AnyOutput2 = new WaveOut();
            VarispeedSampleProvider speedControl_2 = null;
            WaveChannel32 wave32_2 = null;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked == true)//output 2
            {
                wave32_2 = new WaveChannel32(wav2, volumeFloat, 0f); //output 2
                wave32_2.PadWithZeroes = false;
                speedControl_2 = new VarispeedSampleProvider(new WaveToSampleProvider(wave32_2), 2000, new SoundTouchProfile(useTempo, false));//output 2
                speedControl_2.PlaybackRate = pitchFloat;//output 2
                                                         // AnyOutput2 = new WaveOut();
                AnyOutput2.DeviceNumber = AudioDevices.getCurrentOutputDevice2();
                AnyOutput2.Init(speedControl_2);
                AnyOutput2.Play();
                ct.Register(async () => AnyOutput2.Stop());
            }


            //this is where i would save files
            //  WaveFileWriter.CreateWaveFile(@"TextOut\file.wav", speedControl.ToWaveProvider());

            ct.Register(async () => TTSMessageQueue.PlayNextInQueue());
            //   ct.Register(async () => Thread.Sleep(50));
            //  ct.Register(async () => ct = new());
            float delayTime = pitchFloat;
            if (rate != 5) { delayTime = rateFloat; }
            int delayInt = (int)Math.Ceiling((int)wave32.TotalTime.TotalMilliseconds / delayTime);
            Thread.Sleep(delayInt);
            // VERY IMPORTANT HIS IS x2 since THE AUDIO CAN ONLY GO AS SLOW AS .5 TIMES SPEED IF IT GOES SLOWER THIS WILL NEED TO BE CHANGED
            Thread.Sleep(100);



            AnyOutput.Stop();
            AnyOutput.Dispose();
            //  AnyOutput = null;
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


            AnyOutput2.Stop();
            AnyOutput2.Dispose();
            //  AnyOutput2 = null;
            if (wave32_2 != null)
            {
                speedControl_2.Dispose();
                speedControl_2 = null;
                wave32_2.Dispose();
                wave32_2 = null;
                wav2.Dispose();
                wav2 = null;
            }


            if (!ct.IsCancellationRequested)
            {
                TTSMessageQueue.PlayNextInQueue();
            }




            //   Debug.WriteLine("disposed of all");
        }









    }
}
