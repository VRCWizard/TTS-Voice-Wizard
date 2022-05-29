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
       // private const string CredentialsPath = "credentials.json";
        //  private static readonly string? clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
        private static readonly string? clientId = "8ed3657eca864590843f45a659ec2976";
        //private static EmbedIOAuthServer _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
        private static EmbedIOAuthServer _server;
        private static SpotifyClient myClient =null;
        private static PKCETokenResponse myPKCEToken = null;
        private static string lastSong = "";
        private static string globalVerifier = "";







        /*   private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
           {
               await _server.Stop();

               var config = SpotifyClientConfig.CreateDefault();
               var tokenResponse = await new OAuthClient(config).RequestToken(
                 new AuthorizationCodeTokenRequest(
                   clientId, clientSecret, response.Code, new Uri("http://localhost:5000/callback")
                 )
               );

               var spotify = new SpotifyClient(tokenResponse.AccessToken);
               // do calls with Spotify and save token?

               myClient = spotify;
               myToken = tokenResponse;

           } 
         private static async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
         {
           await _server.Stop();
           var spotify = new SpotifyClient(response.AccessToken);
           // do calls with Spotify
            myClient = spotify;

         }
          */
        public static async Task getCurrentSongInfo(VoiceWizardWindow MainForm)
        {
            //  var newResponse = await new OAuthClient().RequestToken(
            //  new AuthorizationCodeRefreshRequest(clientId, clientSecret, myToken.RefreshToken));

            //   myClient = new SpotifyClient(newResponse.AccessToken);




           var authenticator = new PKCEAuthenticator(clientId, myPKCEToken);

            var config = SpotifyClientConfig.CreateDefault()
            .WithAuthenticator(authenticator);
            var spotify = new SpotifyClient(config);
            myClient = spotify;

            if (myClient != null)
            {


                // var spotify = new SpotifyClient(tokenResponse.AccessToken);
                FullTrack m_currentTrack;
                CurrentlyPlaying m_currentlyPlaying;
                m_currentlyPlaying = await myClient.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());
                var title = "";
                var artist = "";
                var duration = "";
                if (m_currentlyPlaying != null)
                {
                    IPlayableItem currentlyPlayingItem = m_currentlyPlaying.Item;
                    m_currentTrack = currentlyPlayingItem as FullTrack;
                    // System.Diagnostics.Debug.WriteLine(m_currentTrack.Name);
                    
                    if (m_currentTrack != null)
                    {

                        title = m_currentTrack.Name;
                        

                        string currentArtists = string.Empty;
                        foreach (SimpleArtist currentArtist in m_currentTrack.Artists)
                        {
                            currentArtists += currentArtist.Name + ", ";
                        }

                        artist = currentArtists.Remove(currentArtists.Length - 2, 2);
                        // AlbumLabel.Content = m_currentTrack.Album.Name;
                        duration = new TimeSpan(0, 0, 0, 0, m_currentTrack.DurationMs).ToString(@"hh\:mm\:ss");
                    }
                }
                if (lastSong != title && !string.IsNullOrWhiteSpace(title))
                {
                    lastSong = title;
                    System.Diagnostics.Debug.WriteLine(title);
                    System.Diagnostics.Debug.WriteLine(artist);
                    System.Diagnostics.Debug.WriteLine(duration);

                    var theString = "Now listening to '" + title + "' by '" + artist + "' on Spotify";
                    var ot = new OutputText();
                    ot.outputLog(MainForm, theString);
                    ot.outputVRChat(MainForm, theString);
                }

               
            }

        }
        
       


       private static async Task OnErrorReceived(object sender, string error, string state)
       {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }


        public async Task SpotifyConnect()
        {
            /*

               // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!
               _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
               await _server.Start();

               // _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
               _server.ImplictGrantReceived += OnImplicitGrantReceived;
               _server.ErrorReceived += OnErrorReceived;

               // var request = new LoginRequest(_server.BaseUri, clientId, LoginRequest.ResponseType.Code)//for OnAuthorizationCodeReceived (do not use 'yet' requires putting secret in code or server)
               var request = new LoginRequest(_server.BaseUri, clientId, LoginRequest.ResponseType.Token) //for OnImplicitGrantReceived
               {
                   Scope = new List<string> { Scopes.UserReadCurrentlyPlaying }
               };
             BrowserUtil.Open(request.ToUri());

               */
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();


            _server.AuthorizationCodeReceived += GetCallback;// for PKCE aka best method because you dont have to pass a clientSecret
            _server.ErrorReceived += OnErrorReceived;


            var (verifier, challenge) = PKCEUtil.GenerateCodes();
            globalVerifier = verifier;

            var loginRequest = new LoginRequest(_server.BaseUri, clientId,LoginRequest.ResponseType.Code)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challenge,
                Scope = new[] { Scopes.UserReadCurrentlyPlaying }
            };
             BrowserUtil.Open(loginRequest.ToUri());
        }

        // This method should be called from your web-server when the user visits "http://localhost:5000/callback"
        private static async Task GetCallback(object sender, AuthorizationCodeResponse response)
        {
            System.Diagnostics.Debug.WriteLine("code: " + response.Code.ToString());
            // Note that we use the verifier calculated above!
            var initialResponse = await new OAuthClient().RequestToken(new PKCETokenRequest(clientId, response.Code, new Uri("http://localhost:5000/callback"), globalVerifier));

            //var spotify = new SpotifyClient(initialResponse.AccessToken);
            // Also important for later: response.RefreshToken
            myPKCEToken = initialResponse;
            
        }





    }
}
