using System;
using System.Collections.Generic;
using System.Text;
using CoreOSC;
using OSCVRCWiz.Resources;
using OSCVRCWiz.Text;

namespace OSCVRCWiz.Addons
{
    public class VoiceCommands
    {
        static List<string> VCPhrase = new List<string>();
        static List<string> VCAddress = new List<string>();
        static List<string> VCType = new List<string>();
        static List<string> VCValue = new List<string>();
        public static string voiceCommandsStored = "";


        public static void MainDoVoiceCommand(string text)
        {
            VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
            {
                int index = 0;
                foreach (string x in VCPhrase)
                {
                    System.Diagnostics.Debug.WriteLine("checking " + x);
                    if (text.Contains(x, StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine("it contains " + x);
                        VoiceCommands.phraseFound(index);
                    }
                    index++;
                }

                //this is for changing styles on voice command
                if (VoiceWizardWindow.MainFormGlobal.comboBoxTTSMode.SelectedItem.ToString() == "Azure" & VoiceWizardWindow.MainFormGlobal.rjToggleButtonStyle.Checked == true)
                {
                    foreach (var x in VoiceWizardWindow.MainFormGlobal.comboBox1.Items)
                    {
                        System.Diagnostics.Debug.WriteLine("checking " + x.ToString());
                        if (text.Contains(x.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            System.Diagnostics.Debug.WriteLine("it contains " + x.ToString());
                            VoiceWizardWindow.MainFormGlobal.comboBox1.SelectedItem = x.ToString();
                            // var vc = new VoiceCommands();
                            //vc.phraseFound(index);
                        }
                        index++;
                    }
                }

            });
        }

        public static void clearVoiceCommands()
        {
            VCAddress.Clear();
            VCPhrase.Clear();
            VCValue.Clear();
            VCType.Clear();
        }
        public static void removeVoiceCommandsAt(int index)
        {
            VCAddress.RemoveAt(index);
            VCPhrase.RemoveAt(index);
            VCValue.RemoveAt(index);
            VCType.RemoveAt(index);
        }


        public static void voiceCommands()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();
            string words = voiceCommandsStored;

            string[] split = words.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in split)
            {

                if (s.Trim() != "")
                {

                    string words2 = s;
                    int count = 1;

                    string[] split2 = words2.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s2 in split2)
                    {

                        // if (s2.Trim() != "")
                        // {
                        if (count == 1)
                        {
                            VCPhrase.Add(s2);
                            System.Diagnostics.Debug.WriteLine("Phrase Added: " + s2);

                        }
                        if (count == 2)
                        {
                            VCAddress.Add(s2);
                            System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }
                        if (count == 3)
                        {
                            VCType.Add(s2);
                            System.Diagnostics.Debug.WriteLine("typeadded: " + s2);

                        }
                        if (count == 4)
                        {
                            VCValue.Add(s2);
                            System.Diagnostics.Debug.WriteLine("value added: " + s2);

                        }
                        count++;
                    }
                }
            }
        }
        public static void phraseFound(int index)
        {
            if (VoiceWizardWindow.MainFormGlobal.checkedListBox1.GetItemChecked(index) == true)
            {


                // int index = VCPhrase.FindIndex(a => a.Contains(phrase));
                string address = VCAddress[index];
                string type = VCType[index];
                var VCMessage = new OscMessage(address, true);

                //    System.Diagnostics.Debug.WriteLine("THE VALUE WAS: "+ VoiceWizardWindow.VCValue[index]);
                //    Console.WriteLine("THE VALUE WAS: " + VoiceWizardWindow.VCValue[index]);
                // VoiceWizardWindow.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "THE VALUE WAS: " + VoiceWizardWindow.VCValue[index]);



                switch (type)
                {
                    case "Bool":
                        if (string.Equals(VCValue[index], "true", StringComparison.InvariantCultureIgnoreCase))
                        {
                            VCMessage = new OscMessage(address, true);
                        }
                        if (string.Equals(VCValue[index], "false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            VCMessage = new OscMessage(address, false);
                        }
                        break;


                    case "Float":
                        float value1 = float.Parse(VCValue[index]);
                        VCMessage = new OscMessage(address, value1);
                        break;
                    case "Int":
                        int value2 = int.Parse(VCValue[index]);
                        VCMessage = new OscMessage(address, value2);
                        break;
                    default:
                        VCMessage = new OscMessage(address, true);
                        break;
                }



                OSC.OSCSender.Send(VCMessage);
                OutputText.outputLog("[OSC message sent with voice command '" + VCPhrase[index] + "' " + "Value: " + VCValue[index] + "]");
            }



        }
        public static void refreshCommandList()
        {
            // VoiceWizardWindow.MainFormGlobal.richTextBox13.Clear();
            VoiceWizardWindow.MainFormGlobal.checkedListBox1.Items.Clear();
            voiceCommandsStored = "";
            for (var index = 0; index < VCAddress.Count; index++)
            {
                commandListHelper($"ID: {index + 1} | Phrase: {VCPhrase[index]} | Address: {VCAddress[index]} | Data Type: {VCType[index]} | Value: {VCValue[index]}");
                voiceCommandsStored += $"{VCPhrase[index]}:{VCAddress[index]}:{VCType[index]}:{VCValue[index]};";

            }
        }
        public static  void commandListHelper(string line)
        {
            //  if (VoiceWizardWindow.MainFormGlobal.InvokeRequired)
            //  {
            //    VoiceWizardWindow.MainFormGlobal.Invoke(new Action<string>(commandListHelper), new object[] { line });
            //     return;
            //  }
            //  VoiceWizardWindow.MainFormGlobal.richTextBox13.Select(0, 0);
            //  VoiceWizardWindow.MainFormGlobal.richTextBox13.SelectedText = line + "\r\n";

            // VoiceWizardWindow.MainFormGlobal.checkedListBox1.Items.Clear();
            VoiceWizardWindow.MainFormGlobal.checkedListBox1.Items.Add(line, true);

        }
    }
}
