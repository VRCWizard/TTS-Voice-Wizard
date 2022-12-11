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
//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public class HttpServer
    {
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
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //  System.Diagnostics.Debug.WriteLine("testing if this still works");
           // watch.Start();
            if (_listener.IsListening)
                {
                    var context = _listener.EndGetContext(result);
                    var request = context.Request;



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
                 //   watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"web cap string processing time: {watch.ElapsedMilliseconds} ms");

                    //VoiceCommand task
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(s));


                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "[Web Captioner]: "+s);
                        VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(VoiceWizardWindow.MainFormGlobal, s);
                    }
                
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                    {                     
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(VoiceWizardWindow.MainFormGlobal, s,"tts"));

                       
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
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked == true)//azure new is incorrect now means any tts
                    {
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
                    Stop();
                    Start();

                }
            }
        //VRChat OSCQuery Test Code
      /*  public async void VRChatTesting()
        {
            int randomInt = new Random().Next();
            var service = new OSCQueryService("TTS Voice Wizard - Beta", 8081, 9000); //beta testing VRCHAT (default TCP=8080, default OSC=9000 vrchats sending port)
                                                                                      //You should now be able to visit http://localhost:8081 in a browser and see raw JSON describing an empty root node.
                                                                                      //You can also visit http://localhost:tcpPort?HOST_INFO to get information about the supported attributes of this OSCQuery Server.
                                                                                      // service.AddEndpoint()

            string path = "/avatar/parameters/Testing";
          
            //   string path = $"/{name}";
            service.AddEndpoint<int>(path,Attributes.AccessValues.ReadOnly,randomInt.ToString());//this is how the information is being set
            //  Response String Test: { "DESCRIPTION":"","FULL_PATH":"/avatar/parameters/AngularY","ACCESS":1,"TYPE":"i","VALUE":"525619974"}



            var response = await new HttpClient().GetAsync($"http://localhost:{9001}{path}"); //this is how we are getting the information 

            var responseString = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("Response String Test:"+responseString);
           //  var responseObject = JObject.Parse(responseString);

           //  Assert.That(responseObject[Attributes.VALUE]!.Value<int>(), Is.EqualTo(randomInt));

           service.Dispose();

        }
        public async void VRChatTestingUpdate()
        {
            var path = "/avatar/parameters/GestureLeft";
            var response = await new HttpClient().GetAsync($"http://localhost:{9001}{path}"); //this is how we are getting the information 

            var responseString = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine("Response String Test:" + responseString);

            

        } */

        


      
      



        }
    }
  

