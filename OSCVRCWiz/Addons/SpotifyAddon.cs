using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using OSCVRCWiz.Settings;
using System.Diagnostics;

namespace OSCVRCWiz.Addons
{

    public class SpotifyAddon

    {
        private static string clientIdLegacy = "8ed3657eca864590843f45a659ec2976"; //TTSVoiceWizard Spotify Client ID
        //private static string clientIdLegacy = "8ed3657eca864590843f45a659ec2976"; //TTSVoiceWizard Spotify Client ID
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
        public static string spotifyPausedIndicator = "▶️";



        public static async Task getCurrentSongInfo(VoiceWizardWindow MainForm)
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
                            VoiceWizardWindow.MainFormGlobal.ot.outputLog("[Spotify Connected]",Color.Green);
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
                    title = "";
                    var artist = "";
                    var duration = "";
                    var progress = "";
                    var durationHours = "";
                    var progressHours = "";
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


                            string currentArtists = string.Empty;
                            int counter = 0;
                            foreach (SimpleArtist currentArtist in m_currentTrack.Artists)
                            {
                                if (counter <= 1)
                                {
                                    currentArtists += currentArtist.Name + ", ";
                                }

                                counter++;
                            }

                            artist = currentArtists.Remove(currentArtists.Length - 2, 2);
                            // AlbumLabel.Content = m_currentTrack.Album.Name;
                            progress = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs).ToString(@"mm\:ss");
                            duration = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"mm\:ss");
                            progressHours = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs).ToString(@"hh\:mm\:ss");
                            durationHours = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"hh\:mm\:ss");
                        }
                    }
                    if ((lastSong != title || MainForm.justShowTheSong == true || MainForm.rjToggleButtonPeriodic.Checked == true) && !string.IsNullOrWhiteSpace(title) && title != "" && VoiceWizardWindow.pauseSpotify != true)
                    {
                        // VoiceWizardWindow.pauseBPM = true; pause removed to fix with spotify 
                        // lastSong = title;

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

                        theString = MainForm.textBoxCustomSpot.Text.ToString();
                        theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                        theString = theString.Replace("{title}", title);
                        theString = theString.Replace("{artist}", artist);
                        theString = theString.Replace("{progressMinutes}", progress);
                        theString = theString.Replace("{durationMinutes}", duration);
                        theString = theString.Replace("{progressHours}", progressHours);
                        theString = theString.Replace("{durationHours}", durationHours);
                        theString = theString.Replace("{bpm}", HeartbeatAddon.globalBPM);
                        theString = theString.Replace("{averageTrackerBattery}", HeartbeatAddon.globalAverageTrackerBattery.ToString());
                        theString = theString.Replace("{leftControllerBattery}", HeartbeatAddon.globalLeftControllerBattery.ToString());
                        theString = theString.Replace("{rightControllerBattery}", HeartbeatAddon.globalRightControllerBattery.ToString());
                        theString = theString.Replace("{averageControllerBattery}", HeartbeatAddon.globalAverageControllerBattery.ToString());
                        theString = theString.Replace("{pause}", spotifyPausedIndicator);
                        //theString = theString.Replace("{deviceType}", deviceType);
                        theString = theString.Replace("{spotifyVolume}", deviceVolume);

                        if (fullSongPauseCheck != progress && MainForm.rjToggleButtonPlayPaused.Checked == true || MainForm.rjToggleButtonPlayPaused.Checked == false)//stop outputting periodically if song paused
                        {


                            //  var ot = new OutputText();
                            if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                            {
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(theString));

                            }
                            if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonSpotifyKatDisable.Checked == false)
                            {
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(theString, "spotify"));
                            }
                            if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonSpotifyChatboxDisable.Checked == false)
                            {
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(theString, "spotify")); //original

                            }
                        }

                        // lastSong = title;
                        MainForm.justShowTheSong = false;
                        fullSongPauseCheck = progress;


                    }


                }
            }
            catch (APITooManyRequestsException e)
            {


                Debug.WriteLine("Spotify Feature timed out for: " + e.RetryAfter.ToString());

                // var ot = new OutputText();
                if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                {
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog("Spotify Feature timed out for: " + e.RetryAfter.ToString()));
                }

            }

        }
        public static async Task getCurrentDataInfo(VoiceWizardWindow MainForm)
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true)  //10 periodic // 9 media mode
            {
                if (VoiceWizardWindow.pauseBPM == false)
                {
                    if (WindowsMedia.mediaStatus != "Paused")
                    {
                        spotifyPausedIndicator = "▶️";
                    }
                    else
                    {
                        spotifyPausedIndicator = "⏸️";
                    }

                    var theString = "";
                    theString = MainForm.textBoxCustomSpot.Text.ToString();

                    theString = theString.Replace("{bpm}", HeartbeatAddon.globalBPM);
                    theString = theString.Replace("{averageTrackerBattery}", HeartbeatAddon.globalAverageTrackerBattery.ToString());
                    theString = theString.Replace("{leftControllerBattery}", HeartbeatAddon.globalLeftControllerBattery.ToString());
                    theString = theString.Replace("{rightControllerBattery}", HeartbeatAddon.globalRightControllerBattery.ToString());
                    theString = theString.Replace("{averageControllerBattery}", HeartbeatAddon.globalAverageControllerBattery.ToString());
                    theString = theString.Replace("{title}", WindowsMedia.mediaTitle);
                    theString = theString.Replace("{artist}", WindowsMedia.mediaArtist);
                    theString = theString.Replace("{source}", WindowsMedia.mediaSource);
                    theString = theString.Replace("{pause}", spotifyPausedIndicator);




                    //  var ot = new OutputText();
                    if (MainForm.rjToggleButtonPlayPaused.Checked == true && WindowsMedia.mediaStatus != "Paused" || MainForm.rjToggleButtonPlayPaused.Checked == false)//stop outputting periodically if song paused
                    {
                        if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(theString));

                        }
                        if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonSpotifyKatDisable.Checked == false)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(theString, "bpm"));
                        }
                        if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonSpotifyChatboxDisable.Checked == false)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(theString, "bpm")); //original

                        }
                    }

                }
            }
        }

        public static async Task getCurrentMediaInfo(VoiceWizardWindow MainForm)
        {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == false)  //10 periodic // 9 media mode
            {


                if (VoiceWizardWindow.pauseBPM == false)
                {
                    if (WindowsMedia.mediaStatus != "Paused")
                    {
                        spotifyPausedIndicator = "▶️";
                    }
                    else
                    {
                        spotifyPausedIndicator = "⏸️";
                    }

                    var theString = "";
                    theString = MainForm.textBoxCustomSpot.Text.ToString();

                    theString = theString.Replace("{bpm}", HeartbeatAddon.globalBPM);
                    theString = theString.Replace("{averageTrackerBattery}", HeartbeatAddon.globalAverageTrackerBattery.ToString());
                    theString = theString.Replace("{leftControllerBattery}", HeartbeatAddon.globalLeftControllerBattery.ToString());
                    theString = theString.Replace("{rightControllerBattery}", HeartbeatAddon.globalRightControllerBattery.ToString());
                    theString = theString.Replace("{averageControllerBattery}", HeartbeatAddon.globalAverageControllerBattery.ToString());
                    theString = theString.Replace("{title}", WindowsMedia.mediaTitle);
                    theString = theString.Replace("{artist}", WindowsMedia.mediaArtist);
                    theString = theString.Replace("{source}", WindowsMedia.mediaSource);
                    theString = theString.Replace("{pause}", spotifyPausedIndicator);




                    //   var ot = new OutputText();
                    if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                    {
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(theString));

                    }
                    if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonSpotifyKatDisable.Checked == false)
                    {
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(theString, "bpm"));
                    }
                    if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonSpotifyChatboxDisable.Checked == false)
                    {
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(theString, "bpm")); //original

                    }

                }
            }
        }




        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }


        public async Task SpotifyConnect(VoiceWizardWindow MainForm)
        {
            MainForm.getSpotify = true;

            Debug.WriteLine("Connect spotify");
            /*

               // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!
               _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
             BrowserUtil.Open(request.ToUri());

               */
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
            // Note that we use the verifier calculated above!
            // var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientIdLegacy, response.Code, new Uri("http://localhost:5000/callback"), globalVerifier));
            // var vw = new VoiceWizardWindow();

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





    }
}
