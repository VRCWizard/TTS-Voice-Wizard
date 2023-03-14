using System;
using System.Runtime.InteropServices;
using System.Text;


//from naudio/varispeed-sample repo

namespace VarispeedDemo.SoundTouch
{
    class SoundTouch : IDisposable
    {
        private IntPtr handle;
        private string versionString;
        private readonly bool is64Bit;
        public SoundTouch()
        {
            is64Bit = Marshal.SizeOf<IntPtr>() == 8;

            handle = is64Bit ? SoundTouchInterop64.soundtouch_createInstance() :
                SoundTouchInterop32.soundtouch_createInstance();
        }

        public string VersionString
        {
            get
            {
                if (versionString == null)
                {
                    var s = new StringBuilder(100);
                    if (is64Bit)
                        SoundTouchInterop64.soundtouch_getVersionString2(s, s.Capacity);
                    else
                        SoundTouchInterop32.soundtouch_getVersionString2(s, s.Capacity);
                    versionString = s.ToString();
                }
                return versionString;
            }
        }

        public void SetPitchOctaves(float pitchOctaves)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setPitchOctaves(handle, pitchOctaves);
            else
                SoundTouchInterop32.soundtouch_setPitchOctaves(handle, pitchOctaves);
        }

        public void SetSampleRate(int sampleRate)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setSampleRate(handle, (uint) sampleRate);
            else 
                SoundTouchInterop32.soundtouch_setSampleRate(handle, (uint)sampleRate);
        }

        public void SetChannels(int channels)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setChannels(handle, (uint) channels);
            else
                SoundTouchInterop32.soundtouch_setChannels(handle, (uint)channels);
        }

        private void DestroyInstance()
        {
            if (handle != IntPtr.Zero)
            {
                if (is64Bit)
                    SoundTouchInterop64.soundtouch_destroyInstance(handle);
                else
                    SoundTouchInterop32.soundtouch_destroyInstance(handle);
                handle = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            DestroyInstance();
            GC.SuppressFinalize(this);
        }

        ~SoundTouch()
        {
            DestroyInstance();
        }

        public void PutSamples(float[] samples, int numSamples)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_putSamples(handle, samples, numSamples);
            else
                SoundTouchInterop32.soundtouch_putSamples(handle, samples, numSamples);
        }

        public int ReceiveSamples(float[] outBuffer, int maxSamples)
        {
            if (is64Bit)
                return (int)SoundTouchInterop64.soundtouch_receiveSamples(handle, outBuffer, (uint)maxSamples);
            return (int)SoundTouchInterop32.soundtouch_receiveSamples(handle, outBuffer, (uint)maxSamples);
        }

        public bool IsEmpty
        {
            get
            {
                if (is64Bit)
                    return SoundTouchInterop64.soundtouch_isEmpty(handle) != 0;
                return SoundTouchInterop32.soundtouch_isEmpty(handle) != 0;
            }
        }

        public int NumberOfSamplesAvailable
        {
            get
            {
                if (is64Bit)
                   return (int)SoundTouchInterop64.soundtouch_numSamples(handle);
                return (int)SoundTouchInterop32.soundtouch_numSamples(handle);
            }
        }

        public int NumberOfUnprocessedSamples
        {
            get
            {
                if (is64Bit)
                    return SoundTouchInterop64.soundtouch_numUnprocessedSamples(handle);
                return SoundTouchInterop32.soundtouch_numUnprocessedSamples(handle);
            }
        }

        public void Flush()
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_flush(handle);
            else
                SoundTouchInterop32.soundtouch_flush(handle);
        }

        public void Clear()
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_clear(handle);
            else
                SoundTouchInterop32.soundtouch_clear(handle);
        }

        public void SetRate(float newRate)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setRate(handle, newRate);
            else
                SoundTouchInterop32.soundtouch_setRate(handle, newRate);
        }

        public void SetTempo(float newTempo)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setTempo(handle, newTempo);
            else
                SoundTouchInterop32.soundtouch_setTempo(handle, newTempo);
        }

        public int GetUseAntiAliasing()
        {
            if (is64Bit)
                return SoundTouchInterop64.soundtouch_getSetting(handle, SoundTouchSettings.UseAaFilter);
            return SoundTouchInterop32.soundtouch_getSetting(handle, SoundTouchSettings.UseAaFilter);
        }

        public void SetUseAntiAliasing(bool useAntiAliasing)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setSetting(handle, SoundTouchSettings.UseAaFilter, useAntiAliasing ? 1 : 0);
            else
                SoundTouchInterop32.soundtouch_setSetting(handle, SoundTouchSettings.UseAaFilter, useAntiAliasing ? 1 : 0);
        }

        public void SetUseQuickSeek(bool useQuickSeek)
        {
            if (is64Bit)
                SoundTouchInterop64.soundtouch_setSetting(handle, SoundTouchSettings.UseQuickSeek, useQuickSeek ? 1 : 0);
            else
                SoundTouchInterop32.soundtouch_setSetting(handle, SoundTouchSettings.UseQuickSeek, useQuickSeek ? 1 : 0);
        }

        public int GetUseQuickSeek()
        {
            if (is64Bit)
                return SoundTouchInterop64.soundtouch_getSetting(handle, SoundTouchSettings.UseQuickSeek);
            return SoundTouchInterop32.soundtouch_getSetting(handle, SoundTouchSettings.UseQuickSeek);
        }
    }
}
