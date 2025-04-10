using CoreOSC;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Speech.TextToSpeech;
using OSCVRCWiz.Services.Speech.TranslationAPIs;
using OSCVRCWiz.Services.Text;
using OSCVRCWiz.Speech_Recognition;
using OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Resources.Audio;


namespace OSCVRCWiz.Services.Speech
{
    public class DoSpeech
    {

        static CancellationTokenSource speechCt = new();
        public static string TTSModeSaved = "System Speech";



        public static void TTSButonClick()
        {
            if (Hotkeys.captureEnabled == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonRefocus.Checked == true)//is capturing so turn it off
            {

                // Activate and bring the previous window to the front
                if (Hotkeys.prevFocusedWindow != IntPtr.Zero)
                {
                    Hotkeys.SetForegroundWindow(Hotkeys.prevFocusedWindow);
                }
                Hotkeys.captureEnabled = false;
            }

            string text = "";
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                text = VoiceWizardWindow.MainFormGlobal.richTextBox3.Text.ToString();
                TTSMessageQueue.QueueMessage(VoiceWizardWindow.MainFormGlobal.richTextBox3.Text.ToString(), "Text");
            });
            




            //  if (TTSMessageQueued.STTMode == "Text")
            // {
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSounds.Checked == true)
            {
                try
                {
                    AudioDevices.PlaySoundAsync("TTSButton.wav");
                }
                catch (Exception ex)
                {
                    OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                    OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
                }
            }
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonClear.Checked == true)
                {
                    VoiceWizardWindow.MainFormGlobal.richTextBox3.Clear();

                }

            });

        }

        public static async void MainDoTTS(TTSMessageQueue.TTSMessage TTSMessageQueued)
        {
            try
            {
                if (VoiceWizardWindow.MainFormGlobal.IsHandleCreated)
                {



                    
                    var language = TTSMessageQueued.TranslateLang;
                    //  VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    //  {

                    //  language = TTSMessageQueued.TranslateLang;

                    // });

                    //  string selectedTTSMode = TTSModeSaved;
                    string selectedTTSMode = TTSMessageQueued.TTSMode;
                    //VoiceCommand task

                    if ((AzureRecognition.YourSubscriptionKey == "" && selectedTTSMode == "Azure") && VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked != true)
                    {
                        //  var ot = new OutputText();
                        OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://ttsvoicewizard.com/docs/TTSMethods/AzureTTS ]", Color.DarkOrange);
                    }
                    VoiceCommands.MainDoVoiceCommand(TTSMessageQueued.text);
                    if (selectedTTSMode == "Azure" && VoiceWizardWindow.MainFormGlobal.rjToggleButtonStyle.Checked == true)
                    {
                        TTSMessageQueued.Style = VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.Text.ToString();// fix for speaking style not changing
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleReplaceBeforeTTS.Checked == true)
                    {
                        TTSMessageQueued.text = WordReplacements.MainDoWordReplacement(TTSMessageQueued.text);
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonEnableChatGPT.Checked)
                    {
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4ChatGPT.Checked && VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked)
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > ChatGPT (Pro) 🤖]: {TTSMessageQueued.text}", Color.LightBlue);
                            TTSMessageQueued.text = await VoiceWizardProTTS.CallVoiceProAPIGPT(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued.text);
                            TTSMessageQueued.STTMode = "ChatGPT (Pro) 🤖";
                        }
                        else
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > ChatGPT 🤖]: {TTSMessageQueued.text}", Color.LightBlue);
                            TTSMessageQueued.text = await ChatGPTAPI.GPTResponse(TTSMessageQueued.text);
                            TTSMessageQueued.STTMode = "ChatGPT 🤖";
                        }
                      
                    }
                    var originalText = TTSMessageQueued.text;
                    var writeText = TTSMessageQueued.text;//send to osc

                    var speechText = TTSMessageQueued.text;//send to tts
                    var newText = TTSMessageQueued.text; //translated text
                    var translationMethod = "";

                    if (!String.IsNullOrEmpty(TTSMessageQueued.text))
                    {
                        if (language != "No Translation (Default)")
                        {

                            if ((VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked != true) || VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked != true)
                            {
                                if (TTSMessageQueued.STTMode != "Azure Translate")
                                {
                                    newText = await DeepLTranslate.translateTextDeepL(TTSMessageQueued.text.ToString());
                                    translationMethod = "DeepL Translation";
                                }
                                else
                                {

                                    newText = TTSMessageQueued.AzureTranslateText;
                                    translationMethod = "Azure Translation";
                                }



                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked == true)
                                {
                                    speechText = newText;
                                    TTSMessageQueued.text = speechText;

                                }
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAsTranslated2.Checked == true)
                                {
                                    writeText = newText;

                                }
                            }

                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonStopCurrentTTS.Checked == true)
                        {
                            MainDoStopTTS();
                        }


                        var voiceWizardAPITranslationString = "";
                        speechCt = new();
                        switch (selectedTTSMode)
                        {
                            case "Moonbase":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }

                                Task.Run(() => MoonbaseTTS.FonixTTS(TTSMessageQueued, speechCt.Token));
                                //  }
                                // Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(TTSMessageQueued, speechCt.Token)); //turning off TTS for now
                                break;
                            case "ElevenLabs":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => ElevenLabsTTS.ElevenLabsTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "System Speech":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => SystemSpeechTTS.systemTTSAction(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Azure":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAzure.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    selectedTTSMode = "Azure (Pro)";
                                }
                                else
                                {
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                    {
                                       TTSMessageQueued.TTSMode = "No TTS";
                                        voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                        {
                                            TTSMessageQueued.text = voiceWizardAPITranslationString;
                                        }
                                    }
                                    Task.Run(() => AzureTTS.SynthesizeAudioAsync(TTSMessageQueued, speechCt.Token)); //turning off TTS for now
                                }
                                // Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "TikTok":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => TikTokTTS.TikTokTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;

                            case "TTSMonster":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => TTSMonsterTTS.TTSMonstertts(TTSMessageQueued, speechCt.Token));
                                break;

                            case "NovelAI":
                              //  Task.Run(() => NovelAITTS.NovelAITextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Locally Hosted":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => GladosTTS.GladosTextAsSpeech(TTSMessageQueued, speechCt.Token));
                                break;
                            case "Amazon Polly":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAmazon.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    selectedTTSMode = "Amazon Polly (Pro)";
                                }
                                else
                                {
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                    {
                                        TTSMessageQueued.TTSMode = "No TTS";
                                        voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                        {
                                            TTSMessageQueued.text = voiceWizardAPITranslationString;
                                        }
                                    }
                                    Task.Run(() => AmazonPollyTTS.PollyTTS(TTSMessageQueued, speechCt.Token));
                                }
                                break;
                            case "Google (Pro Only)":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                }
                                else
                                {
                                    Task.Run(() => OutputText.outputLog("[You do not have the VoiceWizardPro API enabled, consider becoming a member: https://www.patreon.com/ttsvoicewizard ]", Color.DarkOrange));
                                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                                    return;
                                }
                                break;
                            case "Deepgram Aura (Pro Only)":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                }
                                else
                                {
                                    Task.Run(() => OutputText.outputLog("[You do not have the VoiceWizardPro API enabled, consider becoming a member: https://www.patreon.com/ttsvoicewizard ]", Color.DarkOrange));
                                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                                    return;
                                }
                                break;
                            case "OpenAI":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4OpenAITTS.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    selectedTTSMode = "OpenAI (Pro)";
                                }
                                else
                                {
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                    {
                                        TTSMessageQueued.TTSMode = "No TTS";
                                        voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                        {
                                            TTSMessageQueued.text = voiceWizardAPITranslationString;
                                        }
                                    }
                                    Task.Run(() => OpenAITTS.OpenAItts(TTSMessageQueued, speechCt.Token));
                                }
                                break;

                            case "Uberduck":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                TTSMessageQueued.Voice = UberDuckTTS.UberVoiceNameAndID[TTSMessageQueued.Voice];
                                Task.Run(() => UberDuckTTS.uberduckTTS(TTSMessageQueued, speechCt.Token));

                                break;
                            case "VoiceForge":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonVoiceWhatLang.Checked)
                                    {
                                        TTSMessageQueued.text = voiceWizardAPITranslationString;
                                    }
                                }
                                Task.Run(() => VoiceForgeTTS.VoiceForgeTextAsSpeech(TTSMessageQueued, speechCt.Token));

                                break;

                            case "IBM Watson (Pro Only)":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true)
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));
                                }
                                else
                                {
                                    Task.Run(() => OutputText.outputLog("[You do not have the VoiceWizardPro API enabled, consider becoming a member: https://www.patreon.com/ttsvoicewizard ]", Color.DarkOrange));
                                    Task.Run(() => TTSMessageQueue.PlayNextInQueue());
                                    return;
                                }
                                break;




                              
                            case "No TTS":
                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true && language != "No Translation (Default)")
                                {
                                    voiceWizardAPITranslationString = await Task.Run(() => VoiceWizardProTTS.VoiceWizardProTextAsSpeech(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), TTSMessageQueued, speechCt.Token));

                                }

                                break;
                            case "Miku":
                                // Task.Run(() => MikuTTS.MikuTextAsSpeech(speechText, speechCt.Token));
                                break;

                            case "Fart to Speech":
                                // Task.Run(() => FartTTS.FartTextAsSpeech(speechText, speechCt.Token)); //april fools
                                break;


                            default:

                                break;
                        }

                        if (language != "No Translation (Default)")
                        {

                            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true)
                            {

                                newText = voiceWizardAPITranslationString;

                                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonAsTranslated2.Checked == true)
                                {
                                    writeText = voiceWizardAPITranslationString;

                                }
                                translationMethod = "VoiceWizardPro Translation";
                                voiceWizardAPITranslationString = "";
                            }

                        }
                    }
                    else
                    {
                        TTSMessageQueue.PlayNextInQueue();
                    }



                    if (VoiceWizardWindow.MainFormGlobal.rjToggleReplaceBeforeTTS.Checked == false)
                    {

                        writeText = WordReplacements.MainDoWordReplacement(writeText);
                        originalText = WordReplacements.MainDoWordReplacement(originalText);


                    }


                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked == true)
                    {
                        if (language == "No Translation (Default)")
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > {selectedTTSMode}]: {writeText}");
                        }
                        else
                        {
                            OutputText.outputLog($"[{TTSMessageQueued.STTMode} > {selectedTTSMode}]: {originalText} [{translationMethod}]: {newText}");
                        }

                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked == true)
                        {
                            OutputText.outputTextFile(originalText, @"Output\TextOutput\OBSText.txt");
                            OutputText.outputTextFile(newText, @"Output\TextOutput\OBSTextTranslated.txt");
                            OutputText.outputTextFile(originalText, @"Output\TextOutput\TranscriptionOnly.txt");
                        }

                    }
                    if (language != "No Translation (Default)" && VoiceWizardWindow.MainFormGlobal.rjToggleBothLanguages.Checked)
                    {
                        string customText = VoiceWizardWindow.MainFormGlobal.textBoxCustomTranslationOuput.Text.ToString();

                        string inputLanguage = TTSMessageQueued.SpokenLang;
                        string outputLanguage = TTSMessageQueued.TranslateLang;

                        var (inputLangName,inputLangCode) = LanguageSelect.ExtractLanguageNameAndCode(inputLanguage);
                        var (outputLangName, outputLangCode) = LanguageSelect.ExtractLanguageNameAndCode(outputLanguage);



                        customText = customText.Replace("{inputLangName}", inputLangName);
                        customText = customText.Replace("{inputLangCode}", inputLangCode);
                        customText = customText.Replace("{outputLangName}", outputLangName);
                        customText = customText.Replace("{outputLangCode}", outputLangCode);
                        customText = customText.Replace("{originalText}", originalText);
                        customText = customText.Replace("{translatedText}", newText);
                        customText = customText.Replace("{nline}", "\u2028");

                        writeText = customText;
                    }


                    if (TTSMessageQueued.chatboxOverride == false)
                    {


                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked == false)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChat(writeText, OutputText.DisplayTextType.TextToSpeech));
                        }
                        if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked == false)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles(writeText, OutputText.DisplayTextType.TextToSpeech)); //original

                        }
                    }
                    else
                    {
                        if(TTSMessageQueued.useKAT)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChat(writeText, OutputText.DisplayTextType.TextToSpeech));
                        }
                        if(TTSMessageQueued.useChatbox)
                        {
                            OSCListener.pauseBPM = true;
                            SpotifyAddon.pauseSpotify = true;
                            Task.Run(() => OutputText.outputVRChatSpeechBubbles(writeText, OutputText.DisplayTextType.TextToSpeech)); //original
                        }
                    }
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked == true && TTSMessageQueued.TTSMode == "No TTS")
                    {
                        Task.Run(() => NoTTSQueue());
                    }

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                    {
                        var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                        OSC.OSCSender.Send(sttListening);
                    }


                }
                else
                {
                    OutputText.outputLog("[DoTTS Handle was not created]", Color.Red);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[DoTTS Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();

                if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked == true || VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked == true)
                {
                    var sttListening = new OscMessage("/avatar/parameters/stt_listening", false);
                    OSC.OSCSender.Send(sttListening);
                }
            }




        }

        //  public static string speechOn = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\sounds", "speechOnButton.wav");
        // public static string speechOff = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets\\sounds", "speechOffButton.wav");

        public static void speechToTextButtonOn()
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.SpeechToTextBigButton.ForeColor = Color.Green;
                    VoiceWizardWindow.MainFormGlobal.SpeechToTextBigButton.IconColor = Color.Green;
                });

            }
            catch(Exception ex)
            {
                OutputText.outputLog("[Button Color Error: " + ex.Message + "]", Color.Red);
            }
        }

        public static void speechToTextButtonOff()
        {
            try
            {
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.SpeechToTextBigButton.ForeColor = Color.White;
                    VoiceWizardWindow.MainFormGlobal.SpeechToTextBigButton.IconColor = Color.White;
                });
            }
            catch(Exception ex) 
            {
                OutputText.outputLog("[Button Color Error: " + ex.Message + "]", Color.Red);
            }
        }

        public static void speechToTextOnSound()
        {
            speechToTextButtonOn();
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSounds.Checked == true)
            {
                try
                {


                    AudioDevices.PlaySoundAsync("speechOnButton.wav");
                }

                catch (Exception ex)
                {
                    OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                    OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
                }
            }
        }

        public static void speechToTextOffSound()
        {
            speechToTextButtonOff();
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonSounds.Checked == true)
            {
                try
                {
                    AudioDevices.PlaySoundAsync("speechOffButton.wav");
                }
                catch (Exception ex)
                {
                    OutputText.outputLog("[Button Sound Error: " + ex.Message + "]", Color.Red);
                    OutputText.outputLog("[This is caused by the sound folder/files being missing or access being denied. Check to make sure the sound folder exists with sound files inside. Try changing the app folders location. Try running as administator. If do not care for button sounds simply disable them]", Color.DarkOrange);
                }
            }
        }

       


        public static  void MainDoSpeechTTS()
        {
          
            try
            {

                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    switch (VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedItem.ToString())
                    {

                        case "Deepgram (Pro Only)":

                            Task.Run(async () => await VoiceWizardProRecognition.doRecognition(VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString(), false));
                            //Task.Run(async () => await ElevenLabsRecognition.doRecognition(false)); //ElevenLabs Testing

                            break;

                        case "ElevenLabs STS":

                            Task.Run(async () => await ElevenLabsRecognition.doRecognition(false));

                            break;

                        case "Vosk":

                            Task.Run(() => VoskRecognition.toggleVosk());

                            break;
                        case "Whisper":
                            if (VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text.ToString() == "no model selected")
                            {
                                WhisperRecognition.downloadWhisperModel();
                                OutputText.outputLog("[Auto installing default Whisper model for you, please wait. To download higher accuracy Whisper model navigate to Speech Provider > Local > Whisper.cpp Model and download/select a bigger model]", Color.DarkOrange);
                            }
                            else
                            {
                                Task.Run(() => WhisperRecognition.toggleWhisper());
                            }

                            break;
                        case "Web Captioner":
                            Task.Run(() => WebCaptionerRecognition.WebCapToggle());
                            break;

                        case "System Speech":

                            Task.Run(() => SystemSpeechRecognition.speechTTSButtonLiteClick());

                            break;
                        case "Azure":
                            if (AzureRecognition.YourSubscriptionKey == "")
                            {
                                OutputText.outputLog("[You appear to be missing an Azure Key, make sure to follow the setup guide: https://ttsvoicewizard.com/docs/TTSMethods/AzureTTS ]", Color.DarkOrange);
                            }
                            if (AzureRecognition.YourSubscriptionKey != "")
                            {
                                //  var azureRec = new AzureRecognition();

                                if (VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString() == "No Translation (Default)" || VoiceWizardWindow.MainFormGlobal.rjToggleDisableAzureTranslation.Checked || (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked == true))
                                {
                                    AzureRecognition.speechSetup(VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString(), VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString()); //only speechSetup needed
                                    System.Diagnostics.Debug.WriteLine("<speechSetup change>");

                                    OutputText.outputLog("[Azure Listening]");
                                    AzureRecognition.speechTTTS(VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString());


                                }
                                else
                                {
                                    AzureRecognition.speechSetup(VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString(), VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString()); //only speechSetup needed
                                    System.Diagnostics.Debug.WriteLine("<speechSetup change>");

                                    OutputText.outputLog("[Azure Translation Listening]");
                                    AzureRecognition.translationSTTTS(VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.Text.ToString(), VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.Text.ToString());


                                }

                            }
                            break;

                        default:

                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                
                OutputText.outputLog("[STTTS Error: " + ex.Message.ToString() + "]", Color.Red);

            }




        }

        public static void MainDoStopTTS()
        {
            try
            {
                speechCt.Cancel();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Stop TTS Error: " + ex.Message + "]", Color.Red);
            }


        }
        private static void NoTTSQueue()
        {

            Task.Delay(Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxDelayAfterNoTTS.Text.ToString())).Wait();

            Task.Run(() => TTSMessageQueue.PlayNextInQueue());


        }
    }
}
