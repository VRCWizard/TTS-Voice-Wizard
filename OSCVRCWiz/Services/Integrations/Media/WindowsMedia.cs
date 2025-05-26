using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using WindowsMediaController;
using static WindowsMediaController.MediaManager; //allows for getting session
using CoreOSC;
using OSCVRCWiz.Resources;
using System.Diagnostics;
using OSCVRCWiz.Services.Text;
using System.Windows.Forms;
using EmbedIO.Sessions;

namespace OSCVRCWiz.Services.Integrations.Media
{
    public class WindowsMedia
    {
        public static MediaManager mediaManager;
        public static string previousTitle = "-|-]][54h]nh734ngeg==--=";
        public static string mediaTitle = "";
        public static string mediaSource = "";
        public static string mediaArtist = "";
        public static string mediaAlbumArtist = "";
        public static string mediaAlbumTitle = "";
        public static string mediaStatus = "Paused";
        public static string mediaSourceNew = "";
        public static bool pauseMedia = false;
        //  private readonly static object _lock = new();
        static List<string> approvedMediaSourceList = new List<string>();
        private static MediaSession getSession = null;

        private static DateTime? playbackStartTime = null;
        private static TimeSpan lastKnownProgress = TimeSpan.Zero;

        //  static TimeSpan newProgress = TimeSpan.FromMinutes(0);
        // static TimeSpan newDuration = TimeSpan.FromMinutes(0);

        public static async Task getWindowsMedia()
        {
            try
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonDisableWindowsMedia.Checked == false)
                {
                    mediaManager = new MediaManager();

                    mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
                    mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
                    mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
                    mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;
                 //   mediaManager.OnAnyTimelinePropertyChanged += MediaManager_OnAnyTimelinePropertyChanged;
                 //   mediaManager.OnFocusedSessionChanged += MediaManager_OnFocusedSessionChanged;


                    mediaManager.Start();

                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("Windows Media Startup Exception: " + ex.Message, Color.Red);
            }

            //  mediaManager.Dispose(); // should dispose manually if nessicary, for instance if I want to stop media completely

        }
        //if time "" then there is no session
        // if time 00:00/00:00 could not get time
        //if time -/- then there was an error
        /*  private static void MediaManager_OnAnyTimelinePropertyChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionTimelineProperties args)
          {
             // OutputText.outputLog($"{sender.Id} timeline is now {args.Position}/{args.EndTime}");
              if (sender.Id == mediaSource)
              {
                  newProgress = args.Position;
                  newDuration = args.EndTime;
              }
          }
          private static void MediaManager_OnFocusedSessionChanged(MediaManager.MediaSession mediaSession)
          {
             // OutputText.outputLog("== Session Focus Changed: " + mediaSession?.ControlSession?.SourceAppUserModelId);
          }*/

        /*public static TimeSpan getMediaProgress()
        {
            TimeSpan time = TimeSpan.FromMinutes(0);
            try
            {

                if (getSession != null)
                {
                    if (getSession.ControlSession != null)
                    {


                       time = getSession.ControlSession.GetTimelineProperties().Position;

                   }
                }
                return time;
            }
            catch (Exception ex)
            {
                OutputText.outputLog("Progress Exception: " + ex.Message, Color.Red);
            }
            return time;
        }*/
        public static TimeSpan getMediaProgress()
        {
            try
            {
                if (getSession != null && getSession.ControlSession != null)
                {
                    var currentPosition = getSession.ControlSession.GetTimelineProperties().Position;
                    if (currentPosition > TimeSpan.Zero)
                    {
                        lastKnownProgress = currentPosition;
                       // if (mediaStatus == "Playing")
                          //  playbackStartTime = DateTime.Now - currentPosition;

                       // return currentPosition;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("Progress Exception: " + ex.Message, Color.Red);
            }

            if (mediaStatus == "Paused")
            {
                return lastKnownProgress;
            }

            if (playbackStartTime.HasValue)
            {
                TimeSpan estimated = DateTime.Now - playbackStartTime.Value;
                var duration = getMediaDuration();
                return estimated < duration ? estimated : duration;
            }

            return lastKnownProgress;
        }
        public static TimeSpan getMediaDuration()
         {
             TimeSpan time = TimeSpan.FromMinutes(0);
             try
             {


                 if (getSession != null)
                 {
                     if (getSession.ControlSession != null)
                     {

                        time = getSession.ControlSession.GetTimelineProperties().EndTime;

                    }
                 }
                 return time;
             }
             catch (Exception ex)
             {
                 OutputText.outputLog("Duration Exception: " + ex.Message, Color.Red);
             }
             return time;
         } 

        public static void MediaManager_OnAnySessionOpened(MediaSession session)
        {


            try
            {


                if (session != null)
                {
                    getSession = session;
                    

                    string info = "[Windows Media New Source: " + session.Id + "]";

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)
                    {
                        Task.Run(() => OutputText.outputLog(info));
                    }
                    mediaSourceNew = session.Id;
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                     {
                         bool inThere = false;
                         for (int i = 0; i < VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Count; i++)
                         {

                             if (VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items[i].ToString() == session.Id.ToString())
                             {
                                 inThere = true;
                             }
                         }

                         if (inThere == false)
                         {
                             VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Add(session.Id.ToString());
                         }

                     });
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnySessionOpened Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnySessionOpened Exception " + ex.Message);
            }




        }
        private static void MediaManager_OnAnySessionClosed(MediaSession session)
        {
            try
            {
                string info = "[Windows Media Removed Source: " + session.Id + "]";
                //  var ot = new OutputText();
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)
                {
                    Task.Run(() => OutputText.outputLog(info));
                }

               

            }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnySessionClosed Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnySessionClosed Exception " + ex.Message);
            }
        }

        private static void MediaManager_OnAnyPlaybackStateChanged(MediaSession sender, GlobalSystemMediaTransportControlsSessionPlaybackInfo args)
        {
            try
            {
                if (approvedMediaSourceList.Contains(sender.Id.ToString()) == true)
                {
                    
                    
                    string info = $"[{sender.Id} is now {args.PlaybackStatus}]"; //use this info to disable media output perioidically like the spotify feature. (like spotifyPause and heartratePause)
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)
                    {
                        Task.Run(() => OutputText.outputLog(info));
                    }

                    mediaStatus = args.PlaybackStatus.ToString();

                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        if(mediaStatus =="Playing")
                        {
                            //Playing

                            VoiceWizardWindow.MainFormGlobal.labelMediaPaused.ForeColor = Color.Green;
                            //OutputText.outputLog("Debug Song playing");
                            playbackStartTime = DateTime.Now - lastKnownProgress;
                        }
                        else
                        {
                            //Paused
                            VoiceWizardWindow.MainFormGlobal.labelMediaPaused.ForeColor = Color.White;
                            //OutputText.outputLog("Debug Song Paused");
                            lastKnownProgress = getMediaProgress();
                            playbackStartTime = null;
                        }
                        VoiceWizardWindow.MainFormGlobal.labelMediaPaused.Text = "Media " + mediaStatus;
                    });
                    

                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnyPlaybackStateChanged Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnyPlaybackStateChanged Exception " + ex.Message);
            }


        }
        private static void MediaManager_OnAnyMediaPropertyChanged(MediaSession sender, GlobalSystemMediaTransportControlsSessionMediaProperties args)
        {

            try
            {

               

                approvedMediaSourceList.Clear();
                foreach (object Item in VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.CheckedItems)
                {
                    approvedMediaSourceList.Add(Item.ToString());
                    Debug.WriteLine(Item.ToString());
                }

                if (approvedMediaSourceList.Contains(sender.Id.ToString()) == true)
                {
                   // sender.ControlSession.TryPlayAsync();
         

                    if (args.Title == mediaTitle)
                    {
                        Debug.WriteLine("double output prevented");
                        return;
                    }

                    string info = $"[{sender.Id} is now playing {args.Title} {(string.IsNullOrEmpty(args.Artist) ? "" : $"by {args.Artist}")}]";
                    //   var ot = new OutputText();
                    if (args.Title != previousTitle && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked == true)
                    {
                        Task.Run(() => OutputText.outputLog(info));
                    }
                    if (args.Artist != null)
                    {
                        mediaArtist = args.Artist;
                    }
                    if (args.AlbumArtist != null)
                    {
                        mediaAlbumArtist = args.AlbumArtist;
                    }
                    if (args.AlbumTitle != null)
                    {
                        mediaAlbumTitle = args.AlbumTitle;
                    }
                    if (args.Title != null)
                    {
                        mediaTitle = args.Title;
                    }
                    if (sender.Id != null)
                    {
                        mediaSource = sender.Id;
                    }

                    if (args.Title != previousTitle)
                    {
                        Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                    }

                  //  OutputText.outputLog("New song, refreshing estimates");
                    lastKnownProgress = TimeSpan.Zero;
                    playbackStartTime = DateTime.Now;
                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnyMediaPropertyChanged Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnyMediaPropertyChanged Exception " + ex.Message);
            }

        }

        public static void GetSoundPadMedia()
        {
        
        
                Process soundPadProcess = Process.GetProcessesByName("Soundpad").FirstOrDefault();

            if (soundPadProcess != null)
            {
                if (soundPadProcess.MainWindowTitle != "Soundpad")
                {
                    if (soundPadProcess.MainWindowTitle.StartsWith(" II  Soundpad - "))
                    {

                        string soundName = soundPadProcess.MainWindowTitle.Replace(" II  Soundpad - ", "");
                        mediaTitle = soundName;
                        mediaSource = "Soundpad";
                        mediaArtist = "";
                        mediaStatus = "Paused";
                    }


                    else 
                    {

                        string soundName = soundPadProcess.MainWindowTitle.Replace("Soundpad - ", "");
                        mediaTitle = soundName;
                        mediaSource = "Soundpad";
                        mediaArtist = "";
                        mediaStatus = "Playing";
                    }
                  //  Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                    // Do something with the soundName
                }
                else
                {
                    mediaTitle = "";
                    mediaSource = "Soundpad";
                    mediaArtist = "";
                  //  Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                }
            }
           
        }
        public static void addSoundPad()
        {
            Process soundPadProcess = Process.GetProcessesByName("Soundpad").FirstOrDefault();

            if (soundPadProcess != null)
            {
                bool inThere = false;
                for (int i = 0; i < VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Count; i++)
                {

                    if (VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items[i].ToString() == "Soundpad")
                    {
                        inThere = true;
                    }
                }
                if (inThere == false)
                {

                    VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Add("Soundpad");
                }
            }
        }
      

    }




}
