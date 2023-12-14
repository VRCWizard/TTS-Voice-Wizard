
using MeaMod.DNS.BaseEncoding;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using OSCVRCWiz;
using OSCVRCWiz.Resources.Audio.SoundTouch;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Media;
using System.Text;
using System.Windows.Media.Media3D;
using Whisper;
using Windows.Media.Devices;
//using NAudio.CoreAudioApi;




namespace OSCVRCWiz.Resources.Audio
{
    public class AudioDevices
    {
        public static int audioOutputIndex = -1;
        public static List<string> comboIn = new List<string>();
        public static List<string> comboOut = new List<string>();
        public static List<string> micIDs = new List<string>();
      //  public static List<MMDevice> micMMs = new List<MMDevice>();
        public static List<string> speakerIDs = new List<string>();
        public static string currentInputDevice = "";
       // public static MMDevice currentInputDeviceMM = null;
        public static string currentOutputDevice = "";
        public static string currentInputDeviceName = "Default";
        public static string currentOutputDeviceName = "Default";
        public static int currentOutputDeviceLite = 0;

        public static string currentOutputDevice2nd = "";
        public static string currentOutputDeviceName2nd = "Default";

        


        public static void InitializeAudioDevices()
        {
            try
            {
                AudioDevices.NAudioSetupInputDevices();
                AudioDevices.NAudioSetupOutputDevices();
            }
            catch (Exception ex) { MessageBox.Show("Audio Device Startup Error: " + ex.Message); }
        }

        public static void NAudioSetupInputDevices()
        {
            VoiceWizardWindow.MainFormGlobal.comboBoxInput.Items.Clear();
            comboIn.Clear();
            micIDs.Clear();


            comboIn.Add("Default");
            micIDs.Add("Default");

            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in
                     enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);
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
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxInput.Text))
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxInput.SelectedItem = "Default";
                }
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
                     enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                Debug.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);

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
                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem.ToString()))
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem = "Default";
                }
            }
            catch
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem = "Default";
            }
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem = Settings1.Default.SpeakerName2;

                if (string.IsNullOrWhiteSpace(VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem.ToString()))
                {
                    VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem = "Default";
                }
            }
            catch
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem = "Default";
            }



        }
        public static int getCurrentInputDevice()
        {

            try
            {
                int waveDevices = WaveIn.DeviceCount;
                List<Tuple<string, int>> devicesList = new List<Tuple<string, int>>();

                for (int waveDevice = 0; waveDevice < waveDevices; waveDevice++)
                {
                    WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveDevice);
                    devicesList.Add(new Tuple<string, int>(deviceInfo.ProductName, waveDevice));
                }


                foreach (var device in devicesList)
                {
                    if (currentInputDeviceName.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        return device.Item2;
                    }
                }


            }
            catch (Exception ex)
            {

                // Handle the exception
                OutputText.outputLog("[Input Device Error: " + ex.Message + "]", Color.Red);
            }

            return 0;

        }







        public static int getCurrentOutputDevice()
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

                //   List<int> matchingDevices = new List<int>();

                foreach (var device in devicesList)
                {
                    if (currentOutputDeviceName.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        // matchingDevices.Add(device.Item2);
                        return device.Item2;
                    }
                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Output Device 1 Error: " + ex.Message + "]", Color.Red);
  
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
                    if (currentOutputDeviceName2nd.Contains(device.Item1, StringComparison.OrdinalIgnoreCase))
                    {
                        return device.Item2;
                    }
                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Ouput Device 2 Error: " + ex.Message + "]", Color.Red);

            }

            return 0;
        }

        public static float ConvertPitchToFloat(int pitchValue)
        {
            // Step 1: Normalize the pitch value to the range 0 to 200
            float normalizedValue = (pitchValue + 100) * 1.0f; // Convert to float to ensure floating-point division

            // Step 2: Divide by 100 to get it in the range 0.0f to 2.0f
            float floatValue = normalizedValue / 100.0f;

            // Ensure the value is within the desired range (0.0f to 2.0f)
            floatValue = Math.Clamp(floatValue, 0.0f, 2.0f);

            if(floatValue == 0f)
            {
                floatValue = 0.01f;
            }
            Debug.WriteLine(floatValue);
            Debug.WriteLine(pitchValue);

            return floatValue;
        }



        public static void PlayAudioStream(Stream audioStream, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct, bool applyAudioEditing, AudioFormat audioFormat)
        {
            try
            {
                //delay before audio
                int delayBeforeAudio = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxDelayBeforeAudio.Text.ToString());
                Thread.Sleep(delayBeforeAudio);

                if(delayBeforeAudio != 0)
                {
                    OutputText.outputLog("[Playing Delayed Audio]");
                }


                MemoryStream memoryStream = new MemoryStream();
                audioStream.Flush();
                audioStream.Seek(0, SeekOrigin.Begin);
                audioStream.CopyTo(memoryStream);

                MemoryStream duplicateStream = new MemoryStream();
                audioStream.Flush();
                audioStream.Seek(0, SeekOrigin.Begin);// go to begining before copying
                audioStream.CopyTo(duplicateStream);


                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked)
                {
                    MemoryStream saveStream = new MemoryStream();
                    audioStream.Flush();
                    audioStream.Seek(0, SeekOrigin.Begin);
                    audioStream.CopyTo(saveStream);

                    saveStream.Flush();
                    saveStream.Seek(0, SeekOrigin.Begin);
                    AudioToFile.WriteAudioToOutput(saveStream, audioFormat, VoiceWizardWindow.MainFormGlobal.rjToggleButtonUniqueWavNames.Checked);
                }

                memoryStream.Flush();
                memoryStream.Seek(0, SeekOrigin.Begin);

                WaveStream audioReader;
                ISampleProvider audioProvider;
                switch (audioFormat)
                {
                    case AudioFormat.Wav:
                        audioReader = new WaveFileReader(memoryStream);
                        break;

                    case AudioFormat.Mp3:
                        audioReader = new Mp3FileReader(memoryStream);
                        break;

                    case AudioFormat.Raw:
                        audioReader = new RawSourceWaveStream(memoryStream, new WaveFormat(11000, 16, 1));
                        break;

                    default:
                        throw new ArgumentException("Invalid audio format specified.");
                }

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
                pitchFloat = ConvertPitchToFloat(pitch);
                rateFloat = ConvertPitchToFloat(rate);



                // Apply audio editing only if specified
                try
                {
                    if (applyAudioEditing)
                    {

                        // Create the WaveChannel32 with the audioReader
                        var wave32 = new WaveChannel32(audioReader, volumeFloat, 0f);

                        VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(true, false));
                        speedControl.PlaybackRate = rateFloat;


                        VarispeedSampleProvider speedControl2 = new VarispeedSampleProvider(speedControl, 100, new SoundTouchProfile(false, false));
                        speedControl2.PlaybackRate = pitchFloat;


                        audioProvider = speedControl2;




                    }
                    else
                    {
                        // No audio editing, use WaveToSampleProvider directly
                        var wave32 = new WaveChannel32(audioReader, volumeFloat, 0f);
                        audioProvider = wave32.ToSampleProvider();

                        //     wave32.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"[Audio editing features could not be applied: {ex.Message}]", Color.Orange);

                    var wave32 = new WaveChannel32(audioReader, volumeFloat, 0f);
                    audioProvider = wave32.ToSampleProvider();
                    applyAudioEditing = false;
                }

            var outputDevice = new WaveOut();
                outputDevice.DeviceNumber = getCurrentOutputDevice();
                outputDevice.Init(audioProvider);
                outputDevice.Play();
                ct.Register(async () => outputDevice.Stop());

                WaveOut outputDevice2 = null;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked)
                {
            

                  duplicateStream.Flush();
                 duplicateStream.Seek(0, SeekOrigin.Begin);

                    WaveStream audioReader2;
                    ISampleProvider audioProvider2;
                    switch (audioFormat)
                    {
                        case AudioFormat.Wav:
                            audioReader2 = new WaveFileReader(duplicateStream);
                            break;

                        case AudioFormat.Mp3:
                            audioReader2 = new Mp3FileReader(duplicateStream);
                            break;

                        case AudioFormat.Raw:
                            audioReader2 = new RawSourceWaveStream(duplicateStream, new WaveFormat(11000, 16, 1));
                            break;

                        default:
                            throw new ArgumentException("Invalid audio format specified.");
                    }
                    try
                    {

                        if (applyAudioEditing)
                        {
                        // Create the WaveChannel32 with the audioReader
                      

                            var wave32 = new WaveChannel32(audioReader2, volumeFloat, 0f);



                            VarispeedSampleProvider speedControl = new VarispeedSampleProvider(new WaveToSampleProvider(wave32), 100, new SoundTouchProfile(true, false));
                            speedControl.PlaybackRate = rateFloat;


                            VarispeedSampleProvider speedControl2 = new VarispeedSampleProvider(speedControl, 100, new SoundTouchProfile(false, false));
                            speedControl2.PlaybackRate = pitchFloat;


                            audioProvider2 = speedControl2;
                       

                            }
                            else
                            {
                                // No audio editing, use WaveToSampleProvider directly
                                var wave32 = new WaveChannel32(audioReader2, volumeFloat, 0f);
                                audioProvider2 = wave32.ToSampleProvider();
                            }
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"[Audio editing features could not be applied: {ex.Message}]", Color.Orange);

                    var wave32 = new WaveChannel32(audioReader, volumeFloat, 0f);
                    audioProvider2 = wave32.ToSampleProvider();
                    applyAudioEditing = false;
                }

            outputDevice2 = new WaveOut();
                    outputDevice2.DeviceNumber = getCurrentOutputDevice2();
                    outputDevice2.Init(audioProvider2);
                    outputDevice2.Play();
                    ct.Register(async () => outputDevice2.Stop());
                }

                ct.Register(async () => TTSMessageQueue.PlayNextInQueue());

               
                int delay = applyAudioEditing ? ((int)Math.Ceiling((audioReader.TotalTime.TotalMilliseconds / rateFloat)/pitchFloat)) : (int)Math.Ceiling(audioReader.TotalTime.TotalMilliseconds);
                Thread.Sleep(delay);
                Thread.Sleep(100);

                outputDevice.Stop();
                outputDevice.Dispose();
                //  audioProvider.Dispose();
                audioReader.Dispose();
                memoryStream.Dispose();
             //   duplicateStream.Dispose();

                if (outputDevice2 != null)
                {
                    outputDevice2.Stop();
                    outputDevice2.Dispose();

                }

                if (!ct.IsCancellationRequested)
                {
                    TTSMessageQueue.PlayNextInQueue();
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Error Playing Audio: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[Your text input may have been invalid]", Color.DarkOrange);
                TTSMessageQueue.PlayNextInQueue();
            }
        }

        public static async void PlaySoundAsync(string soundName)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = "Assets\\sounds\\" + soundName;

                string fullPath = Path.Combine(basePath, relativePath);

                WaveStream waveStream = new WaveFileReader(fullPath);
                WaveOutEvent waveOutButton;
                waveOutButton = new WaveOutEvent();
                waveOutButton.Init(waveStream);
                waveOutButton.Play();

                // Optionally, you can handle the PlaybackStopped event to release resources when the audio finishes playing.
                waveOutButton.PlaybackStopped += (sender, args) =>
                {
                    waveStream.Dispose();
                    waveOutButton.Dispose();
                };
            } 
            catch (Exception ex)
            {
                OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
            }

            /* string sound = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\sounds", soundName);
             var soundPlayer = new SoundPlayer(sound);

             // Use async/await to play the sound asynchronously.
             await Task.Run(() => soundPlayer.Play());*/
        }
        public static MMDevice GetDeviceById(string deviceId)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            MMDevice defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            if (deviceId != "Default")
            {
                foreach (var device in devices)
                {
                    if (device.ID == deviceId)
                    {
                        return device;
                    }
                }
            }
            else
            {
                return defaultDevice;
            }

            // Device with the specified ID not found
            return null;
        }


    }
}
