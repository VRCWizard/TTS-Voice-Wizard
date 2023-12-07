using OSCVRCWiz.Services.Text;
using System.Windows.Shapes;

namespace OSCVRCWiz.Services.Speech.TextToSpeech
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
                Task.Run(() => DoSpeech.MainDoTTS(message));
            }
        }
        public static async Task PlayNextInQueue()
        {

            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true)
            {
                // OutputText.outputLog("Message finished playing the queue has this many messages:" + queueTTS.Count);


                if (isTTSPlaying == true)
                {

                    Task.Delay(int.Parse(VoiceWizardWindow.MainFormGlobal.textBoxQueueDelayBeforeNext.Text.ToString())).Wait();


                }
                isTTSPlaying = false;
                // Task.Delay(100);
                PlayNext();
            }

        }

        public static void QueueMessage(string text, string STTMode, string AzureTranslate = "[ERROR]")
        {
            try {
                if(text ==null)
                {
                    OutputText.outputLog("[Message Queue Error: No text found",Color.Red);
                    return;
                }
                text = text.Replace("\n", "");
                string inputText = text;
                string firstString = "";
                string secondString = "";
                int maxLength = 295;
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSmartStringSplit.Checked == true)
                {
                    maxLength = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSSSCharLimit.Text.ToString());

                    if (inputText.Length > maxLength)
                    {

                        if (char.IsWhiteSpace(inputText[maxLength]))
                        {
                            // Split at the exact 300th character where a space is found
                            firstString = inputText.Substring(0, maxLength);
                            secondString = inputText.Substring(maxLength + 1);
                            text = firstString;
                        }
                        else
                        {
                            int index = maxLength;
                            while (index >= 0 && !char.IsWhiteSpace(inputText[index]))
                            {
                                index--;
                            }

                            firstString = inputText.Substring(0, index);
                            secondString = inputText.Substring(index + 1);
                            text = firstString;
                        }


                    }
                }



                TTSMessageQueue.TTSMessage TTSMessageQueued = new TTSMessageQueue.TTSMessage();
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    TTSMessageQueued.text = text;
                    TTSMessageQueued.TTSMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.Text.ToString();
                    TTSMessageQueued.Voice = VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Text.ToString();
                    TTSMessageQueued.Accent = VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.Text.ToString();
                    TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.Text.ToString();
                    TTSMessageQueued.Pitch = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
                    TTSMessageQueued.Speed = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;
                    TTSMessageQueued.Volume = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
                    TTSMessageQueued.SpokenLang = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString();
                    TTSMessageQueued.TranslateLang = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString();
                    TTSMessageQueued.STTMode = STTMode;
                    TTSMessageQueued.AzureTranslateText = AzureTranslate;
                });
                if (STTMode == "Text")
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueTypedText.Checked == true)
                    {
                        TTSMessageQueue.Enqueue(TTSMessageQueued);
                    }
                    else
                    {
                        Task.Run(() => DoSpeech.MainDoTTS(TTSMessageQueued));
                    }

                }
                else {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true)
                    {

                        TTSMessageQueue.Enqueue(TTSMessageQueued);
                    }
                    else
                    {
                        Task.Run(() => DoSpeech.MainDoTTS(TTSMessageQueued));
                    }
                }

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSmartStringSplit.Checked == true)
                {
                    if (inputText.Length > maxLength)
                    {
                        QueueMessage(secondString, STTMode, AzureTranslate);
                    }
                }

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[TTS Queue Message Error: " + ex.Message + "]", Color.Red);
            {
               
            }
        }

            }

   



        }
}
