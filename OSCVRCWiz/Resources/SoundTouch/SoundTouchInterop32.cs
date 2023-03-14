using System;
using System.Runtime.InteropServices;
using System.Text;
//from naudio/varispeed-sample repo
namespace VarispeedDemo.SoundTouch
{
    class SoundTouchInterop32
    {
        private const string SoundTouchDllName = "SoundTouch.dll";

        /// <summary>
        /// Create a new instance of SoundTouch processor.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr soundtouch_createInstance();

        /// <summary>
        /// Destroys a SoundTouch processor instance.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_destroyInstance(IntPtr h);

        /// <summary>
        /// Get SoundTouch library version string - alternative function for 
        /// environments that can't properly handle character string as return value
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_getVersionString2(StringBuilder versionString, int bufferSize);

        /// <summary>
        /// Get SoundTouch library version Id
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint soundtouch_getVersionId();

        /// <summary>
        /// Sets new rate control value. Normal rate = 1.0, smaller values
        /// represent slower rate, larger faster rates.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setRate(IntPtr h, float newRate);

        /// <summary>
        /// Sets new tempo control value. Normal tempo = 1.0, smaller values
        /// represent slower tempo, larger faster tempo.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setTempo(IntPtr h, float newTempo);

        /// <summary>
        /// Sets new rate control value as a difference in percents compared
        /// to the original rate (-50 .. +100 %);
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setRateChange(IntPtr h, float newRate);

        /// <summary>
        /// Sets new tempo control value as a difference in percents compared
        /// to the original tempo (-50 .. +100 %);
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setTempoChange(IntPtr h, float newTempo);

        /// <summary>
        /// Sets new pitch control value. Original pitch = 1.0, smaller values
        /// represent lower pitches, larger values higher pitch.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setPitch(IntPtr h, float newPitch);

        /// <summary>
        /// Sets pitch change in octaves compared to the original pitch  
        /// (-1.00 .. +1.00);
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setPitchOctaves(IntPtr h, float newPitch);

        /// <summary>
        /// Sets pitch change in semi-tones compared to the original pitch
        /// (-12 .. +12);
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setPitchSemiTones(IntPtr h, float newPitch);

        /// <summary>
        /// Sets the number of channels, 1 = mono, 2 = stereo
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setChannels(IntPtr h, uint numChannels);

        /// <summary>
        /// Sets sample rate.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_setSampleRate(IntPtr h, uint srate);

        /// <summary>
        /// Flushes the last samples from the processing pipeline to the output.
        /// Clears also the internal processing buffers.
        ///
        /// Note: This function is meant for extracting the last samples of a sound
        /// stream. This function may introduce additional blank samples in the end
        /// of the sound stream, and thus it's not recommended to call this function
        /// in the middle of a sound stream.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_flush(IntPtr h);

        /// <summary>
        /// Adds 'numSamples' pcs of samples from the 'samples' memory position into
        /// the input of the object. Notice that sample rate _has_to_ be set before
        /// calling this function, otherwise throws a runtime_error exception.
        /// </summary>
        /// <param name="h">Handle</param>
        /// <param name="samples">Pointer to sample buffer.</param>
        /// <param name="numSamples">Number of samples in buffer. Notice that in case of stereo-sound a single sample contains data for both channels.</param>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_putSamples(IntPtr h, [MarshalAs(UnmanagedType.LPArray)] float[] samples, int numSamples);

        /// <summary>
        /// Clears all the samples in the object's output and internal processing
        /// buffers.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void soundtouch_clear(IntPtr h);

        /// <summary>
        /// Changes a setting controlling the processing system behaviour. See the
        /// 'SETTING_...' defines for available setting ID's.
        /// </summary>
        /// <param name="h">Handle</param>
        /// <param name="settingId">Setting ID number, see SETTING_... defines.</param>
        /// <param name="value">New setting value.</param>
        /// <returns>'TRUE' if the setting was succesfully changed</returns>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool soundtouch_setSetting(IntPtr h, SoundTouchSettings settingId, int value);

        /// <summary>
        /// Reads a setting controlling the processing system behaviour. See the
        /// 'SETTING_...' defines for available setting ID's.
        /// </summary>
        /// <param name="h">Handle</param>
        /// <param name="settingId">Setting ID number, see SETTING_... defines.</param>
        /// <returns>The setting value</returns>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int soundtouch_getSetting(IntPtr h, SoundTouchSettings settingId);

        /// <summary>
        /// Returns number of samples currently unprocessed.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int soundtouch_numUnprocessedSamples(IntPtr h);

        /// <summary>
        ///  Adjusts book-keeping so that given number of samples are removed from beginning of the 
        ///  sample buffer without copying them anywhere. 
        /// 
        ///  Used to reduce the number of samples in the buffer when accessing the sample buffer directly
        ///  with 'ptrBegin' function.
        /// </summary>
        /// <param name="h">Handle</param>
        /// <param name="outBuffer">Buffer where to copy output samples.</param>
        /// <param name="maxSamples">How many samples to receive at max.</param>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint soundtouch_receiveSamples(IntPtr h, [MarshalAs(UnmanagedType.LPArray)] float[] outBuffer, uint maxSamples);

        /// <summary>
        /// Returns number of samples currently available.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint soundtouch_numSamples(IntPtr h);

        /// <summary>
        /// Returns nonzero if there aren't any samples available for outputting.
        /// </summary>
        [DllImport(SoundTouchDllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int soundtouch_isEmpty(IntPtr h);

    }
}