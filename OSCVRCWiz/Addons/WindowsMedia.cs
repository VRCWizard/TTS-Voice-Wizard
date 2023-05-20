using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using WindowsMediaController;
using OSCVRCWiz.Text;
using static WindowsMediaController.MediaManager; //allows for getting session
using CoreOSC;
using OSCVRCWiz.Resources;
using System.Diagnostics;

namespace OSCVRCWiz.Addons
{
    public class WindowsMedia
    {
        public static MediaManager mediaManager;
        public static string previousTitle = "-|-]][54h]nh734ngeg==--=";
        public static string mediaTitle = "";
        public static string mediaSource = "";
        public static string mediaArtist = "";
        public static string mediaStatus = "Paused";
        public static string mediaSourceNew = "";
        public static bool pauseMedia = false;
      //  private readonly static object _lock = new();
        static List<string> approvedMediaSourceList = new List<string>();
        private static MediaSession getSession = null;

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
       public static string getMediaProgress()
        {
            try
            {
                var time = "";
                if (getSession != null)
                {
                    if (getSession.ControlSession != null)
                    {
 
                            time = getSession.ControlSession.GetTimelineProperties().Position.ToString(@"mm\:ss").ToString();
                       
                    }
                }
                return time;
            }
            catch (Exception ex) {
                OutputText.outputLog("Progress Exception: " + ex.Message, Color.Red);
            }
            return "-:-";
        }
        public static string getMediaDuration()
        {
            try
            {
               
                var time = "";
                if (getSession != null)
                {
                    if (getSession.ControlSession != null)
                    {
  
                            time = getSession.ControlSession.GetTimelineProperties().EndTime.ToString(@"mm\:ss").ToString();
  
                    }
                }
                return time;
            }
            catch (Exception ex) {
                OutputText.outputLog("Duration Exception: " + ex.Message, Color.Red);
            }
            return "-:-";
        }
        public static string getMediaProgressHours()
        {
            try
            {
                  var time = "";
                if (getSession != null)
                {
                    if (getSession.ControlSession != null)
                    {
 
                            time = getSession.ControlSession.GetTimelineProperties().Position.ToString(@"hh\:mm\:ss").ToString();
    
                    }
                }
                return time;
                
            }
            catch (Exception ex) {
                OutputText.outputLog("Progress Exception: " + ex.Message, Color.Red);
            }
            return "-:-";
        }
        public static string getMediaDurationHours()
        {
            try
            {
                var time = "";
                if (getSession != null)
                {
                    if (getSession.ControlSession != null)
                    {
    
                            time = getSession.ControlSession.GetTimelineProperties().EndTime.ToString(@"hh\:mm\:ss").ToString();
     
                    }
                }
                return time;
                
            }
            catch (Exception ex) {
                OutputText.outputLog("Duration Exception: " + ex.Message, Color.Red);

            }
            return "-:-"; 
        }
        public static void MediaManager_OnAnySessionOpened(MediaManager.MediaSession session)
        {
          

          try  {

                
               if (session !=null)
                {
                   getSession = session;

                   
                   string info = "[Windows Media New Source: " + session.Id + "]";
                   
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true)
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
            catch (Exception ex) {
                OutputText.outputLog("MediaManager_OnAnySessionOpened Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnySessionOpened Exception " + ex.Message);
            }




        }
        private static void MediaManager_OnAnySessionClosed(MediaManager.MediaSession session)
        {
            try
            {
                string info = "[Windows Media Removed Source: " + session.Id + "]";
                //  var ot = new OutputText();
                if(VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked==true)
                {
                    Task.Run(() => OutputText.outputLog(info));
                }
                
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    // VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Remove(session.Id.ToString());
                });
            
             }
            catch (Exception ex) {
                OutputText.outputLog("MediaManager_OnAnySessionClosed Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnySessionClosed Exception " + ex.Message);
            }
}

        private static void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionPlaybackInfo args)
        {
            try {
            if (approvedMediaSourceList.Contains(sender.Id.ToString()) == true)
            {
                string info = $"[{sender.Id} is now {args.PlaybackStatus}]"; //use this info to disable media output perioidically like the spotify feature. (like spotifyPause and heartratePause)
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true)
                {
                    Task.Run(() => OutputText.outputLog(info));
                }//var ot = new OutputText();

                mediaStatus = args.PlaybackStatus.ToString();
                //      Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(VoiceWizardWindow.MainFormGlobal, mediaStatus));
            }
                //must also make option to output when paused
            }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnyPlaybackStateChanged Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnyPlaybackStateChanged Exception " + ex.Message);
            }


        }
        private static System.Threading.Timer mediaChangeTimer; // Declare a timer variable
        private static void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionMediaProperties args)
        {

            try
            {

              //  if (mediaChangeTimer == null)
             //   {
              //      mediaChangeTimer = new System.Threading.Timer(ThrottledMediaChangeHandler, null, 1000, Timeout.Infinite);



                    approvedMediaSourceList.Clear();
                    foreach (object Item in VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.CheckedItems)
                    {
                        approvedMediaSourceList.Add(Item.ToString());
                        System.Diagnostics.Debug.WriteLine(Item.ToString());
                    }

                    if (approvedMediaSourceList.Contains(sender.Id.ToString()) == true)
                    {
                    if (args.Title == mediaTitle)
                    {
                        Debug.WriteLine("double output prevented");
                        return;
                    }

                    string info = $"[{sender.Id} is now playing {args.Title} {(string.IsNullOrEmpty(args.Artist) ? "" : $"by {args.Artist}")}]";
                        //   var ot = new OutputText();
                        if (args.Title != previousTitle && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true)
                        {

                            Task.Run(() => OutputText.outputLog(info));


                        }
                        if (args.Artist != null)
                        {
                            mediaArtist = args.Artist;
                        }
                        if (args.Title != null)
                        {
                       


                            mediaTitle = args.Title;
                        }
                        if (sender.Id != null)
                        {
                            mediaSource = sender.Id;
                        }
                        //mediaStatus = "Playing";



                        if (args.Title != previousTitle)
                        {

                            var sp = new SpotifyAddon();
                            Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                        }
                    }
                    // previousTitle = args.Title;
              //  }
                
                
            
              }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnyMediaPropertyChanged Exception: " + ex.Message, Color.Red);
                MessageBox.Show("MediaManager_OnAnyMediaPropertyChanged Exception " + ex.Message);
            }

          }
      /*  private static void ThrottledMediaChangeHandler(object state)
        {
            // Reset the timer to allow the next media change event to be processed
            if (mediaChangeTimer != null)
            {
                mediaChangeTimer?.Dispose();
                mediaChangeTimer = null;
             //   Debug.WriteLine("double output prevented");
            }
            
        }*/
    }




}
