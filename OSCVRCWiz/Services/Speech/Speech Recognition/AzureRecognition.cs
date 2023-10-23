using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using CoreOSC;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Speech;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
using System;
using System.Diagnostics;

namespace OSCVRCWiz
{
    public class AzureRecognition
    {
        public static SpeechConfig speechConfig;
        public static SpeechTranslationConfig translationConfig;
        public static Microsoft.CognitiveServices.Speech.SpeechRecognizer speechRecognizer1;
        public static Microsoft.CognitiveServices.Speech.SpeechRecognizer speechRecognizerContinuous;
        public static TranslationRecognizer translationRecognizer1;
        public static string fromLanguage = "en-US";
        public static string toLanguage = "en";
        public static bool continuousListening = false;
        public static bool continuousListeningTranslate = false;
        public static bool profanityFilter = true;

        public static string YourSubscriptionKey;
        public static string YourServiceRegion;

        public static System.Threading.Timer AzureTypingTimer;
        public static string AzureTypingInterval = "2000";
        private static string AzureTyping = "";
     



        public static void speechSetup(string toLanguageFullname, string fromLanguageFullname)//speech to text setup

        {
            //Current Implementation of speechSetup is not useful because I must still call speechSetup before each TTS Operation (because of checking for if default mic is changed from control panel/window settings)
            //Only benefit is that the recognizers are resued but, I am not sure how helpful that is (do testing with both implementations, consider refactoring)
            //Since Setup is still run at the begining of TTS still i can comment out all other occurences of speechSetup
            try
            {
                speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
                translationConfig = SpeechTranslationConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);
                // speechConfig.SetProperty(PropertyId.Speech_LogFilename, "logfile.txt"); //This line of code was the cause for an outstanding bug, if the log file becomes too full it causes issue. Further testing required before adding logging back as a feature.
                fromLanguage = LanguageSelect.fromLanguageNew(fromLanguageFullname, "sourceLanguage", "Azure");
                toLanguage = LanguageSelect.fromLanguageNew(toLanguageFullname, "targetLanguage", "Pro");
              //  fromLanguageID(fromLanguageFullname); //Convert information from selected spoken language and sets fromLanuage to the ID
              //  toLanguageID(toLanguageFullname);//Convert information from selected translation language and sets toLanuage to the ID


                speechConfig.SpeechRecognitionLanguage = fromLanguage;

                if (profanityFilter == false)
                {
                    translationConfig.SetProfanity(ProfanityOption.Raw);
                    speechConfig.SetProfanity(ProfanityOption.Raw);
                }
                if (profanityFilter == true)
                {
                    translationConfig.SetProfanity(ProfanityOption.Masked);
                    speechConfig.SetProfanity(ProfanityOption.Masked);
                }
                translationConfig.SpeechRecognitionLanguage = fromLanguage;
                translationConfig.AddTargetLanguage(toLanguage);
                var audioConfig = AudioConfig.FromMicrophoneInput(AudioDevices.currentInputDevice); 

                if (AudioDevices.currentInputDeviceName == "Default")
                {
                    audioConfig = AudioConfig.FromDefaultMicrophoneInput();

                }
                if (continuousListening == false)//SO THAT STOPPING IT ACTUALLY WORKS
                {
                    translationRecognizer1 = new TranslationRecognizer(translationConfig, audioConfig);
                    speechRecognizer1 = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);
                }

                //THESE SUBSCRIPTIONS ARE FOR CONTINUOUS LISTENING (I DO NEED TO REMOVE REDUNDENCY)
                translationRecognizer1.Canceled += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Result.Text);
                   // var ot = new OutputText(); 
                    Task.Run(() => OutputText.outputLog("[Azure Speech Recognition Canceled (Translating): " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]", Color.Red));

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }
                };

                translationRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == true)
                    {
                        var speechRecognitionResult = eventArgs.Result;


                        var text = speechRecognitionResult.Text; //Dictation string
                        string translatedString = speechRecognitionResult.Translations[toLanguage]; //Dictation string tranlated



                      //  StopAzureTypingTimer();
                        TTSMessageQueue.QueueMessage(text, "Azure Translate", translatedString);

                      

                    }

                };
                translationRecognizer1.Recognizing += (sender, eventArgs) =>
                {
                    AzureTyping = eventArgs.Result.Text;
                  //  OutputText.outputLog(eventArgs.Result.Text);
                };

                speechRecognizer1.Recognizing += (sender, eventArgs) =>
                {
                    AzureTyping = eventArgs.Result.Text;
                   // OutputText.outputLog(eventArgs.Result.Text);
                };
                speechRecognizer1.Canceled += (sender, eventArgs) =>
                {
                   // Console.WriteLine(eventArgs.Result.Text);
                   // var ot = new OutputText();
                    Task.Run(() => OutputText.outputLog("[Azure Speech Recognition Canceled: " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]", Color.Red));
                    OutputText.outputLog("[If this issue occurs often try searching the discord server. The solution has likely already been documented]", Color.DarkOrange);

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }

                };
                speechRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == true)
                    {
                        var text = eventArgs.Result.Text; //Dictation string


                        //    Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text,"Azure"));
                       // StopAzureTypingTimer();
                        TTSMessageQueue.QueueMessage(text, "Azure");




                    }
                };
                ///Phrase List
                var phraseList = PhraseListGrammar.FromRecognizer(speechRecognizer1);
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked == true)
                {
                    string words = VoiceWizardWindow.MainFormGlobal.richTextBoxAzureDict.Text.ToString();

                    string[] split = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in split)
                    {

                        if (s.Trim() != "")
                            phraseList.AddPhrase(s);
                        System.Diagnostics.Debug.WriteLine("Phrase Added: " + s);

                    }
                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked == false)
                {
                    System.Diagnostics.Debug.WriteLine("Phrase list cleared");
                    phraseList.Clear();
                }

            }
            catch (Exception ex)
            {
               // MessageBox.Show("Speech Setup Failed: Make sure that you have setup your Azure Key and Region in the Provider tab (click the 'apply' buttons to apply changes) Reason:" + ex.Message.ToString());
                OutputText.outputLog("[Azure Speech Setup Failed: " + ex.Message + "]", Color.Red);
                OutputText.outputLog("[ Make sure that you have setup your Azure Key and Region in the Provider tab (click the 'apply' buttons to apply changes). Make sure that you have an input and output device selected in Settings > Audio]", Color.DarkOrange);

            }
        }
      
        public static async void speechTTTS(string fromLanguageFullname)//speech to text
        {
            StartAzureTypingTimer();

            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == false)
                {
                    DoSpeech.speechToTextOnSound();
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                        OSC.OSCSender.Send(sttListening);
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        OSCListener.pauseBPM = true;
                        SpotifyAddon.pauseSpotify = true;
                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                        OSC.OSCSender.Send(typingbubble);

                    }
                    var speechRecognitionResult = await speechRecognizer1.RecognizeOnceAsync();
                    var text = speechRecognitionResult.Text; //Dictation string

                    StopAzureTypingTimer();
                    AzureTyping = "";
                    TTSMessageQueue.QueueMessage(text, "Azure");
                    DoSpeech.speechToTextButtonOff();

                }


                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    DoSpeech.speechToTextOnSound();
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");
                    //  var ot = new OutputText();
                    OutputText.outputLog("[Azure Continuous Listening Enabled]");

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                        OSC.OSCSender.Send(sttListening);
                    }

                    await speechRecognizer1.StartContinuousRecognitionAsync();

                }
                else if (continuousListening == true)
                {
                    DoSpeech.speechToTextOffSound();
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");
                    await speechRecognizer1.StopContinuousRecognitionAsync();
                    OutputText.outputLog("[Azure Continuous Listening Disabled]");
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Azure STTTS Failed: " + ex.Message.ToString());

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
                DoSpeech.speechToTextButtonOff();

            }
        }
        public static async void translationSTTTS(string toLanguageFullname, string fromLanguageFullname)//translate speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
           try
            {


                System.Diagnostics.Debug.WriteLine($"Say something in '{fromLanguage}' and ");
                System.Diagnostics.Debug.WriteLine($"we'll translate into '{toLanguage}'.\n");
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == false)
                {
                    DoSpeech.speechToTextOnSound();
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        OSCListener.pauseBPM = true;
                        SpotifyAddon.pauseSpotify = true;
                        var typingbubble = new CoreOSC.OscMessage("/chatbox/typing", true);
                        OSC.OSCSender.Send(typingbubble);

                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                        OSC.OSCSender.Send(sttListening);
                    }

                    var speechRecognitionResult = await translationRecognizer1.RecognizeOnceAsync();

                    if (speechRecognitionResult.Reason == ResultReason.TranslatedSpeech)
                    {
                        System.Diagnostics.Debug.WriteLine($"Recognized: \"{speechRecognitionResult.Text}\"");
                        System.Diagnostics.Debug.WriteLine($"Translated into '{toLanguage}': {speechRecognitionResult.Translations[toLanguage]}");
                    }

                    var text = speechRecognitionResult.Text.ToString(); //Dictation string; Global string used to keep track of result text for default azure speech to text
                    string translatedString = speechRecognitionResult.Translations[toLanguage]; //Global string used to keep track of result text for translation azure speech to text

                    // Task.Run(() => VoiceWizardWindow.MainFormGlobal.MainDoTTS(text, "Azure Translate",translatedString));

                    StopAzureTypingTimer();
                    AzureTyping = "";
                    TTSMessageQueue.QueueMessage(text, "Azure Translate", translatedString);
                    DoSpeech.speechToTextButtonOff();

                }
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    DoSpeech.speechToTextOnSound();
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");

                    OutputText.outputLog("[Azure Continuous Listening Enabled (Translating)]");

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", true);
                        OSC.OSCSender.Send(sttListening);
                    }

                    await translationRecognizer1.StartContinuousRecognitionAsync();

                }
                else if (continuousListening == true)
                {
                    DoSpeech.speechToTextOffSound();
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                    await translationRecognizer1.StopContinuousRecognitionAsync();
                    //   speechRecognizer1.Dispose();
                    //  var ot = new OutputText();
                    OutputText.outputLog("[Azure Continuous Listening Disabled (Translating)]");

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }
                }

           }
            catch (Exception ex)
            {
                OutputText.outputLog("Azure Translation STTTS Failed: Most likely your voice was not picked up by your microphone. Reason: " + ex.Message.ToString());

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }

            }
        }
        public static async void stopContinuousListeningNow()//speech to text
        {
            if (continuousListening == true)
            {
                StopAzureTypingTimer();
                AzureTyping = "";
                continuousListening = false;
                // Make the following call at some point to stop recognition:
                System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                await translationRecognizer1.StopContinuousRecognitionAsync();
                await speechRecognizer1.StopContinuousRecognitionAsync();
                OutputText.outputLog("[Azure Continuous Listening Disabled (Any)]");

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }
        }


        public static void StartAzureTypingTimer()
        {
           AzureTypingTimer = new System.Threading.Timer(heartratetimertick);
           AzureTypingTimer.Change(Int32.Parse(AzureTypingInterval), Int32.Parse(AzureTypingInterval)-1000);
          


        }
        public static void StopAzureTypingTimer()
        {
            
            if (AzureTypingTimer != null)
            {
                AzureTypingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public static void heartratetimertick(object sender)
        {
            Thread t = new Thread(doAzureTypingTimerTick);
            t.Start();
        }

        private static async void doAzureTypingTimerTick()
        {
            if (VoiceWizardWindow.MainFormGlobal.rjTogglePartialResults.Checked)
            {
                if (AzureTyping != "")
                {



                    var messageSpeechBubble = new OscMessage("/chatbox/input", AzureTyping, true, false);
                    OSC.OSCSender.Send(messageSpeechBubble);
                    OutputText.outputLog($"[Partial Results]: {AzureTyping}");

                    AzureTypingTimer.Change(Int32.Parse(AzureTypingInterval), 0);
                }
            }
            
            
        }


     }
}