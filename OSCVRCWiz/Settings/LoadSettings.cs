using System;
using System.Collections.Generic;
using System.Text;
using OSCVRCWiz.Settings;

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
            VoiceWizardWindow.MainFormGlobal.activationWord = Settings1.Default.activationWord;
            if (Settings1.Default.recognition == true)
            {
                var va = new VoiceActivation();

                va.loadSpeechRecognition(VoiceWizardWindow.MainFormGlobal);
                MessageBox.Show("[STTTS Voice Activation Initiated]");
            }
            VoiceWizardWindow.MainFormGlobal.textBox2.Text = Settings1.Default.yourKey;
            VoiceWizardWindow.MainFormGlobal.textBox3.Text = Settings1.Default.yourRegion;
            VoiceWizardWindow.YourSubscriptionKey = Settings1.Default.yourKey;
            VoiceWizardWindow.YourServiceRegion = Settings1.Default.yourRegion;

            VoiceWizardWindow.MainFormGlobal.textBoxDelay.Text = Settings1.Default.delayDebugValueSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonProfan.Checked = Settings1.Default.profanityFilterSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonLog.Checked = Settings1.Default.logOrNotSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOSC.Checked = Settings1.Default.sendOSCSetting;//on
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonClear.Checked = Settings1.Default.clearTTSWindowSetting;//off
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonOnTop2.Checked = Settings1.Default.alwaysTopSetting;//off
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonDisableTTS2.Checked = Settings1.Default.disableTTSSetting; //off
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonAsTranslated2.Checked = Settings1.Default.wordsTranslateVRCSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonHideDelay2.Checked = Settings1.Default.hideDelaySetting;//off
            VoiceWizardWindow.MainFormGlobal.textBoxErase.Text = Settings1.Default.hideDelayValue;
            VoiceWizardWindow.MainFormGlobal.richTextBox6.Text = Settings1.Default.phraseListValue;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPhraseList2.Checked = Settings1.Default.phraseListBoolSetting;
            VoiceWizardWindow.YourSubscriptionKey = Settings1.Default.yourKey;
            VoiceWizardWindow.YourServiceRegion = Settings1.Default.yourRegion;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonKeyRegion2.Checked = Settings1.Default.remember;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton3.Checked = Settings1.Default.EmojiSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCurrentSong.Checked = Settings1.Default.SpotifyOutputSetting;
            VoiceWizardWindow.HRInternalValue = Convert.ToInt32(Settings1.Default.HRIntervalSetting);
            VoiceWizardWindow.heartRatePort = Convert.ToInt32(Settings1.Default.HRPortSetting);
            VoiceWizardWindow.MainFormGlobal.rjToggleButton2.Checked = Settings1.Default.BPMSpamSetting;


            VoiceWizardWindow.MainFormGlobal.comboBox5.SelectedIndex = Settings1.Default.voiceLanguage;//voice language (make this save)


            // comboBox2.SelectedIndex = Settings1.Default.voiceBoxSetting;//voice
            // comboBox1.SelectedIndex = Settings1.Default.styleBoxSetting;//style (must be set after voice)
            VoiceWizardWindow.MainFormGlobal.comboBox3.SelectedIndex = Settings1.Default.langToBoxSetting;//language to
            VoiceWizardWindow.MainFormGlobal.comboBox4.SelectedIndex = Settings1.Default.langSpokenSetting;//language from [5 is english0
            VoiceWizardWindow.MainFormGlobal.comboBoxPitch.SelectedIndex = Settings1.Default.pitchSetting;
            VoiceWizardWindow.MainFormGlobal.comboBoxVolume.SelectedIndex = Settings1.Default.volumeSetting;
            VoiceWizardWindow.MainFormGlobal.comboBoxRate.SelectedIndex = Settings1.Default.rateSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButton4.Checked = Settings1.Default.STTTSContinuous;
            VoiceWizardWindow.MainFormGlobal.comboBoxInput.SelectedItem = Settings1.Default.MicName;
            VoiceWizardWindow.MainFormGlobal.comboBoxOutput.SelectedItem = Settings1.Default.SpeakerName;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonLiteMode.Checked = Settings1.Default.useBuiltInSetting;
            VoiceWizardWindow.MainFormGlobal.comboLiteInput.SelectedIndex = 0;

            VoiceWizardWindow.MainFormGlobal.comboBoxLite.SelectedIndex = Settings1.Default.BuiltInVoiceSetting;
            VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedIndex = 0;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton5.Checked = Settings1.Default.bannerSetting;
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButton5.Checked == true)
            {
                VoiceWizardWindow.MainFormGlobal.webView21.Dispose();
                VoiceWizardWindow.MainFormGlobal.button10.Dispose();
                VoiceWizardWindow.MainFormGlobal.button9.Dispose();
            }

            try
            {
                VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedItem = Settings1.Default.BuiltInOutputSetting;

            }
            catch (Exception ex)
            {
                VoiceWizardWindow.MainFormGlobal.comboLiteOutput.SelectedIndex = 0;

            }

            // comboLiteOutput.SelectedIndex = Settings1.Default.BuiltInOutputSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonPeriodic.Checked = Settings1.Default.SpotifyPeriodicallySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSpotifySpam.Checked = Settings1.Default.SpotifySpamSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text = Settings1.Default.SpotifyTimerIntervalSetting;
            VoiceWizardWindow.MainFormGlobal.timer1.Interval = Int32.Parse(VoiceWizardWindow.MainFormGlobal.textBoxSpotifyTime.Text.ToString());

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonCancelAudio.Checked = Settings1.Default.AudioCancelSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxCultureInfo.Text = Settings1.Default.cultureInfoSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxSpotKey.Text = Settings1.Default.SpotifyKey;
            VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked = Settings1.Default.SpotifyLegacySetting;
            SpotifyAddon.legacyState = VoiceWizardWindow.MainFormGlobal.rjToggleSpotLegacy.Checked;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBox.Checked = Settings1.Default.SendVRCChatBoxSetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonShowKeyboard.Checked = Settings1.Default.ChatBoxKeyboardSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButton6.Checked = Settings1.Default.minimizeToolBarSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonGreenScreen.Checked = Settings1.Default.GreenScreenSetting;
            VoiceWizardWindow.MainFormGlobal.textBoxFont.Text = Settings1.Default.fontSizeSetting;

            VoiceWizardWindow.MainFormGlobal.textBoxCustomSpot.Text = Settings1.Default.SpotifyCustomSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleSoundNotification.Checked = Settings1.Default.VRCSoundNotifySetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonSystemTray.Checked = Settings1.Default.SystemTraySetting;
            VoiceWizardWindow.MainFormGlobal.rjToggleButtonMedia.Checked = Settings1.Default.playMediaSetting;

            VoiceWizardWindow.MainFormGlobal.rjToggleButtonChatBoxUseDelay.Checked = Settings1.Default.VRCUseDelay;


            VoiceWizardWindow.MainFormGlobal.rjToggleButton7.Checked = Settings1.Default.WebcaptionerSetting;


            VoiceWizardWindow.MainFormGlobal.EmojiBox1.Text = Settings.emojiSettings.Default.emoji1;
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
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxPara.SelectedIndex = Settings1.Default.SyncParaValue;

            });
        }
    }
}
