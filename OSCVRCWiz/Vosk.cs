using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;
using PortAudioSharp;
using Vosk;
using Swan.Formatters;
using Newtonsoft.Json.Linq;

namespace OSCVRCWiz
{
    public class Vosk
    {

        //VOSK C# implementation modified from https://github.com/juliengabryelewicz/MicrophoneVosk
        static StreamParameters oParams;
        static Model model = new Model("model");
        static VoskRecognizer rec = new VoskRecognizer(model, 16000.0f);
        public static PortAudioSharp.Stream voskStream;

        public static void doVosk()
        {
            try
            {
                PortAudio.LoadNativeLibrary();
                PortAudio.Initialize();

                oParams.device = PortAudio.DefaultInputDevice;
                if (oParams.device == PortAudio.NoDevice)
                    throw new Exception("No default audio input device available");

                oParams.channelCount = 1;
                oParams.sampleFormat = SampleFormat.Int16;
                oParams.hostApiSpecificStreamInfo = IntPtr.Zero;

                var callbackData = new VoskCallbackData()
                {
                    textResult = String.Empty
                };

                voskStream = new PortAudioSharp.Stream(
                    oParams,
                    null,
                    16000,
                    8192,
                    StreamFlags.ClipOff,
                    playCallback,
                    callbackData
                );

                voskStream.Start();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk Listening]");
            }
            catch( Exception e )
            {
                MessageBox.Show(e.Message);
            }


        }
        public static void stopVosk()
        {
            try
            {
                voskStream.Stop();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk Stopped Listening]");
            }
            catch( Exception e )
            {
                MessageBox.Show(e.Message);
            }
        }


        class VoskCallbackData
        {
            public String textResult { get; set; }
        }

        private static StreamCallbackResult playCallback(
            IntPtr input, IntPtr output,
            System.UInt32 frameCount,
            ref StreamCallbackTimeInfo timeInfo,
            StreamCallbackFlags statusFlags,
            IntPtr dataPtr
        )
        {
            byte[] buffer = new byte[frameCount];
            Marshal.Copy(input, buffer, 0, buffer.Length);
            System.IO.Stream streamInput = new MemoryStream(buffer);
            using (System.IO.Stream source = streamInput)
            {
                byte[] bufferRead = new byte[frameCount];
                int bytesRead;
                while ((bytesRead = source.Read(bufferRead, 0, bufferRead.Length)) > 0)
                {
                    if (rec.AcceptWaveform(bufferRead, bytesRead))
                    {
                        // System.Diagnostics.Debug.WriteLine(rec.Result());
                        string json = rec.Result();
                        var s = JObject.Parse(json)["text"].ToString();
                        System.Diagnostics.Debug.WriteLine("Vosk: "+s);



                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(s));


                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                        {
                            VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Vosk]: " + s);
                            VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(VoiceWizardWindow.MainFormGlobal, s);
                        }

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, s, "tts"));


                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {//
                         // VoiceWizardWindow.pauseBPM = true;                                          
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, s, "tts"));
                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(VoiceWizardWindow.MainFormGlobal, s, "tts")); //original

                        }
                        //Send Text to TTS
                        //  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked == true)//azure new is incorrect now means any tts
                        //  {
                        string ttsModeNow = VoiceWizardWindow.TTSModeSaved;

                        switch (ttsModeNow)
                        {
                            case "FonixTalk":
                                var fx = new FonixTalkTTS();
                                Task.Run(() => fx.FonixTTS(s));
                                break;

                            case "TikTok":

                                Task.Run(() => TikTok.TikTokTextAsSpeech(s));
                                break;


                            case "System Speech":
                                var sys = new WindowsBuiltInSTTTS();
                                Task.Run(() => sys.systemTTSAction(s));

                                break;
                            case "Azure":
                                SetDefaultTTS.SetVoicePresets();
                                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, s, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                break;
                            default:

                                break;
                        }

                    }
                }
            }

            return StreamCallbackResult.Continue;

        }
    }
}
