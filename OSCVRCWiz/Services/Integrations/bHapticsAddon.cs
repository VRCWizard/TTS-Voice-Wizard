using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
//using bHapticsLib;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Integrations
{
    internal class bHapticsAddon
    {
        /*   private static byte[] TestPacket = new byte[bHapticsManager.MaxMotorsPerDotPoint] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
           private static HapticPattern testFeedback;
           private static HapticPattern testFeedbackSwapped;
           private static bHapticsConnection Connection;

           public static void OnStartUp()
           {

               string testFeedbackPath = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "testfeedback.tact");
               testFeedback = HapticPattern.LoadFromFile("testfeedback", testFeedbackPath);

               for (int i = 0; i < 30; i++)
                   testFeedbackSwapped = HapticPattern.LoadSwappedFromFile($"testFeedbackSwapped{i}", testFeedbackPath);

               Debug.WriteLine("Initializing...");

               bHapticsManager.Connect("bHapticsLib", "TestApplication", maxRetries: 0);

               Thread.Sleep(1000);

               Connection = new bHapticsConnection("bHapticsLib2", "AdditionalConnection", maxRetries: 0);
               Connection.BeginInit();

               Debug.WriteLine(Connection.Status);


               Debug.WriteLine($"{nameof(bHapticsManager.GetConnectedDeviceCount)}(): {bHapticsManager.GetConnectedDeviceCount()}");
               Debug.WriteLine($"{nameof(bHapticsManager.IsAnyDevicesConnected)}(): {bHapticsManager.IsAnyDevicesConnected()}");
           }
         */
    }






}