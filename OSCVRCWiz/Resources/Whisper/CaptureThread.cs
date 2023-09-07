using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Speech_Recognition;
using System.Diagnostics;
using Whisper;



namespace OSCVRCWiz.Resources.Whisper
{
    //MODIFIED FROM Const-me/Whisper/ example
    public sealed class CaptureThread : CaptureCallbacks
    {
        public CaptureThread(CommandLineArgs args, Context context, iAudioCapture source)
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                callbacks = new TranscribeCallbacks(args);
                this.context = context;
                this.source = source;
                thread = new Thread(threadMain) { Name = "Capture Thread" };
                thread.Start();


                var startTime = 0;
                bool couldntStart = false;
                while (!callbacks.WhisperStartedListening)
                {
                    if (++startTime > 12000)
                    {
                        //MessageBox.Show("Whisper is taking abnormally long to start.");
                        OutputText.outputLog("[Whisper is taking abornmally long to start, if this continues try restarting the app and select a smaller Whisper model]");
                        couldntStart = true;
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (!couldntStart)
                {
                    OutputText.outputLog("[Whisper Listening]");
                }
                stopwatch.Stop();
                TimeSpan elapsedTime = stopwatch.Elapsed;
                OutputText.outputLog($"[Whisper Startup time: {elapsedTime.TotalMilliseconds} ms]");
            

            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper CaptureThread Error: " + ex.Message.ToString());

            }
        }

    

        public void join()
        {
            try
            {
                thread.Join();
            }
            catch (Exception ex)
            {
                MessageBox.Show("[Whisper join Error: " + ex.Message.ToString());
            }
        }

        volatile bool shouldQuit = false;

        internal void Stop()
            => shouldQuit = true;

        protected override bool shouldCancel(Context sender) =>
            shouldQuit;

        protected override void captureStatusChanged(Context sender, eCaptureStatus status)
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.WhisperDebugLabel.Text = $"Whisper Debug: {status}";
            });
        }

        readonly TranscribeCallbacks callbacks;
        readonly Thread thread;
        readonly Context context;
        readonly iAudioCapture source;

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
