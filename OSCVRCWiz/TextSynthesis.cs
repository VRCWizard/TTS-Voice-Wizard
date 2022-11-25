using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace OSCVRCWiz
{
    public class TextSynthesis
    {
        public SpeechConfig speechConfig;
        public SpeechTranslationConfig translationConfig;
        public static Microsoft.CognitiveServices.Speech.SpeechRecognizer speechRecognizer1;
        public static Microsoft.CognitiveServices.Speech.SpeechRecognizer speechRecognizerContinuous;
        public static TranslationRecognizer translationRecognizer1;
        public string fromLanguage = "en-US";
        public string toLanguage = "en";
        public static bool continuousListening = false;
        public static bool continuousListeningTranslate = false;
       // public static CancellationTokenSource SpeechCt = new(); // using cancellation tokens caused more issue then for how helpful it could be.

        public void fromLanguageID(string fullname)
        {
             fromLanguage = "en-US";
            switch (fullname)
            {
                case "Arabic [ar-EG]": fromLanguage = "ar-EG"; break;
                case "Chinese [zh-CN]": fromLanguage = "zh-CN"; break;
                case "Czech [cs-CZ]": fromLanguage = "cs-CZ"; break;
                case "Danish [da-DK]": fromLanguage = "da-DK"; break;
                case "Dutch [nl-NL]": fromLanguage = "nl-NL"; break;
                case "English [en-US] (Default)": fromLanguage = "en-US"; break;
                case "Estonian [et-EE]": fromLanguage = "et-EE"; break;
                case "Filipino [fil-PH]": fromLanguage = "fil-PH"; break;
                case "Finnish [fi-FI]": fromLanguage = "fi-FI"; break;
                case "French [fr-FR]": fromLanguage = "fr-FR"; break;
                case "German [de-DE]": fromLanguage = "de-DE"; break;
                case "Hindi [hi-IN]": fromLanguage = "hi-IN"; break;
                case "Hungarian [hu-HU]": fromLanguage = "hu-HU"; break;
                case "Irish [ga-IE]": fromLanguage = "ga-IE"; break;
                case "Italian [it-IT]": fromLanguage = "it-IT"; break;
                case "Japanese [ja-JP]": fromLanguage = "ja-JP"; break;
                case "Korean [ko-KR]": fromLanguage = "ko-KR"; break;
                case "Norwegian [nb-NO]": fromLanguage = "nb-NO"; break;
                case "Polish [pl-PL]": fromLanguage = "pl-PL"; break;
                case "Portuguese [pt-BR]": fromLanguage = "pt-BR"; break;
                //place holder^^
                case "Russian [ru-RU]": fromLanguage = "ru-RU"; break;
                case "Spanish [es-MX]": fromLanguage = "es-MX"; break;
                //place holder^^
                case "Swedish [sv-SE]": fromLanguage = "sv-SE"; break;
                case "Thai [th-TH]": fromLanguage = "th-TH"; break;
                case "Ukrainian [uk-UA]": fromLanguage = "uk-UA"; break;
                case "Vietnamese [vi-VN]": fromLanguage = "vi-VN"; break;
                default: fromLanguage = "en-US"; break; // if translation to english happens something is wrong
            }       
        }

        public void toLanguageID(string fullname)
        {
            toLanguage = "en";
          

            switch (fullname)
            {
                case "Arabic [ar]": toLanguage = "ar"; break;
                case "Chinese [zh]": toLanguage = "zh-Hans"; break;
                case "Czech [cs]": toLanguage = "cs"; break;
                case "Danish [da]": toLanguage = "da"; break;
                case "Dutch [nl]": toLanguage = "nl"; break;
                case "English [en]": toLanguage = "en"; break;
                case "Estonian [et]": toLanguage = "et"; break;
                case "Filipino [fil]": toLanguage = "fil"; break;
                case "Finnish [fi]": toLanguage = "fi"; break;
                case "French [fr]": toLanguage = "fr"; break;
                case "German [de]": toLanguage = "de"; break;
                case "Hindi [hi]": toLanguage = "hi"; break;
                case "Hungarian [hu]": toLanguage = "hu"; break;
                case "Irish [ga]": toLanguage = "ga"; break;
                case "Italian [it]": toLanguage = "it"; break;
                case "Japanese [ja]": toLanguage = "ja"; break;
                case "Korean [ko]": toLanguage = "ko"; break;
                case "Norwegian [nb]": toLanguage = "nb"; break;
                case "Polish [pl]": toLanguage = "pl"; break;
                case "Portuguese [pt]": toLanguage = "pt"; break;
                case "Russian [ru]": toLanguage = "ru"; break;
                case "Spanish [es]": toLanguage = "es"; break;
                case "Swedish [sv]": toLanguage = "sv"; break;
                case "Thai [th]": toLanguage = "th"; break;
                case "Ukrainian [uk]": toLanguage = "uk"; break;
                case "Vietnamese [vi]": toLanguage = "vi"; break;
                default: toLanguage = "en"; break; // if translation to english happens something is wrong
            }
        }

        public void speechSetup(VoiceWizardWindow MainForm, string toLanguageFullname, string fromLanguageFullname)//speech to text setup

        {
            //Current Implementation of speechSetup is not useful because I must still call speechSetup before each TTS Operation (because of checking for if default mic is changed from control panel/window settings)
            //Only benefit is that the recognizers are resued but, I am not sure how helpful that is (do testing with both implementations, consider refactoring)
            //Since Setup is still run at the begining of TTS still i can comment out all other occurences of speechSetup
            try
            {
                speechConfig = SpeechConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
                translationConfig = SpeechTranslationConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
               // speechConfig.SetProperty(PropertyId.Speech_LogFilename, "logfile.txt"); //This line of code was the cause for an outstanding bug, if the log file becomes too full it causes issue. Further testing required before adding logging back as a feature.

                fromLanguageID(fromLanguageFullname); //Convert information from selected spoken language and sets fromLanuage to the ID
                toLanguageID(toLanguageFullname);//Convert information from selected translation language and sets toLanuage to the ID


                speechConfig.SpeechRecognitionLanguage = fromLanguage;

                if (MainForm.profanityFilter == false)
                {
                    translationConfig.SetProfanity(ProfanityOption.Raw);
                    speechConfig.SetProfanity(ProfanityOption.Raw);
                }
                if (MainForm.profanityFilter == true)
                {
                    translationConfig.SetProfanity(ProfanityOption.Masked);
                    speechConfig.SetProfanity(ProfanityOption.Masked);
                }
                translationConfig.SpeechRecognitionLanguage = fromLanguage;
                translationConfig.AddTargetLanguage(toLanguage);
                var audioConfig = AudioConfig.FromMicrophoneInput(MainForm.currentInputDevice);

                if (MainForm.currentInputDeviceName == "Default")
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
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Speech Recognition Canceled (Translating): " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]"));
                };

                translationRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (MainForm.rjToggleButton4.Checked == true)
                    {
                        var speechRecognitionResult = eventArgs.Result;


                        MainForm.dictationString = speechRecognitionResult.Text; //Dictation string
                        string translatedString = speechRecognitionResult.Translations[toLanguage]; //Dictation string tranlated

                        //i probably need to add a setting for which language activates the voice command.....(only english for now)
                        //VoiceCommand task
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(MainForm.dictationString));

                        SetDefaultTTS.SetVoicePresets();

                      //  var ot = new OutputText();
                        if (MainForm.rjToggleButtonLog.Checked == true)
                        {
                            VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, MainForm.dictationString + " [" + fromLanguage + ">" + toLanguage + "]: " + "[" + translatedString + "]");
                        }
                        //Send Text to TTS
                        if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                        {
                            string ttsModeNow = VoiceWizardWindow.TTSModeSaved;
                            switch (ttsModeNow)
                            {
                                case "FonixTalk":
                                    var fx = new FonixTalkTTS();
                                    Task.Run(() => fx.FonixTTS(translatedString));
                                    break;

                                case "System Speech":
                                    var sys = new WindowsBuiltInSTTTS();
                                    Task.Run(() => sys.systemTTSAction(translatedString));

                                    break;
                                case "Azure":
                                    SetDefaultTTS.SetVoicePresets();
                                  /*  var maybeStyle = Regex.Match(translatedString, @"^([\w\-]+)");
                                    string useThisEmotion = VoiceWizardWindow.emotion;
                                    switch (maybeStyle.Value)
                                    {
                                        case "Normal": break;
                                        case "Angry": break;
                                        case "Cheerful": break;
                                        case "Excited": break;
                                        case "Friendly": break;
                                        case "Hopeful": break;
                                        case "Sad": break;
                                        case "Shouting": break;
                                        case "Terrified": break;
                                        case "Unfriendly": break;
                                        case "Whispering": break;
                                        case "Assistant": break;
                                        case "Chat": break;
                                       // case "Customer Service": break;
                                        case "Newscast": break;
                                        default: break;
                                    }*/

                                    Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, translatedString, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                    break;
                                default:
                                  
                                    break;
                            }
                        }
                        //Refactor Needed - This code could be improved with switch case
                        //Send Text to Vrchat
                        if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                        {
                            if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                            else
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                        }
                        if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                            else
                            {
                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }


                        }
                        if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                        }

                    }



                };
                speechRecognizer1.Canceled += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Result.Text);
                   // var ot = new OutputText();
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Speech Recognition Canceled: " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]"));
                };
                speechRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (MainForm.rjToggleButton4.Checked == true)
                    {
                        MainForm.dictationString = eventArgs.Result.Text; //Dictation string

                        //VoiceCommand task
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(MainForm.dictationString));

                        SetDefaultTTS.SetVoicePresets();
                    //    var ot = new OutputText();
                        if (MainForm.rjToggleButtonLog.Checked == true)
                        {
                            VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, MainForm.dictationString);
                        }
                        if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                        {
                            string ttsModeNow = VoiceWizardWindow.TTSModeSaved;
                            switch (ttsModeNow)
                            {
                                case "FonixTalk":
                                    var fx = new FonixTalkTTS();
                                    Task.Run(() => fx.FonixTTS(MainForm.dictationString));
                                    break;

                                case "System Speech":
                                    var sys = new WindowsBuiltInSTTTS();
                                    Task.Run(() => sys.systemTTSAction(MainForm.dictationString));

                                    break;
                                case "Azure":
                                    SetDefaultTTS.SetVoicePresets();
                                    Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, MainForm.dictationString, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                    break;
                                default:
                                 
                                    break;
                            }
                            
                        }
                        //Send Text to Vrchat
                        if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            //ot.outputVRChat(MainForm, MainForm.dictationString);
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, MainForm.dictationString, "tts"));
                        }
                        if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            //ot.outputVRChat(MainForm, MainForm.dictationString);
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString, "tts"));

                        }
                        if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                        }
                    }
                };
                ///Phrase List
                var phraseList = PhraseListGrammar.FromRecognizer(speechRecognizer1);
                if (MainForm.rjToggleButtonPhraseList2.Checked == true)
                {
                    string words = MainForm.richTextBox6.Text.ToString();

                    string[] split = words.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in split)
                    {

                        if (s.Trim() != "")
                            phraseList.AddPhrase(s);
                        System.Diagnostics.Debug.WriteLine("Phrase Added: " + s);

                    }
                }
                if (MainForm.rjToggleButtonPhraseList2.Checked == false)
                {
                    System.Diagnostics.Debug.WriteLine("Phrase list cleared");
                    phraseList.Clear();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Speech Setup Failed: Make sure that you have setup your Azure Key and Region in the Provider tab (click the change buttons to apply changes) Reason:" + ex.Message.ToString());

            }
        }
      
        public async void speechTTTS(VoiceWizardWindow MainForm, string fromLanguageFullname)//speech to text
        {

            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {

                if (MainForm.rjToggleButton4.Checked == false)
                {
                    var speechRecognitionResult = await speechRecognizer1.RecognizeOnceAsync();
                    
                    //OutputSpeechRecognitionResult(speechRecognitionResult);
                    MainForm.dictationString = speechRecognitionResult.Text; //Dictation string


                    //VoiceCommand task
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(MainForm.dictationString));

                    SetDefaultTTS.SetVoicePresets();
                  //  var ot = new OutputText();
                    if (MainForm.rjToggleButtonLog.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, MainForm.dictationString);
                    }
                    if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                    {
                        string ttsModeNow = VoiceWizardWindow.TTSModeSaved;
                        switch (ttsModeNow)
                        {
                            case "FonixTalk":               
                                var fx = new FonixTalkTTS();
                                Task.Run(() => fx.FonixTTS(MainForm.dictationString));
                                break;

                            case "System Speech":
                                var sys = new WindowsBuiltInSTTTS();
                                Task.Run(() => sys.systemTTSAction(MainForm.dictationString));

                                break;
                            case "Azure":
                                SetDefaultTTS.SetVoicePresets();
                                /*  var maybeStyle = Regex.Match(MainForm.dictationString, @"^([\w\-]+)");
                                   string useThisEmotion = VoiceWizardWindow.emotion;
                                string useThisString = MainForm.dictationString;
                                   switch (maybeStyle.Value)
                                   {
                                       case "Normal": useThisEmotion = "normal"; break;
                                        
                                       case "Angry": useThisEmotion = "angry"; break;c
                                       case "Cheerful": useThisEmotion = "cheerful"; break;
                                       case "Excited": useThisEmotion = "excited"; break;
                                       case "Friendly": useThisEmotion = "friendly"; break;
                                       case "Hopeful": useThisEmotion = "hopeful"; break;
                                       case "Sad": useThisEmotion = "sad"; break;
                                       case "Shouting": useThisEmotion = "shouting"; break;
                                       case "Terrified": useThisEmotion = "terrified"; break;
                                       case "Unfriendly": useThisEmotion = "unfriendly"; break;
                                       case "Whispering": useThisEmotion = "whispering"; break;
                                       case "Assistant": useThisEmotion = "assistant"; break;
                                       case "Chat": useThisEmotion = "chat"; break;
                                      // case "Customer Service": break;
                                       case "Newscast": useThisEmotion = "normal"; break;
                                       default: useThisEmotion = VoiceWizardWindow.emotion; break;
                                   }*/
                               // MainForm.dictationString = Regex.Replace(MainForm.dictationString, useThisEmotion, ""); break;
                                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, MainForm.dictationString, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                break;
                            default:
                              
                                break;
                        }
                    }
                    //Send Text to Vrchat
                    if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                    {

                        VoiceWizardWindow.pauseBPM = true;
                        VoiceWizardWindow.pauseSpotify = true;
                        //ot.outputVRChat(MainForm, MainForm.dictationString);
                       Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, MainForm.dictationString, "tts"));
                    }
                    if(MainForm.rjToggleButtonChatBox.Checked==true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                    {
                        VoiceWizardWindow.pauseBPM = true;
                        VoiceWizardWindow.pauseSpotify = true;
                        //ot.outputVRChat(MainForm, MainForm.dictationString);
                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString, "tts"));

                    }
                    if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                    {
                         Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                    }
                }


                if (MainForm.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");
                    //  var ot = new OutputText();
                    VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Azure Continuous Listening Enabled]");

                    await speechRecognizer1.StartContinuousRecognitionAsync();
                }
                else if (continuousListening == true)
                {
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");
                    await speechRecognizer1.StopContinuousRecognitionAsync();
                    VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Azure Continuous Listening Disabled]");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("STTTS Failed: " + ex.Message.ToString());

            }
        }
        public async void translationSTTTS(VoiceWizardWindow MainForm, string toLanguageFullname, string fromLanguageFullname)//translate speech to text
        {
            System.Diagnostics.Debug.WriteLine("Speak into your microphone.");
            try
            {


                System.Diagnostics.Debug.WriteLine($"Say something in '{fromLanguage}' and ");
                System.Diagnostics.Debug.WriteLine($"we'll translate into '{toLanguage}'.\n");
                if (MainForm.rjToggleButton4.Checked == false)
                {

                    var speechRecognitionResult = await translationRecognizer1.RecognizeOnceAsync();

                    if (speechRecognitionResult.Reason == ResultReason.TranslatedSpeech)
                    {
                        System.Diagnostics.Debug.WriteLine($"Recognized: \"{speechRecognitionResult.Text}\"");
                        System.Diagnostics.Debug.WriteLine($"Translated into '{toLanguage}': {speechRecognitionResult.Translations[toLanguage]}");
                    }

                    MainForm.dictationString = speechRecognitionResult.Text; //Dictation string; Global string used to keep track of result text for default azure speech to text
                    string translatedString = speechRecognitionResult.Translations[toLanguage]; //Global string used to keep track of result text for translation azure speech to text


                    //i probably need to add a setting for which language activates the voice command.....(only english for now)
                    //VoiceCommand task
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.doVoiceCommand(MainForm.dictationString));


                    SetDefaultTTS.SetVoicePresets();
                    if (MainForm.rjToggleButtonLog.Checked == true)
                    {
                        VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, MainForm.dictationString + " [" + fromLanguage + ">" + toLanguage + "]: " + "[" + translatedString + "]");
                    }
                    //Send Text to TTS
                    if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                    {

                        string ttsModeNow = VoiceWizardWindow.TTSModeSaved;
                        switch (ttsModeNow)
                        {
                            case "FonixTalk":
                                
                                var fx = new FonixTalkTTS();
                                Task.Run(() => fx.FonixTTS(translatedString));
                                break;

                            case "System Speech":
                                var sys = new WindowsBuiltInSTTTS();
                                Task.Run(() => sys.systemTTSAction(translatedString));

                                break;
                            case "Azure":
                                SetDefaultTTS.SetVoicePresets();
                                Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(VoiceWizardWindow.MainFormGlobal, translatedString, VoiceWizardWindow.emotion, VoiceWizardWindow.rate, VoiceWizardWindow.pitch, VoiceWizardWindow.volume, VoiceWizardWindow.voice)); //turning off TTS for now
                                break;

                            default:
                              
                                break;
                        }
                    }

                    //Send Text to Vrchat
                    if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                    {
                        if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }
                        else
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }

                    }
                    if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                    {
                       // VoiceWizardWindow.pauseBPM = true;
                        if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                             Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }
                        else
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                           Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }


                    }
                    if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                    {
                       Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                    }


                }
                if (MainForm.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");

                    VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Azure Continuous Listening Enabled (Translating)]");

                    await translationRecognizer1.StartContinuousRecognitionAsync();

                }
                else if (continuousListening == true)
                {
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                    await translationRecognizer1.StopContinuousRecognitionAsync();
                    //   speechRecognizer1.Dispose();
                    //  var ot = new OutputText();
                    VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Azure Continuous Listening Disabled (Translating)]");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Translation STTTS Failed: This error usually occurs when attempting to translate your speech while your mic is not being picked up (check input device). Reason:" + ex.Message.ToString());

            }
        }
        public async void stopContinuousListeningNow(VoiceWizardWindow MainForm)//speech to text
        {
            if (continuousListening == true)
            {
                continuousListening = false;
                // Make the following call at some point to stop recognition:
                System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                await translationRecognizer1.StopContinuousRecognitionAsync();
                await speechRecognizer1.StopContinuousRecognitionAsync();
                //   speechRecognizer1.Dispose();
                //  var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[Azure Continuous Listening Disabled (Any)]");
            }
        }


    }
}