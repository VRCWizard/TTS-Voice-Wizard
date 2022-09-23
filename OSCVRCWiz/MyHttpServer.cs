using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using DeepL_Translation;


namespace OSCVRCWiz
{

    public class HttpServer
    {
        // This example requires the System and System.Net namespaces.
      

            public int Port = 8080;
            public static string recievedString ="";

            private HttpListener _listener;

            public void Start()
            {
                _listener = new HttpListener();
            // _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");//MUST RUN AS ADMIN //http://127.0.0.1:8080/
            _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/"); //THIS IS THE EASIEST WAY TO MAKE USERS NOT HAVE TO RUN PROGRAM AS ADMINISTRATOR EVREY TIME!!! //http://localhost:8080/
            _listener.Start();
               
                Task.Run(() => Receive());
            }

            public void Stop()
            {
                _listener.Stop();
            }

            private void Receive()
            {
                _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                
            }

            private void ListenerCallback(IAsyncResult result)
            {
              //  System.Diagnostics.Debug.WriteLine("testing if this still works");
                if (_listener.IsListening)
                {
                    var context = _listener.EndGetContext(result);
                    var request = context.Request;

                    // do something with the request
              //      System.Diagnostics.Debug.WriteLine($"{request.Url}");
                    //  System.Diagnostics.Debug.WriteLine($"{request.Url.OriginalString}");
              //      System.Diagnostics.Debug.WriteLine($"{request.HttpMethod} {request.Url}");

                    if (request.HasEntityBody)
                    {
                        var body = request.InputStream;
                        var encoding = request.ContentEncoding;
                        var reader = new StreamReader(body, encoding);
                        if (request.ContentType != null)
                        {
                     //       System.Diagnostics.Debug.WriteLine("Client data content type {0}", request.ContentType);
                        }
                      //  System.Diagnostics.Debug.WriteLine("Client data content length {0}", request.ContentLength64);

                        System.Diagnostics.Debug.WriteLine("Start of data:");
                        string s = reader.ReadToEnd();
                        s = s.Replace("{\"transcript\":\"","");
                        s = s.Replace("\",\"sequence\"", "");
                        s = s.Substring(0, s.IndexOf(":"));
                        recievedString = s;
                        System.Diagnostics.Debug.WriteLine(s);
                    //    System.Diagnostics.Debug.WriteLine("End of data:");
                        reader.Close();
                        body.Close();
                        var ot = new OutputText();
                    //   ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Web Captioner]: "+s);
                    //  ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, s,"tts");

                    /*          VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate () //DEEPL TRANSLATION 
                             {

                            if (VoiceWizardWindow.MainFormGlobal.comboBox3.Text.ToString() != "No Translation (Default)")
                             {
                                 var deep = new DeepLC();
                                 deep.translateTextDeepL(s);
                                     s = DeepLC.DeepLTranslationText;


                             }
                             });*/



                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                    {
                        ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Web Captioner]: "+s);
                    }
                    //Send Text to TTS
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked == true)
                    {
                        SetDefaultTTS.SetVoicePresets();
                        AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, s, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice); //turning off TTS for now
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapSystem.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.stream = new MemoryStream();
                        VoiceWizardWindow.MainFormGlobal.synthesizerLite.SetOutputToWaveStream(VoiceWizardWindow.MainFormGlobal.stream);
                        VoiceWizardWindow.MainFormGlobal.synthesizerLite.Speak(s);
                        var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
                        var waveSource = new MediaFoundationDecoder(VoiceWizardWindow.MainFormGlobal.stream);
                        waveOut.Initialize(waveSource);
                        waveOut.Play();
                        waveOut.WaitForStopped();
                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                    {
                       

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, s,"tts"));

                       
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        VoiceWizardWindow.pauseBPM = true;                                          
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChatSpeechBubbles(VoiceWizardWindow.MainFormGlobal, s, "tts"));
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked == true)
                    {
                        Task.Run(() => ot.outputGreenScreen(VoiceWizardWindow.MainFormGlobal, s, "tts")); //original

                    }



                }
                    Stop();
                    Start();

                   // Task.Run(() => Receive());
                    

                }
            }
        }
    }
  

