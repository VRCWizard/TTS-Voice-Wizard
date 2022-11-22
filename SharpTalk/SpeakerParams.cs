using System.Runtime.InteropServices;

namespace SharpTalk
{
    /// <summary>
    /// Contains parameters used to modify the sound of a TTS voice.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SpeakerParams
    {
        /// <summary>
        /// The sex of the speaker
        /// Design voice alias: sx
        /// </summary>
        [MarshalAs(UnmanagedType.I2)]
        public Sex Sex;

        /// <summary>
        /// Smoothness, in %
        /// Design voice alias: sm
        /// </summary>
        public short Smoothness;

        /// <summary>
        /// Assertiveness, in %
        /// Design voice alias: as
        /// </summary>
        public short Assertiveness;

        /// <summary>
        /// Average pitch, in Hz
        /// Design voice alias: ap
        /// </summary>
        public short AveragePitch;

        /// <summary>
        /// Breathiness, in decibels (dB)
        /// Design voice alias: br
        /// </summary>
        public short Breathiness;

        /// <summary>
        /// Richness, in %
        /// Design voice alias: ri
        /// </summary>
        public short Richness;

        /// <summary>
        /// Number of fixed samples of open glottis
        /// Design voice alias: nf
        /// </summary>
        public short NumFixedSampOG;

        /// <summary>
        /// Laryngealization, in %
        /// Design voice alias: la
        /// </summary>
        public short Laryngealization;

        /// <summary>
        /// Head size, in %
        /// Design voice alias: hs
        /// </summary>
        public short HeadSize;

        /// <summary>
        /// Fourth formant resonance frequency, in Hz
        /// Design voice alias: f4
        /// </summary>
        public short Formant4ResFreq;

        /// <summary>
        /// Fourth formant bandwidth, in Hz
        /// Design voice alias: b4
        /// </summary>
        public short Formant4Bandwidth;

        /// <summary>
        /// Fifth formant resonance frequency, in Hz
        /// Design voice alias: f5
        /// </summary>
        public short Formant5ResFreq;

        /// <summary>
        /// Fifth formant bandwidth, in Hz
        /// Design voice alias: b5
        /// </summary>
        public short Formant5Bandwidth;

        /// <summary>
        /// Parallel fourth formant frequency, in Hz
        /// </summary>
        public short Parallel4Freq;

        /// <summary>
        /// Parallel fifth formant frequency, in Hz
        /// </summary>
        public short Parallel5Freq;

        /// <summary>
        /// Gain of frication source, in dB
        /// Design voice alias: gf
        /// </summary>
        public short GainFrication;

        /// <summary>
        /// Gain of aspiration source, in dB
        /// Design voice alias: gh
        /// </summary>
        public short GainAspiration;

        /// <summary>
        /// Gain of voicing source, in dB
        /// Design voice alias: gv
        /// </summary>
        public short GainVoicing;

        /// <summary>
        /// Gain of nasalization, in dB
        /// Design voice alias: gn
        /// </summary>
        public short GainNasalization;

        /// <summary>
        /// Gain of cascade formant resonator 1, in dB
        /// Design voice alias: g1
        /// </summary>
        public short GainCFR1;

        /// <summary>
        /// Gain of cascade formant resonator 2, in dB
        /// Design voice alias: g2
        /// </summary>
        public short GainCFR2;

        /// <summary>
        /// Gain of cascade formant resonator 3, in dB
        /// Design voice alias: g3
        /// </summary>
        public short GainCFR3;

        /// <summary>
        /// Gain of cascade formant resonator 4, in dB
        /// Design voice alias: g4
        /// </summary>
        public short GainCFR4;

        /// <summary>
        /// Loudness, gain input to cascade 1st formant in dB
        /// Design voice alias: g5
        /// </summary>
        public short Loudness;

        /// <summary>
        /// Not the slightest clue what this does.
        /// </summary>
        public short SpectralTilt;

        /// <summary>
        /// Baseline fall, in Hz
        /// Design voice alias: bf
        /// </summary>
        public short BaselineFall;

        /// <summary>
        /// Lax breathiness, in %
        /// Design voice alias: lx
        /// </summary>
        public short LaxBreathiness;

        /// <summary>
        /// Quickness, in %
        /// Design voice alias: qu
        /// </summary>
        public short Quickness;

        /// <summary>
        /// Hat rise, in Hz
        /// Design voice alias: hr
        /// </summary>
        public short HatRise;

        /// <summary>
        /// Stress rise, in Hz
        /// Design voice alias: sr
        /// </summary>
        public short StressRise;

        /// <summary>
        /// Glottal speed
        /// </summary>
        public short GlottalSpeed;

        /// <summary>
        /// Output gain multiplier for FVTM
        /// </summary>
        public short OutputGainMultiplier;
    }

    /// <summary>
    /// Provides gender selection options for SpeakerParams.
    /// </summary>
    public enum Sex : short
    {
        /// <summary>
        /// Indicates a female voice.
        /// Design voice value: 0 
        /// </summary>
        Female = 0,
        /// <summary>
        /// Indicates a male voice.
        /// Design voice value: 1
        /// </summary>
        Male = 1
    }
}
