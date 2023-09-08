using Octokit;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;


namespace OSCVRCWiz.Services.Integrations.Heartrate
{
    public class HeartratePulsoid
    {
        public static System.Threading.Timer heartRateTimer;
        public static string heartrateIntervalPulsoid = "1500";
        static int HRPrevious = 0;
        static int currentHR = 0;
        public static bool pulsoidEnabled = false;

        static CancellationTokenSource  PulsoidCt = new();




        public static void OnStartUp()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleActivatePulsoidStart.Checked == true)//turn on osc listener on start
            {
                if (!HeartratePulsoid.pulsoidEnabled)
                {
                    HeartratePulsoid.PulsoidHeartRate(VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text.ToString());
                }

            }
        }

        public static void PulsoidStop()
        {
            try
            {
                if (pulsoidEnabled == true)
                {
                    PulsoidCt.Cancel();
                    pulsoidEnabled = false;
                    StopHeartTimer();
                    var message1 = new CoreOSC.OscMessage("/avatar/parameters/isHRConnected", false);
                    OSC.OSCSender.Send(message1);
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Red;
                    });
                    OutputText.outputLog($"[Pulsoid WebSocket Disabled]");
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Error Stopping Pulsoid: " + ex.Message + "]", Color.Red);
            }


        }

        public static async Task PulsoidHeartRate(string accessToken)
        {
           
            PulsoidCt = new();


            Uri uri = new Uri($"wss://dev.pulsoid.net/api/v1/data/real_time?access_token={accessToken}");

            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                try
                {
                    await clientWebSocket.ConnectAsync(uri, PulsoidCt.Token);

                    //make heart green
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.ForeColor = Color.Green;
                        if (VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor != Color.Green)
                        {
                            VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor = Color.Green;
                            VoiceWizardWindow.MainFormGlobal.HeartrateLabel.ForeColor = Color.Green;
                        }
                    });
                    var message1 = new CoreOSC.OscMessage("/avatar/parameters/isHRConnected", true);
                    OSC.OSCSender.Send(message1);

                    OutputText.outputLog($"[Pulsoid WebSocket Activated]",Color.Green);
                    StartHeartTimer();
                    pulsoidEnabled = true;

                    while (clientWebSocket.State == WebSocketState.Open)
                    {

                        if (PulsoidCt.Token.IsCancellationRequested)
                        {
                            // Perform cleanup and close the WebSocket gracefully
                            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Cancellation requested", CancellationToken.None);
                            break; // Exit the loop
                        }

                        await ReceiveData(clientWebSocket);
                    }
                }
                catch (WebSocketException ex)
                {
                    OutputText.outputLog($"Pulsoid WebSocketException: {ex.Message}",Color.Red);
                    OutputText.outputLog($"Try re-adding your Pulsoid authorization token", Color.Orange);
                    pulsoidEnabled = false;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonPulsoidConnect.BackColor = Color.Red;                  
                    });
                }
            }


        }

        static async Task ReceiveData(ClientWebSocket clientWebSocket)
        {
            var buffer = new byte[1024];
            var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var data = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Debug.WriteLine($"Received data: {data}");

                string jsonMessage = data.ToString();
                // Deserialize JSON and extract the heart rate value
                HeartRateResponse heartRateResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<HeartRateResponse>(jsonMessage);


                currentHR = heartRateResponse.data.heart_rate;
                OSCListener.globalBPM = currentHR.ToString();



            }

        }
        public static void StartHeartTimer()
        {
            heartRateTimer = new System.Threading.Timer(heartratetimertick);
            heartRateTimer.Change(Int32.Parse(heartrateIntervalPulsoid), 0);

        
        }
        public static void StopHeartTimer()
        {
            heartRateTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void heartratetimertick(object sender)
        {
            Thread t = new Thread(doHeartrateTimerTick);
            t.Start();
        }

        private static async void doHeartrateTimerTick()
        {

            var message0 = new CoreOSC.OscMessage("/avatar/parameters/HR", currentHR);
            OSC.OSCSender.Send(message0);

            int hundreds = currentHR / 100;
            int tens = (currentHR / 10) % 10;
            int ones = currentHR % 10;

            var message1 = new CoreOSC.OscMessage("/avatar/parameters/onesHR", ones);
            OSC.OSCSender.Send(message1);
            var message2 = new CoreOSC.OscMessage("/avatar/parameters/tensHR", tens);
            OSC.OSCSender.Send(message2);
            var message3 = new CoreOSC.OscMessage("/avatar/parameters/hundredsHR", hundreds);
            OSC.OSCSender.Send(message3);



          //  Debug.WriteLine(currentHR + "--" + HRPrevious);

            var labelBattery = $"❤️ {OSCListener.globalBPM}";

            if (currentHR > HRPrevious)
            {
                OSCListener.HREleveated = "🔺";
                labelBattery += " " + OSCListener.HREleveated;
            }
            else if (currentHR < HRPrevious)
            {
                OSCListener.HREleveated = "🔻";
                labelBattery += " " + OSCListener.HREleveated;

            }
            else if (currentHR == HRPrevious)
            {
                OSCListener.HREleveated = "";
                labelBattery += " " + OSCListener.HREleveated;

            }

            HRPrevious = currentHR;

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.HeartrateLabel.Text = labelBattery;
            });



            if (VoiceWizardWindow.MainFormGlobal.rjToggleOutputHeartrateDirect.Checked)
            {


                if ((Int32.Parse(heartrateIntervalPulsoid)) < 1500)
                {
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = true;
                    });
                    // return;
                }
                else
                {
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = false;
                    });






                    if (OSCListener.stopBPM == false && OSCListener.pauseBPM == false)
                    {

                        // var ot = new OutputText();
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleOSCListenerSpamLog.Checked)
                        {
                            OutputText.outputLog("Heartrate: " + currentHR.ToString() + " bpm");

                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                        {
                            OutputText.outputVRChat("Heartrate: " + currentHR.ToString() + " bpm", "bpm");

                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                        {
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles("💓 " + currentHR.ToString() + " bpm", "bpm"));


                        }

                    }
                }
            }


                heartRateTimer.Change(Int32.Parse(heartrateIntervalPulsoid), 0);
            


        }
       
    }

    class HeartRateResponse
    {
        public long measured_at { get; set; }
        public HeartRateData data { get; set; }
    }

    class HeartRateData
    {
        public int heart_rate { get; set; }
    }
}


//gottz initiate timer
/*
    public static void initiateTimer()
    {
        heartRateTimer = new System.Threading.Timer(heartratetimertick);
        heartRateTimer.Change(Int32.Parse(heartrateInterval), 0);
    }

    public static void heartratetimertick(object sender)
    {
        Thread t = new Thread(doHeartrateTimerTick);
        t.Start();
    }

    private static async void doHeartrateTimerTick()
    {
        if (VoiceWizardWindow.MainFormGlobal.rjTogglePulsoidHREnable.Checked)
        {
            string authToken = VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text.ToString(); // Replace with your auth token
            int currentHR = await GetHeartRateAsync(authToken);




            VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = false;

            var labelBattery = $"❤️ {OSCListener.globalBPM}";

            if (currentHR > HRPrevious)
            {
                OSCListener.HREleveated = "🔺";
                labelBattery += " " + OSCListener.HREleveated;
            }
            else if (currentHR < HRPrevious)
            {
                OSCListener.HREleveated = "🔻";
                labelBattery += " " + OSCListener.HREleveated;

            }
            else if (currentHR == HRPrevious)
            {
                OSCListener.HREleveated = "";
                labelBattery += " " + OSCListener.HREleveated;

            }

            HRPrevious = currentHR;

            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.HeartrateLabel.Text = labelBattery;
            });



            if (!VoiceWizardWindow.MainFormGlobal.rjToggleOutputHeartrateDirect.Checked) { return; }
            if ((Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxPulsoidInterval.Text.ToString()) < 1500))
            {
                VoiceWizardWindow.MainFormGlobal.labelHeartIntervalTooFast.Visible = true;
                return;
            }

            if (OSCListener.stopBPM == false)
            {

                // var ot = new OutputText();
                if (VoiceWizardWindow.MainFormGlobal.rjToggleOSCListenerSpamLog.Checked)
                {
                    OutputText.outputLog("Heartbeat: " + currentHR.ToString() + " bpm");

                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true)
                {
                    OutputText.outputVRChat("Heartbeat: " + currentHR.ToString() + " bpm", "bpm");  //add pack emoji toggle (add emoji selection page

                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    Task.Run(() => OutputText.outputVRChatSpeechBubbles("Heartbeat: " + currentHR.ToString() + " bpm", "bpm")); //original


                }
            }


        }

        heartRateTimer.Change(Int32.Parse(heartrateInterval), 0);


    }*/

/* static async Task<int> GetHeartRateAsync(string authToken)
 {
     string apiUrl = "https://dev.pulsoid.net/api/v1/data/heart_rate/latest";

     using (HttpClient client = new HttpClient())
     {
         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

         HttpResponseMessage response = await client.GetAsync(apiUrl);

         if (response.IsSuccessStatusCode)
         {
             //make green
             VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
             {
                 if (VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor != Color.Green)
                 {
                     VoiceWizardWindow.MainFormGlobal.groupBoxHeartrate.ForeColor = Color.Green;
                     VoiceWizardWindow.MainFormGlobal.HeartrateLabel.ForeColor = Color.Green;
                 }
             });

             string jsonResponse = await response.Content.ReadAsStringAsync();
             HeartRateResponse heartRateResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<HeartRateResponse>(jsonResponse);
             HRPrevious = heartRateResponse.data.heart_rate;
             return heartRateResponse.data.heart_rate;
         }
         else
         {
             OutputText.outputLog("Pulsoid Request Failed: " + response.StatusCode,Color.Red);
             return HRPrevious;
         }
     }
 }*/




