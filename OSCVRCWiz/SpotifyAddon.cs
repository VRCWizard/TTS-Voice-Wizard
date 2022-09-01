using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;

namespace OSCVRCWiz
{
 
    public class SpotifyAddon 

    {
        private static string clientIdLegacy = "8ed3657eca864590843f45a659ec2976"; //TTSVoiceWizard Spotify Client ID
        //private static string clientIdLegacy = "8ed3657eca864590843f45a659ec2976"; //TTSVoiceWizard Spotify Client ID
        private static EmbedIOAuthServer _server;
        private static SpotifyClient myClient =null;
        private static PKCETokenResponse myPKCEToken = null;
        public static string lastSong = "";
        private static string globalVerifier = "";
        static bool spotifyConnect = false;
        public static string title = "";
        public static string spotifyurllink = "https://open.spotify.com/";
        public static bool legacyState = false;
        static string fullSongPauseCheck="";



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
                            System.Diagnostics.Debug.WriteLine("----Spotify token refreshed Attempt-----");
                            PKCETokenRefreshRequest refreshRequest = new PKCETokenRefreshRequest(clientIdLegacy, Settings1.Default.PKCERefreshToken);
                            PKCETokenResponse refreshResponse = await new OAuthClient().RequestToken(refreshRequest);
                            myClient = new SpotifyClient(refreshResponse.AccessToken);
                            Settings1.Default.PKCERefreshToken = refreshResponse.RefreshToken;
                            Settings1.Default.PKCEAccessToken = refreshResponse.AccessToken;
                            Settings1.Default.Save();
                            System.Diagnostics.Debug.WriteLine("----Spotify token refreshed Successful-----");
                        }
                        else
                        {
                            string clientId = Settings1.Default.SpotifyKey;
                            System.Diagnostics.Debug.WriteLine("----Spotify token refreshed Attempt-----");
                            PKCETokenRefreshRequest refreshRequest = new PKCETokenRefreshRequest(clientId, Settings1.Default.PKCERefreshToken);
                            PKCETokenResponse refreshResponse = await new OAuthClient().RequestToken(refreshRequest);
                            myClient = new SpotifyClient(refreshResponse.AccessToken);
                            Settings1.Default.PKCERefreshToken = refreshResponse.RefreshToken;
                            Settings1.Default.PKCEAccessToken = refreshResponse.AccessToken;
                            Settings1.Default.Save();
                            System.Diagnostics.Debug.WriteLine("----Spotify token refreshed Successful-----");

                        }

                        if (spotifyConnect == false)
                        {
                            var ot = new OutputText();
                            ot.outputLog(MainForm, "[Spotify Connected]");
                            spotifyConnect = true;

                        }
                    }




                    catch (APIException ex)
                    {
                        System.Diagnostics.Debug.WriteLine("-----Spotify token doesn't need to refresh-----" + ex.Response.Body.ToString());

                    }
                    FullTrack m_currentTrack;
                    CurrentlyPlaying m_currentlyPlaying;
                    m_currentlyPlaying = await myClient.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                    title = "";
                    var artist = "";
                    var duration = "";
                    var progress = "";
                    var durationHours = "";
                    var progressHours = "";
                    if (m_currentlyPlaying != null)
                    {
                        IPlayableItem currentlyPlayingItem = m_currentlyPlaying.Item;
                        m_currentTrack = currentlyPlayingItem as FullTrack;
                        // System.Diagnostics.Debug.WriteLine(m_currentTrack.Name);

                        if (m_currentTrack != null)
                        {
                            //spotifyurllink = m_currentTrack.PreviewUrl; //does not seem to work correctly
                            title = m_currentTrack.Name;


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
                    if ((lastSong != title || MainForm.justShowTheSong == true || MainForm.rjToggleButtonPeriodic.Checked == true) && (!string.IsNullOrWhiteSpace(title) && title != "" && VoiceWizardWindow.pauseSpotify != true))
                    {
                        VoiceWizardWindow.pauseBPM = true;
                        // lastSong = title;
                        System.Diagnostics.Debug.WriteLine(title);
                        System.Diagnostics.Debug.WriteLine(artist);
                        System.Diagnostics.Debug.WriteLine(duration);

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


                        //  if (MainForm.rjToggleButtonPeriodic.Checked==true)
                        //   {
                        //  if (MainForm.rjToggleButton3.Checked == false)
                        //  {
                        //   if(MainForm.rjToggleButtonMugi.Checked==true)
                        //   {
                        //        theString = title + " - '" + artist + "'";
                        //     }
                        //  else
                        //     {
                        //        theString = "Listening to '" + title + "' by '" + artist + "' " + progress + "/" + duration + " on Spotify";

                        //     }


                        //    }
                        //  if (MainForm.rjToggleButton3.Checked == true)
                        //   {
                        //        theString = "ふ Listening to '" + title + "' by '" + artist + "' " + progress + "/" + duration;
                        //    }

                        //   }
                        //    if (MainForm.rjToggleButtonPeriodic.Checked == false)
                        //   {
                        //    if (MainForm.rjToggleButton3.Checked == false)
                        //    {
                        //      theString = "Listening to '" + title + "' by '" + artist + "'" + " on Spotify";

                        //  }
                        //    if (MainForm.rjToggleButton3.Checked == true)
                        //    {
                        //       theString = "ふ Listening to '" + title + "' by '" + artist + "' ";
                        //    }

                        //  }

                        if (fullSongPauseCheck != progress)//stop outputting periodically is song paused
                        {


                            var ot = new OutputText();
                            if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                            {
                                Task.Run(() => ot.outputLog(MainForm, theString));
                            }
                            if (MainForm.rjToggleButtonOSC.Checked == true)
                            {
                                Task.Run(() => ot.outputVRChat(MainForm, theString, "spotify"));
                            }
                            if (MainForm.rjToggleButtonChatBox.Checked == true)
                            {
                                Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, theString, "spotify")); //original

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


                System.Diagnostics.Debug.WriteLine("Spotify Feature timed out for: "+e.RetryAfter.ToString());

                var ot = new OutputText();
                if (MainForm.rjToggleButtonSpotifySpam.Checked == true)
                {
                    Task.Run(() => ot.outputLog(MainForm, "Spotify Feature timed out for: " + e.RetryAfter.ToString()));
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
            
            System.Diagnostics.Debug.WriteLine("Connect spotify");
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
                if(legacyState==true)
            {
                var loginRequest = new LoginRequest(_server.BaseUri, clientIdLegacy, LoginRequest.ResponseType.Code)
                {
                    CodeChallengeMethod = "S256",
                    CodeChallenge = challenge,
                    Scope = new[] { Scopes.UserReadCurrentlyPlaying }
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
                    Scope = new[] { Scopes.UserReadCurrentlyPlaying }
                };
                BrowserUtil.Open(loginRequest.ToUri());

            }

          
        }

        // This method should be called from your web-server when the user visits "http://localhost:5000/callback"
        public static async Task GetCallback(object sender, AuthorizationCodeResponse response)
        {
            System.Diagnostics.Debug.WriteLine("Getcallback code: " + response.Code.ToString());
            // Note that we use the verifier calculated above!
            // var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientIdLegacy, response.Code, new Uri("http://localhost:5000/callback"), globalVerifier));
           // var vw = new VoiceWizardWindow();

            if (legacyState== true)
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





    }
}
