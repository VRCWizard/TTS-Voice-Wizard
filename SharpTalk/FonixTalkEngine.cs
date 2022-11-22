using System;
using System.Text;
using System.IO;

using System.Runtime.InteropServices;

namespace SharpTalk
{
    /// <summary>
    /// Wraps the functions contained in the FonixTalk TTS engine.
    /// </summary>
    public class FonixTalkEngine : IDisposable
    {
        #region Events
        /// <summary>
        /// Fired when a phoneme event is invoked by the engine.
        /// </summary>
        public event EventHandler<PhonemeEventArgs> Phoneme;
        #endregion

        #region Defaults
        /// <summary>
        /// The default speaking rate assigned to new instances of the engine.
        /// </summary>
        public const uint DefaultRate = 200;

        /// <summary>
        /// The default voice assigned to new instances of the engine.
        /// </summary>
        public const TtsVoice DefaultSpeaker = TtsVoice.Paul;
        #endregion

        #region P/Invoke stuff
        #region FonixTalk functions

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void DtCallbackRoutine(
            int lParam1,
            int lParam2,
            uint drCallbackParameter,
            uint uiMsg);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechStartupEx(
            out IntPtr handle,
            uint uiDeviceNumber,
            uint dwDeviceOptions,
            DtCallbackRoutine callback,
            ref IntPtr dwCallbackParameter);

        [DllImport("FonixTalk.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TextToSpeechSelectLang(IntPtr handle, uint lang);

        [DllImport("FonixTalk.dll")]
        static extern uint TextToSpeechStartLang(
            [MarshalAs(UnmanagedType.LPStr)]
            string lang);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSetSpeaker(IntPtr handle, TtsVoice speaker);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechGetSpeaker(IntPtr handle, out TtsVoice speaker);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechGetRate(IntPtr handle, out uint rate);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSetRate(IntPtr handle, uint rate);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSpeakA(IntPtr handle,
            [MarshalAs(UnmanagedType.LPStr)] 
            string msg,
            uint flags);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechShutdown(IntPtr handle);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechPause(IntPtr handle);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechResume(IntPtr handle);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechReset(IntPtr handle,
            [MarshalAs(UnmanagedType.Bool)]
            bool bReset);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSync(IntPtr handle);

        /* These don't seem to have any effect, but I'll keep them here in case a fix is found.
         * 
        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSetVolume(IntPtr handle, int type, int volume);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechGetVolume(IntPtr handle, int type, out int volume);
         * 
        */

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechSetSpeakerParams(IntPtr handle, IntPtr spDefs);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechGetSpeakerParams(IntPtr handle, uint uiIndex,
             out IntPtr ppspCur,
             out IntPtr ppspLoLimit,
             out IntPtr ppspHiLimit,
             out IntPtr ppspDefault);

        [DllImport("FonixTalk.dll")]
        static unsafe extern MMRESULT TextToSpeechAddBuffer(IntPtr handle, TtsBufferManaged.TTS_BUFFER_T* buffer);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechOpenInMemory(IntPtr handle, uint format);

        [DllImport("FonixTalk.dll")]
        static extern MMRESULT TextToSpeechCloseInMemory(IntPtr handle);

        /*
        [DllImport("FonixTalk.dll")]
        static unsafe extern MMRESULT TextToSpeechReturnBuffer(IntPtr handle, TtsBufferManaged.TTS_BUFFER_T* buffer);
        */
        #endregion

        #region Win32 functions
        [DllImport("user32.dll")]
        private static extern uint RegisterWindowMessage(
            [MarshalAs(UnmanagedType.LPStr)]
            string lpString);

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        #endregion
        #endregion

        #region API Constants
        private const uint WaveFormat_1M16 = 0x00000004;
        private const uint TtsNotSupported = 0x7FFF;
        private const uint TtsNotAvailable = 0x7FFE;
        private const uint TtsLangError = 0x4000;
        #endregion

        #region Window messages

        // Message types
        private static readonly uint UiIndexMsg = RegisterWindowMessage("DECtalkIndexMessage");
        private static readonly uint UiErrorMsg = RegisterWindowMessage("DECtalkErrorMessage");
        private static readonly uint UiBufferMsg = RegisterWindowMessage("DECtalkBufferMessage");
        private static readonly uint UiPhonemeMsg = RegisterWindowMessage("DECtalkVisualMessage");

        #endregion

        #region Non-public fields
        private IntPtr _handle;
        private IntPtr _speakerParamsPtr, _dummy1, _dummy2, _dummy3;
        private DtCallbackRoutine _callback;
        private TtsBufferManaged _buffer;
        private Stream _bufferStream;
        #endregion

        #region Constructors and Initialization logic
        /// <summary>
        /// Initializes a new instance of the engine.
        /// </summary>
        /// <param name="language">The language to load.</param>
        public FonixTalkEngine(string language)
        {
            Init(language);
        }

        /// <summary>
        /// Initializes a new instance of the engine in US English.
        /// </summary>
        public FonixTalkEngine()
        {
            Init("US");
        }

        /// <summary>
        /// Initialize a new instance of the engine with the specified language, rate, and speaker voice.
        /// </summary>
        /// <param name="language">The language ID.</param>
        /// <param name="rate">The speaking rate to set.</param>
        /// <param name="speaker">The speaker voice to set.</param>
        public FonixTalkEngine(string language, uint rate, TtsVoice speaker)
        {
            Init(language);
            Voice = speaker;
            Rate = rate;
        }

        /// <summary>
        /// Initializes a new instance of the engine in US English with the specified rate and speaker voice.
        /// </summary>
        /// <param name="rate">The speaking rate to set.</param>
        /// <param name="speaker">The speaker voice to set.</param>
        public FonixTalkEngine(uint rate, TtsVoice speaker)
        {
            Init(LanguageCode.EnglishUS);
            Voice = speaker;
            Rate = rate;
        }

        private void Init(string lang)
        {
            _callback = TtsCallback;
            _buffer = null;
            _bufferStream = null;

            if (lang != LanguageCode.None)
            {
                var langid = TextToSpeechStartLang(lang);

                if ((langid & TtsLangError) != 0)
                {
                    switch (langid)
                    {
                        case TtsNotSupported:
                            throw new FonixTalkException("This version of DECtalk does not support multiple languages.");
                        case TtsNotAvailable:
                            throw new FonixTalkException("The specified language was not found.");
                    }
                }

                if (!TextToSpeechSelectLang(IntPtr.Zero, langid))
                {
                    throw new FonixTalkException("The specified language failed to load.");
                }
            }

            Check(TextToSpeechStartupEx(out _handle, 0xFFFFFFFF, 0, _callback, ref _handle));

            Speak("[:phone on]"); // Enable singing by default
        }

        #endregion

        #region Callback and phoneme structures
        [StructLayout(LayoutKind.Sequential)]
        private struct PhonemeMark
        {
            public byte ThisPhoneme;
            public byte NextPhoneme;
            public ushort Duration;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PhonemeTag
        {
            [FieldOffset(0)]
            public PhonemeMark PMData;
            [FieldOffset(0)]
            public int DWData;
        }

        private void TtsCallback(int lParam1, int lParam2, uint drCallbackParameter, uint uiMsg)
        {
            if (uiMsg == UiPhonemeMsg && Phoneme != null)
            {
                var tag = new PhonemeTag { DWData = lParam2 };
                Phoneme(this, new PhonemeEventArgs((char)tag.PMData.ThisPhoneme, tag.PMData.Duration));
            }
            else if (uiMsg == UiBufferMsg)
            {
                _bufferStream.Write(_buffer.GetBufferBytes(), 0, (int)_buffer.Length);
                var full = _buffer.Full;
                _buffer.Reset();
                unsafe { Check(TextToSpeechAddBuffer(_handle, _buffer.ValuePointer)); }
            }
            else if (uiMsg == UiErrorMsg)
            {
                // You fucked up!
            }
            else if (uiMsg == UiIndexMsg)
            {
                // I don't even know what index messages are for...
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the voice currently assigned to the engine.
        /// </summary>
        public TtsVoice Voice
        {
            get
            {
                TtsVoice voice;
                Check(TextToSpeechGetSpeaker(_handle, out voice));
                return voice;
            }
            set
            {
                Check(TextToSpeechSetSpeaker(_handle, value));
            }
        }

        /// <summary>
        /// Gets or sets the current speaking rate of the TTS voice.
        /// </summary>
        public uint Rate
        {
            get
            {
                uint rate;
                Check(TextToSpeechGetRate(_handle, out rate));
                return rate;
            }
            set
            {
                Check(TextToSpeechSetRate(_handle, value));
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Writes speech data to an internal buffer and returns it as a byte array containing 16-bit 11025Hz mono PCM data.
        /// </summary>
        /// <param name="input">The input text to process.</param>
        /// <returns></returns>
        public byte[] SpeakToMemory(string input)
        {
            using (_bufferStream = new MemoryStream())
            {
                using (OpenInMemory(WaveFormat_1M16))
                using (ReadyBuffer())
                {
                    Speak(input);
                    Sync();
                    TextToSpeechReset(_handle, false);
                }
                return ((MemoryStream)_bufferStream).ToArray();
            }
        }

        /// <summary>
        /// Writes speech data to the specified stream as 16-bit 11025Hz mono PCM data.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="input">The input text to process.</param>
        /// <returns></returns>
        public void SpeakToStream(Stream stream, string input)
        {
            _bufferStream = stream;
            using (OpenInMemory(WaveFormat_1M16))
            using (ReadyBuffer())
            {
                Speak(input);
                Sync();
                TextToSpeechReset(_handle, false);
            }
            _bufferStream = null;
        }

        /// <summary>
        /// Writes speech data to a PCM WAV file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="input">The input text to process.</param>
        public void SpeakToWavFile(string path, string input)
        {
            const int headerSize = 44;
            const int formatChunkSize = 16;
            const short waveAudioFormat = 1;
            const short numChannels = 1;
            const int sampleRate = 11025;
            const short bitsPerSample = 16;
            const int byteRate = (numChannels * bitsPerSample * sampleRate) / 8;
            const short blockAlign = numChannels * bitsPerSample / 8;

            using (var dataStream = new MemoryStream())
            {
                SpeakToStream(dataStream, input);
                var sizeInBytes = (int)dataStream.Length;
                using (var writer = new BinaryWriter(File.Create(path), Encoding.ASCII))
                {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(sizeInBytes + headerSize - 8);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(formatChunkSize);
                    writer.Write(waveAudioFormat);
                    writer.Write(numChannels);
                    writer.Write(sampleRate);
                    writer.Write(byteRate);
                    writer.Write(blockAlign);
                    writer.Write(bitsPerSample);
                    writer.Write("data".ToCharArray());
                    writer.Write(sizeInBytes);
                    dataStream.Position = 0;
                    dataStream.CopyTo(writer.BaseStream);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current speaker parameters.
        /// </summary>
        public SpeakerParams SpeakerParams
        {
            get
            {
                Check(TextToSpeechGetSpeakerParams(_handle, 0, out _speakerParamsPtr, out _dummy1, out _dummy2, out _dummy3));
                return (SpeakerParams)Marshal.PtrToStructure(_speakerParamsPtr, typeof(SpeakerParams));
            }
            set
            {
                Check(TextToSpeechGetSpeakerParams(_handle, 0, out _speakerParamsPtr, out _dummy1, out _dummy2, out _dummy3));
                Marshal.StructureToPtr(value, _speakerParamsPtr, false);
                Check(TextToSpeechSetSpeakerParams(_handle, _speakerParamsPtr));
            }
        }

        /// <summary>
        /// Pauses TTS audio output.
        /// </summary>
        public void Pause()
        {
            Check(TextToSpeechPause(_handle));
        }

        /// <summary>
        /// Resumes previously paused TTS audio output.
        /// </summary>
        public void Resume()
        {
            Check(TextToSpeechResume(_handle));
        }

        /// <summary>
        /// Flushes all previously queued text from the TTS system and stops any audio output.
        /// </summary>
        public void Reset()
        {
            Check(TextToSpeechReset(_handle, false));
        }

        /// <summary>
        /// Blocks until all previously queued text is processed.
        /// </summary>
        public void Sync()
        {
            Check(TextToSpeechSync(_handle));
        }

        /// <summary>
        /// Causes the engine to begin asynchronously speaking a specified phrase. If the engine is in the middle of speaking, the message passed will be queued.
        /// </summary>
        /// <param name="msg">The phrase for the engine to speak.</param>
        public void Speak(string msg)
        {
            Check(TextToSpeechSpeakA(_handle, msg, (uint)SpeakFlags.Force));
        }

        #endregion

        #region Non-public methods
        private static void Check(MMRESULT code)
        {
            if (code != MMRESULT.MMSYSERR_NOERROR)
            {
                throw new FonixTalkException(code);
            }
        }

        #region OpenCloseInMemory
        private InMemoryRaiiHelper OpenInMemory(uint format)
        {
            Check(TextToSpeechOpenInMemory(_handle, format));
            return new InMemoryRaiiHelper(this);
        }

        private void CloseInMemory()
        {
            Check(TextToSpeechCloseInMemory(_handle));
        }

        private struct InMemoryRaiiHelper : IDisposable
        {
            private readonly FonixTalkEngine _engine;

            public InMemoryRaiiHelper(FonixTalkEngine engine)
            {
                _engine = engine;
            }

            public void Dispose()
            {
                _engine.CloseInMemory();
            }
        }
        #endregion

        #region Buffer
        private BufferRaiiHelper ReadyBuffer()
        {
            if (_buffer != null)
            {
                // Buffer was created by previous call to this method
                throw new InvalidOperationException("Buffer already exists.");
            }
            _buffer = new TtsBufferManaged();
            unsafe { Check(TextToSpeechAddBuffer(_handle, _buffer.ValuePointer)); }
            return new BufferRaiiHelper(this);
        }

        private void FreeBuffer()
        {
            _buffer.Dispose();
            _buffer = null;
        }

        // I'm putting this here because it's the only place in this file I can think of it fits.
        private struct BufferRaiiHelper : IDisposable
        {
            private readonly FonixTalkEngine _engine;

            public BufferRaiiHelper(FonixTalkEngine engine)
            {
                _engine = engine;
            }

            public void Dispose()
            {
                _engine.FreeBuffer();
            }
        }
        #endregion
        #endregion

        #region Disposal

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        ~FonixTalkEngine()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            TextToSpeechShutdown(_handle);
            if (_buffer != null)
            {
                // This is probably never called.
                _buffer.Dispose();
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
