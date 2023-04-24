using OSCVRCWiz.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources
{
    public class TTSMessageQueue
    {
        public static Queue<TTSMessage> queueTTS = new Queue<TTSMessage>();
        public static bool isTTSPlaying = false;
        public struct TTSMessage //use then when setting up presets
        {
            public string text;
            public string TTSMode;
            public string Voice;
            public string Accent;
            public string SpokenLang;
            public string TranslateLang;
            public string Style;
            public int Pitch;
            public int Volume;
            public int Speed;
            public string STTMode;
            public string AzureTranslateText;
        }
        public static void Enqueue(TTSMessage message)
        {
          
            queueTTS.Enqueue(message);
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.labelQueueSize.Text = queueTTS.Count.ToString();
            });
           // OutputText.outputLog("Enqueued, queue has this many messages:" + queueTTS.Count);
            PlayNext();
        }
        private static void PlayNext()
        {
            if (!isTTSPlaying && queueTTS.Count > 0)
            {
                TTSMessage message = queueTTS.Dequeue();
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.labelQueueSize.Text = queueTTS.Count.ToString();
                });
                isTTSPlaying = true;
                VoiceWizardWindow.MainFormGlobal.MainDoTTS(message);
            }
        }
        public static async Task PlayNextInQueue()
        {
           
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true)
            {
                // OutputText.outputLog("Message finished playing the queue has this many messages:" + queueTTS.Count);
              
               
                if (isTTSPlaying==true)
                {
                    
                        Task.Delay(Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxQueueDelayBeforeNext.Text.ToString())).Wait();
                   
                   
                }
                isTTSPlaying = false;
               // Task.Delay(100);
               PlayNext();
            }
            
        }

    }
}
