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
    public class LoadSettings
    {
        public static void LoadingSettings()
        {
            
            VoiceWizardWindow.MainFormGlobal.textBoxAzureKey.Text = Settings1.Default.yourKey;
            VoiceWizardWindow.MainFormGlobal.textBox3.Text = Settings1.Default.yourRegion;
            AzureRecognition.YourSubscriptionKey = Settings1.Default.yourKey;
            AzureRecognition.YourServiceRegion = Settings1.Default.yourRegion;

            VoiceWizardWindow.MainFormGlobal.textBoxDelay.Text = Settings1.Default.delayDebugValueSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProfan.Checked = Settings1.Default.profanityFilterSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked = Settings1.Default.logOrNotSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked = Settings1.Default.sendOSCSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonClear.Checked = Settings1.Default.clearTTSWindowSetting;//off
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOnTop2.Checked = Settings1.Default.alwaysTopSetting;//off
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAsTranslated2.Checked = Settings1.Default.wordsTranslateVRCSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked = Settings1.Default.hideDelaySetting;//off
            VoiceWizardWindow.MainFormGlobal.textBoxErase.Text = Settings1.Default.hideDelayValue;
            VoiceWizardWindow.MainFormGlobal.richTextBoxAzureDict.Text = Settings1.Default.phraseListValue;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked = Settings1.Default.phraseListBoolSetting;
            AzureRecognition.YourSubscriptionKey = Settings1.Default.yourKey;
            AzureRecognition.YourServiceRegion = Settings1.Default.yourRegion;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonKeyRegion2.Checked = Settings1.Default.remember;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked = Settings1.Default.EmojiSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked = Settings1.Default.SpotifyOutputSetting;
            OSCListener.HRInternalValue = Convert.ToInt32(Settings1.Default.HRIntervalSetting);
            OSCListener.OSCReceiveport = Convert.ToInt32(Settings1.Default.HRPortSetting);
            VoiceWizardWindow.MainFormGlobal.textBoxHRPort.Text = Settings1.Default.HRPortSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleOSCListenerSpamLog.Checked = Settings1.Default.BPMSpamSetting;

            try
            {
               // VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedIndex = Settings1.Default.voiceLanguage;//voice language (make this save)
                VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedItem = Settings1.Default.voiceLanguageNew;
            }
            catch (Exception ex)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxAccentSelect.SelectedIndex = 0;
            }


            VoiceWizardWindow.MainFormGlobal.rjToggleDarkMode.Checked = Settings1.Default.saveDarkMode;


            VoiceWizardWindow.MainFormGlobal.comboBoxTranslationLanguage.SelectedIndex = Settings1.Default.langToBoxSetting;
            VoiceWizardWindow.MainFormGlobal.comboBoxSpokenLanguage.SelectedItem = Settings1.Default.langSpokenSettingNew;
 

            VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value = Settings1.Default.pitchNew;
            VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value = Settings1.Default.volumeNew;
            VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value = Settings1.Default.speedNew;
            VoiceWizardWindow.MainFormGlobal.trackBarSilence.Value = Settings1.Default.silenceBarValue;

            VoiceWizardWindow.MainFormGlobal.updateAllTrackBarLabels();

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked = Settings1.Default.use2ndDevice;


            VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked = Settings1.Default.STTTSContinuous;



            VoiceWizardWindow.MainFormGlobal.rjToggleBannerOff.Checked = Settings1.Default.bannerSetting;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleBannerOff.Checked == true)
            {
             //   VoiceWizardWindow.MainFormGlobal.webView21.Dispose();
                VoiceWizardWindow.MainFormGlobal.button10.Dispose();
                VoiceWizardWindow.MainFormGlobal.buttonPreviousBanner.Dispose();
                VoiceWizardWindow.MainFormGlobal.buttonNextBanner.Dispose();
                VoiceWizardWindow.MainFormGlobal.pictureBox5.Dispose();
            }



            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked = Settings1.Default.SpotifyPeriodicallySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked = Settings1.Default.SpotifySpamSetting;

            SpotifyAddon.spotifyInterval = Settings1.Default.SpotifyTimerIntervalSetting;
            VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text = SpotifyAddon.spotifyInterval;
            

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonStopCurrentTTS.Checked = Settings1.Default.AudioCancelSetting;


            VoiceWizardWindow.MainFormGlobal.textBoxSpotKey.Text = Settings1.Default.SpotifyKey;
            VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked = Settings1.Default.SpotifyLegacySetting;
            SpotifyAddon.legacyState = VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked = Settings1.Default.SendVRCChatBoxSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked = Settings1.Default.ChatBoxKeyboardSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton6.Checked = Settings1.Default.minimizeToolBarSetting;



            VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text = Settings1.Default.SpotifyCustomSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked = Settings1.Default.VRCSoundNotifySetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked = Settings1.Default.SystemTraySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSounds.Checked = Settings1.Default.playMediaSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked = Settings1.Default.VRCUseDelay;





           
            VoiceWizardWindow.MainFormGlobal.textBoxOSCPort.Text = Settings1.Default.rememberPort;

            VoiceWizardWindow.MainFormGlobal.rjToggleActivateOSCListenerStart.Checked = Settings1.Default.activateOSCStart;



            VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked = Settings1.Default.chatBoxSpotifyOnly;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked = Settings1.Default.chatBoxSpotifyOnly4VRC;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked = Settings1.Default.SpotifyNoUseChatbox;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked = Settings1.Default.SpotifyNoUseKat;

            VoiceWizardWindow.MainFormGlobal.rjToggleOutputHeartrateDirect.Checked = Settings1.Default.heartrateOutput;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonWindowsMedia.Checked = Settings1.Default.enableMedia;

       

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked = Settings1.Default.StopOnPause;

            VoiceCommands.voiceCommandsStored = Settings1.Default.voiceCommandList;

            VoiceWizardWindow.MainFormGlobal.comboBox3Type.SelectedIndex = 0;
            VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedIndex = 0;
            

            VoiceWizardWindow.MainFormGlobal.modelTextBox.Text = Settings1.Default.modelnamesave;

            VoiceWizardWindow.MainFormGlobal.textBoxDeepLKey.Text = Settings1.Default.deepLKeysave;

            //OSC.OSCAddress = Settings1.Default.OSCAddress;
            // OSC.OSCPort = Settings1.Default.OSCPort;
           

            VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked = Settings1.Default.saveToast;

            VoicePresets.presetsStored = Settings1.Default.presetSave;
            VoicePresets.presetsLoad();

            MediaPresets.mediaPresetsStored = Settings1.Default.mediaPresetSave;
            MediaPresets.presetsLoad();

            WordReplacements.wordReplacemntsStored = Settings1.Default.replaceSave;
            WordReplacements.replacementsLoad();


            EmojiAddon.emojiReplacemntsStored = Settings1.Default.emojiNewSave;
            EmojiAddon.emojiReplacementsLoad();

            VoiceWizardWindow.MainFormGlobal.textBoxDiscordPara.Text = Settings1.Default.discordParaSave;
            VoiceWizardWindow.MainFormGlobal.textBoxDiscTimer.Text = Settings1.Default.discordTimerSave;

            VoiceWizardWindow.MainFormGlobal.rjToggleReplaceBeforeTTS.Checked = Settings1.Default.wordReplaceBeforeTTS;



            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text = Settings1.Default.minDuration;
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text = Settings1.Default.maxDuration;
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text = Settings1.Default.dropStartSilence;
            VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text = Settings1.Default.pauseDuration;

            StartUps.fontSize = Settings1.Default.fontSize;
            VoiceWizardWindow.MainFormGlobal.textBoxDelayAfterNoTTS.Text = Settings1.Default.delayAfterNoTTS;
            VoiceWizardWindow.MainFormGlobal.textBoxQueueDelayBeforeNext.Text = Settings1.Default.delayBeforeNewTTS;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked = Settings1.Default.saveToggleReadTextFile;
            VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text = Settings1.Default.saveTxtFilePath;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked = Settings1.Default.saveQueueSystem;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueTypedText.Checked = Settings1.Default.QueueWithTypedText;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonWhisperFilterInLog.Checked = Settings1.Default.WhisperFilterLog;

            string[] split = Settings1.Default.approvedSource.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {
                string trimmed = s.Trim();
                if (trimmed != "")
                {
                    VoiceWizardWindow.MainFormGlobal.checkedListBoxApproved.Items.Add(trimmed, true);
                }

            }





            VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked = Settings1.Default.hotketSave;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton12.Checked = Settings1.Default.shortcutStop;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonQuickTypeEnabled.Checked = Settings1.Default.shortcutQuickType;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoRefreshKAT.Checked = Settings1.Default.saveAutoRefreshKat;

            Hotkeys.modifierKeySTTTS = Settings1.Default.modHotKey;
            VoiceWizardWindow.MainFormGlobal.textBox4.Text = Hotkeys.modifierKeySTTTS;
            Hotkeys.normalKeySTTTS = Settings1.Default.normalHotKey;
            VoiceWizardWindow.MainFormGlobal.textBox1.Text = Hotkeys.normalKeySTTTS;

            Hotkeys.modifierKeyStopTTS = Settings1.Default.modHotkeyStop;
            VoiceWizardWindow.MainFormGlobal.textBoxStopTTS1.Text = Hotkeys.modifierKeyStopTTS;
            Hotkeys.normalKeyStopTTS = Settings1.Default.normalHotkeyStop;
            VoiceWizardWindow.MainFormGlobal.textBoxStopTTS2.Text = Hotkeys.normalKeyStopTTS;

            Hotkeys.modifierKeyQuickType = Settings1.Default.modHotkeyQuick;
            VoiceWizardWindow.MainFormGlobal.textBoxQuickType1.Text = Hotkeys.modifierKeyQuickType;
            Hotkeys.normalKeyQuickType = Settings1.Default.normalHotkeyQuick;
            VoiceWizardWindow.MainFormGlobal.textBoxQuickType2.Text = Hotkeys.normalKeyQuickType;


            Hotkeys.modifierKeyScrollUp = Settings1.Default.modifierKeyScrollUp;
            VoiceWizardWindow.MainFormGlobal.textBoxVoiceScrollUp1.Text = Hotkeys.modifierKeyScrollUp;
            Hotkeys.normalKeyScrollUp = Settings1.Default.normalKeyScrollUp;
            VoiceWizardWindow.MainFormGlobal.textBoxVoiceScrollUp2.Text = Hotkeys.normalKeyScrollUp;


            Hotkeys.modifierKeyScrollDown = Settings1.Default.modifierKeyScrollDown;
            VoiceWizardWindow.MainFormGlobal.textBoxVoiceScrollDown1.Text = Hotkeys.modifierKeyScrollDown;
            Hotkeys.normalKeyScrollDown = Settings1.Default.normalKeyScrollDown;
            VoiceWizardWindow.MainFormGlobal.textBoxVoiceScrollDown2.Text = Hotkeys.normalKeyScrollDown;


            VoiceWizardWindow.MainFormGlobal.textBoxAmazonKey.Text = Settings1.Default.yourAWSKey;
            VoiceWizardWindow.MainFormGlobal.textBoxAmazonSecret.Text = Settings1.Default.yourAWSSecret;
            VoiceWizardWindow.MainFormGlobal.textBox8.Text = Settings1.Default.yourAWSRegion;

            VoiceWizardWindow.MainFormGlobal.textBoxElevenLabsKey.Text = Settings1.Default.elevenLabsAPIKey;
            VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text = Settings1.Default.whisperModel;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOutputVRCCountersOnContact.Checked = Settings1.Default.VRCOnRecieve;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked = Settings1.Default.VRCAFK;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked = Settings1.Default.VRCSpamLog;



            VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text = Settings1.Default.AFKMsg;
            VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text = Settings1.Default.VRCPort;
            VRChatListener.FromVRChatPort = Settings1.Default.VRCPort;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCActivate.Checked = Settings1.Default.VRCListemOnStart;


            VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text = Settings1.Default.Counter1Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text = Settings1.Default.Counter2Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text = Settings1.Default.Counter3Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text = Settings1.Default.Counter4Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text = Settings1.Default.Counter5Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text = Settings1.Default.Counter6Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter7.Text = Settings1.Default.Counter7Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter8.Text = Settings1.Default.Counter8Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter9.Text = Settings1.Default.Counter9Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter10.Text = Settings1.Default.Counter10Para;

            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text = Settings1.Default.Counter1Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text = Settings1.Default.Counter2Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text = Settings1.Default.Counter3Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text = Settings1.Default.Counter4Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text = Settings1.Default.Counter5Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text = Settings1.Default.Counter6Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage7.Text = Settings1.Default.Counter7Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage8.Text = Settings1.Default.Counter8Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage9.Text = Settings1.Default.Counter9Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage10.Text = Settings1.Default.Counter10Msg;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked = Settings1.Default.SaveCounter;


            VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedIndex = 0;
           // VoiceWizardWindow.MainFormGlobal.comboBoxMediaPreset.SelectedIndex = 20; //test for errors


            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked == true)
            {
                VRChatListener.prevCounter1 = Settings1.Default.Counter1;
                VRChatListener.counter1 = Settings1.Default.Counter1;

                VRChatListener.prevCounter2 = Settings1.Default.Counter2;
                VRChatListener.counter2 = Settings1.Default.Counter2;

                VRChatListener.prevCounter3 = Settings1.Default.Counter3;
                VRChatListener.counter3 = Settings1.Default.Counter3;

                VRChatListener.prevCounter4 = Settings1.Default.Counter4;
                VRChatListener.counter4 = Settings1.Default.Counter4;

                VRChatListener.prevCounter5 = Settings1.Default.Counter5;
                VRChatListener.counter5 = Settings1.Default.Counter5;

                VRChatListener.prevCounter6 = Settings1.Default.Counter6;
                VRChatListener.counter6 = Settings1.Default.Counter6;

                VRChatListener.prevCounter7 = Settings1.Default.Counter7;
                VRChatListener.counter7 = Settings1.Default.Counter7;

                VRChatListener.prevCounter8 = Settings1.Default.Counter8;
                VRChatListener.counter8 = Settings1.Default.Counter8;

                VRChatListener.prevCounter9 = Settings1.Default.Counter9;
                VRChatListener.counter9 = Settings1.Default.Counter9;

                VRChatListener.prevCounter10 = Settings1.Default.Counter10;
                VRChatListener.counter10 = Settings1.Default.Counter10;
            }

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSmartStringSplit.Checked = Settings1.Default.SmartStringSplit;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOBSText.Checked = Settings1.Default.OBSTextFile;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia4OBS.Checked = Settings1.Default.OBS4Media;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonFilterNoiseWhisper.Checked = Settings1.Default.FilterNoiseWhisper;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonDisableWindowsMedia.Checked = Settings1.Default.WindowsMediaDisable;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonForwardData.Checked = Settings1.Default.forwardData;

            VoiceWizardWindow.MainFormGlobal.comboBoxWhisperModelDownload.SelectedItem = Settings1.Default.modelSelected;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonForceMedia.Checked = Settings1.Default.forceMediaToggle;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonRefocus.Checked = Settings1.Default.refocusWindow;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAzure.Checked = Settings1.Default.voiceWizProAzure;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAmazon.Checked = Settings1.Default.voiceWizProAmazon;
   
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked = Settings1.Default.useVoiceWizardPro;
            VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text = Settings1.Default.voiceWizardProKey;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked = Settings1.Default.voiceWizProTranslation;
            VoiceWizardWindow.MainFormGlobal.textBoxUberKey.Text = Settings1.Default.uberDuckKey;
            VoiceWizardWindow.MainFormGlobal.textBoxUberSecret.Text = Settings1.Default.uberDuckSecret;


            VoiceWizardWindow.MainFormGlobal.comboBoxLabsModelID.SelectedIndex = Settings1.Default.labsModelID;
            VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedIndex = Settings1.Default.labsOptimize;



            VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value = Settings1.Default.labsSimboost;
            VoiceWizardWindow.MainFormGlobal.trackBarStability.Value = Settings1.Default.labsStability;

            VoiceWizardWindow.MainFormGlobal.trackBarStyleExaggeration.Value = Settings1.Default.labsStyleExagg;
            VoiceWizardWindow.MainFormGlobal.rjToggleSpeakerBoost.Checked = Settings1.Default.labsSpeakerBoost;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked = Settings1.Default.saveToWav;

            VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text = Settings1.Default.charsUsed;
            VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text = Settings1.Default.transCharsUsed;
            VoiceWizardWindow.MainFormGlobal.labelChatGPTCharacters.Text = Settings1.Default.GPTUsageLabel;
            VoiceWizardWindow.MainFormGlobal.SpeechHoursUsed.Text = Settings1.Default.hoursUsed;

            VoiceWizardWindow.MainFormGlobal.minimumAudio.Text = Settings1.Default.minAudioDuration;
            VoiceWizardWindow.MainFormGlobal.maximumAudio.Text = Settings1.Default.maxAudioDuration;
            VoiceWizardWindow.MainFormGlobal.textBoxSilence.Text = Settings1.Default.SilenceThreshold;

            VoiceWizardWindow.MainFormGlobal.counterOutputInterval.Text = Settings1.Default.CounterOutputInterval;

            //  VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            //   {
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPara.SelectedIndex = Settings1.Default.SyncParaValue;
            }
            catch 
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPara.SelectedIndex = 0;
            }

           // });

            VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedItem = Settings1.Default.STTModeSave;
            VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem = Settings1.Default.ttsMode;

            VoiceWizardWindow.MainFormGlobal.textBoxSSSCharLimit.Text = Settings1.Default.SSSCharacterLimit;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAutoSend.Checked = Settings1.Default.autoSend;

            VoiceWizardWindow.MainFormGlobal.comboBoxProBranch.SelectedItem = Settings1.Default.ProAPIBranch;

            VoiceWizardWindow.MainFormGlobal.KATLineLengthTextBox.Text = Settings1.Default.KatLineLength;


            VoiceWizardWindow.MainFormGlobal.pulsoidAuthToken.Text = Settings1.Default.PulsoidAuthToken;

            VoiceWizardWindow.MainFormGlobal.textBoxPulsoidInterval.Text = Settings1.Default.PulsoidUpdateInterval;
            HeartratePulsoid.heartrateIntervalPulsoid = Settings1.Default.PulsoidUpdateInterval;

            VoiceWizardWindow.MainFormGlobal.rjToggleActivatePulsoidStart.Checked = Settings1.Default.PulsoidOnStart;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonUniqueWavNames.Checked = Settings1.Default.WaveUniqueNames;


            VoiceWizardWindow.MainFormGlobal.rjToggleTranslateSameLanguage.Checked = Settings1.Default.cheatAzureTranslation;
            VoiceWizardWindow.MainFormGlobal.WebsocketServerPortText.Text = Settings1.Default.websocketPort;
            WebSocketServer.WebSocketServerPort = Settings1.Default.websocketPort;
            VoiceWizardWindow.MainFormGlobal.rjToggleActivateWebsocketOnStart.Checked = Settings1.Default.websocketOnStart;

            VoiceWizardWindow.MainFormGlobal.rjToggleShowConnectURISpotify.Checked = Settings1.Default.showSpotifyURi;


            VoiceWizardWindow.MainFormGlobal.textBoxDelayBeforeAudio.Text = Settings1.Default.delayBeforeOuput;


            VoiceWizardWindow.MainFormGlobal.rjTogglePartialResults.Checked = Settings1.Default.azurePartialResults;

            VoiceWizardWindow.MainFormGlobal.textBoxPartialResultsInterval.Text = Settings1.Default.partialResultsInterval;

            VoiceWizardWindow.MainFormGlobal.comboBoxVADMode.SelectedIndex = Settings1.Default.WhisperVADMode;

            VoiceWizardWindow.MainFormGlobal.rjToggleVAD.Checked = Settings1.Default.VADForWhisper;
            VoiceWizardWindow.MainFormGlobal.rjToggleBothLanguages.Checked = Settings1.Default.bothLanguages;

            VoiceWizardWindow.MainFormGlobal.textBoxCustomTranslationOuput.Text = Settings1.Default.customTranslateText;


            VoiceWizardWindow.MainFormGlobal.trackBarSilence.Value = Settings1.Default.silenceBarValue;
            VoiceWizardWindow.MainFormGlobal.rjToggleDeepgramDebug.Checked = Settings1.Default.deepgramLogSpam;
            VoiceWizardWindow.MainFormGlobal.rjToggleDeepGramContinuous.Checked = Settings1.Default.deepGramContinuous;
            VoiceWizardWindow.MainFormGlobal.textBoxSilenceScale.Text = Settings1.Default.silenceScale;
            VoiceWizardWindow.MainFormGlobal.textBoxMinValidDeepgramDur.Text = Settings1.Default.deepGramValidDuration;

            VoiceWizardWindow.MainFormGlobal.textBoxWhisperVADOffset.Text = Settings1.Default.whisperVadOffset;

            VoiceWizardWindow.MainFormGlobal.rjToggleSwitchVoicePresetsBind.Checked = Settings1.Default.switchVoicePresetBindToggle;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonEnableChatGPT.Checked = Settings1.Default.useChatGPT;
            VoiceWizardWindow.MainFormGlobal.rjToggleUseMaxChatHistory.Checked= Settings1.Default.UseMaxChatHistory;

            VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text= Settings1.Default.ChatGPTAPIKey;
            VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text = Settings1.Default.ChatGPTModel;
            VoiceWizardWindow.MainFormGlobal.textBoxChatGPTMaxHistory.Text = Settings1.Default.ChatGPTMaxHistory;

            VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4ChatGPT.Checked = Settings1.Default.useChatGPTPro;
            VoiceWizardWindow.MainFormGlobal.rjToggleUseContextWithGPT.Checked = Settings1.Default.useContextWithGPT;
            VoiceWizardWindow.MainFormGlobal.rjToggleUsePro4OpenAITTS.Checked = Settings1.Default.useOpenAITTSPro;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonTypingIndicator.Checked = Settings1.Default.typeIndicator;

            VoiceWizardWindow.MainFormGlobal.richTextBoxGPTPrompt.Text = Settings1.Default.PromptText;
            VoiceWizardWindow.MainFormGlobal.rjToggleGPTUsePrompt.Checked = Settings1.Default.enablePrompt;

            VoiceWizardWindow.MainFormGlobal.rjToggleDisableAzureTranslation.Checked = Settings1.Default.DisableAzureTranslation;

           VoiceWizardWindow.MainFormGlobal.rjToggleExplicitPunctuation.Checked = Settings1.Default.ExplicitPunctuation;

            VoiceWizardWindow.MainFormGlobal.rjToggleUseWordBoundaries.Checked = Settings1.Default.useWordBoundaries;




            //  VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedItem = Settings1.Default.saveVoicePreset;
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = Settings1.Default.saveVoicePresetIndex;
            }
            catch(Exception ex)
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = 0;
            }
            

        }
    }
}
