using System;
using System.Collections.Generic;
using System.Text;
using EmbedIO.Sessions;
using OSCVRCWiz.Addons;
using OSCVRCWiz.TranslationAPIs;
using OSCVRCWiz.Settings;
using OSCVRCWiz.Resources;
using System.Printing;
using OSCVRCWiz.Text;
using Settings;
using Addons;

namespace OSCVRCWiz
{
    public class LoadSettings
    {
        public static void LoadingSettings()
        {
            //  try
            //   {
            //      File.WriteAllText("logfile.txt", String.Empty);
            //   }
            //   catch (Exception ex) { }


            //rjToggleButtonActivation.Checked = Settings1.Default.recognition; //activation phrase off
            //textBoxActivationWord.Text = Settings1.Default.activationWord;
            //VoiceWizardWindow.MainFormGlobal.activationWord = Settings1.Default.activationWord;
            VoiceWizardWindow.MainFormGlobal.textBox2.Text = Settings1.Default.yourKey;
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
            VoiceWizardWindow.MainFormGlobal.richTextBox6.Text = Settings1.Default.phraseListValue;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked = Settings1.Default.phraseListBoolSetting;
            AzureRecognition.YourSubscriptionKey = Settings1.Default.yourKey;
            AzureRecognition.YourServiceRegion = Settings1.Default.yourRegion;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonKeyRegion2.Checked = Settings1.Default.remember;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked = Settings1.Default.EmojiSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked = Settings1.Default.SpotifyOutputSetting;
            OSCListener.HRInternalValue = Convert.ToInt32(Settings1.Default.HRIntervalSetting);
            OSCListener.OSCReceiveport = Convert.ToInt32(Settings1.Default.HRPortSetting);
            VoiceWizardWindow.MainFormGlobal.textBoxHRPort.Text = Settings1.Default.HRPortSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton2.Checked = Settings1.Default.BPMSpamSetting;

            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedIndex = Settings1.Default.voiceLanguage;//voice language (make this save)
            }
            catch (Exception ex)
            {
                VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedIndex = 0;
            }

            // comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
            // comboBox1.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
            // VoiceWizardWindow.TTSModeSaved=Settings1.Default.ttsMode;
            VoiceWizardWindow.MainFormGlobal.rjToggleDarkMode.Checked = Settings1.Default.saveDarkMode;


            VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedIndex = Settings1.Default.langToBoxSetting;//language to
            VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedIndex = Settings1.Default.langSpokenSetting;//language from [5 is english0
            try
            {
                //  VoiceWizardWindow.MainFormGlobal.comboBoxPitch.SelectedIndex = Settings1.Default.pitchSetting;
                //  VoiceWizardWindow.MainFormGlobal.comboBoxVolume.SelectedIndex = Settings1.Default.volumeSetting;
                //   VoiceWizardWindow.MainFormGlobal.comboBoxRate.SelectedIndex = Settings1.Default.rateSetting;
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[One of your TTS setttings was invalid (pitch, volume or speed). Setting to defaults.]", Color.Red);
                //   VoiceWizardWindow.MainFormGlobal.comboBoxPitch.SelectedItem = "default";
                //   VoiceWizardWindow.MainFormGlobal.comboBoxVolume.SelectedItem = "default";
                //   VoiceWizardWindow.MainFormGlobal.comboBoxRate.SelectedItem = "default";

            }

            VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value = Settings1.Default.pitchNew;
            VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value = Settings1.Default.volumeNew;
            VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value = Settings1.Default.speedNew;
            VoiceWizardWindow.MainFormGlobal.updateAllTrackBarLabels();

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked = Settings1.Default.use2ndDevice;


            VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked = Settings1.Default.STTTSContinuous;






            // VoiceWizardWindow.MainFormGlobal.rjToggleButtonLiteMode.Checked = Settings1.Default.useBuiltInSetting;
            //VoiceWizardWindow.MainFormGlobal.comboLiteInput.SelectedIndex = 0;


            // VoiceWizardWindow.MainFormGlobal.comboBoxLite.SelectedIndex = Settings1.Default.BuiltInVoiceSetting;
            // VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedIndex = 0;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton5.Checked = Settings1.Default.bannerSetting;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButton5.Checked == true)
            {
                VoiceWizardWindow.MainFormGlobal.webView21.Dispose();
                VoiceWizardWindow.MainFormGlobal.button10.Dispose();
                VoiceWizardWindow.MainFormGlobal.button9.Dispose();
            }

            try
            {
                // VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedItem = Settings1.Default.BuiltInOutputSetting;

            }
            catch (Exception ex)
            {
                // VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedIndex = 0;

            }

            // comboLiteOutput.SelectedIndex = Settings1.Default.BuiltInOutputSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked = Settings1.Default.SpotifyPeriodicallySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked = Settings1.Default.SpotifySpamSetting;

            SpotifyAddon.spotifyInterval = Settings1.Default.SpotifyTimerIntervalSetting;
            VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text = SpotifyAddon.spotifyInterval;
            //VoiceWizardWindow.MainFormGlobal.timer1.Interval = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text.ToString());
            // VoiceWizardWindow.MainFormGlobal.timer1.Interval = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text.ToString());
            VoiceWizardWindow.spotifyTimer = new System.Threading.Timer(VoiceWizardWindow.MainFormGlobal.spotifytimertick);
            VoiceWizardWindow.spotifyTimer.Change(Int32.Parse(SpotifyAddon.spotifyInterval), 0);

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonStopCurrentTTS.Checked = Settings1.Default.AudioCancelSetting;

            //  VoiceWizardWindow.MainFormGlobal.textBoxCultureInfo.Text = Settings1.Default.cultureInfoSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxSpotKey.Text = Settings1.Default.SpotifyKey;
            VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked = Settings1.Default.SpotifyLegacySetting;
            SpotifyAddon.legacyState = VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked = Settings1.Default.SendVRCChatBoxSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked = Settings1.Default.ChatBoxKeyboardSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton6.Checked = Settings1.Default.minimizeToolBarSetting;

            // VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked = Settings1.Default.GreenScreenSetting;
            //   VoiceWizardWindow.MainFormGlobal.textBoxFont.Text = Settings1.Default.fontSizeSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text = Settings1.Default.SpotifyCustomSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked = Settings1.Default.VRCSoundNotifySetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked = Settings1.Default.SystemTraySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia.Checked = Settings1.Default.playMediaSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked = Settings1.Default.VRCUseDelay;





            VoiceWizardWindow.MainFormGlobal.textBoxOSCAddress.Text = Settings1.Default.rememberAddress;
            VoiceWizardWindow.MainFormGlobal.textBoxOSCPort.Text = Settings1.Default.rememberPort;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton8.Checked = Settings1.Default.activateOSCStart;

            //  VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked= Settings1.Default.independentSpotifyText;


            // VoiceWizardWindow.MainFormGlobal.rjToggleButton7.Checked = Settings1.Default.

            //  VoiceWizardWindow.MainFormGlobal.rjToggleButton7.Checked = Settings1.Default.WebcaptionerSetting; //already saves
            //  VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked = Settings1.Default.saveUseAzure;
            // VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapSystem.Checked = Settings1.Default.saveUseSystem;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked = Settings1.Default.chatBoxSpotifyOnly;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked = Settings1.Default.chatBoxSpotifyOnly4VRC;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked = Settings1.Default.SpotifyNoUseChatbox;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked = Settings1.Default.SpotifyNoUseKat;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton1.Checked = Settings1.Default.heartrateOutput;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked = Settings1.Default.enableMedia;

            //  VoiceWizardWindow.MainFormGlobal.richTextBox11.Text = Settings1.Default.approvedSource;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPlayPaused.Checked = Settings1.Default.StopOnPause;

            VoiceCommands.voiceCommandsStored = Settings1.Default.voiceCommandList;

            VoiceWizardWindow.MainFormGlobal.comboBox3Type.SelectedIndex = 0;
            VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedIndex = 0;
            VoiceWizardWindow.MainFormGlobal.comboBoxPreset.SelectedIndex = 0;

            VoiceWizardWindow.MainFormGlobal.modelTextBox.Text = Settings1.Default.modelnamesave;

            // DeepLTranslate.DeepLKey= Settings1.Default.deepLKeysave;
            VoiceWizardWindow.MainFormGlobal.textBox5.Text = Settings1.Default.deepLKeysave;

            OSC.OSCAddress = Settings1.Default.OSCAddress;
            OSC.OSCPort = Settings1.Default.OSCPort;
            VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text = Settings1.Default.OSCPort;

            VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked = Settings1.Default.saveToast;

            VoicePresets.presetsStored = Settings1.Default.presetSave;
            VoicePresets.presetsLoad();

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

            VoiceWizardWindow.fontSize = Settings1.Default.fontSize;
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

            VoiceWizardWindow.modifierKeySTTTS = Settings1.Default.modHotKey;
            VoiceWizardWindow.MainFormGlobal.textBox4.Text = VoiceWizardWindow.modifierKeySTTTS;
            VoiceWizardWindow.normalKeySTTTS = Settings1.Default.normalHotKey;
            VoiceWizardWindow.MainFormGlobal.textBox1.Text = VoiceWizardWindow.normalKeySTTTS;

            VoiceWizardWindow.modifierKeyStopTTS = Settings1.Default.modHotkeyStop;
            VoiceWizardWindow.MainFormGlobal.textBoxStopTTS1.Text = VoiceWizardWindow.modifierKeyStopTTS;
            VoiceWizardWindow.normalKeyStopTTS = Settings1.Default.normalHotkeyStop;
            VoiceWizardWindow.MainFormGlobal.textBoxStopTTS2.Text = VoiceWizardWindow.normalKeyStopTTS;

            VoiceWizardWindow.modifierKeyQuickType = Settings1.Default.modHotkeyQuick;
            VoiceWizardWindow.MainFormGlobal.textBoxQuickType1.Text = VoiceWizardWindow.modifierKeyQuickType;
            VoiceWizardWindow.normalKeyQuickType = Settings1.Default.normalHotkeyQuick;
            VoiceWizardWindow.MainFormGlobal.textBoxQuickType2.Text = VoiceWizardWindow.normalKeyQuickType;


            VoiceWizardWindow.MainFormGlobal.textBox9.Text = Settings1.Default.yourAWSKey;
            VoiceWizardWindow.MainFormGlobal.textBox10.Text = Settings1.Default.yourAWSSecret;
            VoiceWizardWindow.MainFormGlobal.textBox8.Text = Settings1.Default.yourAWSRegion;

            VoiceWizardWindow.MainFormGlobal.textBox12.Text = Settings1.Default.elevenLabsAPIKey;
            VoiceWizardWindow.MainFormGlobal.whisperModelTextBox.Text = Settings1.Default.whisperModel;


            VoiceWizardWindow.MainFormGlobal.rjToggleButton13.Checked = Settings1.Default.VRCOnRecieve;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked = Settings1.Default.VRCAFK;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked = Settings1.Default.VRCSpamLog;



            VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text = Settings1.Default.AFKMsg;
            VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text = Settings1.Default.VRCPort;
            OSC.FromVRChatPort = Settings1.Default.VRCPort;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton11.Checked = Settings1.Default.VRCListemOnStart;


            VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text = Settings1.Default.Counter1Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text = Settings1.Default.Counter2Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text = Settings1.Default.Counter3Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text = Settings1.Default.Counter4Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text = Settings1.Default.Counter5Para;
            VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text = Settings1.Default.Counter6Para;

            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text = Settings1.Default.Counter1Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text = Settings1.Default.Counter2Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text = Settings1.Default.Counter3Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text = Settings1.Default.Counter4Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text = Settings1.Default.Counter5Msg;
            VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text = Settings1.Default.Counter6Msg;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked = Settings1.Default.SaveCounter;



            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked == true)
            {
                OSC.prevCounter1 = Settings1.Default.Counter1;
                OSC.counter1 = Settings1.Default.Counter1;

                OSC.prevCounter2 = Settings1.Default.Counter2;
                OSC.counter2 = Settings1.Default.Counter2;

                OSC.prevCounter3 = Settings1.Default.Counter3;
                OSC.counter3 = Settings1.Default.Counter3;

                OSC.prevCounter4 = Settings1.Default.Counter4;
                OSC.counter4 = Settings1.Default.Counter4;

                OSC.prevCounter5 = Settings1.Default.Counter5;
                OSC.counter5 = Settings1.Default.Counter5;

                OSC.prevCounter6 = Settings1.Default.Counter6;
                OSC.counter6 = Settings1.Default.Counter6;
            }


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
            //  VoiceWizardWindow.MainFormGlobal.rjToggleButtonProMoonbase.Checked = Settings1.Default.voiceWizProMoonbase;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked = Settings1.Default.useVoiceWizardPro;
            VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text = Settings1.Default.voiceWizardProKey;


            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked = Settings1.Default.voiceWizProTranslation;
            VoiceWizardWindow.MainFormGlobal.textBoxUberKey.Text = Settings1.Default.uberDuckKey;
            VoiceWizardWindow.MainFormGlobal.textBoxUberSecret.Text = Settings1.Default.uberDuckSecret;


            VoiceWizardWindow.MainFormGlobal.comboBoxLabsModelID.SelectedIndex = Settings1.Default.labsModelID;
            VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedIndex = Settings1.Default.labsOptimize;



            VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value = Settings1.Default.labsSimboost;
            VoiceWizardWindow.MainFormGlobal.trackBarStability.Value = Settings1.Default.labsStability;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked = Settings1.Default.saveToWav;

            VoiceWizardWindow.MainFormGlobal.labelTTSCharacters.Text = Settings1.Default.charsUsed;
            VoiceWizardWindow.MainFormGlobal.labelTranslationCharacters.Text = Settings1.Default.transCharsUsed;

            // VoiceWizardWindow.MainFormGlobal.rjToggleButtonAprilFools.Checked = Settings1.Default.aprilFools2023;

            /*  VoiceWizardWindow.MainFormGlobal.EmojiBox1.Text = Settings.emojiSettings.Default.emoji1;
              VoiceWizardWindow.MainFormGlobal.EmojiBox2.Text = emojiSettings.Default.emoji2;
              VoiceWizardWindow.MainFormGlobal.EmojiBox3.Text = emojiSettings.Default.emoji3;
              VoiceWizardWindow.MainFormGlobal.EmojiBox4.Text = emojiSettings.Default.emoji4;
              VoiceWizardWindow.MainFormGlobal.EmojiBox5.Text = emojiSettings.Default.emoji5;
              VoiceWizardWindow.MainFormGlobal.EmojiBox6.Text = emojiSettings.Default.emoji6;
              VoiceWizardWindow.MainFormGlobal.EmojiBox7.Text = emojiSettings.Default.emoji7;
              VoiceWizardWindow.MainFormGlobal.EmojiBox8.Text = emojiSettings.Default.emoji8;
              VoiceWizardWindow.MainFormGlobal.EmojiBox9.Text = emojiSettings.Default.emoji9;
              VoiceWizardWindow.MainFormGlobal.EmojiBox10.Text = emojiSettings.Default.emoji10;
              VoiceWizardWindow.MainFormGlobal.EmojiBox11.Text = emojiSettings.Default.emoji11;
              VoiceWizardWindow.MainFormGlobal.EmojiBox12.Text = emojiSettings.Default.emoji12;
              VoiceWizardWindow.MainFormGlobal.EmojiBox13.Text = emojiSettings.Default.emoji13;
              VoiceWizardWindow.MainFormGlobal.EmojiBox14.Text = emojiSettings.Default.emoji14;
              VoiceWizardWindow.MainFormGlobal.EmojiBox15.Text = emojiSettings.Default.emoji15;
              VoiceWizardWindow.MainFormGlobal.EmojiBox16.Text = emojiSettings.Default.emoji16;
              VoiceWizardWindow.MainFormGlobal.EmojiBox17.Text = emojiSettings.Default.emoji17;
              VoiceWizardWindow.MainFormGlobal.EmojiBox18.Text = emojiSettings.Default.emoji18;
              VoiceWizardWindow.MainFormGlobal.EmojiBox19.Text = emojiSettings.Default.emoji19;
              VoiceWizardWindow.MainFormGlobal.EmojiBox20.Text = emojiSettings.Default.emoji20;
            */
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPara.SelectedIndex = Settings1.Default.SyncParaValue;

            });
            VoiceWizardWindow.MainFormGlobal.comboBoxSTT.SelectedItem = Settings1.Default.STTModeSave;
            VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem = Settings1.Default.ttsMode;

        }
    }
}
