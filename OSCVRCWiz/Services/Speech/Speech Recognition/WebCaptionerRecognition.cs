using System.Net;
using Newtonsoft.Json.Linq;
using CoreOSC;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Resources.StartUp.StartUp;
//using VRC.OSCQuery; // Beta Testing dll (added the project references)


namespace OSCVRCWiz
{

    public class WebCaptionerRecognition
    {
            public static int Port = 54026;
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

        public static void autoStopWebCap()
        {
            if (webCapEnabled == true)
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
            OutputText.outputLog("[Starting HTTP listener for Web Captioner Started. Webhook URL: http://localhost:54026/ ]");
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
            try
            {
                _listener = new HttpListener();
                // _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");//MUST RUN AS ADMIN //http://127.0.0.1:8080/
                _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/"); //THIS IS THE EASIEST WAY TO MAKE USERS NOT HAVE TO RUN PROGRAM AS ADMINISTRATOR EVREY TIME!!! //http://localhost:8080/
                _listener.Start();

                Task.Run(() => Receive());

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                    OSC.OSCSender.Send(sttListening);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[HTTP listener Unexpected Error (failed to start): " + ex.Message + "]", Color.Red);
                webCapEnabled = false;

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }

            }
        }

            public static void Stop()
            {
            try
            {
                _listener.Stop();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[HTTP listener Unexpected Error (still listening): " + ex.Message + "]", Color.Red);
                webCapEnabled = true;

            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
            {
                var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                OSC.OSCSender.Send(sttListening);
            }



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
                try
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
                        //  Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Web Captioner"));
                        TTSMessageQueue.QueueMessage(text, "Web Captioner");



                    }
                    Stop();
                    Start();

                }
            catch (Exception ex)
            {
                    OutputText.outputLog("[HTTP listener Unexpected Error Try Again: "+ ex.Message+"]",Color.Red);
                    

                }

           }
            
             

            }

       









    }

    }
  

