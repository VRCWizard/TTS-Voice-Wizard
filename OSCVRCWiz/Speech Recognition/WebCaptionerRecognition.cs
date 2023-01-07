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

using static System.Net.Mime.MediaTypeNames;
using OSCVRCWiz.TTS;
using Newtonsoft.Json.Linq;
using OSCVRCWiz.Text;
//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public class WebCaptionerRecognition
    {
            public static int Port = 8080;
            public static string recievedString ="";
            private static HttpListener _listener;
            static bool webCapEnabled = false;



        public static void WebCapToggle()
        {
            if (webCapEnabled == false)
            {
                webCapOn();
                webCapEnabled = true;
            }
            else
            {
                webCapOff();
                webCapEnabled = false;

            }
        }
        private static void webCapOn()
        {
            System.Diagnostics.Debug.WriteLine("Starting HTTP listener...");
            // var httpServer = new HttpServer();
            Task.Run(() => Start());
            System.Diagnostics.Debug.WriteLine("Starting HTTP listener Started");
            OutputText.outputLog("[Starting HTTP listener for Web Captioner Started. Go to https://webcaptioner.com/captioner > Settings (bottom right) > Channels > Webhook > set 'http://localhost:8080/' as the Webhook URL and experiment with different chunking values (I recommend a large value so it only sends when you finish talking). Now you're all set to click 'Start Captioning' in Web Captioner]");
            //button11.Enabled = false;
        }
        private static void webCapOff()
        {
            System.Diagnostics.Debug.WriteLine("Stopping HTTP listener...");
            //  var httpServer = new HttpServer();
            Task.Run(() => Stop());
            System.Diagnostics.Debug.WriteLine("Stopped HTTP listener");
            OutputText.outputLog("[HTTP listener for Web Captioner Stopped.]");
            // button11.Enabled = false;
        }



        public static void Start()
            {
                _listener = new HttpListener();
            // _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");//MUST RUN AS ADMIN //http://127.0.0.1:8080/
            _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/"); //THIS IS THE EASIEST WAY TO MAKE USERS NOT HAVE TO RUN PROGRAM AS ADMINISTRATOR EVREY TIME!!! //http://localhost:8080/
            _listener.Start();
               
                Task.Run(() => Receive());
            }

            public static void Stop()
            {
                _listener.Stop();
            }

            private static void Receive()
            {
                _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                
            }

            private static async void ListenerCallback(IAsyncResult result)
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
                        string json = reader.ReadToEnd();
                        var text = JObject.Parse(json)["transcript"].ToString();   
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text));

                   
                
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
  

