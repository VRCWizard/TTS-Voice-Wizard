using System;
using System.Collections.Generic;
using System.Text;
using CoreOSC;

namespace OSCVRCWiz.Addons
{
    public class VoiceCommands
    {

        public static void voiceCommands()
        {
            //  string words = VoiceWizardWindow.MainFormGlobal.richTextBox2.Text.ToString();
            string words = VoiceWizardWindow.voiceCommandsStored;

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
                            VoiceWizardWindow.VCPhrase.Add(s2);
                            System.Diagnostics.Debug.WriteLine("Phrase Added: " + s2);

                        }
                        if (count == 2)
                        {
                            VoiceWizardWindow.VCAddress.Add(s2);
                            System.Diagnostics.Debug.WriteLine("address added: " + s2);

                        }
                        if (count == 3)
                        {
                            VoiceWizardWindow.VCType.Add(s2);
                            System.Diagnostics.Debug.WriteLine("typeadded: " + s2);

                        }
                        if (count == 4)
                        {
                            VoiceWizardWindow.VCValue.Add(s2);
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
                string address = VoiceWizardWindow.VCAddress[index];
                string type = VoiceWizardWindow.VCType[index];
                var VCMessage = new OscMessage(address, true);

                //    System.Diagnostics.Debug.WriteLine("THE VALUE WAS: "+ VoiceWizardWindow.VCValue[index]);
                //    Console.WriteLine("THE VALUE WAS: " + VoiceWizardWindow.VCValue[index]);
                // VoiceWizardWindow.ot.outputLog(VoiceWizardWindow.MainFormGlobal, "THE VALUE WAS: " + VoiceWizardWindow.VCValue[index]);



                switch (type)
                {
                    case "Bool":
                        if (string.Equals(VoiceWizardWindow.VCValue[index], "true", StringComparison.InvariantCultureIgnoreCase))
                        {
                            VCMessage = new OscMessage(address, true);
                        }
                        if (string.Equals(VoiceWizardWindow.VCValue[index], "false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            VCMessage = new OscMessage(address, false);
                        }
                        break;


                    case "Float":
                        float value1 = float.Parse(VoiceWizardWindow.VCValue[index]);
                        VCMessage = new OscMessage(address, value1);
                        break;
                    case "Int":
                        int value2 = int.Parse(VoiceWizardWindow.VCValue[index]);
                        VCMessage = new OscMessage(address, value2);
                        break;
                    default:
                        VCMessage = new OscMessage(address, true);
                        break;
                }



                VoiceWizardWindow.MainFormGlobal.sender3.Send(VCMessage);
                VoiceWizardWindow.MainFormGlobal.ot.outputLog("[OSC message sent with voice command '" + VoiceWizardWindow.VCPhrase[index] + "' " + "Value: " + VoiceWizardWindow.VCValue[index] + "]");
            }



        }
        public static void refreshCommandList()
        {
            // VoiceWizardWindow.MainFormGlobal.richTextBox13.Clear();
            VoiceWizardWindow.MainFormGlobal.checkedListBox1.Items.Clear();
            VoiceWizardWindow.voiceCommandsStored = "";
            for (var index = 0; index < VoiceWizardWindow.VCAddress.Count; index++)
            {
                commandListHelper($"ID: {index + 1} | Phrase: {VoiceWizardWindow.VCPhrase[index]} | Address: {VoiceWizardWindow.VCAddress[index]} | Data Type: {VoiceWizardWindow.VCType[index]} | Value: {VoiceWizardWindow.VCValue[index]}");
                VoiceWizardWindow.voiceCommandsStored += $"{VoiceWizardWindow.VCPhrase[index]}:{VoiceWizardWindow.VCAddress[index]}:{VoiceWizardWindow.VCType[index]}:{VoiceWizardWindow.VCValue[index]};";

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
