using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class HomeScreenBanner
    {
        //  public static System.Threading.Timer rotationTimer;

        static int bannerPage = 0;
        public static void initiateTimer()
        {
            //  rotationTimer = new System.Threading.Timer(rotationtimertick);
            //   rotationTimer.Change(25000, 0);
        }

        public static void rotationtimertick(object sender)
        {

            Thread t = new Thread(doRotationTimerTick);
            t.Start();
        }

        private static void doRotationTimerTick() //Home Screen Banner Rotation
        {
            /* if (!VoiceWizardWindow.MainFormGlobal.webView21.IsDisposed)
             {
                 try
                 {
                     VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                     {
                         if (!VoiceWizardWindow.MainFormGlobal.webView21.Source.ToString().Contains("ko-fi"))
                         {
                             if (bannerPage == 0)
                             {
                                 Uri uri = new Uri("https://voicewizardpro.carrd.co/");
                                 VoiceWizardWindow.MainFormGlobal.webView21.Source = uri;
                                 bannerPage = 1;
                             }
                             else
                             {
                                 Uri uri = new Uri("https://voicewizardsponsors.carrd.co/");
                                 VoiceWizardWindow.MainFormGlobal.webView21.Source = uri;
                                 bannerPage = 0;
                             }
                         }
                     });
                     Debug.WriteLine("it happened" + bannerPage);
                 }
                 catch (Exception ex)
                 {

                 }
                 rotationTimer.Change(25000, 0);
             }*/
        }
    }
}
