using OSCVRCWiz.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Control;
using WindowsMediaController;
using OSCVRCWiz.Text;


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

        public static async Task getWindowsMedia()
        {
            mediaManager = new MediaManager();

            mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
            mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
            mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
            mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;

            mediaManager.Start();

            //  mediaManager.Dispose(); // should dispose manually if nessicary, for instance if I want to stop media completely

        }
        private static void MediaManager_OnAnySessionOpened(MediaManager.MediaSession session)
        {
            string info = "[Windows Media New Source: " + session.Id + "]";
            //   var ot = new OutputText();
            Task.Run(() => OutputText.outputLog(info));
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
        private static void MediaManager_OnAnySessionClosed(MediaManager.MediaSession session)
        {
            string info = "[Windows Media Removed Source: " + session.Id + "]";
            //  var ot = new OutputText();
            Task.Run(() => OutputText.outputLog(info));
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                // VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Remove(session.Id.ToString());
            });
        }

        private static void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession sender, GlobalSystemMediaTransportControlsSessionPlaybackInfo args)
        {
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
    }




}
