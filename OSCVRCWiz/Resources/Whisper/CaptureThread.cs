using OSCVRCWiz.Services.Text;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Windows.Shapes;
using Whisper;
using Whisper.Internal;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OSCVRCWiz.Resources.Whisper
{
    //MODIFIED FROM Const-me/Whisper/ example
    public sealed class CaptureThread : CaptureCallbacks
    {
        public CaptureThread(CommandLineArgs args, Context context, iAudioCapture source)
        {
            try
            {
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
                        MessageBox.Show("Whisper is taking abnormally long to start.");
                        couldntStart = true;
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (!couldntStart)
                {
                    OutputText.outputLog("[Whisper Listening]");
                }


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
