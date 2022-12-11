using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;//free Windows

using System.Speech;//free windows

using System.Speech.Synthesis;//free windows
using CSCore;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using CSCore.SoundIn;
using CSCore.CoreAudioAPI;
using NAudio.Wave;
//using NAudio.Wave;

namespace OSCVRCWiz
{
    public class WindowsBuiltInSTTTS
    {
        //public static SpeechSynthesizer synthesizer;
        static bool listeningCurrently = false;
        SpeechRecognitionEngine recognizer;
        private static bool _userRequestedAbort = false;
        public SpeechStreamer _stream = new(12800);



        public void TTSButtonLiteClick(VoiceWizardWindow MainForm)//TTS
            {
                // synthesizer = new SpeechSynthesizer();
                //synthesizer.SelectVoice("Microsoft Zira Desktop");
                // synthesizer.SetOutputToDefaultAudioDevice();


                string text = "";

               text = VoiceWizardWindow.TTSLiteText;

             //   var ot = new OutputText();
                //Send Text to Vrchat
                if (MainForm.rjToggleButtonLog.Checked == true)
                {
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, text);
                VoiceWizardWindow.MainFormGlobal.ot.outputTextFile(MainForm, text);
                }
                if (MainForm.rjToggleButtonDisableTTS2.Checked == false)
                {

                Task.Run(() => systemTTSAction(text));


            }
               

                if (MainForm.rjToggleButtonOSC.Checked == true && MainForm.rjToggleButtonNoTTSKAT.Checked == false)
                {

                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChat(MainForm, text,"tts")); //original
                                                                 // ot.outputVRChat(this, text);//new
                }
                if (MainForm.rjToggleButtonChatBox.Checked == true && MainForm.rjToggleButtonNoTTSChat.Checked == false)
                {

                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.ot.outputVRChatSpeechBubbles(MainForm, text, "tts")); //original
                                                                            // ot.outputVRChat(this, text);//new
                }



        }
        public void systemTTSAction(string text)
        {
            VoiceWizardWindow.MainFormGlobal.stream = new MemoryStream();
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.SetOutputToWaveStream(VoiceWizardWindow.MainFormGlobal.stream);
            VoiceWizardWindow.MainFormGlobal.synthesizerLite.Speak(text);
      //      var waveOut = new WaveOut { Device = new WaveOutDevice(VoiceWizardWindow.MainFormGlobal.currentOutputDeviceLite) }; //StreamReader closes the underlying stream automatically when being disposed of. The using statement does this automatically.
            var waveSource = new MediaFoundationDecoder(VoiceWizardWindow.MainFormGlobal.stream);
            //  waveOut.Initialize(waveSource);
            //  waveOut.Play();
            //  waveOut.WaitForStopped();

            var testOut = new CSCore.SoundOut.WasapiOut();
            var enumerator = new MMDeviceEnumerator();
            foreach (var endpoint in enumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
            {
                if (endpoint.DeviceID == VoiceWizardWindow.MainFormGlobal.currentOutputDevice)
                {
                    testOut.Device = endpoint;
                }
            }
            testOut.Initialize(waveSource);
            testOut.Play();
            testOut.WaitForStopped();
        



    }
        public void startListeningNow(VoiceWizardWindow MainForm)
            {
                string cultureHere = "en-US";

              //  cultureHere = MainForm.CultureSelected;
            

                try
                {
                    using (recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo(cultureHere)))
                    {
                        // Create and load a dictation grammar.  
                        recognizer.LoadGrammar(new DictationGrammar());

                        // Add a handler for the speech recognized event.  
                        recognizer.SpeechRecognized +=
                          new EventHandler<SpeechRecognizedEventArgs>(MainForm.recognizer_SpeechRecognized);

                        // Configure input to the speech recognizer.  
                        recognizer.SetInputToDefaultAudioDevice();
                   // var enumerator = new MMDeviceEnumerator();

                  //  NAudio.Wave.WaveIn soundIn = new NAudio.Wave.WaveIn();
                    
                    //  soundIn.DataAvailable += async (s, a) =>
                    //  {
                    //     await _stream.Write(a.Data);
                    //  };
                 //   soundIn.DataAvailable += OnDataAvailable;
                 //   foreach (var endpoint in enumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active))
                 //   {
                      //  if (endpoint.DeviceID == VoiceWizardWindow.MainFormGlobal.currentOutputDevice)
                     //  {
                         //  soundIn.DeviceNumber = 0;
                     //   }
                  // }

                  //  soundIn.StartRecording();
                 // soundIn.
                    
                    

                 //   recognizer.SetInputToAudioStream(_stream, new(16000, System.Speech.AudioFormat.AudioBitsPerSample.Sixteen, System.Speech.AudioFormat.AudioChannel.Mono));
                 //   recognizer.RecognizeAsync(RecognizeMode.Multiple);
                   
                    //  WaveFormat = new(16000, 1)


                    //  var instream = new SpeechStreamer();
                    // recognizer.SetInputToAudioStream(soundIn., formatInfo);

                    //  WaveInProxy _microphone = new();
                    // recognizer.SetInputToAudioStream
                    // recognizer.SetInputToAudioStream( _convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                    //   _speechEngine.RecognizeAsync(RecognizeMode.Multiple);

                    bool completed = false;

                        // Attach event handlers.
                        recognizer.RecognizeCompleted += (o, e) =>
                        {
                            if (e.Error != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Error occurred during recognition: {0}", e.Error);
                            }
                            else if (e.InitialSilenceTimeout)
                            {
                                System.Diagnostics.Debug.WriteLine("Detected silence");
                            }
                            else if (e.BabbleTimeout)
                            {
                                System.Diagnostics.Debug.WriteLine("Detected babbling");
                            }
                            else if (e.InputStreamEnded)
                            {
                                System.Diagnostics.Debug.WriteLine("Input stream ended early");
                            }
                            else if (e.Result != null)
                            {
                                System.Diagnostics.Debug.WriteLine("Grammar = {0}; Text = {1}; Confidence = {2}", e.Result.Grammar.Name, e.Result.Text, e.Result.Confidence);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("No result");
                            }

                            completed = true;
                        };
                        // Start asynchronous, continuous speech recognition.  
                        recognizer.RecognizeAsync(RecognizeMode.Multiple);


                        while (!completed)
                        {
                            if (_userRequestedAbort)
                            {
                                recognizer.RecognizeAsyncCancel();
                                break;
                            }

                            Thread.Sleep(333);
                        }

                        Console.WriteLine("Done.");

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System Speech STT error:" + ex.Message.ToString()+" | (Most likely you are trying to use a language that is not installed on your PC, therefore you must add that language for more info check the FAQ in the discord)");

                }

            }

            public void speechTTSButtonLiteClick(VoiceWizardWindow MainForm)
            {

                if (listeningCurrently == false)
                {
                    _userRequestedAbort = false;
                    Task.Run(() => startListeningNow(MainForm));
                //  waveIn.StartRecording();
                // var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[System Speech Started Listening]");
                    listeningCurrently = true;
                }
                else
                {
                    _userRequestedAbort = true;
                // recognizer.RecognizeAsyncStop();
                ///  waveIn.StopRecording();
                //    var ot = new OutputText();
                VoiceWizardWindow.MainFormGlobal.ot.outputLog(MainForm, "[System Speech Stop Listening]");
                    listeningCurrently = false;
                }




            }
        public void OnDataAvailable(object? sender, WaveInEventArgs e)=> _stream.Write(e.Buffer, 0, e.BytesRecorded);




    }
}
public class SpeechStreamer : Stream
{
    private AutoResetEvent _writeEvent;
    private List<byte> _buffer;
    private int _buffersize;
    private int _readposition;
    private int _writeposition;
    private bool _reset;

    public SpeechStreamer(int bufferSize)
    {
        _writeEvent = new AutoResetEvent(false);
        _buffersize = bufferSize;
        _buffer = new List<byte>(_buffersize);
        for (int i = 0; i < _buffersize; i++)
            _buffer.Add(new byte());
        _readposition = 0;
        _writeposition = 0;
    }

    public override bool CanRead
    {
        get { return true; }
    }

    public override bool CanSeek
    {
        get { return false; }
    }

    public override bool CanWrite
    {
        get { return true; }
    }

    public override long Length
    {
        get { return -1L; }
    }

    public override long Position
    {
        get { return 0L; }
        set { }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return 0L;
    }

    public override void SetLength(long value)
    {

    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int i = 0;
        while (i < count && _writeEvent != null)
        {
            if (!_reset && _readposition >= _writeposition)
            {
                _writeEvent.WaitOne(100, true);
                continue;
            }
            buffer[i] = _buffer[_readposition + offset];
            _readposition++;
            if (_readposition == _buffersize)
            {
                _readposition = 0;
                _reset = false;
            }
            i++;
        }

        return count;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        for (int i = offset; i < offset + count; i++)
        {
            _buffer[_writeposition] = buffer[i];
            _writeposition++;
            if (_writeposition == _buffersize)
            {
                _writeposition = 0;
                _reset = true;
            }
        }
        _writeEvent.Set();

    }

    public override void Close()
    {
        _writeEvent.Close();
        _writeEvent = null;
        base.Close();
    }

    public override void Flush()
    {

    }
}
