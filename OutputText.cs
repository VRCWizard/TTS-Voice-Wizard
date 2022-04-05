using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz
{
    public class OutputText
    {
        bool currentlyPrinting = false;
        public async void outputLog(VoiceWizardWindow MainForm, string textstring)
        {
            //  MainForm.AppendTextBox("You Said: " + textstring + "\r");
            MainForm.logLine("["+DateTime.Now.ToString("h:mm:ss tt") +"]" +": " + textstring);

        }
        public async void outputVRChat(VoiceWizardWindow MainForm, string textstring)
        {
           // currentlyPrinting = true;

            var sender2 = new SharpOSC.UDPSender("127.0.0.1", 9000);



            textstring = textstring.ToLower();
            int stringleng = 0;
            foreach (char h in textstring)
            {
                stringleng += 1;
            }
            System.Diagnostics.Debug.WriteLine("textstring length =" + textstring.Length);


            if (stringleng % 4 == 1)
            {
                textstring += "   ";

            }
            if (stringleng % 4 == 2)
            {
                textstring += "  ";

            }
            if (stringleng % 4 == 3)
            {
                textstring += " ";

            }
            System.Diagnostics.Debug.WriteLine("textstring length =" + textstring.Length);

            float letter = 0.0F;
            int charCounter = 0;
            int stringPoint = 1;


            float letterFloat0 = 0;
            float letterFloat1 = 0;
            float letterFloat2 = 0;
            float letterFloat3 = 0;

            var message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", 1);
            var message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
            var message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
            var message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
            var message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);


            var message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", true);
            sender2.Send(message0);

            foreach (char c in textstring)
            {
                switch (c)
                {
                    case 'a': letter = 0.511811f; break;
                    case 'b': letter = 0.51968503f; break;
                    case 'c': letter = 0.52755904f; break;
                    case 'd': letter = 0.53543305f; break;
                    case 'e': letter = 0.54330707f; break;
                    case 'f': letter = 0.5511811f; break;
                    case 'g': letter = 0.5590551f; break;
                    case 'h': letter = 0.56692916f; break;
                    case 'i': letter = 0.5748032f; break;
                    case 'j': letter = 0.5826772f; break;
                    case 'k': letter = 0.5905512f; break;
                    case 'l': letter = 0.5984252f; break;
                    case 'm': letter = 0.6062992f; break;
                    case 'n': letter = 0.61417323f; break;
                    case 'o': letter = 0.62204725f; break;
                    case 'p': letter = 0.62992126f; break;
                    case 'q': letter = 0.63779527f; break;
                    case 'r': letter = 0.6456693f; break;
                    case 's': letter = 0.6535433f; break;
                    case 't': letter = 0.6614173f; break;
                    case 'u': letter = 0.6692913f; break;
                    case 'v': letter = 0.6771653f; break;
                    case 'w': letter = 0.68503934f; break;
                    case 'x': letter = 0.6929134f; break;
                    case 'y': letter = 0.7007874f; break;
                    case 'z': letter = 0.70866144f; break;
                    case '1': letter = 0.13385826f; break;
                    case '2': letter = 0.14173229f; break;
                    case '3': letter = 0.1496063f; break;
                    case '4': letter = 0.15748031f; break;
                    case '5': letter = 0.16535433f; break;
                    case '6': letter = 0.17322835f; break;
                    case '7': letter = 0.18110237f; break;
                    case '8': letter = 0.18897638f; break;
                    case '9': letter = 0.19685039f; break;
                    case '0': letter = 0.12598425f; break;

                    case '$': letter = 0.031496063f; break;
                    case '\'': letter = 0.05511811f; break;
                    case '.': letter = 0.11023622f; break;

                    case ',': letter = 0.09448819f; break;
                    case ':': letter = 0.20472442f; break;
                    case ';': letter = 0.21259843f; break;
                    case '!': letter = 0.007874016f; break;

                    case ' ': letter = 0.0f; break;
                    case '?': letter = 0.24409449f; break;

                    case '[': letter = 0.46456692f; break;
                    case ']': letter = 0.48031497f; break;



                    default: letter = 0.0f; break;
                }
                switch (charCounter)
                {
                    case 0:
                        letterFloat0 = letter;
                        break;
                    case 1:
                        letterFloat1 = letter;
                        break;
                    case 2:
                        letterFloat2 = letter;
                        break;
                    case 3:

                        Task.Delay(MainForm.debugDelayValue).Wait();
                        letterFloat3 = letter;
                        message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", stringPoint);
                        message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", letterFloat0);
                        message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", letterFloat1);
                        message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", letterFloat2);
                        message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", letterFloat3);
                        message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", true);

                        sender2.Send(message1);
                        sender2.Send(message2);
                        sender2.Send(message3);
                        sender2.Send(message4);
                        sender2.Send(message5);
                        sender2.Send(message0);

                        System.Diagnostics.Debug.WriteLine("Sending working 3");
                        stringPoint += 1;
                        charCounter = -1;
                        letterFloat0 = 0;
                        letterFloat1 = 0;
                        letterFloat2 = 0;
                        letterFloat3 = 0;
                        break;
                    default: break;
                }


                charCounter += 1;
              //  currentlyPrinting = true;




            }
           // currentlyPrinting = false; //does not work as intended, look into sharing betwwen threads possibly?
            int startingPoint = stringPoint;
            int frenzyDisplayLimit = 128;
            int frenzyDisplayMaxChar = 4;
            if (currentlyPrinting == false)
            {
             
                    //Natural Erase just incase previous sentence was longer than the current sentence (and erase not checked)
                    System.Diagnostics.Debug.WriteLine("Natural erase begun");
                    for (int z = startingPoint; z <= (frenzyDisplayLimit / frenzyDisplayMaxChar); z++) //INCREASE VALUE TO Killfrenzy limit!!! (old limit 128/4=32 chracters)
                    {

                        Task.Delay(MainForm.debugDelayValue).Wait();
                        message1 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Pointer", z);
                        message2 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync0", 0.0f);
                        message3 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync1", 0.0f);
                        message4 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync2", 0.0f);
                        message5 = new SharpOSC.OscMessage("/avatar/parameters/KAT_CharSync3", 0.0f);
                        message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", true);

                    sender2.Send(message1);
                        sender2.Send(message2);
                        sender2.Send(message3);
                        sender2.Send(message4);
                        sender2.Send(message5);
                    sender2.Send(message0);


                }



                if (MainForm.checkBox6.Checked) //inactive hide
                {
                    System.Diagnostics.Debug.WriteLine("Outputing text to vrchat finished");

                    System.Diagnostics.Debug.WriteLine("Begun scheduled hide text");
                    Task.Delay(MainForm.eraseDelay).Wait();

                    message0 = new SharpOSC.OscMessage("/avatar/parameters/KAT_Visible", false);

                    sender2.Send(message0);
                }

             
            }





            //currently error if u talk past word limit (setting point back to 1 around here should fix it)









        }
    }
}
