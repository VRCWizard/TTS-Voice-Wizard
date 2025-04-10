using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Settings;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Animation;

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
        //static string fullSongPauseCheck = "";

        public static bool pauseSpotify = false;
        public static string spotifyInterval = "1500";

        public static string previousError = "";


        static DateTime tokenExpirationTime;


        //  check if the token is expired
        private static bool IsTokenExpired()
        {
            if (myPKCEToken != null)
            {
                // DateTime tokenExpirationTime = DateTime.Now.AddSeconds(myPKCEToken.ExpiresIn);

                // OutputText.outputLog("checking if token expired: "+ (tokenExpirationTime <= DateTime.Now).ToString());
                return (tokenExpirationTime <= DateTime.Now);
            }
            else
            {
                Debug.WriteLine("----- updating tokens -----");
                return true;
            }
        }

        // refresh the access token
        private static async Task<PKCETokenResponse> RefreshAccessToken()
        {
            OutputText.outputLog("Debug: Updating Spotify Tokens");
            string clientId = legacyState ? clientIdLegacy : Settings1.Default.SpotifyKey;
            Debug.WriteLine("----Spotify token refresh Attempt-----");
            PKCETokenRefreshRequest refreshRequest = new PKCETokenRefreshRequest(clientId, Settings1.Default.PKCERefreshToken);
            PKCETokenResponse refreshResponse = await new OAuthClient().RequestToken(refreshRequest);
            myPKCEToken = refreshResponse;
            OutputText.outputLog("[Your Spotify Token will refresh in " + ((myPKCEToken.ExpiresIn / 60) - 1) + " minutes]", Color.Green);

            tokenExpirationTime = DateTime.Now.AddSeconds(myPKCEToken.ExpiresIn - 60);
            // OutputText.outputLog((DateTime.Now.AddSeconds(myPKCEToken.ExpiresIn - 60) <= DateTime.Now).ToString());
            return refreshResponse;
        }

        public static async Task spotifyGetCurrentSongInfo(bool playOnce)
        {
            try
            {


                if (myClient == null)
                {
                    myClient = new SpotifyClient(Settings1.Default.PKCEAccessToken);

                }
                else
                {
                    try
                    {
                        bool tokenExpired = IsTokenExpired();
                        if (tokenExpired)
                        {
                            Debug.WriteLine("----Spotify token needs refreshing-----");
                            PKCETokenResponse refreshResponse = await RefreshAccessToken();
                            myClient = new SpotifyClient(refreshResponse.AccessToken);
                            Settings1.Default.PKCERefreshToken = refreshResponse.RefreshToken;
                            Settings1.Default.PKCEAccessToken = refreshResponse.AccessToken;
                            Settings1.Default.Save();
                            Debug.WriteLine("----Spotify token refreshed Successfully-----");
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
                        Debug.WriteLine("-----Spotify error-----" + ex.Response.Body.ToString());

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
                    var albumArtist = "";
                    // var progressBar = "";
                    TimeSpan progressT = TimeSpan.FromMinutes(0);
                    TimeSpan durationT = TimeSpan.FromMinutes(0);

                    

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

                            if(title == "Advertisement")
                            {
                                title = "";
                                artist = "";
                            }


                            allArtists = string.Join(", ", m_currentTrack.Artists.Select(artist => artist.Name.ToString()));

                            progressT = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs);
                            durationT = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs);

                            progress = progressT.ToString(@"mm\:ss");
                            duration = durationT.ToString(@"mm\:ss");



                            progressHours = new TimeSpan(0, 0, 0, 0, (int)m_currentlyPlaying.ProgressMs).ToString(@"hh\:mm\:ss");
                            durationHours = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"hh\:mm\:ss");


                            album = m_currentTrack.Album.Name.ToString();
                            try
                            {
                                albumArtist = m_currentTrack.Album.Artists[0].ToString();
                            }
                            catch 
                            {
                                albumArtist = artist;
                            }
                        }
                    }
                    if ((lastSong != title || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked == true || playOnce) && !string.IsNullOrWhiteSpace(title) && title != "" && pauseSpotify != true)
                    {
                        // VoiceWizardWindow.pauseBPM = true; pause removed to fix with spotify 
                        // lastSong = title;
                        var spotifyPausedIndicator = "▶️";

                       // if (fullSongPauseCheck != progress || playOnce)
                       if(m_currentlyPlaying.IsPlaying || playOnce)
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
                        theString = theString.Replace("{albumArtist}", albumArtist);
                        theString = theString.Replace("{nline}", "\u2028");
                        theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                        theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                        theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                        theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                        theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                        theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                        theString = theString.Replace("{lyrics}", OSCListener.spotifyLyrics);
                        theString = theString.Replace("{time}", DateTime.Now.ToString("h:mm tt"));
                        theString = theString.Replace("{timeSec}", DateTime.Now.ToString("h:mm:ss tt"));
                        theString = theString.Replace("{time24}", DateTime.Now.ToString("HH:mm"));
                        theString = theString.Replace("{time24Sec}", DateTime.Now.ToString("HH:mm:ss"));

                        theString = replaceProgresBar(theString, progressT, durationT);
                        theString = replaceHREmoji(theString, Int16.Parse(OSCListener.globalBPM));




                        
                       // if (fullSongPauseCheck != progress && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == false)//stop outputting periodically if song paused
                       if (m_currentlyPlaying.IsPlaying && VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked == false)
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

                                Task.Run(() => OutputText.outputVRChat(textTime, OutputText.DisplayTextType.Spotify));
                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
                            {
                                //  theString = LineBreakerChatbox(theString, 28);//must always be the last
                                Task.Run(() => OutputText.outputVRChatSpeechBubbles(theString, OutputText.DisplayTextType.Spotify)); //original

                            }
                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked == true)
                            {
                                OutputText.outputTextFile(theString, @"Output\TextOutput\OBSText.txt");
                                OutputText.outputTextFile(theString, @"Output\TextOutput\MediaIntegration.txt");
                            }
                        }

                        // lastSong = title;
                        // MainForm.justShowTheSong = false;
                        SpotifyAddon.lastSong = SpotifyAddon.title;
                        // WindowsMedia.previousTitle = WindowsMedia.mediaTitle;
                       // fullSongPauseCheck = progress;


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
            catch (APIException e)
            {
                if (e.Message != previousError) 
                {
                    OutputText.outputLog("Spotify API Exception: " + e.Message + "Status Code: " + e.Response?.StatusCode, Color.Red);
                }
                previousError = e.Message;
                try
                {
                    if (e.InnerException != null)
                    {
                        OutputText.outputLog("Spotify API Inner Exception: " + e.InnerException?.Message + "Status Code: "+e.Response?.StatusCode, Color.Red);
                    }

                }
                catch { }

               // OutputText.outputLog("[If this continues, click the Connect Spotify button again.]", Color.DarkOrange);

                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {


                    if (VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor != Color.Red)
                    {
                        VoiceWizardWindow.MainFormGlobal.buttonSpotify.ForeColor = Color.Red;
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
                        var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;
                       // OutputText.outputLog("Spotify Exception: " + ex.Message, Color.Red);
                        previousError = ex.Message.ToString();
                        try
                        {
                            if (ex.InnerException != null)
                            {
                                errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;
                               // OutputText.outputLog("Spotify Inner Exception: " + ex.InnerException.Message, Color.Red);
                            }

                        }
                        catch { }
                        OutputText.outputLog("Spotify Exception: " + errorMsg, Color.Red);
                        // if (ex.Message.Contains("The access token expired"))
                        // {
                       // OutputText.outputLog("[If this continues, click the Connect Spotify button again.]", Color.DarkOrange);

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

                    TimeSpan progressT = WindowsMedia.getMediaProgress();
                    TimeSpan durationT = WindowsMedia.getMediaDuration();


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
                    theString = theString.Replace("{album}", WindowsMedia.mediaAlbumTitle);
                    theString = theString.Replace("{albumArtist}", WindowsMedia.mediaAlbumArtist);
                    theString = theString.Replace("{source}", WindowsMedia.mediaSource);
                    theString = theString.Replace("{progressMinutes}", progressT.ToString(@"mm\:ss"));
                    theString = theString.Replace("{durationMinutes}", durationT.ToString(@"mm\:ss"));
                    theString = theString.Replace("{progressHours}", progressT.ToString(@"hh\:mm\:ss"));
                    theString = theString.Replace("{durationHours}", durationT.ToString(@"hh\:mm\:ss"));
                    theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                    theString = theString.Replace("{nline}", "\u2028");
                    theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                    theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                    theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                    theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                    theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                    theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                    theString = theString.Replace("{lyrics}", OSCListener.spotifyLyrics);
                    theString = theString.Replace("{time}", DateTime.Now.ToString("h:mm tt"));
                    theString = theString.Replace("{timeSec}", DateTime.Now.ToString("h:mm:ss tt"));
                    theString = theString.Replace("{time24}", DateTime.Now.ToString("HH:mm"));
                    theString = theString.Replace("{time24Sec}", DateTime.Now.ToString("HH:mm:ss"));

                    theString = replaceProgresBar(theString, progressT, durationT);
                    theString = replaceHREmoji(theString, Int16.Parse(OSCListener.globalBPM));
                    // replaceHREmoji();



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
                    TimeSpan progressT = WindowsMedia.getMediaProgress();
                    TimeSpan durationT = WindowsMedia.getMediaDuration();

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
                    theString = theString.Replace("{progressMinutes}", progressT.ToString(@"mm\:ss"));
                    theString = theString.Replace("{durationMinutes}", durationT.ToString(@"mm\:ss"));
                    theString = theString.Replace("{progressHours}", progressT.ToString(@"hh\:mm\:ss"));
                    theString = theString.Replace("{durationHours}", durationT.ToString(@"hh\:mm\:ss"));
                    theString = theString.Replace("{spotifySymbol}", spotifySymbol);
                    theString = theString.Replace("{nline}", "\u2028");
                    theString = theString.Replace("{counter1}", VRChatListener.counter1.ToString());
                    theString = theString.Replace("{counter2}", VRChatListener.counter2.ToString());
                    theString = theString.Replace("{counter3}", VRChatListener.counter3.ToString());
                    theString = theString.Replace("{counter4}", VRChatListener.counter4.ToString());
                    theString = theString.Replace("{counter5}", VRChatListener.counter5.ToString());
                    theString = theString.Replace("{counter6}", VRChatListener.counter6.ToString());
                    theString = theString.Replace("{pause}", spotifyPausedIndicator);
                    theString = replaceProgresBar(theString, progressT, durationT);
                    theString = replaceHREmoji(theString, Int16.Parse(OSCListener.globalBPM));
                    theString = theString.Replace("{time}", DateTime.Now.ToString("h:mm tt"));
                    theString = theString.Replace("{timeSec}", DateTime.Now.ToString("h:mm:ss tt"));
                    theString = theString.Replace("{time24}", DateTime.Now.ToString("HH:mm"));
                    theString = theString.Replace("{time24Sec}", DateTime.Now.ToString("HH:mm:ss"));


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

                Task.Run(() => OutputText.outputVRChat(textTime, OutputText.DisplayTextType.WindowsMedia));
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked == false)
            {
                //  text = LineBreakerChatbox(text, 28);//must always be the last
                Task.Run(() => OutputText.outputVRChatSpeechBubbles(text, OutputText.DisplayTextType.WindowsMedia)); //original

            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked == true)
            {
                OutputText.outputTextFile(text, @"Output\TextOutput\OBSText.txt");
                OutputText.outputTextFile(text, @"Output\TextOutput\MediaIntegration.txt");
            }
            WindowsMedia.previousTitle = WindowsMedia.mediaTitle;

        }





        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }


        public static async Task SpotifyConnect()
        {

            Debug.WriteLine("Connect spotify");

            _server = new EmbedIOAuthServer(new Uri("http://127.0.0.1:5000/callback"), 5000);
            await _server.Start();


            _server.AuthorizationCodeReceived += GetCallback;// for PKCE aka best method because you dont have to pass a clientSecret
            _server.ErrorReceived += OnErrorReceived;


            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            globalVerifier = verifier;

            //  var loginRequest = new LoginRequest(_server.BaseUri, clientId,LoginRequest.ResponseType.Code)
            string clientId = legacyState ? clientIdLegacy : Settings1.Default.SpotifyKey;
            var loginRequest = new LoginRequest(_server.BaseUri, clientId, LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPlaybackState }
            };


            string url = loginRequest.ToUri().ToString();
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });//this link doesnt like the other method
            if (VoiceWizardWindow.MainFormGlobal.rjToggleShowConnectURISpotify.Checked)
            {
                OutputText.outputLog(loginRequest.ToUri().ToString());
            }




        }

        // This method should be called from your web-server when the user visits "http://127.0.0.1:5000/callback"
        public static async Task GetCallback(object sender, AuthorizationCodeResponse response) //this function gets and saves the 
        {
            Debug.WriteLine("Getcallback code: " + response.Code.ToString());

            string clientId = legacyState ? clientIdLegacy : Settings1.Default.SpotifyKey;
            var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientId, response.Code, new Uri("http://127.0.0.1:5000/callback"), globalVerifier));
            Settings1.Default.PKCERefreshToken = initialResponse.RefreshToken;
            Settings1.Default.PKCEAccessToken = initialResponse.AccessToken;
            Settings1.Default.Save();



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
                Task.Run(() => SpotifyAddon.spotifyGetCurrentSongInfo(false));
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

                        if (WindowsMedia.mediaTitle != "" && WindowsMedia.mediaTitle != WindowsMedia.previousTitle)
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


        public static void printMediaOnce()
        {


            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked == true)
            {

                Task.Run(() => SpotifyAddon.spotifyGetCurrentSongInfo(true));
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
                    Task.Run(() => SpotifyAddon.soundpadGetSongInfo());


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
                    Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                }

            }




        }


        static string songProgressBar(TimeSpan progress, TimeSpan duration, string emoji, int progressBarLength)
        {

            // Calculate the position of the circle in the progress bar
            int circlePosition = CalculateCirclePosition(progress, duration, progressBarLength);

            // Display the progress bar
            string bar = DisplayProgressBar(circlePosition, progressBarLength, emoji);
            return bar;

        }


        static int CalculateCirclePosition(TimeSpan progress, TimeSpan duration, int progressBarLength)
        {
            double progressRatio = progress.TotalSeconds / duration.TotalSeconds;
            int circlePosition = (int)Math.Round(progressRatio * (progressBarLength - 1));
            return circlePosition;
        }

        static string DisplayProgressBar(int circlePosition, int progressBarLength, string emoji)
        {
            //string bar = "┣";
            string bar = "\u2523";


            for (int i = 0; i < progressBarLength; i++)
            {
                if (i == circlePosition)
                {
                    bar += emoji;
                }
                else
                {
                    // bar += "-";
                    // bar += "\u2014";
                    // bar += "━";
                    bar += "\u2501";
                   
                }
            }
            bar += "\u252B";
           // bar += "┫";
            return bar;
        }

        static string replaceProgresBar(string theString, TimeSpan progressT, TimeSpan durationT)
        {
            try
            {
                string pattern = @"\{progressBar E:(?<Emoji>.*?) L:(?<LValue>\d+)\}";
                Match match = Regex.Match(theString, pattern);

                if (match.Success)
                {
                    string emojiValue = match.Groups["Emoji"].Value;
                    int lValue = int.Parse(match.Groups["LValue"].Value);
                    var progressBar = songProgressBar(progressT, durationT, emojiValue, lValue);
                    theString = Regex.Replace(theString, pattern, progressBar);

                }
                return theString;
            }
            catch (Exception e)
            {
                OutputText.outputLog("Error Creating ProgressBar: " + e.Message, Color.Red);
                OutputText.outputLog(e.StackTrace, Color.Red);
                return theString;

            }
        }
    
           // string input = "{HREmoji (BPM: 0 E: 💀)(BPM: 40-59 E: 💔)(BPM: 60-100 E: ❤️)(BPM: 101-120 E: 💓)}";

        static string replaceHREmoji(string theString, int BPM)
        {
          //  theString += "{HREmoji (BPM: 0 E: 💀)(BPM: 40-59 E: 💔)(BPM: 60-100 E: ❤️)(BPM: 101-120 E: 💓)}";
            int index = 0;
            string emoji = "";
            
           // string pattern = @"\{HREmoji(?:\s*\(BPM:\s*(\d+|\d+-\d+)\s*E:\s*([^\s)]+)\))*\}";
            string pattern = @"\{HREmoji(?:\s*\(BPM:\s*(\d+|\d+\s*-\s*\d+)\s*E:\s*([^)]+)\))*\}";

            MatchCollection matches = Regex.Matches(theString, pattern);

            foreach (Match match in matches)
            {
                // Extracting BPM and Emoji values
                foreach (Capture captureBPM in match.Groups[1].Captures)
                {
                    Debug.WriteLine("MADE IT PAST THE FIRST CAPTURRE0**($&YBYB*TNT&");
                    string bpmValue = captureBPM.Value;
                    if (IsBPMInRange(BPM, bpmValue))
                    {
                        int emojiIndex = 0;
                        // Extracting Emoji value
                        foreach (Capture captureEmoji in match.Groups[2].Captures)
                        {
                            if (emojiIndex == index)
                            {
                                emoji = captureEmoji.Value;
                            }
                            emojiIndex++;
                            
                        }
                    }      
                    index++;
                }
            }

           theString = theString = Regex.Replace(theString, pattern, emoji);
           return theString;
        }

        static bool IsBPMInRange(int bpm, string bpmValue)
        {
            if (bpmValue.Contains("-"))
            {
                // Range case
                string[] range = bpmValue.Split('-');
                int minBPM = int.Parse(range[0].Trim());
                int maxBPM = int.Parse(range[1].Trim());
                return bpm >= minBPM && bpm <= maxBPM;
            }
            else
            {
                // Single value case
                int singleBPM = int.Parse(bpmValue);
                return bpm == singleBPM;
            }
        }











    }
}
