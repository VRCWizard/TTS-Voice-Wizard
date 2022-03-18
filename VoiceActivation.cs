using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace OSCVRCWiz
{
    public class VoiceActivation
    {
        private VoiceWizardWindow MainForm;
        SpeechRecognitionEngine recognizer;

        public void loadSpeechRecognition(VoiceWizardWindow form)
        {
            MainForm = form;
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            var c = getChoiceLibray();
            var gb = new GrammarBuilder(c);

            var g = new System.Speech.Recognition.Grammar(gb);
            g.Weight = 0.1f;
            recognizer.LoadGrammar(g);

            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
            recognizer.SetInputToDefaultAudioDevice();
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        private Choices getChoiceLibray()
        {
           // var frm = new Form1();
            Choices myChoices = new Choices();
            myChoices.Add(MainForm.activationWord);

            return myChoices;
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
           
          
            if (e.Result.Text == MainForm.activationWord)
            {
                MainForm.AppendTextBox("Recognized Activation Word: " + MainForm.activationWord + "\r");
                MainForm.speechTTSButton.PerformClick();
               // MessageBox.Show("This ran");

            }

        }
    }
}
