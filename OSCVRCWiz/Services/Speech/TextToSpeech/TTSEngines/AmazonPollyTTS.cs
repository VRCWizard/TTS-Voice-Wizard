using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Polly;
using Amazon.Polly.Model;
using NAudio.Wave;
using Polly.Caching;
//using SharpTalk;
using System.Xml.Linq;
using Amazon.Internal;
using Amazon;
using System.Diagnostics;
using OSCVRCWiz.Settings;
using static OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines.ElevenLabsTTS;
using System.Windows;
using NAudio.Wave.SampleProviders;
using OSCVRCWiz.Resources.Audio;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Speech.TextToSpeech.TTSEngines
{
    public class AmazonPollyTTS
    {

        public static async Task PollyTTS(TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct = default)
        {
            try
            {
                var AWSaccessKeyId = Settings1.Default.yourAWSKey;
                var AWSsecretKey = Settings1.Default.yourAWSSecret;
                var AWSRegion = RegionEndpoint.GetBySystemName(Settings1.Default.yourAWSRegion);



                var client = new AmazonPollyClient(AWSaccessKeyId, AWSsecretKey, AWSRegion);


                var response = await PollySynthesizeSpeech(client, TTSMessageQueued);

                MemoryStream memoryStream = new MemoryStream();
                WriteSpeechToStream(response.AudioStream, memoryStream);



                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, false, AudioFormat.Mp3);
                memoryStream.Dispose();

                client.Dispose();
                client = null;
                response.Dispose();
                response = null;



            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Amazon Polly TTS Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();
            }
        }

        /// <summary>
        /// Calls the Amazon Polly SynthesizeSpeechAsync method to convert text
        /// to speech.
        /// </summary>
        /// <param name="client">The Amazon Polly client object used to connect
        /// to the Amazon Polly service.</param>
        /// <param name="text">The text to convert to speech.</param>
        /// <returns>A SynthesizeSpeechResponse object that includes an AudioStream
        /// object with the converted text.</returns>
        private static async Task<SynthesizeSpeechResponse> PollySynthesizeSpeech(IAmazonPolly client, TTSMessageQueue.TTSMessage TTSMessageQueued)
        {
            var tts = new SynthesizeSpeechRequest();


            tts.OutputFormat = OutputFormat.Mp3;
            tts.VoiceId = VoiceId.Joanna;

            tts.Engine = Engine.Standard;

            tts.TextType = TextType.Ssml;


            // tts.TextType = TextType.Text;




            int rate = TTSMessageQueued.Speed;
            int pitch = TTSMessageQueued.Pitch;
            int volume = TTSMessageQueued.Volume;
            string voice = TTSMessageQueued.Voice;



            var ratePercent = (int)Math.Floor((0.5f + rate * 0.1f - 1) * 100);
            // var ratePercent = "+10";
            var pitchPercent = (int)Math.Floor((0.5f + pitch * 0.1f - 1) * 100);
            var volumePercent = (int)Math.Floor((volume * 0.1f - 1) * 10);

            string rateString = "<prosody rate=\"";
            if (ratePercent > 0)
            {
                rateString += "+";
            }


            rateString += ratePercent + "%\">"; //1
            string pitchString = "<prosody pitch=\"" + pitchPercent + "%\">"; //1
            string volumeString = "<prosody volume=\"" + volumePercent + "dB\">"; //1

            Debug.WriteLine("rate: " + ratePercent);
            Debug.WriteLine("pitch: " + pitchPercent);

            Debug.WriteLine("volume: " + volumePercent);
            Debug.WriteLine("volume: " + volumeString);


            string ssml0 = "<speak>";
            // string ssml0 = "<speak";
            // ssml0 += " xmlns=\"http://www.w3.org/2001/10/synthesis\"";
            // ssml0 += " xml:lang=\"en-US\">";


            if (rate != 5)//5 = default /middle of track bar
            {
                ssml0 += rateString;


            }
            if (pitch != 5)
            {
                ssml0 += pitchString;


            }
            if (volume != 10)
            {
                ssml0 += volumeString;


            }
            ssml0 += TTSMessageQueued.text;

            // ssml0 += Encoding.UTF8.GetString(Encoding.Default.GetBytes(TTSMessageQueued.text));
            if (rate != 5) { ssml0 += "</prosody>"; }
            if (pitch != 5) { ssml0 += "</prosody>"; }
            if (volume != 10) { ssml0 += "</prosody>"; }

            ssml0 += "</speak>";




            tts.Text = ssml0;
            //  tts.Text = TTSMessageQueued.text;
            //  tts.Text = "Привет! Меня зовут Татьяна. Я прочитаю любой текст который вы введете здесь.";

            //  Debug.WriteLine(ssml0);




            switch (voice)
            {
                case "Salli [en-US]":
                    tts.VoiceId = VoiceId.Salli;
                    break;
                case "Salli [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Salli;
                    tts.Engine = Engine.Neural;
                    break;
                case "Kimberly [en-US]":
                    tts.VoiceId = VoiceId.Kimberly;
                    break;
                case "Kimberly [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Kimberly;
                    tts.Engine = Engine.Neural;
                    break;
                case "Kendra [en-US]":
                    tts.VoiceId = VoiceId.Kendra;
                    break;
                case "Kendra [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Kendra;
                    tts.Engine = Engine.Neural;
                    break;
                case "Joanna [en-US]":
                    tts.VoiceId = VoiceId.Joanna;
                    break;
                case "Joanna [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Joanna;
                    tts.Engine = Engine.Neural;
                    break;
                case "Ivy [en-US]":
                    tts.VoiceId = VoiceId.Ivy;
                    break;
                case "Ivy [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Ivy;
                    tts.Engine = Engine.Neural;
                    break;
                case "Ruth [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Russell;
                    tts.Engine = Engine.Neural;
                    break;
                case "Kevin [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Brian;
                    tts.Engine = Engine.Neural;
                    break;
                case "Matthew [en-US]":
                    tts.VoiceId = VoiceId.Matthew;
                    break;
                case "Matthew [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Matthew;
                    tts.Engine = Engine.Neural;
                    break;
                case "Justin [en-US]":
                    tts.VoiceId = VoiceId.Justin;
                    break;
                case "Justin [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Justin;
                    tts.Engine = Engine.Neural;
                    break;
                case "Joey [en-US]":
                    tts.VoiceId = VoiceId.Joey;
                    break;
                case "Joey [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Joey;
                    tts.Engine = Engine.Neural;
                    break;
                case "Stephen [en-US] ($Neural)":
                    tts.VoiceId = VoiceId.Brian;
                    tts.Engine = Engine.Neural;
                    break;
                case "Nicole [en-AU]":
                    tts.VoiceId = VoiceId.Nicole;
                    break;
                case "Olivia [en-AU] ($Neural)":
                    tts.VoiceId = VoiceId.Olivia;
                    tts.Engine = Engine.Neural;
                    break;
                case "Russell [en-AU]":
                    tts.VoiceId = VoiceId.Russell;
                    break;
                case "Amy [en-GB]":
                    tts.VoiceId = VoiceId.Amy;
                    break;
                case "Amy [en-GB] ($Neural)":
                    tts.VoiceId = VoiceId.Amy;
                    tts.Engine = Engine.Neural;
                    break;
                case "Emma [en-GB]":
                    tts.VoiceId = VoiceId.Emma;
                    break;
                case "Emma [en-GB] ($Neural)":
                    tts.VoiceId = VoiceId.Emma;
                    tts.Engine = Engine.Neural;
                    break;
                case "Brian [en-GB]":
                    tts.VoiceId = VoiceId.Brian;
                    break;
                case "Brian [en-GB] ($Neural)":
                    tts.VoiceId = VoiceId.Brian;
                    tts.Engine = Engine.Neural;
                    break;
                case "Arthur [en-GB] ($Neural)":
                    tts.VoiceId = VoiceId.Arthur;
                    tts.Engine = Engine.Neural;
                    break;
                case "Geraint [en-GB-WLS]":
                    tts.VoiceId = VoiceId.Geraint;
                    break;
                case "Aditi [en-IN]":
                    tts.VoiceId = VoiceId.Aditi;
                    break;
                case "Raveena [en-IN]":
                    tts.VoiceId = VoiceId.Raveena;
                    break;
                case "Kajal [en-IN] ($Neural)":
                    tts.VoiceId = VoiceId.Kajal;
                    tts.Engine = Engine.Neural;
                    break;
                case "Aria [en-NZ] ($Neural)":
                    tts.VoiceId = VoiceId.Aria;
                    tts.Engine = Engine.Neural;
                    break;
                case "Ayanda [en-ZA] ($Neural)":
                    tts.VoiceId = VoiceId.Ayanda;
                    tts.Engine = Engine.Neural;
                    break;
                case "Zeina [ar]":
                    tts.VoiceId = VoiceId.Zeina;
                    break;
                case "Hala [ar-AE] ($Neural)":
                    tts.VoiceId = VoiceId.Hala;
                    tts.Engine = Engine.Neural;
                    break;
                case "Arlet [ca-ES] ($Neural)":
                    tts.VoiceId = VoiceId.Arlet;
                    tts.Engine = Engine.Neural;
                    break;
                case "Hiujin [yue-CN] ($Neural)":
                    tts.VoiceId = VoiceId.Hiujin;
                    tts.Engine = Engine.Neural;
                    break;
                case "Zhiyu [cmn-CN]":
                    tts.VoiceId = VoiceId.Zhiyu;
                    break;
                case "Zhiyu [cmn-CN] ($Neural)":
                    tts.VoiceId = VoiceId.Zhiyu;
                    tts.Engine = Engine.Neural;
                    break;
                case "Naja [da-DK]":
                    tts.VoiceId = VoiceId.Naja;
                    break;
                case "Mads [da-DK]":
                    tts.VoiceId = VoiceId.Mads;
                    break;
                case "Laura [nl-NL] ($Neural)":
                    tts.VoiceId = VoiceId.Laura;
                    tts.Engine = Engine.Neural;
                    break;

                case "Lotte [nl-NL]": tts.VoiceId = VoiceId.Lotte; tts.Engine = Engine.Standard; break;
                case "Ruben [nl-NL]": tts.VoiceId = VoiceId.Ruben; tts.Engine = Engine.Standard; break;
                case "Suvi [fi-FI]": tts.VoiceId = VoiceId.Suvi; tts.Engine = Engine.Standard; break;
                case "Céline [fr-FR]": tts.VoiceId = VoiceId.Celine; tts.Engine = Engine.Standard; break;
                case "Léa [fr-FR]": tts.VoiceId = VoiceId.Lea; tts.Engine = Engine.Standard; break;
                case "Léa [fr-FR] ($Neural)": tts.VoiceId = VoiceId.Lea; tts.Engine = Engine.Neural; break;
                case "Mathieu [fr-FR]": tts.VoiceId = VoiceId.Mathieu; tts.Engine = Engine.Standard; break;
                case "Rémi [fr-FR] ($Neural)": tts.VoiceId = VoiceId.Remi; tts.Engine = Engine.Neural; break;
                case "Chantal [fr-CA]": tts.VoiceId = VoiceId.Chantal; tts.Engine = Engine.Standard; break;
                case "Gabrielle [fr-CA] ($Neural)": tts.VoiceId = VoiceId.Gabrielle; tts.Engine = Engine.Neural; break;
                case "Liam [fr-CA] ($Neural)": tts.VoiceId = VoiceId.Liam; tts.Engine = Engine.Neural; break;
                case "Marlene [de-DE]": tts.VoiceId = VoiceId.Marlene; tts.Engine = Engine.Standard; break;
                case "Vicki [de-DE]": tts.VoiceId = VoiceId.Vicki; tts.Engine = Engine.Standard; break;
                case "Vicki [de-DE] ($Neural)": tts.VoiceId = VoiceId.Vicki; tts.Engine = Engine.Neural; break;
                case "Hans [de-DE]": tts.VoiceId = VoiceId.Hans; tts.Engine = Engine.Standard; break;
                case "Daniel [de-DE] ($Neural)": tts.VoiceId = VoiceId.Daniel; tts.Engine = Engine.Neural; break;
                case "Hannah [de-AT]": tts.VoiceId = VoiceId.Hannah; tts.Engine = Engine.Standard; break;
                case "Dóra [is-IS]": tts.VoiceId = VoiceId.Dora; tts.Engine = Engine.Standard; break;
                case "Karl [is-IS]": tts.VoiceId = VoiceId.Karl; tts.Engine = Engine.Standard; break;
                case "Carla [it-IT]": tts.VoiceId = VoiceId.Carla; tts.Engine = Engine.Standard; break;
                case "Bianca [it-IT]": tts.VoiceId = VoiceId.Bianca; tts.Engine = Engine.Standard; break;
                case "Bianca [it-IT] ($Neural)": tts.VoiceId = VoiceId.Bianca; tts.Engine = Engine.Neural; break;
                case "Giorgio [it-IT]": tts.VoiceId = VoiceId.Giorgio; tts.Engine = Engine.Standard; break;

                case "Adriano [it-IT] ($Neural)":
                    tts.VoiceId = VoiceId.Adriano;
                    tts.Engine = Engine.Neural;
                    break;
                case "Mizuki [ja-JP]":
                    tts.VoiceId = VoiceId.Mizuki;
                    break;
                case "Takumi [ja-JP]":
                    tts.VoiceId = VoiceId.Takumi;
                    break;
                case "Takumi [ja-JP] ($Neural)":
                    tts.VoiceId = VoiceId.Takumi;
                    tts.Engine = Engine.Neural;
                    break;
                case "Kazuha [ja-JP] ($Neural)":
                    tts.VoiceId = VoiceId.Kazuha;
                    tts.Engine = Engine.Neural;
                    break;
                case "Tomoko [ja-JP] ($Neural)":
                    tts.VoiceId = VoiceId.Tomoko;
                    tts.Engine = Engine.Neural;
                    break;
                case "Seoyeon [ko-KR]":
                    tts.VoiceId = VoiceId.Seoyeon;
                    break;
                case "Seoyeon [ko-KR] ($Neural)":
                    tts.VoiceId = VoiceId.Seoyeon;
                    tts.Engine = Engine.Neural;
                    break;
                case "Liv [nb-NO]":
                    tts.VoiceId = VoiceId.Liv;
                    break;
                case "Ida [nb-NO] ($Neural)":
                    tts.VoiceId = VoiceId.Ida;
                    tts.Engine = Engine.Neural;
                    break;
                case "Ewa [pl-PL]":
                    tts.VoiceId = VoiceId.Ewa;
                    break;
                case "Maja [pl-PL]":
                    tts.VoiceId = VoiceId.Maja;
                    break;
                case "Jacek [pl-PL]":
                    tts.VoiceId = VoiceId.Jacek;
                    break;
                case "Jan [pl-PL]":
                    tts.VoiceId = VoiceId.Jan;
                    break;
                case "Ola [pl-PL] ($Neural)":
                    tts.VoiceId = VoiceId.Ola;
                    tts.Engine = Engine.Neural;
                    break;
                case "Camila [pt-BR]":
                    tts.VoiceId = VoiceId.Camila;
                    break;
                case "Camila [pt-BR] ($Neural)":
                    tts.VoiceId = VoiceId.Camila;
                    tts.Engine = Engine.Neural;
                    break;
                case "Vitória [pt-BR]":
                    tts.VoiceId = VoiceId.Vitoria;
                    break;
                case "Vitória [pt-BR] ($Neural)":
                    tts.VoiceId = VoiceId.Vitoria;
                    tts.Engine = Engine.Neural;
                    break;
                case "Ricardo [pt-BR]":
                    tts.VoiceId = VoiceId.Ricardo;
                    break;
                case "Thiago [pt-BR] ($Neural)":
                    tts.VoiceId = VoiceId.Thiago;
                    tts.Engine = Engine.Neural;
                    break;
                case "Inês [pt-PT]":
                    tts.VoiceId = VoiceId.Ines;
                    break;
                case "Inês [pt-PT] ($Neural)":
                    tts.VoiceId = VoiceId.Ines;
                    tts.Engine = Engine.Neural;
                    break;

                case "Cristiano [pt-PT]": tts.VoiceId = VoiceId.Cristiano; break;
                case "Carmen [ro-RO]": tts.VoiceId = VoiceId.Carmen; break;
                case "Tatyana [ru-RU]":
                    tts.VoiceId = VoiceId.Tatyana;


                    break;
                case "Maxim [ru-RU]":
                    tts.VoiceId = VoiceId.Maxim;


                    break;
                case "Conchita [es-ES]": tts.VoiceId = VoiceId.Conchita; break;
                case "Lucia [es-ES]": tts.VoiceId = VoiceId.Lucia; break;
                case "Lucia [es-ES] ($Neural)": tts.VoiceId = VoiceId.Lucia; tts.Engine = Engine.Neural; break;
                case "Enrique [es-ES]": tts.VoiceId = VoiceId.Enrique; break;
                case "Sergio [es-ES] ($Neural)": tts.VoiceId = VoiceId.Sergio; tts.Engine = Engine.Neural; break;
                case "Mia [es-MX]": tts.VoiceId = VoiceId.Mia; break;
                case "Mia [es-MX] ($Neural)": tts.VoiceId = VoiceId.Mia; tts.Engine = Engine.Neural; break;
                case "Andrés [es-MX] ($Neural)": tts.VoiceId = VoiceId.Andres; tts.Engine = Engine.Neural; break;
                case "Lupe [es-US]": tts.VoiceId = VoiceId.Lupe; break;
                case "Lupe [es-US] ($Neural)": tts.VoiceId = VoiceId.Lupe; tts.Engine = Engine.Neural; break;
                case "Penélope [es-US]": tts.VoiceId = VoiceId.Penelope; break;
                case "Miguel [es-US]": tts.VoiceId = VoiceId.Miguel; break;
                case "Pedro [es-US] ($Neural)": tts.VoiceId = VoiceId.Pedro; tts.Engine = Engine.Neural; break;
                case "Astrid [sv-SE]": tts.VoiceId = VoiceId.Astrid; break;
                case "Elin [sv-SE] ($Neural)": tts.VoiceId = VoiceId.Elin; tts.Engine = Engine.Neural; break;
                case "Filiz [tr-TR]": tts.VoiceId = VoiceId.Filiz; break;
                case "Gwyneth [cy-GB]": tts.VoiceId = VoiceId.Gwyneth; break;

                default: tts.VoiceId = VoiceId.Joanna; break;

            }

            var synthesizeSpeechResponse = await client.SynthesizeSpeechAsync(tts);

            return synthesizeSpeechResponse;

            /*
             *  comboBox2.Items.Add("Nicole");
                    comboBox2.Items.Add("Olivia ($Neural)");
                    comboBox2.Items.Add("Russell");

                    comboBox2.Items.Add("Amy");
                    comboBox2.Items.Add("Emma");
                    comboBox2.Items.Add("Brian");
                    comboBox2.Items.Add("Arthur ($Neural)");

                    comboBox2.Items.Add("Aditi");
                    comboBox2.Items.Add("Reveena");
                    comboBox2.Items.Add("Kajal ($Neural)");

                    comboBox2.Items.Add("Aria ($Neural)");

                    comboBox2.Items.Add("Ayanda ($Neural)");

                    comboBox2.Items.Add("Geraint");*/
        }

        /// <summary>
        /// Writes the AudioStream returned from the call to
        /// SynthesizeSpeechAsync to a file in MP3 format.
        /// </summary>
        /// <param name="audioStream">The AudioStream returned from the
        /// call to the SynthesizeSpeechAsync method.</param>
        /// <param name="outputFileName">The full path to the file in which to
        /// save the audio stream.</param>
        public static void WriteSpeechToStream(Stream audioStream, MemoryStream output)
        {

            byte[] buffer = new byte[2 * 1024];
            int readBytes;

            while ((readBytes = audioStream.Read(buffer, 0, 2 * 1024)) > 0)
            {
                output.Write(buffer, 0, readBytes);
            }

            // Flushes the buffer to avoid losing the last second or so of
            // the synthesized text.
            output.Flush();

        }
        public static void SynthesisGetAvailableVoices(string fromLanguageFullname)
        {
            VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Clear();

            switch (fromLanguageFullname)
            {
                case "Arabic [ar]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Zeina [ar]");//new 
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Hala [ar-AE] ($Neural)");

                    break;
                case "Catalan [ca]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Arlet [ca-ES] ($Neural)");



                    break;
                case "Chinese [zh]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Hiujin [yue-CN] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Zhiyu [cmn-CN]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Zhiyu [cmn-CN] ($Neural)");
                    break;
                case "Czech [cs]": break;
                case "Danish [da]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Naja [da-DK]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Mads [da-DK]");
                    break;
                case "Dutch [nl]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Laura [nl-NL] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Lotte [nl-NL]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ruben [nl-NL]");

                    break;
                case "English [en]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Salli [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Salli [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kimberly [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kimberly [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kendra [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kendra [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Joanna [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Joanna [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ivy [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ivy [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ruth [en-US] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kevin [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Matthew [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Matthew [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Justin [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Justin [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Joey [en-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Joey [en-US] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Stephen [en-US] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Nicole [en-AU]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Olivia [en-AU] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Russell [en-AU]");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Amy [en-GB]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Amy [en-GB] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Emma [en-GB]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Emma [en-GB] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Brian [en-GB]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Brian [en-GB] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Arthur [en-GB] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Geraint [en-GB-WLS]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Aditi [en-IN]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Raveena [en-IN]");//new
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kajal [en-IN] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Aria [en-NZ] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ayanda [en-ZA] ($Neural)");

                    break;


                case "Finnish [fi]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Suvi [fi-FI]");
                    break;
                case "French [fr]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Céline [fr-FR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Léa [fr-FR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Léa [fr-FR] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Mathieu [fr-FR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Rémi [fr-FR] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Chantal [fr-CA]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Gabrielle [fr-CA] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Liam [fr-CA] ($Neural)");

                    break;
                case "German [de]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Marlene [de-DE]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Vicki [de-DE]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Vicki [de-DE] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Hans [de-DE]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Daniel [de-DE] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Hannah [de-AT]");

                    break;

                case "Hindi [hi]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Aditi [hi-IN]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kajal [hi-IN] ($Neural)");
                    break;
                case "Icelandic [is]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Dóra [is-IS]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Karl [is-IS]");
                    break;
                case "Italian [it]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Carla [it-IT]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Bianca [it-IT]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Bianca [it-IT] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Giorgio [it-IT]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Adriano [it-IT] ($Neural)");
                    break;

                case "Japanese [ja]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Mizuki [ja-JP]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Takumi [ja-JP]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Takumi [ja-JP] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Kazuha [ja-JP] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Tomoko [ja-JP] ($Neural)");
                    break;
                case "Korean [ko]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Seoyeon [ko-KR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Seoyeon [ko-KR] ($Neural)");
                    break;
                case "Norwegian [nb]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Liv [nb-NO]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ida [nb-NO] ($Neural)");
                    break;
                case "Polish [pl]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ewa [pl-PL]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Maja [pl-PL]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Jacek [pl-PL]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Jan [pl-PL]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ola [pl-PL] ($Neural)");
                    break;
                case "Portuguese [pt]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Camila [pt-BR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Camila [pt-BR] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Vitória [pt-BR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Vitória [pt-BR] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Ricardo [pt-BR]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Thiago [pt-BR] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Inês [pt-PT]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Inês [pt-PT] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Cristiano [pt-PT]");
                    break;

                case "Romanian [ro]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Carmen [ro-RO]");
                    break;
                case "Russian [ru]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Tatyana [ru-RU]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Maxim [ru-RU]");
                    break;
                case "Spanish [es]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Conchita [es-ES]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Lucia [es-ES]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Lucia [es-ES] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Enrique [es-ES]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Sergio [es-ES] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Mia [es-MX]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Mia [es-MX] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Andrés [es-MX] ($Neural)");

                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Lupe [es-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Lupe [es-US] ($Neural)");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Penélope [es-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Miguel [es-US]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Pedro [es-US] ($Neural)");
                    break;
                case "Swedish [sv]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Astrid [sv-SE]");
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Elin [sv-SE] ($Neural)");
                    break;

                case "Turkish [tr]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Filiz [tr-TR]");
                    break;

                case "Welsh [cy]":
                    VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Add("Gwyneth [cy-GB]");
                    break;




                default: break;
            }
            if (VoiceWizardWindow.MainFormGlobal.rjToggleButtonUsePro.Checked == true && VoiceWizardWindow.MainFormGlobal.rjToggleButtonProAmazon.Checked == true)
            {
                for (int i = VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.Count - 1; i >= 0; i--)
                {
                    if (VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items[i].ToString().EndsWith("($Neural)"))
                    {
                        VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.Items.RemoveAt(i);
                    }
                }
            }
            try
            {
                VoiceWizardWindow.MainFormGlobal.comboBoxVoiceSelect.SelectedIndex = 0;
            }
            catch { }
        }
        public static async void AmazonPlayAudioPro(string audioString, TTSMessageQueue.TTSMessage TTSMessageQueued, CancellationToken ct)
        {
            try
            {
                var audiobytes = Convert.FromBase64String(audioString);
                MemoryStream memoryStream = new MemoryStream(audiobytes);

                AudioDevices.PlayAudioStream(memoryStream, TTSMessageQueued, ct, false, AudioFormat.Mp3);
                memoryStream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Amazon Polly TTS Error: " + ex.Message + "]", Color.Red);
                TTSMessageQueue.PlayNextInQueue();
            }

}
        public static void SetVoices(ComboBox voices, ComboBox styles, ComboBox accents)
        {
            accents.Items.Clear();
            var voiceAccentsAmazon = new List<string>()
                    {
                        "Arabic [ar]",
                        "Catalan [ca]",
                        "Chinese [zh]",
                        "Danish [da]",
                        "Dutch [nl]",
                        "English [en]",
                        "Finnish [fi]",
                        "French [fr]",
                        "German [de]",
                        "Hindi [hi]",
                        "Icelandic [is]",//
                        "Italian [it]",
                        "Japanese [ja]",
                        "Korean [ko]",
                        "Norwegian [nb]",
                        "Polish [pl]",
                        "Portuguese [pt]",
                         "Romanian [ro]",
                        "Russian [ru]",
                        "Spanish [es]",
                        "Swedish [sv]",
                        "Welsh [cy]"
                    };
            foreach (var accent in voiceAccentsAmazon)
            {
                accents.Items.Add(accent);
            }
            accents.SelectedIndex = 5;

            voices.Items.Clear();
            AmazonPollyTTS.SynthesisGetAvailableVoices(accents.Text.ToString());

            voices.SelectedIndex = 0;
            styles.SelectedIndex = 0;

           
            styles.Enabled = false;
            voices.Enabled = true;

        }

    }
}
