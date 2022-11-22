using System;
using System.Runtime.InteropServices;

namespace SharpTalk
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe class TtsBufferManaged : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        public struct TTS_BUFFER_T
        {
            public const int BufferSize = 16384;

            public IntPtr DataPtr;
            public TTS_PHONEME_T* PhonemeArrayPtr;
            public TTS_INDEX_T* IndexArrayPtr;

            public uint MaxBufferLength;
            public uint MaxPhonemeChanges;
            public uint MaxIndexMarks;

            public uint BufferLength;
            public uint PhonemeChangeCount;
            public uint IndexMarkCount;

            public uint _reserved;
        }

        TTS_BUFFER_T _value;
        GCHandle _pinHandle;

        public TtsBufferManaged()
        {
            _value = new TTS_BUFFER_T();
            _pinHandle = GCHandle.Alloc(this, GCHandleType.Pinned);
            _value.MaxBufferLength = TTS_BUFFER_T.BufferSize;
            _value.DataPtr = Marshal.AllocHGlobal(TTS_BUFFER_T.BufferSize);
        }

        public bool Full
        {
            get { return _value.BufferLength == _value.MaxBufferLength; }
        }

        public byte[] GetBufferBytes()
        {
            var buffer = new byte[_value.BufferLength];
            Marshal.Copy(_value.DataPtr, buffer, 0, (int)_value.BufferLength);
            return buffer;
        }

        public uint Length
        {
            get { return _value.BufferLength; }
        }

        public void Reset()
        {
            _value.BufferLength = 0;
            _value.PhonemeChangeCount = 0;
            _value.IndexMarkCount = 0;
        }

        public TTS_BUFFER_T* ValuePointer
        {
            get
            {
                fixed (TTS_BUFFER_T* p = &_value)
                {
                    // This is fine here only because the class is always pinned.
                    return p;
                }
            }
        }

        public void Dispose()
        {
            // No managed resources
            Marshal.FreeHGlobal(_value.DataPtr);
            _pinHandle.Free();
            GC.SuppressFinalize(this);
        }
    }
}
