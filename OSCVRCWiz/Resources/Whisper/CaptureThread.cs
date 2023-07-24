using OSCVRCWiz.Services.Text;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Whisper;
using Whisper.Internal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OSCVRCWiz.Resources.Whisper
{
    //MODIFIED FROM Const-me/Whisper/ example
    public sealed class CaptureThread : CaptureCallbacks
    {
        public static CaptureThread ctt = null;
        public CaptureThread(CommandLineArgs args, Context context, iAudioCapture source)
        {
            try
            {
                callbacks = new TranscribeCallbacks(args);
                this.context = context;
                this.source = source;

                thread = new Thread(threadMain) { Name = "Capture Thread" };
                thread.Start();
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper CaptureThread Error: " + ex.Message.ToString());

            }
        }

        static void readKeyCallback(object? state)
        {
            try
            {
                ctt = state as CaptureThread;



                // Console.ReadKey();
                //  ct.shouldQuit = true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper readKeyCallback Error: " + ex.Message.ToString());

            }
        }
        public static void stopWhisper()
        {
               
                ctt.shouldQuit = true;
                ctt.thread.Join();//wait for thread to finish to prevent crashing
                ctt = null;//free resources
  

        }

        public void join()
        {
            try
            {
                ThreadPool.QueueUserWorkItem(readKeyCallback, this);
                thread.Join();
                edi?.Throw();
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper join Error: " + ex.Message.ToString());
            }
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
                MessageBox.Show("[Whisper threadMain Error: " + ex.Message.ToString());
            }
        }
    }
}
