using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using WindowsMediaController;
using OSCVRCWiz.Text;
using static WindowsMediaController.MediaManager; //allows for getting session

namespace OSCVRCWiz.Addons
{
    public class WindowsMedia
    {
        static MediaManager mediaManager;
        public static string previousTitle = "-|-]][54h]nh734ngeg==--=";
        public static string mediaTitle = "";
        public static string mediaSource = "";
        public static string mediaArtist = "";
        public static string mediaStatus = "Paused";
        public static string mediaSourceNew = "";
        public static bool pauseMedia = false;
        private readonly static object _lock = new();
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
            try
            {
                getSession = session;
                string info = "[Windows Media New Source: " + session.Id + "]";
                //   var ot = new OutputText();
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
            catch (Exception ex) {
                OutputText.outputLog("MediaManager_OnAnySessionOpened Exception: " + ex.Message, Color.Red);
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
            }


        }

        private static void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionMediaProperties args)
        {

            //  //used to gather the approved soruce list and split/store entries in actual list for later.
            //     string words = "";
            //    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            //  {
            //  words = VoiceWizardWindow.MainFormGlobal.richTextBox11.Text.ToString();

            // });

            //  VoiceWizardWindow.approvedMediaSourceList.Clear();
            //  string[] split = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //   foreach (string s in split)
            //    {
            //       string trimmed = s.Trim();
            //       if (trimmed != "")
            //       {
            //           VoiceWizardWindow.approvedMediaSourceList.Add(trimmed);
            //       }
            //       System.Diagnostics.Debug.WriteLine(trimmed);
            //    }
            //   System.Diagnostics.Debug.WriteLine(VoiceWizardWindow.approvedMediaSourceList.Count);
            try
            {
                approvedMediaSourceList.Clear();
                foreach (object Item in VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.CheckedItems)
                {
                    approvedMediaSourceList.Add(Item.ToString());
                    System.Diagnostics.Debug.WriteLine(Item.ToString());
                }

                if (approvedMediaSourceList.Contains(sender.Id.ToString()) == true)
                {

                    string info = $"[{sender.Id} is now playing {args.Title} {(string.IsNullOrEmpty(args.Artist) ? "" : $"by {args.Artist}")}]";
                    //   var ot = new OutputText();
                    if (args.Title != previousTitle && VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked == true)
                    {

                        Task.Run(() => OutputText.outputLog(info));


                    }
                    if (args.Artist != null)
                    {
                        mediaTitle = args.Title;
                        mediaArtist = args.Artist;
                        mediaSource = sender.Id;
                        //mediaStatus = "Playing";


                    }
                    if (args.Title != previousTitle)
                    {
                        var sp = new SpotifyAddon();
                        Task.Run(() => SpotifyAddon.windowsMediaGetSongInfo());
                    }
                    previousTitle = args.Title;

                }
            
              }
            catch (Exception ex)
            {
                OutputText.outputLog("MediaManager_OnAnyMediaPropertyChanged Exception: " + ex.Message, Color.Red);
            }

}
    }




}
