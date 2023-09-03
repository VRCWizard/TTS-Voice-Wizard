﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using OSCVRCWiz.Settings;
using System.Diagnostics;
using System.Windows;
using OSCVRCWiz.Resources;
using static System.Net.Mime.MediaTypeNames;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.RJControls;
using Newtonsoft.Json.Linq;

namespace OSCVRCWiz.Services.Integrations.Media
{

    public class SpotifyAddon

    {
        private static string clientIdLegacy = "8ed3657eca864590843f45a659ec2976"; //TTSVoiceWizard Spotify Client ID
        private static EmbedIOAuthServer _server;
        private static SpotifyClient myClient = null;
        private static PKCETokenResponse myPKCEToken = null;
        public static string lastSong = "";
        private static string globalVerifier = "";
        static bool spotifyConnect = false;
        public static string title = "";
        public static string spotifyurllink = "https://open.spotify.com/";
        public static bool legacyState = false;
        static string fullSongPauseCheck = "";

        public static bool pauseSpotify = false;
        public static string spotifyInterval = "1500";

        public static string previousError = "";



        public static async Task spotifyGetCurrentSongInfo()
        {
            try
            {


                if (myClient == null)
                {
                    myClient = new SpotifyClient(Settings1.Default.PKCEAccessToken);
                }
                if (myClient != null)
                {
                    try
                    {
                        if (legacyState == true)
                        {
                            Debug.WriteLine("----Spotify token refreshed Attempt-----");
                            PKCETokenRefreshRequest refreshRequest = new PKCETokenRefreshRequest(clientIdLegacy, Settings1.Default.PKCERefreshToken);
                            PKCETokenResponse refreshResponse = await new OAuthClient().RequestToken(refreshRequest);
                            myClient = new SpotifyClient(refreshResponse.AccessToken);
                            Settings1.Default.PKCERefreshToken = refreshResponse.RefreshToken;
                            Settings1.Default.PKCEAccessToken = refreshResponse.AccessToken;
                            Settings1.Default.Save();
                            Debug.WriteLine("----Spotify token refreshed Successful-----");
                        }
                        else
                        {
                            string clientId = Settings1.Default.SpotifyKey;
                            Debug.WriteLine("----Spotify token refreshed Attempt-----");
                            PKCETokenRefreshRequest refreshRequest = new PKCETokenRefreshRequest(clientId, Settings1.Default.PKCERefreshToken);
                            PKCETokenResponse refreshResponse = await new OAuthClient().RequestToken(refreshRequest);
                            myClient = new SpotifyClient(refreshResponse.AccessToken);
                            Settings1.Default.PKCERefreshToken = refreshResponse.RefreshToken;
                            Settings1.Default.PKCEAccessToken = refreshResponse.AccessToken;
                            Settings1.Default.Save();
                            Debug.WriteLine("----Spotify token refreshed Successful-----");

                        }

                        if (spotifyConnect == false)
                        {
                            // var ot = new OutputText();
                            OutputText.outputLog("[Spotify Connected]", Color.Green);
                            spotifyConnect = true;

                        }
                    }

                    catch (APIException ex)
                    {
                        Debug.WriteLine("-----Spotify token doesn't need to refresh-----" + ex.Response.Body.ToString());

                    }
                    FullTrack m_currentTrack;
                    CurrentlyPlaying m_currentlyPlaying;

                    m_currentlyPlaying = await myClient.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                    var m_testing = await myClient.Player.GetAvailableDevices();
                    //  var m_testing2 = await myClient.Player.GetCurrentPlayback();
                    title = "";
                    var artist = "";
                    var allArtists = "";
                    var duration = "";
                    var progress = "";
                    var durationHours = "";
                    var progressHours = "";
                    var album = "";
                    //  var deviceType = "";
                    var deviceVolume = "";
                    if (m_currentlyPlaying != null)
                    {
                        IPlayableItem currentlyPlayingItem = m_currentlyPlaying.Item;
                        m_currentTrack = currentlyPlayingItem as FullTrack;


                        if (m_currentTrack != null)
                        {
                            //spotifyurllink = m_currentTrack.PreviewUrl; //does not seem to work correctly
                            title = m_currentTrack.Name;
                            try
                            {
                                // deviceType = m_testing.Devices[0].Type;
                                deviceVolume = m_testing.Devices[0].VolumePercent.ToString();
                            }
                            catch
                            {


                            }

                            artist = m_currentTrack.Artists[0].Name.ToString();
                            allArtists = string.Join(", ", m_currentTrack.Artists.Select(artist => artist.Name.ToString()));

                            progress = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs).ToString(@"mm\:ss");
                            duration = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"mm\:ss");
                            progressHours = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs).ToString(@"hh\:mm\:ss");
                            durationHours = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"hh\:mm\:ss");

                            album = m_currentTrack.Album.Name.ToString();
                        }
                    }
                    if ((lastSong != title || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true) && !string.IsNullOrWhiteSpace(title) && title != "" && pauseSpotify != true)
                    {
                        // VoiceWizardWindow.pauseBPM = true; pause removed to fix with spotify 
                        // lastSong = title;
                        var spotifyPausedIndicator = "▶️";

                        if (fullSongPauseCheck != progress)
                        {
                            spotifyPausedIndicator = "▶️";
                        }
                        else
                        {
                            spotifyPausedIndicator = "⏸️";
                        }

                        Debug.WriteLine(title);
                        Debug.WriteLine(artist);
                        Debug.WriteLine(duration);

                        var spotifySymbol = "ふ";
                        var theString = "Listening to '" + title + "' by '" + artist + "' " + progress + "/" + duration + " on Spotify";

                        theString = $"{spotifySymbol}Listening to {title} by {artist} {progress}/{duration} on Spotify";

                        theString = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString();
                        theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                        theString = theString.Replace("{title}", title);
                        theString = theString.Replace("{artist}", artist);
                        theString = theString.Replace("{allArtists}", allArtists);
                        theString = theString.Replace("{progressMinutes}", progress);
                        theString = theString.Replace("{durationMinutes}", duration);
                        theString = theString.Replace("{progressHours}", progressHours);
                        theString = theString.Replace("{durationHours}", durationHours);
                        theString = theString.Replace("{bpm}", OSCListener.globalBPM);
                        theString = theString.Replace("{bpmStats}", OSCListener.HREleveated);
                        theString = theString.Replace("{averageTrackerBattery}", OSCListener.globalAverageTrackerBattery.ToString());
                        theString = theString.Replace("{TCharge}", OSCListener.trackerCharge);
                        theString = theString.Replace("{leftControllerBattery}", OSCListener.globalLeftControllerBattery.ToString());
                        theString = theString.Replace("{rightControllerBattery}", OSCListener.globalRightControllerBattery.ToString());
                        theString = theString.Replace("{averageControllerBattery}", OSCListener.globalAverageControllerBattery.ToString());
                        theString = theString.Replace("{HMDBattery}", OSCListener.globalHMDBattery.ToString());
                        theString = theString.Replace("{RCharge}", OSCListener.controllerChargeR);
                        theString = theString.Replace("{LCharge}", OSCListener.controllerChargeL);
                        theString = theString.Replace("{AVGCharge}", OSCListener.controllerChargeAVG);
                        theString = theString.Replace("{HMDCharge}", OSCListener.controllerChargeHMD);
                        theString = theString.Replace("{pause}", spotifyPausedIndicator);
                        theString = theString.Replace("{spotifyVolume}", deviceVolume);
                        theString = theString.Replace("{album}", album);
                        theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                        theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                        theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                        theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                        theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                        theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                       

                        if (fullSongPauseCheck != progress && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == false)//stop outputting periodically if song paused
                        {
                            var textTime = theString;
                            textTime = textTime.Replace("{time}", DateTime.Now.ToString("h:mm:ss tt"));


                            //  var ot = new OutputText();
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true)
                            {
                                Task.Run(() => OutputText.outputLog(textTime));

                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked == false)
                            {

                                Task.Run(() => OutputText.outputVRChat(textTime, "spotify"));
                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
                            {
                              //  theString = LineBreakerChatbox(theString, 28);//must always be the last
                                Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, "spotify")); //original

                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked == true)
                            {
                                OutputText.outputTextFile(theString, @"Output\TextOut\OBSText.txt");
                            }
                        }

                        // lastSong = title;
                        // MainForm.justShowTheSong = false;
                        fullSongPauseCheck = progress;


                    }


                }
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    if (VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor != Color.Green)
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Green;
                    }
                });
            }
            catch (Exception ex)
            {


                if (ex.Message == "Exception of type 'SpotifyAPI.Web.APITooManyRequestsException' was thrown.")//this will not work if they are translating their winform to a different language
                {



                    var APIException = (APITooManyRequestsException)ex.InnerException;
                    OutputText.outputLog("Spotify APITooManyRequests Exception:  timed out for: " + APIException.RetryAfter, Color.Red);

                }
                else
                {

                    if (previousError != "The access token expired" && previousError != "String is empty or null (Parameter 'clientId')" && previousError != "Exception of type 'SpotifyAPI.Web.APIException' was thrown.")//only say these once, dont spam them
                    {
                        OutputText.outputLog("Spotify API Exception: " + ex.Message, Color.Red);
                        previousError = ex.Message.ToString();
                        try
                        {
                            if (ex.InnerException != null)
                            {
                                OutputText.outputLog("Spotify API Inner Exception: " + ex.InnerException.Message, Color.Red);
                            }

                        }
                        catch { }
                        // if (ex.Message.Contains("The access token expired"))
                        // {
                        OutputText.outputLog("[Spotify access token may be expired. Click the Connect Spotify button again.]", Color.DarkOrange);

                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {


                            if (VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor != Color.Red)
                            {
                                VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Red;
                            }
                        });
                        // }
                    }
                }




            }

        }
        public static async Task windowsMediaGetSongInfo()
        {
            var spotifyPausedIndicator = "▶️";
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)  //10 media mode
            {
                if (pauseSpotify == false)
                {
                    if (WindowsMedia.mediaStatus == "Paused")
                    {
                        spotifyPausedIndicator = "⏸️";
                    }
                    else
                    {
                        spotifyPausedIndicator = "▶️";
                    }

                    var spotifySymbol = "ふ";
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString();

                    theString = theString.Replace("{bpm}", OSCListener.globalBPM);
                    theString = theString.Replace("{bpmStats}", OSCListener.HREleveated);
                    theString = theString.Replace("{averageTrackerBattery}", OSCListener.globalAverageTrackerBattery.ToString());
                    theString = theString.Replace("{TCharge}", OSCListener.trackerCharge);
                    theString = theString.Replace("{leftControllerBattery}", OSCListener.globalLeftControllerBattery.ToString());
                    theString = theString.Replace("{rightControllerBattery}", OSCListener.globalRightControllerBattery.ToString());
                    theString = theString.Replace("{averageControllerBattery}", OSCListener.globalAverageControllerBattery.ToString());
                    theString = theString.Replace("{HMDBattery}", OSCListener.globalHMDBattery.ToString());
                    theString = theString.Replace("{RCharge}", OSCListener.controllerChargeR);
                    theString = theString.Replace("{LCharge}", OSCListener.controllerChargeL);
                    theString = theString.Replace("{AVGCharge}", OSCListener.controllerChargeAVG);
                    theString = theString.Replace("{HMDCharge}", OSCListener.controllerChargeHMD);
                    theString = theString.Replace("{title}", WindowsMedia.mediaTitle);
                    theString = theString.Replace("{artist}", WindowsMedia.mediaArtist);
                    theString = theString.Replace("{source}", WindowsMedia.mediaSource);
                    theString = theString.Replace("{progressMinutes}", WindowsMedia.getMediaProgress());
                    theString = theString.Replace("{durationMinutes}", WindowsMedia.getMediaDuration());
                    theString = theString.Replace("{progressHours}", WindowsMedia.getMediaProgressHours());
                    theString = theString.Replace("{durationHours}", WindowsMedia.getMediaDurationHours());
                    theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                    theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                    theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                    theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                    theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                    theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                    theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true)
                    {
                        theString = theString.Replace("{pause}", spotifyPausedIndicator);
                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true)
                    {
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == true && WindowsMedia.mediaStatus != "Paused" || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == false)//stop outputting periodically if song paused
                        {

                            MediaOutput(theString);
                        }

                    }
                    else
                    {
                        theString = theString.Replace("{pause}", "▶️");
                        MediaOutput(theString);
                    }


                }
            }
        }

        public static async Task soundpadGetSongInfo()
        {
            var spotifyPausedIndicator = "▶️";
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)  //10 media mode
            {
                if (pauseSpotify == false)
                {
                    if (WindowsMedia.mediaStatus == "Paused")
                    {
                        spotifyPausedIndicator = "⏸️";
                    }
                    else
                    {
                        spotifyPausedIndicator = "▶️";
                    }

                    var spotifySymbol = "ふ";
                    var theString = "";
                    theString = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString();

                    theString = theString.Replace("{bpm}", OSCListener.globalBPM);
                    theString = theString.Replace("{bpmStats}", OSCListener.HREleveated);
                    theString = theString.Replace("{averageTrackerBattery}", OSCListener.globalAverageTrackerBattery.ToString());
                    theString = theString.Replace("{TCharge}", OSCListener.trackerCharge);
                    theString = theString.Replace("{leftControllerBattery}", OSCListener.globalLeftControllerBattery.ToString());
                    theString = theString.Replace("{rightControllerBattery}", OSCListener.globalRightControllerBattery.ToString());
                    theString = theString.Replace("{averageControllerBattery}", OSCListener.globalAverageControllerBattery.ToString());
                    theString = theString.Replace("{HMDBattery}", OSCListener.globalHMDBattery.ToString());
                    theString = theString.Replace("{RCharge}", OSCListener.controllerChargeR);
                    theString = theString.Replace("{LCharge}", OSCListener.controllerChargeL);
                    theString = theString.Replace("{AVGCharge}", OSCListener.controllerChargeAVG);
                    theString = theString.Replace("{HMDCharge}", OSCListener.controllerChargeHMD);
                    theString = theString.Replace("{title}", WindowsMedia.mediaTitle);
                    theString = theString.Replace("{artist}", WindowsMedia.mediaArtist);
                    theString = theString.Replace("{source}", WindowsMedia.mediaSource);
                    theString = theString.Replace("{progressMinutes}", WindowsMedia.getMediaProgress());
                    theString = theString.Replace("{durationMinutes}", WindowsMedia.getMediaDuration());
                    theString = theString.Replace("{progressHours}", WindowsMedia.getMediaProgressHours());
                    theString = theString.Replace("{durationHours}", WindowsMedia.getMediaDurationHours());
                    theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                    theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                    theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                    theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                    theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                    theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                    theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                    theString = theString.Replace("{pause}", spotifyPausedIndicator);


                    MediaOutput(theString);
                    


                }
            }
        }

        private static void MediaOutput(string text)
        {
            var textTime = text;
            textTime = textTime.Replace("{time}", DateTime.Now.ToString("h:mm:ss tt"));

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true)
            {
                Task.Run(() => OutputText.outputLog(textTime));

            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked == false)
            {

                Task.Run(() => OutputText.outputVRChat(textTime, "spotify"));
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
            {
              //  text = LineBreakerChatbox(text, 28);//must always be the last
                Task.Run(() => OutputText.outputVRChatSpeechBubbles(text, "media")); //original

            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked == true)
            {
                OutputText.outputTextFile(text, @"Output\TextOut\OBSText.txt");
            }

        }





        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }


        public static async Task SpotifyConnect()
        {

            Debug.WriteLine("Connect spotify");

            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();


            _server.AuthorizationCodeReceived += GetCallback;// for PKCE aka best method because you dont have to pass a clientSecret
            _server.ErrorReceived += OnErrorReceived;


            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            globalVerifier = verifier;

            //  var loginRequest = new LoginRequest(_server.BaseUri, clientId,LoginRequest.ResponseType.Code)
            if (legacyState == true)
            {
                var loginRequest = new LoginRequest(_server.BaseUri, clientIdLegacy, LoginRequest.ResponseType.Code)
                {
                    CodeChallengeMethod = "S256",
                    CodeChallenge = challenge,
                    Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackState }
                };
                BrowserUtil.Open(loginRequest.ToUri());

            }
            else
            {
                string clientId = Settings1.Default.SpotifyKey;
                var loginRequest = new LoginRequest(_server.BaseUri, clientId, LoginRequest.ResponseType.Code)
                {
                    CodeChallengeMethod = "S256",
                    CodeChallenge = challenge,
                    Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackState }
                };
                BrowserUtil.Open(loginRequest.ToUri());

            }


        }

        // This method should be called from your web-server when the user visits "http://localhost:5000/callback"
        public static async Task GetCallback(object sender, AuthorizationCodeResponse response)
        {
            Debug.WriteLine("Getcallback code: " + response.Code.ToString());

            if (legacyState == true)
            {
                var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientIdLegacy, response.Code, new Uri("http://localhost:5000/callback"), globalVerifier));
                Settings1.Default.PKCERefreshToken = initialResponse.RefreshToken;
                Settings1.Default.PKCEAccessToken = initialResponse.AccessToken;
                Settings1.Default.Save();

            }
            else
            {
                string clientId = Settings1.Default.SpotifyKey;
                var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientId, response.Code, new Uri("http://localhost:5000/callback"), globalVerifier));
                Settings1.Default.PKCERefreshToken = initialResponse.RefreshToken;
                Settings1.Default.PKCEAccessToken = initialResponse.AccessToken;
                Settings1.Default.Save();
            }

        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.ASCII.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.ASCII.GetString(base64EncodedBytes);
        }



        public static System.Threading.Timer spotifyTimer;

        public static void initiateTimer()
        {
            spotifyTimer = new System.Threading.Timer(spotifytimertick);
            spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);
        }

        public static void spotifytimertick(object sender)
        {

            Thread t = new Thread(doSpotifyTimerTick);
            t.Start();
        }

        private static void doSpotifyTimerTick()
        {


            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked == true)
            {
                Task.Run(() => SpotifyAddon.spotifyGetCurrentSongInfo());
            }

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)
            {
                bool soundpad = false;
                foreach (object Item in VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.CheckedItems)
                {
                    if (Item.ToString() == "Soundpad")
                    {
                        soundpad = true;
                    }
   
                }

                if (soundpad == true) 
                {
                    Task.Run(() => WindowsMedia.GetSoundPadMedia());

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true)
                    {

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == false)
                        {
                            Task.Run(() => SpotifyAddon.soundpadGetSongInfo());
                        }
                        else
                        {
                            if (WindowsMedia.mediaTitle != "" && WindowsMedia.mediaStatus != "Paused")
                            {
                                Task.Run(() => SpotifyAddon.soundpadGetSongInfo());
                            }
                        }
                    }
                    else
                    {

                        if (WindowsMedia.mediaTitle != "")
                        {
                            Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                        }
                    }
                   
                }

                else
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonForceMedia.Checked == true)
                    {
                        if (WindowsMedia.mediaManager != null)
                        {
                            WindowsMedia.mediaManager.ForceUpdate();//windows media will be forced to update on this interval, this is for debug
                            Debug.WriteLine("forced media");
                        }
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true)
                    {

                        Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                    }
                }
              
            }

            spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);


        }

        public static void ChangeMediaUpdateInterval()
        {
            var mills = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text.ToString());
            if (mills < 1500)
            {
                SpotifyAddon.spotifyInterval = "1500";
                VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text = SpotifyAddon.spotifyInterval;
                spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);
            }
            else
            {
                SpotifyAddon.spotifyInterval = VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text.ToString();
                spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);
            }
        }
      





    }
}
