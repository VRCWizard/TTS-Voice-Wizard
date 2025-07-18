using OSCVRCWiz;
using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OSCVRCWiz.Services.Integrations
{
    public class EmojiAddon
    {

        public static string emojiReplacemntsStored = "";
        public static List<string> ReplacePhraseList = new List<string>()
        {
             "<3",

            "cry emoji",
            "glasses emoji",
            "heart face emoji",
            "roxy",
            "aqua cry",
            "thumbs up",
            "confused",
            "angry emoji",
            "laugh emoji",
            "poggers emoji",

              "EE12",
              "EE13",
              "EE14",
              "EE15",
              "EE16",
              "EE17",
              "EE18",
              "EE19",
              "EE20",

              "EE21",
              "EE22",
              "EE23",
              "EE24",
              "EE25",
              "EE26",
              "EE27",
              "EE28",
              "EE29",
              "EE30",

              "EE31",
              "EE32",
              "EE33",
              "EE34",
              "EE35",
              "EE36",
              "EE37",
              "EE38",
              "EE39",
              "EE40",

              "EE41",
              "EE42",
              "EE43",
              "EE44",
              "EE45",
              "EE46",
              "EE47",
              "EE48",
              "EE49",
              "EE40",

              "EE51",
              "EE52",
              "EE53",
              "EE54",
              "EE55",
              "EE56",
              "EE57",
              "EE58",
              "EE59",
              "EE60",

              "EE61",
              "EE62",
              "EE63",
              "EE64",

        };


        public static List<string> EmojisTextList = new List<string>()
        {


            "ぬ",

            "あう",
            "えお",
            "やゆ",
            "よわ",
            "をほ",
            "へた",
            "てい",
            "すか",
            "んな",
            "にら",

            "せち",
            "とし",
            "はき",
            "くま",
            "のり",
            "れけ",
            "むつ",
            "さそ",
            "ひこ",
            "みも",

            "ねる",
            "めろ",
            "。ぶ",
            "ぷぼ",
            "ぽべ",
            "ぺだ",
            "でず",
            "がぜ",
            "ぢど",
            "じば",

            "ぱぎ",
            "ぐげ",
            "づざ",
            "ぞび",
            "ぴご",
            "ぁぃ",
            "ぅぇ",
            "ぉゃ",
            "ゅょ",
            "ヌフ",

            "アウ",
            "エオ",
            "ヤユ",
            "ヨワ",
            "ヲホ",
            "ヘタ",
            "テイ",
            "スカ",
            "ンナ",
            "ニラ",

            "セチ",
            "トシ",
            "ハキ",
            "クマ",
            "ノリ",
            "レケ",
            "ムツ",
            "サソ",
            "ヒコ",
            "ミモ",

            "ネル",
            "メロ",
            "〝°",

        };





        public static string DoEmojiReplacement(string text)
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((System.Windows.Forms.MethodInvoker)delegate ()
            {
                int index = 0;
                foreach (var phrase in ReplacePhraseList)
                {

                    if (text.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                    {


                        if (VoiceWizardWindow.MainFormGlobal.checkedListBox2.GetItemCheckState(index) == CheckState.Checked)
                        {
                            //text = text.Replace(phrase, EmojisTextList.ElementAt(index));

                            text = Regex.Replace(text, phrase, EmojisTextList.ElementAt(index), RegexOptions.IgnoreCase);

                        }

                    }
                    index++;
                }

            });
            return text;
        }

        public static void emojiEdit(int index, string phrase)
        {
            VoiceWizardWindow.MainFormGlobal.checkedListBox2.Items[index - 1] = "Emoji " + index + ": " + phrase;
            ReplacePhraseList[index - 1] = phrase;
            emojiReplacementsSave();

        }

        public static void emojiReplacementsLoad()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();

            if (emojiReplacemntsStored != "") // if it's empty it will use the default list set above
            {
                ReplacePhraseList.Clear();
                string words = emojiReplacemntsStored;
                string[] split = words.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                int index = 1;
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                    {
                        string phrase = s;

                        try
                        {
                            //VoiceWizardWindow.MainFormGlobal.comboBoxPreset.Items.Add(saveThisPreset.PresetName);
                            ReplacePhraseList.Add(phrase);
                            VoiceWizardWindow.MainFormGlobal.checkedListBox2.Items.Add("Emoji " + index + ": " + phrase, true);

                        }
                        catch (Exception ex)
                        {
                            OutputText.outputLog(ex.Message, Color.DarkOrange);
                        }
                    }
                    index++;
                }
            }
            else// this will only run ONCE EVER WHEN U RUN A NEW VERSION OF TTS VOICE WIZARD because after that it will have a saved list to load each time
            {
                int index = 1;
                foreach (var phrase in ReplacePhraseList)
                {

                    VoiceWizardWindow.MainFormGlobal.checkedListBox2.Items.Add("Emoji " + index + ": " + phrase, true);
                    index++;
                }
            }

        }
        public static void emojiReplacementsSave()
        {
            emojiReplacemntsStored = "";
            foreach (var phrase in ReplacePhraseList)
            {

                emojiReplacemntsStored += $"{phrase};";
            }
        }



    }
}
