using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Addons;
using EmbedIO.Sessions;
using OSCVRCWiz.Addons;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Settings;
using OSCVRCWiz.TranslationAPIs;
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
            Settings1.Default.phraseListValue = VoiceWizardWindow.MainFormGlobal.richTextBox6.Text.ToString();
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
            Settings1.Default.BPMSpamSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton2.Checked;
            Settings1.Default.voiceBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBox2.SelectedIndex;
            Settings1.Default.styleBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedIndex;

            Settings1.Default.voiceLanguage = VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedIndex;//voice language (make this save)
            Settings1.Default.langToBoxSetting = VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedIndex;
            Settings1.Default.langSpokenSetting = VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedIndex;



            //   Settings1.Default.pitchSetting = VoiceWizardWindow.MainFormGlobal.comboBoxPitch.SelectedIndex;
            //   Settings1.Default.volumeSetting = VoiceWizardWindow.MainFormGlobal.comboBoxVolume.SelectedIndex;
            //   Settings1.Default.rateSetting = VoiceWizardWindow.MainFormGlobal.comboBoxRate.SelectedIndex;

            Settings1.Default.pitchNew = VoiceWizardWindow.MainFormGlobal.trackBarPitch.Value;
            Settings1.Default.volumeNew = VoiceWizardWindow.MainFormGlobal.trackBarVolume.Value;
            Settings1.Default.speedNew = VoiceWizardWindow.MainFormGlobal.trackBarSpeed.Value;

            Settings1.Default.use2ndDevice = VoiceWizardWindow.MainFormGlobal.rjToggleButtonUse2ndOutput.Checked;


            Settings1.Default.STTTSContinuous = VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked;
            // Settings1.Default.useBuiltInSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonLiteMode.Checked;
            // Settings1.Default.BuiltInVoiceSetting = VoiceWizardWindow.MainFormGlobal.comboBoxLite.SelectedIndex;
            // Settings1.Default.BuiltInOutputSetting = VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedItem.ToString();

            Settings1.Default.SpotifyPeriodicallySetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked;
            Settings1.Default.SpotifySpamSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked;
            Settings1.Default.SpotifyTimerIntervalSetting = SpotifyAddon.spotifyInterval;

            Settings1.Default.AudioCancelSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonStopCurrentTTS.Checked;
            // Settings1.Default.cultureInfoSetting = VoiceWizardWindow.MainFormGlobal.textBoxCultureInfo.Text.ToString();

            Settings1.Default.bannerSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton5.Checked;

            Settings1.Default.SpotifyKey = VoiceWizardWindow.MainFormGlobal.textBoxSpotKey.Text.ToString();
            Settings1.Default.SpotifyLegacySetting = VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked;

            Settings1.Default.SendVRCChatBoxSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked;
            Settings1.Default.ChatBoxKeyboardSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked;

            Settings1.Default.minimizeToolBarSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton6.Checked;
            //  Settings1.Default.GreenScreenSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked;
            Settings1.Default.SpotifyCustomSetting = VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text.ToString();


            Settings1.Default.VRCSoundNotifySetting = VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked;

            Settings1.Default.SystemTraySetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked;
            Settings1.Default.playMediaSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia.Checked;

            Settings1.Default.VRCUseDelay = VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked;



            Settings1.Default.activateOSCStart = VoiceWizardWindow.MainFormGlobal.rjToggleButton8.Checked;

            //  Settings1.Default.independentSpotifyText = VoiceWizardWindow.MainFormGlobal.rjToggleButton9.Checked;


            //Settings1.Default.WebcaptionerSetting = VoiceWizardWindow.MainFormGlobal.rjToggleButton7.Checked;
            // Settings1.Default.saveUseAzure = VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapAzure.Checked;
            // Settings1.Default.saveUseSystem = VoiceWizardWindow.MainFormGlobal.rjToggleButtonWebCapSystem.Checked;

            Settings1.Default.chatBoxSpotifyOnly = VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSKAT.Checked;
            Settings1.Default.chatBoxSpotifyOnly4VRC = VoiceWizardWindow.MainFormGlobal.rjToggleButtonNoTTSChat.Checked;

            Settings1.Default.SpotifyNoUseChatbox = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyChatboxDisable.Checked;
            Settings1.Default.SpotifyNoUseKat = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifyKatDisable.Checked;

            Settings1.Default.heartrateOutput = VoiceWizardWindow.MainFormGlobal.rjToggleButton1.Checked;

            Settings1.Default.enableMedia = VoiceWizardWindow.MainFormGlobal.rjToggleButton10.Checked;

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

            Settings1.Default.OSCAddress = OSC.OSCAddress;
            Settings1.Default.OSCPort = OSC.OSCPort;

            Settings1.Default.saveToast = VoiceWizardWindow.MainFormGlobal.rjToggleDiscordToast.Checked;

            VoicePresets.presetsSave();
            Settings1.Default.presetSave = VoicePresets.presetsStored;

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

            Settings1.Default.modHotKey = VoiceWizardWindow.modifierKeySTTTS;
            Settings1.Default.normalHotKey = VoiceWizardWindow.normalKeySTTTS;

            Settings1.Default.modHotkeyStop = VoiceWizardWindow.modifierKeyStopTTS;
            Settings1.Default.normalHotkeyStop = VoiceWizardWindow.normalKeyStopTTS;

            Settings1.Default.modHotkeyQuick = VoiceWizardWindow.modifierKeyQuickType;
            Settings1.Default.normalHotkeyQuick = VoiceWizardWindow.normalKeyQuickType;



            Settings1.Default.VRCOnRecieve = VoiceWizardWindow.MainFormGlobal.rjToggleButton13.Checked;
            Settings1.Default.VRCAFK = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAFK.Checked;
            Settings1.Default.VRCListemOnStart = VoiceWizardWindow.MainFormGlobal.rjToggleButton11.Checked;
            Settings1.Default.VRCSpamLog = VoiceWizardWindow.MainFormGlobal.rjToggleButtonVRCSpamLog.Checked;

            Settings1.Default.AFKMsg = VoiceWizardWindow.MainFormGlobal.textBoxAFK.Text.ToString();
            Settings1.Default.VRCPort = VoiceWizardWindow.MainFormGlobal.textBoxVRChatOSCPort.Text.ToString();

            Settings1.Default.Counter1Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter1.Text.ToString();
            Settings1.Default.Counter2Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter2.Text.ToString();
            Settings1.Default.Counter3Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter3.Text.ToString();
            Settings1.Default.Counter4Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter4.Text.ToString();
            Settings1.Default.Counter5Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter5.Text.ToString();
            Settings1.Default.Counter6Para = VoiceWizardWindow.MainFormGlobal.textBoxCounter6.Text.ToString();

            Settings1.Default.Counter1Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage1.Text.ToString();
            Settings1.Default.Counter2Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage2.Text.ToString();
            Settings1.Default.Counter3Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage3.Text.ToString();
            Settings1.Default.Counter4Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage4.Text.ToString();
            Settings1.Default.Counter5Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage5.Text.ToString();
            Settings1.Default.Counter6Msg = VoiceWizardWindow.MainFormGlobal.textBoxCounterMessage6.Text.ToString();

            Settings1.Default.SaveCounter = VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked;


            Settings1.Default.minDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperMinDuration.Text;
            Settings1.Default.maxDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperMaxDuration.Text;
            Settings1.Default.dropStartSilence = VoiceWizardWindow.MainFormGlobal.textBoxWhisperDropSilence.Text;
            Settings1.Default.pauseDuration = VoiceWizardWindow.MainFormGlobal.textBoxWhisperPauseDuration.Text;

            Settings1.Default.fontSize = VoiceWizardWindow.fontSize;

            Settings1.Default.delayAfterNoTTS = VoiceWizardWindow.MainFormGlobal.textBoxDelayAfterNoTTS.Text;
            Settings1.Default.delayBeforeNewTTS = VoiceWizardWindow.MainFormGlobal.textBoxQueueDelayBeforeNext.Text;

            Settings1.Default.saveQueueSystem = VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueSystem.Checked;
            Settings1.Default.QueueWithTypedText = VoiceWizardWindow.MainFormGlobal.rjToggleButtonQueueTypedText.Checked;

            Settings1.Default.saveToggleReadTextFile = VoiceWizardWindow.MainFormGlobal.rjToggleButtonReadFromFile.Checked;
            Settings1.Default.saveTxtFilePath = VoiceWizardWindow.MainFormGlobal.textBoxReadFromTXTFile.Text;
            ///

            /*  if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonCounterSaver.Checked == true)
              {
                  Settings1.Default.Counter1 = OSC.counter1;
                  Settings1.Default.Counter2 = OSC.counter2;
                  Settings1.Default.Counter3 = OSC.counter3;
                  Settings1.Default.Counter4 = OSC.counter4;
                  Settings1.Default.Counter5 = OSC.counter5;
                  Settings1.Default.Counter6 = OSC.counter6;
              }*/

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
            //  Settings1.Default.voiceWizProMoonbase = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProMoonbase.Checked;
            Settings1.Default.useVoiceWizardPro = VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked;
            Settings1.Default.voiceWizardProKey = VoiceWizardWindow.MainFormGlobal.textBoxWizardProKey.Text.ToString();


            Settings1.Default.voiceWizProTranslation = VoiceWizardWindow.MainFormGlobal.rjToggleButtonProTranslation.Checked;
            Settings1.Default.uberDuckKey = VoiceWizardWindow.MainFormGlobal.textBoxUberKey.Text.ToString();
            Settings1.Default.uberDuckSecret = VoiceWizardWindow.MainFormGlobal.textBoxUberSecret.Text.ToString();

            Settings1.Default.labsModelID = VoiceWizardWindow.MainFormGlobal.comboBoxLabsModelID.SelectedIndex;
            Settings1.Default.labsOptimize = VoiceWizardWindow.MainFormGlobal.comboBoxLabsOptimize.SelectedIndex;
            Settings1.Default.labsSimboost = VoiceWizardWindow.MainFormGlobal.trackBarSimilarity.Value;
            Settings1.Default.labsStability = VoiceWizardWindow.MainFormGlobal.trackBarStability.Value;

            Settings1.Default.saveToWav = VoiceWizardWindow.MainFormGlobal.rjToggleButtonSaveToWav.Checked;


            // Settings1.Default.aprilFools2023 = VoiceWizardWindow.MainFormGlobal.rjToggleButtonAprilFools.Checked;


            /*
            emojiSettings.Default.emoji1 = VoiceWizardWindow.MainFormGlobal.EmojiBox1.Text.ToString();
            emojiSettings.Default.emoji2 = VoiceWizardWindow.MainFormGlobal.EmojiBox2.Text.ToString();
            emojiSettings.Default.emoji3 = VoiceWizardWindow.MainFormGlobal.EmojiBox3.Text.ToString();
            emojiSettings.Default.emoji4 = VoiceWizardWindow.MainFormGlobal.EmojiBox4.Text.ToString();
            emojiSettings.Default.emoji5 = VoiceWizardWindow.MainFormGlobal.EmojiBox5.Text.ToString();
            emojiSettings.Default.emoji6 = VoiceWizardWindow.MainFormGlobal.EmojiBox6.Text.ToString();
            emojiSettings.Default.emoji7 = VoiceWizardWindow.MainFormGlobal.EmojiBox7.Text.ToString();
            emojiSettings.Default.emoji8 = VoiceWizardWindow.MainFormGlobal.EmojiBox8.Text.ToString();
            emojiSettings.Default.emoji9 = VoiceWizardWindow.MainFormGlobal.EmojiBox9.Text.ToString();
            emojiSettings.Default.emoji10 = VoiceWizardWindow.MainFormGlobal.EmojiBox10.Text.ToString();
            emojiSettings.Default.emoji11 = VoiceWizardWindow.MainFormGlobal.EmojiBox11.Text.ToString();
            emojiSettings.Default.emoji12 = VoiceWizardWindow.MainFormGlobal.EmojiBox12.Text.ToString();
            emojiSettings.Default.emoji13 = VoiceWizardWindow.MainFormGlobal.EmojiBox13.Text.ToString();
            emojiSettings.Default.emoji14 = VoiceWizardWindow.MainFormGlobal.EmojiBox14.Text.ToString();
            emojiSettings.Default.emoji15 = VoiceWizardWindow.MainFormGlobal.EmojiBox15.Text.ToString();
            emojiSettings.Default.emoji16 = VoiceWizardWindow.MainFormGlobal.EmojiBox16.Text.ToString();
            emojiSettings.Default.emoji17 = VoiceWizardWindow.MainFormGlobal.EmojiBox17.Text.ToString();
            emojiSettings.Default.emoji18 = VoiceWizardWindow.MainFormGlobal.EmojiBox18.Text.ToString();
            emojiSettings.Default.emoji19 = VoiceWizardWindow.MainFormGlobal.EmojiBox19.Text.ToString();
            emojiSettings.Default.emoji20 = VoiceWizardWindow.MainFormGlobal.EmojiBox20.Text.ToString(); */



            Settings1.Default.Save();
            //emojiSettings.Default.Save();
            VoiceWizardWindow.MainFormGlobal.webView21.Dispose();
        }
    }
}
