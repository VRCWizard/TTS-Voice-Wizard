using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        public static CancellationTokenSource SpeechCt = new();


        public void speechSetup(VoiceWizardWindow MainForm, string toLanguageFullname, string fromLanguageFullname)//speech to text setup

        {
            //Current Implementation of speechSetup is not useful because I must still call speechSetup before each TTS Operation (because of checking for if default mic is changed from control panel/window settings)
            //Only benefit is that the recognizers are resued but, I am not sure how helpful that is
            //Since Setup is still run at the begining of TTS still i can comment out all other occurences of speechSetup
            try
            {
                speechConfig = SpeechConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
                translationConfig = SpeechTranslationConfig.FromSubscription(VoiceWizardWindow.YourSubscriptionKey, VoiceWizardWindow.YourServiceRegion);
               // speechConfig.SetProperty(PropertyId.Speech_LogFilename, "logfile.txt");

                fromLanguage = "en-US";
                toLanguage = "en";
                switch (fromLanguageFullname)
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
                    case "Hendi [hi-IN]": fromLanguage = "hi-IN"; break;
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
                switch (toLanguageFullname)
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
                    case "Hendi [hi]": toLanguage = "hi"; break;
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
                translationRecognizer1.Canceled += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Result.Text);
                    var ot = new OutputText();
                    Task.Run(() => ot.outputLog(MainForm, "[Speech Recognition Canceled (Translating): " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]"));
                };

                translationRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (MainForm.rjToggleButton4.Checked == true)
                    {
                        var speechRecognitionResult = eventArgs.Result;


                        MainForm.dictationString = speechRecognitionResult.Text; //Dictation string
                        string translatedString = speechRecognitionResult.Translations[toLanguage]; //Dictation string
                        string emotion = "normal";
                        string rate = "default";
                        string pitch = "default";
                        string volume = "default";
                        string voice = "Sara";
                        MainForm.Invoke((MethodInvoker)delegate ()
                        {
                            if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString())) { emotion = "normal"; }
                            else { emotion = MainForm.comboBox1.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString())) { rate = "default"; } else { rate = MainForm.comboBoxRate.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString())) { pitch = "default"; }
                            else { pitch = MainForm.comboBoxPitch.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString())) { volume = "default"; }
                            else { volume = MainForm.comboBoxVolume.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString())) { voice = "Sara"; }
                            else { voice = MainForm.comboBox2.Text.ToString(); }


                        });
                        var ot = new OutputText();
                        if (MainForm.rjToggleButtonLog.Checked == true)
                        {
                            ot.outputLog(MainForm, MainForm.dictationString + " [" + fromLanguage + ">" + toLanguage + "]: " + "[" + translatedString + "]");
                        }
                        //Send Text to TTS
                        if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                        {
                            if (MainForm.rjToggleButtonCancelAudio.Checked == true)
                            {
                                SpeechCt.Cancel();
                            }
                            SpeechCt = new();
                            Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm, translatedString, emotion, rate, pitch, volume, voice, SpeechCt.Token));
                        }

                        //Send Text to Vrchat
                        if (MainForm.rjToggleButtonOSC.Checked == true)
                        {
                            if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => ot.outputVRChat(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                            else
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                        }
                        if (MainForm.rjToggleButtonChatBox.Checked == true)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                            {

                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }
                            else
                            {
                                VoiceWizardWindow.pauseBPM = true;
                                VoiceWizardWindow.pauseSpotify = true;
                                Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                            }


                        }
                        if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                        }

                    }



                };
                speechRecognizer1.Canceled += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Result.Text);
                    var ot = new OutputText();
                    Task.Run(() => ot.outputLog(MainForm, "[Speech Recognition Canceled: " + eventArgs.Result.Text + " Reason: " + eventArgs.Result.Reason.ToString() + " Error Details: " + eventArgs.ErrorDetails.ToString() + "]"));
                };

                speechRecognizer1.Recognized += (sender, eventArgs) =>
                {
                    if (MainForm.rjToggleButton4.Checked == true)
                    {
                        MainForm.dictationString = eventArgs.Result.Text; //Dictation string

                        string emotion = "normal";
                        string rate = "default";
                        string pitch = "default";
                        string volume = "default";
                        string voice = "Sara";
                        MainForm.Invoke((MethodInvoker)delegate ()
                        {
                            if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString())) { emotion = "normal"; }
                            else { emotion = MainForm.comboBox1.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString())) { rate = "default"; } else { rate = MainForm.comboBoxRate.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString())) { pitch = "default"; }
                            else { pitch = MainForm.comboBoxPitch.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString())) { volume = "default"; }
                            else { volume = MainForm.comboBoxVolume.Text.ToString(); }
                            if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString())) { voice = "Sara"; }
                            else { voice = MainForm.comboBox2.Text.ToString(); }
                        });
                        var ot = new OutputText();
                        if (MainForm.rjToggleButtonLog.Checked == true)
                        {
                            ot.outputLog(MainForm, MainForm.dictationString);
                        }
                        if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                        {
                            if (MainForm.rjToggleButtonCancelAudio.Checked == true)
                            {
                                SpeechCt.Cancel();
                            }
                            SpeechCt = new();
                            Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm, MainForm.dictationString, emotion, rate, pitch, volume, voice, SpeechCt.Token));
                        }
                        //Send Text to Vrchat
                        if (MainForm.rjToggleButtonOSC.Checked == true)
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            //ot.outputVRChat(MainForm, MainForm.dictationString);
                            Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString, "tts"));
                        }
                        if (MainForm.rjToggleButtonChatBox.Checked == true)
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            //ot.outputVRChat(MainForm, MainForm.dictationString);
                           Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString, "tts"));

                        }
                        if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                        {
                            Task.Run(() => ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                        }
                        //Send Text to TTS

                        // Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm.dictationString, emotion, rate, pitch, volume, voice));

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

                    string emotion = "normal";
                    string rate = "default";
                    string pitch = "default";
                    string volume = "default";
                    string voice = "Sara";
                    MainForm.Invoke((MethodInvoker)delegate ()
                    {
                        if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString())) { emotion = "normal"; }
                        else { emotion = MainForm.comboBox1.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString())) { rate = "default"; } else { rate = MainForm.comboBoxRate.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString())) { pitch = "default"; }
                        else { pitch = MainForm.comboBoxPitch.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString())) { volume = "default"; }
                        else { volume = MainForm.comboBoxVolume.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString())) { voice = "Sara"; }
                        else { voice = MainForm.comboBox2.Text.ToString(); }


                    });
                    var ot = new OutputText();
                    if (MainForm.rjToggleButtonLog.Checked == true)
                    {
                        ot.outputLog(MainForm, MainForm.dictationString);
                    }
                    if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                    {
                        SpeechCt.Cancel();
                        SpeechCt = new();
                        Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm, MainForm.dictationString, emotion, rate, pitch, volume, voice, SpeechCt.Token));
                    }
                    //Send Text to Vrchat
                    if (MainForm.rjToggleButtonOSC.Checked == true)
                    {

                        VoiceWizardWindow.pauseBPM = true;
                        VoiceWizardWindow.pauseSpotify = true;
                        //ot.outputVRChat(MainForm, MainForm.dictationString);
                       Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString, "tts"));
                    }
                    if(MainForm.rjToggleButtonChatBox.Checked==true)
                    {
                        VoiceWizardWindow.pauseBPM = true;
                        VoiceWizardWindow.pauseSpotify = true;
                        //ot.outputVRChat(MainForm, MainForm.dictationString);
                        Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString, "tts"));

                    }
                    if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                    {
                        Task.Run(() => ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                    }
                }


                if (MainForm.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    //  await translationRecognizer1.StopContinuousRecognitionAsync();//may cause issues
                    // stopRecognition = new TaskCompletionSource<int>();//testing
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");
                    var ot = new OutputText();
                    ot.outputLog(MainForm, "[Azure Continuous Listening Enabled]");

                    await speechRecognizer1.StartContinuousRecognitionAsync();
                    // Waits for completion. Use Task.WaitAny to keep the task rooted.
                    // Task.Run(() => Task.WaitAny(new[] { stopRecognition.Task }));

                    // Make the following call at some point to stop recognition:
                    // await recognizer.StopContinuousRecognitionAsync();
                }
                else if (continuousListening == true)
                {
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                    await speechRecognizer1.StopContinuousRecognitionAsync();
                    //   speechRecognizer1.Dispose();
                    var ot = new OutputText();
                    ot.outputLog(MainForm, "[Azure Continuous Listening Disabled]");
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

                    MainForm.dictationString = speechRecognitionResult.Text; //Dictation string
                    string translatedString = speechRecognitionResult.Translations[toLanguage]; //Dictation string
                    string emotion = "normal";
                    string rate = "default";
                    string pitch = "default";
                    string volume = "default";
                    string voice = "Sara";
                    MainForm.Invoke((MethodInvoker)delegate ()
                    {
                        if (string.IsNullOrWhiteSpace(MainForm.comboBox1.Text.ToString())) { emotion = "normal"; }
                        else { emotion = MainForm.comboBox1.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxRate.Text.ToString())) { rate = "default"; } else { rate = MainForm.comboBoxRate.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxPitch.Text.ToString())) { pitch = "default"; }
                        else { pitch = MainForm.comboBoxPitch.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBoxVolume.Text.ToString())) { volume = "default"; }
                        else { volume = MainForm.comboBoxVolume.Text.ToString(); }
                        if (string.IsNullOrWhiteSpace(MainForm.comboBox2.Text.ToString())) { voice = "Sara"; }
                        else { voice = MainForm.comboBox2.Text.ToString(); }
                    });
                    var ot = new OutputText();
                    if (MainForm.rjToggleButtonLog.Checked == true)
                    {
                        ot.outputLog(MainForm, MainForm.dictationString + " [" + fromLanguage + ">" + toLanguage + "]: " + "[" + translatedString + "]");
                    }
                    //Send Text to TTS
                    if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                    {
                        SpeechCt.Cancel();
                        SpeechCt = new();
                        Task.Run(() => AudioSynthesis.SynthesizeAudioAsync(MainForm, translatedString, emotion, rate, pitch, volume, voice, SpeechCt.Token));
                    }

                    //Send Text to Vrchat
                    if (MainForm.rjToggleButtonOSC.Checked == true)
                    {
                        if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChat(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }
                        else
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChat(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }

                    }
                    if (MainForm.rjToggleButtonChatBox.Checked == true)
                    {
                       // VoiceWizardWindow.pauseBPM = true;
                        if (MainForm.rjToggleButtonAsTranslated2.Checked == true) //changed from checkbox7
                        {

                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, translatedString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }
                        else
                        {
                            VoiceWizardWindow.pauseBPM = true;
                            VoiceWizardWindow.pauseSpotify = true;
                            Task.Run(() => ot.outputVRChatSpeechBubbles(MainForm, MainForm.dictationString + "[" + fromLanguage + " > " + toLanguage + "]", "tts"));

                        }


                    }
                    if (MainForm.rjToggleButtonGreenScreen.Checked == true)
                    {
                        Task.Run(() => ot.outputGreenScreen(MainForm, MainForm.dictationString, "tts")); //original

                    }


                }
                if (MainForm.rjToggleButton4.Checked == true && continuousListening == false)
                {
                    //   await speechRecognizer1.StopContinuousRecognitionAsync();//may cause issues
                    // stopRecognition = new TaskCompletionSource<int>();//testing
                    continuousListening = true;
                    System.Diagnostics.Debug.WriteLine("continuousListening Enabled------------------------------");
                    var ot = new OutputText();
                    ot.outputLog(MainForm, "[Azure Continuous Listening Enabled (Translating)]");

                    await translationRecognizer1.StartContinuousRecognitionAsync();
                    // Waits for completion. Use Task.WaitAny to keep the task rooted.
                    // Task.Run(() => Task.WaitAny(new[] { stopRecognition.Task }));

                    // Make the following call at some point to stop recognition:
                    // await recognizer.StopContinuousRecognitionAsync();
                }
                else if (continuousListening == true)
                {
                    continuousListening = false;
                    // Make the following call at some point to stop recognition:
                    System.Diagnostics.Debug.WriteLine("continuousListening Disabled------------------------------");

                    await translationRecognizer1.StopContinuousRecognitionAsync();
                    //   speechRecognizer1.Dispose();
                    var ot = new OutputText();
                    ot.outputLog(MainForm, "[Azure Continuous Listening Disabled (Translating)]");
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
                var ot = new OutputText();
                ot.outputLog(MainForm, "[Azure Continuous Listening Disabled (Any)]");
            }
        }


    }
}