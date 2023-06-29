//using CSCore.XAudio2.X3DAudio;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Text;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace OSCVRCWiz.Services.Integrations
{
    public class ToastNotification
    {
        static UserNotificationListener listener;

        public static List<uint> alreadySeen = new List<uint>();
        public static void IsSupported()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                // Listener supported!

                OutputText.outputLog("[Toast Listener Supported]", Color.Green);
            }

            else
            {
                // Older version of Windows, no Listener
                OutputText.outputLog("[Toast Listener NOT Supported]", Color.Red);
            }
        }

        public async static void ToastListen()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
            {
                IsSupported();
            }
            // Get the listener
            UserNotificationListener listener = UserNotificationListener.Current;

            // And request access to the user's notifications (must be called from UI thread)
            UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();

            switch (accessStatus)
            {
                // This means the user has granted access.
                case UserNotificationListenerAccessStatus.Allowed:

                    // Yay! Proceed as normal
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                    {
                        OutputText.outputLog("[Toast Listener has Access]", Color.Green);
                    }
                    // Subscribe to foreground event
                    try
                    {

                        Task.Run(() => LoopListener());
                    }
                    catch (Exception ex)
                    {
                        //  OutputText.outputLog("[App must be run as administrator for Toast Listener to work]", Color.Green);
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                        {
                            OutputText.outputLog("Toast Notification Error: " + ex.Message, Color.Red);
                        }
                    }
                    break;

                // This means the user has denied access.
                // Any further calls to RequestAccessAsync will instantly
                // return Denied. The user must go to the Windows settings
                // and manually allow access.
                case UserNotificationListenerAccessStatus.Denied:

                    // Show UI explaining that listener features will not
                    // work until user allows access.
                    OutputText.outputLog("Toast Listener does not have access, go to windows settings to fix this!", Color.Red);
                    break;

                // This means the user closed the prompt without
                // selecting either allow or deny. Further calls to
                // RequestAccessAsync will show the dialog again.
                case UserNotificationListenerAccessStatus.Unspecified:

                    // Show UI that allows the user to bring up the prompt again
                    OutputText.outputLog("Toast Listener does not have access, go to windows settings to fix this!", Color.Red);
                    break;
            }



        }
        private static async void LoopListener()
        {
            while (true)
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked == true)
                {


                    UserNotificationListener listener = UserNotificationListener.Current;
                    IReadOnlyList<UserNotification> notifs = await listener.GetNotificationsAsync(NotificationKinds.Toast);
                    foreach (UserNotification noti in notifs)
                    {
                        if (!alreadySeen.Contains(noti.Id))
                        {
                            NotificationBinding toastBinding = noti.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                            if (toastBinding != null)
                            {
                                string appName = noti.AppInfo.DisplayInfo.DisplayName;
                                if (appName == "Discord")
                                {
                                    IReadOnlyList<AdaptiveNotificationText> textElements = toastBinding.GetTextElements();
                                    string username = textElements.FirstOrDefault()?.Text;
                                    if (username != null && username != "")
                                    {
                                        alreadySeen.Add(noti.Id);

                                        // string bodyText = string.Join("\n", textElements.Skip(1).Select(t => t.Text)); this will get the actuall text whhich u dont wanna dispaly for everyone to see


                                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                                        {
                                            OutputText.outputLog("Discord message recieved from " + username);

                                        }

                                        try
                                        {
                                            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                                            {
                                                var message0 = new CoreOSC.OscMessage(VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString(), true);
                                                OSC.OSCSender.Send(message0);
                                                ToastNotification.toastTimer.Change(int.Parse(VoiceWizardWindow.MainFormGlobal.textBoxDiscTimer.Text.ToString()), 0);
                                            });

                                        }
                                        catch (Exception ex)
                                        {
                                            OutputText.outputLog("[Discord Toast Error: " + ex.Message + "]", Color.Red);

                                        }



                                    }
                                }
                            }
                        }
                    }


                }
                Thread.Sleep(1000);
            }

        }


        public static System.Threading.Timer toastTimer;

        public static void initiateTimer()
        {
            toastTimer = new System.Threading.Timer(toasttimertick);
            toastTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void toasttimertick(object sender)
        {

            Thread t = new Thread(doToastTimerTick);
            t.Start();
        }

        private static void doToastTimerTick()  //Send Discord Parameter
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    var message0 = new CoreOSC.OscMessage(VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString(), false);
                    OSC.OSCSender.Send(message0);
                });
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Discord Toast Error: " + ex.Message + "]", Color.Red);

            }

        }


    }
}
