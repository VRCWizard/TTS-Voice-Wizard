using NAudio.Wave;
using OSCVRCWiz.Resources.StartUp;
using OSCVRCWiz.Resources.StartUp.StartUp;
using OSCVRCWiz.Services.Integrations;
using OSCVRCWiz.Services.Integrations.Heartrate;
using OSCVRCWiz.Services.Integrations.Media;
using OSCVRCWiz.Services.Speech.Speech_Recognition;
using OSCVRCWiz.Settings;
using Settings;

namespace OSCVRCWiz
{
    public class SaveSettings
    {
        public static void SavingSettings()
        {

            //  UnregisterHotKey(this.Handle, 1);
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonKeyRegion2.Checked == false)
            {
                Settings1.Default.yourRegion = "";
                Settings1.Default.yourKey = "";
            }
            // Settings1.Default.recognition = rjToggleButtonActivation.Checked;
            Settings1.Default.profanityFilterSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProfan.Checked;
            Settings1.Default.logOrNotSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked;
            Settings1.Default.sendOSCSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked;
            Settings1.Default.clearTTSWindowSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonClear.Checked;
            Settings1.Default.alwaysTopSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonOnTop2.Checked;
            Settings1.Default.wordsTranslateVRCSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAsTranslated2.Checked;
            Settings1.Default.hideDelaySetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked;
            Settings1.Default.hideDelayValue = VoiceWizardWindow.MainFormGlobal.textBoxErase.Text.ToString();
            Settings1.Default.phraseListValue = VoiceWizardWindow.MainFormGlobal.richTextBoxAzureDict.Text.ToString();
            Settings1.Default.phraseListBoolSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked;
            try
            {
                Settings1.Default.MicName = VoiceWizardWindow.MainFormGlobal.comboBoxInput.SelectedItem.ToString();
            }
            catch
            {

                Settings1.Default.MicName = "Default";
            }
            try
            {
                Settings1.Default.SpeakerName = VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem.ToString();
            }
            catch
            {
                Settings1.Default.SpeakerName = "Default";
            }
            try
            {
                Settings1.Default.SpeakerName2 = VoiceWizardWindow.MainFormGlobal.comboBoxOutput2.SelectedItem.ToString();
            }
            catch
            {
                Settings1.Default.SpeakerName2 = "Default";
            }
            Settings1.Default.EmojiSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked;
            Settings1.Default.SpotifyOutputSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked;
            Settings1.Default.HRIntervalSetting = OSCListener.HRInternalValue.ToString();
            Settings1.Default.HRPortSetting = OSCListener.OSCReceiveport.ToString();
            Settings1.Default.BPMSpamSetting = VoiceWizardWindow.MainFormGlobal.rjToggleOSCListenerSpamLog.Checked;
            Settings1.Default.voiceBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex;
            Settings1.Default.styleBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBoxStyleSelect.SelectedIndex;

           // Settings1.Default.voiceLanguage = VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedIndex;//voice language (make this save)
            Settings1.Default.voiceLanguageNew = VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedItem.ToString();
            Settings1.Default.langToBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.SelectedIndex;

            Settings1.Default.langSpokenSetting = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedIndex;

            Settings1.Default.langSpokenSettingNew = VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem.ToString();





            Settings1.Default.pitchNew = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
            Settings1.Default.volumeNew = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
            Settings1.Default.speedNew = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;

            Settings1.Default.use2ndDevice = VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked;


            Settings1.Default.STTTSContinuous = VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked;


            Settings1.Default.SpotifyPeriodicallySetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked;
            Settings1.Default.SpotifySpamSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked;
            Settings1.Default.SpotifyTimerIntervalSetting = SpotifyAddon.spotifyInterval;

            Settings1.Default.AudioCancelSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonStopCurrentTTS.Checked;
   

            Settings1.Default.bannerSetting = VoiceWizardWindow.MainFormGlobal.rjToggleBannerOff.Checked;

            Settings1.Default.SpotifyKey = VoiceWizardWindow.MainFormGlobal.textBoxSpotKey.Text.ToString();
            Settings1.Default.SpotifyLegacySetting = VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked;

            Settings1.Default.SendVRCChatBoxSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked;
            Settings1.Default.ChatBoxKeyboardSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked;

            Settings1.Default.minimizeToolBarSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton6.Checked;
            //  Settings1.Default.GreenScreenSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked;
            Settings1.Default.SpotifyCustomSetting = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString();


            Settings1.Default.VRCSoundNotifySetting = VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked;

            Settings1.Default.SystemTraySetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked;
            Settings1.Default.playMediaSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSounds.Checked;

            Settings1.Default.VRCUseDelay = VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked;



            Settings1.Default.activateOSCStart = VoiceWizardWindow.MainFormGlobal.rjToggleActivateOSCListenerStart.Checked;



            Settings1.Default.chatBoxSpotifyOnly = VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked;
            Settings1.Default.chatBoxSpotifyOnly4VRC = VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked;

            Settings1.Default.SpotifyNoUseChatbox = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked;
            Settings1.Default.SpotifyNoUseKat = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked;

            Settings1.Default.heartrateOutput = VoiceWizardWindow.MainFormGlobal.rjToggleOutputHeartrateDirect.Checked;

            Settings1.Default.enableMedia = VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked;

            Settings1.Default.ttsMode = VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem.ToString();

            //  Settings1.Default.approvedSource = VoiceWizardWindow.MainFormGlobal.richTextBox11.Text.ToString();

            Settings1.Default.StopOnPause = VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked;

            Settings1.Default.voiceCommandList = VoiceCommands.voiceCommandsStored;

            string approvedString = "";
            foreach (object Item in VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.CheckedItems)
            {
                approvedString += Item.ToString() + ",";

            }
            Settings1.Default.approvedSource = approvedString;


            Settings1.Default.saveDarkMode = VoiceWizardWindow.MainFormGlobal.rjToggleDarkMode.Checked;
            Settings1.Default.saveVoiceActStyle = VoiceWizardWindow.MainFormGlobal.rjToggleButtonStyle.Checked;
            Settings1.Default.STTModeSave = VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedItem.ToString();
            Settings1.Default.modelnamesave = VoiceWizardWindow.MainFormGlobal.modelTextBox.Text.ToString();

            Settings1.Default.whisperModel = VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text.ToString();

            //Settings1.Default.deepLKeysave= DeepLTranslate.DeepLKey;

           // Settings1.Default.OSCAddress = OSC.OSCAddress;
           // Settings1.Default.OSCPort = OSC.OSCPort;

            Settings1.Default.saveToast = VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked;

            VoicePresets.presetsSave();
            Settings1.Default.presetSave = VoicePresets.presetsStored;

            MediaPresets.presetsSave();
            Settings1.Default.mediaPresetSave = MediaPresets.mediaPresetsStored;

            WordReplacements.replacementSave();
            Settings1.Default.replaceSave = WordReplacements.wordReplacemntsStored;


            EmojiAddon.emojiReplacementsSave();
            Settings1.Default.emojiNewSave = EmojiAddon.emojiReplacemntsStored;

            Settings1.Default.discordParaSave = VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text.ToString();
            Settings1.Default.discordTimerSave = VoiceWizardWindow.MainFormGlobal.textBoxDiscTimer.Text.ToString();


            Settings1.Default.wordReplaceBeforeTTS = VoiceWizardWindow.MainFormGlobal.rjToggleReplaceBeforeTTS.Checked;

            Settings1.Default.hotketSave = VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked;
            Settings1.Default.shortcutStop = VoiceWizardWindow.MainFormGlobal.rjToggleButton12.Checked;
            Settings1.Default.shortcutQuickType = VoiceWizardWindow.MainFormGlobal.rjToggleButtonQuickTypeEnabled.Checked;

            Settings1.Default.saveAutoRefreshKat = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked;

            Settings1.Default.modHotKey = Hotkeys.modifierKeySTTTS;
            Settings1.Default.normalHotKey = Hotkeys.normalKeySTTTS;

            Settings1.Default.modHotkeyStop = Hotkeys.modifierKeyStopTTS;
            Settings1.Default.normalHotkeyStop = Hotkeys.normalKeyStopTTS;

            Settings1.Default.modHotkeyQuick = Hotkeys.modifierKeyQuickType;
            Settings1.Default.normalHotkeyQuick = Hotkeys.normalKeyQuickType;

            Settings1.Default.modifierKeyScrollUp = Hotkeys.modifierKeyScrollUp;
            Settings1.Default.normalKeyScrollUp = Hotkeys.normalKeyScrollUp;

            Settings1.Default.modifierKeyScrollDown = Hotkeys.modifierKeyScrollDown;
            Settings1.Default.normalKeyScrollDown = Hotkeys.normalKeyScrollDown;



            Settings1.Default.VRCOnRecieve = VoiceWizardWindow.MainFormGlobal.rjToggleButtonOutputVRCCountersOnContact.Checked;
            Settings1.Default.VRCAFK = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked;
            Settings1.Default.VRCListemOnStart = VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCActivate.Checked;
            Settings1.Default.VRCSpamLog = VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked;

            Settings1.Default.AFKMsg = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();
            Settings1.Default.VRCPort = VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text.ToString();

            Settings1.Default.Counter1Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text.ToString();
            Settings1.Default.Counter2Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text.ToString();
            Settings1.Default.Counter3Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text.ToString();
            Settings1.Default.Counter4Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text.ToString();
            Settings1.Default.Counter5Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text.ToString();
            Settings1.Default.Counter6Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text.ToString();
            Settings1.Default.Counter7Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter7.Text.ToString();
            Settings1.Default.Counter8Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter8.Text.ToString();
            Settings1.Default.Counter9Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter9.Text.ToString();
            Settings1.Default.Counter10Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter10.Text.ToString();

            Settings1.Default.Counter1Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text.ToString();
            Settings1.Default.Counter2Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text.ToString();
            Settings1.Default.Counter3Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text.ToString();
            Settings1.Default.Counter4Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text.ToString();
            Settings1.Default.Counter5Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text.ToString();
            Settings1.Default.Counter6Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text.ToString();
            Settings1.Default.Counter7Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage7.Text.ToString();
            Settings1.Default.Counter8Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage8.Text.ToString();
            Settings1.Default.Counter9Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage9.Text.ToString();
            Settings1.Default.Counter10Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage10.Text.ToString();

            Settings1.Default.SaveCounter = VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked;


            Settings1.Default.minDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text;
            Settings1.Default.maxDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text;
            Settings1.Default.dropStartSilence = VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text;
            Settings1.Default.pauseDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text;

            Settings1.Default.fontSize = StartUps.fontSize;

            Settings1.Default.delayAfterNoTTS = VoiceWizardWindow.MainFormGlobal.textBoxDelayAfterNoTTS.Text;
            Settings1.Default.delayBeforeNewTTS = VoiceWizardWindow.MainFormGlobal.textBoxQueueDelayBeforeNext.Text;

            Settings1.Default.saveQueueSystem = VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked;
            Settings1.Default.QueueWithTypedText = VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueTypedText.Checked;

            Settings1.Default.saveToggleReadTextFile = VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked;
            Settings1.Default.saveTxtFilePath = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text;


            Settings1.Default.OBSTextFile = VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked;
            Settings1.Default.OBS4Media = VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked;

            Settings1.Default.FilterNoiseWhisper = VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked;

            Settings1.Default.WindowsMediaDisable = VoiceWizardWindow.MainFormGlobal.rjToggleButtonDisableWindowsMedia.Checked;

            Settings1.Default.forwardData = VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked;

            Settings1.Default.modelSelected = VoiceWizardWindow.MainFormGlobal.comboBoxWhisperModelDownload.Text.ToString();
            Settings1.Default.WhisperFilterLog = VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked;

            Settings1.Default.forceMediaToggle = VoiceWizardWindow.MainFormGlobal.rjToggleButtonForceMedia.Checked;
            Settings1.Default.refocusWindow = VoiceWizardWindow.MainFormGlobal.rjToggleButtonRefocus.Checked;


            Settings1.Default.voiceWizProAzure = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAzure.Checked;
            Settings1.Default.voiceWizProAmazon = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAmazon.Checked;
 
            Settings1.Default.useVoiceWizardPro = VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked;
            Settings1.Default.voiceWizardProKey = VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString();


            Settings1.Default.voiceWizProTranslation = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked;
            Settings1.Default.uberDuckKey = VoiceWizardWindow.MainFormGlobal.textBoxUberKey.Text.ToString();
            Settings1.Default.uberDuckSecret = VoiceWizardWindow.MainFormGlobal.textBoxUberSecret.Text.ToString();

            Settings1.Default.labsModelID = VoiceWizardWindow.MainFormGlobal.comboBoxLabsModelID.SelectedIndex;
            Settings1.Default.labsOptimize = VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedIndex;
            Settings1.Default.labsSimboost = VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value;
            Settings1.Default.labsStability = VoiceWizardWindow.MainFormGlobal.trackBarStability.Value;
            Settings1.Default.labsStyleExagg = VoiceWizardWindow.MainFormGlobal.trackBarStyleExaggeration.Value;
            Settings1.Default.labsSpeakerBoost = VoiceWizardWindow.MainFormGlobal.rjToggleSpeakerBoost.Checked;

            Settings1.Default.saveToWav = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked;


            Settings1.Default.minAudioDuration = VoiceWizardWindow.MainFormGlobal.minimumAudio.Text;
            Settings1.Default.maxAudioDuration = VoiceWizardWindow.MainFormGlobal.maximumAudio.Text;
            Settings1.Default.SilenceThreshold = VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text;


            Settings1.Default.CounterOutputInterval = VoiceWizardWindow.MainFormGlobal.counterOutputInterval.Text;

            Settings1.Default.SmartStringSplit = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSmartStringSplit.Checked;

            Settings1.Default.SSSCharacterLimit = VoiceWizardWindow.MainFormGlobal.textBoxSSSCharLimit.Text;


            Settings1.Default.autoSend = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoSend.Checked;

            Settings1.Default.ProAPIBranch = VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.Text.ToString();

            Settings1.Default.KatLineLength = VoiceWizardWindow.MainFormGlobal.KATLineLengthTextBox.Text.ToString();

            Settings1.Default.PulsoidAuthToken = VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text.ToString();
            Settings1.Default.PulsoidUpdateInterval = HeartratePulsoid.heartrateIntervalPulsoid;
            Settings1.Default.PulsoidOnStart = VoiceWizardWindow.MainFormGlobal.rjToggleActivatePulsoidStart.Checked;
            Settings1.Default.WaveUniqueNames = VoiceWizardWindow.MainFormGlobal.rjToggleButtonUniqueWavNames.Checked;

            Settings1.Default.cheatAzureTranslation = VoiceWizardWindow.MainFormGlobal.rjToggleTranslateSameLanguage.Checked;
            Settings1.Default.websocketOnStart = VoiceWizardWindow.MainFormGlobal.rjToggleActivateWebsocketOnStart.Checked;
            Settings1.Default.websocketPort = WebSocketServer.WebSocketServerPort;

            Settings1.Default.showSpotifyURi = VoiceWizardWindow.MainFormGlobal.rjToggleShowConnectURISpotify.Checked;


            Settings1.Default.SyncParaValue = VoiceWizardWindow.MainFormGlobal.comboBoxPara.SelectedIndex;

            Settings1.Default.delayBeforeOuput = VoiceWizardWindow.MainFormGlobal.textBoxDelayBeforeAudio.Text;

            Settings1.Default.azurePartialResults = VoiceWizardWindow.MainFormGlobal.rjTogglePartialResults.Checked;

            Settings1.Default.partialResultsInterval = VoiceWizardWindow.MainFormGlobal.textBoxPartialResultsInterval.Text.ToString();

            Settings1.Default.WhisperVADMode = VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex;
            //Settings1.Default.WhisperGPU = VoiceWizardWindow.MainFormGlobal.comboBoxGPUSelection.SelectedItem.ToString();

            Settings1.Default.VADForWhisper = VoiceWizardWindow.MainFormGlobal.rjToggleVAD.Checked;

            Settings1.Default.bothLanguages = VoiceWizardWindow.MainFormGlobal.rjToggleBothLanguages.Checked;

            Settings1.Default.customTranslateText= VoiceWizardWindow.MainFormGlobal.textBoxCustomTranslationOuput.Text;

            Settings1.Default.silenceBarValue= VoiceWizardWindow.MainFormGlobal.trackBarSilence.Value;
            Settings1.Default.deepgramLogSpam = VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked;
            Settings1.Default.deepGramContinuous = VoiceWizardWindow.MainFormGlobal.rjToggleDeepGramContinuous.Checked;
            Settings1.Default.silenceScale = VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text;
            Settings1.Default.deepGramValidDuration = VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text;

            Settings1.Default.whisperVadOffset = VoiceWizardWindow.MainFormGlobal.textBoxWhisperVADOffset.Text;

            Settings1.Default.switchVoicePresetBindToggle = VoiceWizardWindow.MainFormGlobal.rjToggleSwitchVoicePresetsBind.Checked;

            Settings1.Default.useChatGPT = VoiceWizardWindow.MainFormGlobal.rjToggleButtonEnableChatGPT.Checked;
           Settings1.Default.UseMaxChatHistory = VoiceWizardWindow.MainFormGlobal.rjToggleUseMaxChatHistory.Checked;

           Settings1.Default.ChatGPTAPIKey = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text;
           Settings1.Default.ChatGPTModel = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text;
           Settings1.Default.ChatGPTMaxHistory = VoiceWizardWindow.MainFormGlobal.textBoxChatGPTMaxHistory.Text;

            Settings1.Default.useChatGPTPro = VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4ChatGPT.Checked;
            Settings1.Default.useContextWithGPT = VoiceWizardWindow.MainFormGlobal.rjToggleUseContextWithGPT.Checked;
            Settings1.Default.useOpenAITTSPro = VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4OpenAITTS.Checked;

            Settings1.Default.typeIndicator = VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked;

            Settings1.Default.PromptText = VoiceWizardWindow.MainFormGlobal.richTextBoxGPTPrompt.Text;
            Settings1.Default.enablePrompt = VoiceWizardWindow.MainFormGlobal.rjToggleGPTUsePrompt.Checked;

            Settings1.Default.DisableAzureTranslation = VoiceWizardWindow.MainFormGlobal.rjToggleDisableAzureTranslation.Checked;

            Settings1.Default.ExplicitPunctuation = VoiceWizardWindow.MainFormGlobal.rjToggleExplicitPunctuation.Checked;

            Settings1.Default.useWordBoundaries = VoiceWizardWindow.MainFormGlobal.rjToggleUseWordBoundaries.Checked;


            Settings1.Default.Save();
            //emojiSettings.Default.Save();
         //   VoiceWizardWindow.MainFormGlobal.webView21.Dispose();
        }
    }
}
