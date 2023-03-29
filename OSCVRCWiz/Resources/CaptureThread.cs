using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Whisper;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OSCVRCWiz.Resources
{
       //MODIFIED FROM Const-me/Whisper/ example
       public sealed class CaptureThread : CaptureCallbacks
       {
           public static CaptureThread ctt =null;
           public CaptureThread(CommandLineArgs args, Context context, iAudioCapture source)
           {
               callbacks = new TranscribeCallbacks(args);
               this.context = context;
               this.source = source;

               thread = new Thread(threadMain) { Name = "Capture Thread" };
             //  Debug.WriteLine("Press any key to quit");
               thread.Start();
           }

           static void readKeyCallback(object? state)
           {
            
               ctt = (state as CaptureThread) ?? throw new ApplicationException();
              // Console.ReadKey();
             //  ct.shouldQuit = true; //this line has been making it quit automatically dumb dumb
           }
           public static void stopWhisper()
           {
               ctt.shouldQuit = true;
               ctt = null;
           }

           public void join()
           {
               ThreadPool.QueueUserWorkItem(readKeyCallback, this);
               thread.Join();
              edi?.Throw();
           }

           volatile bool shouldQuit = false;

           protected override bool shouldCancel(Context sender) =>
               shouldQuit;

           protected override void captureStatusChanged(Context sender, eCaptureStatus status)
           {
               Debug.WriteLine($"CaptureStatusChanged: {status}");
           }

           readonly TranscribeCallbacks callbacks;
           readonly Thread thread;
           readonly Context context;
           readonly iAudioCapture source;
           ExceptionDispatchInfo? edi = null;

           void threadMain()
           {
               try
               {
                   context.runCapture(source, callbacks, this);

               }
               catch (Exception ex)
               {
                   edi = ExceptionDispatchInfo.Capture(ex);
                   Debug.WriteLine(ex.Message.ToString());
               }
           }
       }
}
