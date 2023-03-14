//from naudio/varispeed-sample repo
namespace VarispeedDemo.SoundTouch
{
    enum SoundTouchSettings
    {
        /// <summary>
        /// Available setting IDs for the 'setSetting' and 'get_setting' functions.
        /// Enable/disable anti-alias filter in pitch transposer (0 = disable)
        /// </summary>
        UseAaFilter = 0,

        /// <summary>
        /// Pitch transposer anti-alias filter length (8 .. 128 taps, default = 32)
        /// </summary>
        AaFilterLength = 1,

        /// <summary>
        /// Enable/disable quick seeking algorithm in tempo changer routine
        /// (enabling quick seeking lowers CPU utilization but causes a minor sound
        ///  quality compromising)
        /// </summary>
        UseQuickSeek = 2,

        /// <summary>
        /// Time-stretch algorithm single processing sequence length in milliseconds. This determines 
        /// to how long sequences the original sound is chopped in the time-stretch algorithm. 
        /// See "STTypes.h" or README for more information.
        /// </summary>
        SequenceMs = 3,

        /// <summary>
        /// Time-stretch algorithm seeking window length in milliseconds for algorithm that finds the 
        /// best possible overlapping location. This determines from how wide window the algorithm 
        /// may look for an optimal joining location when mixing the sound sequences back together. 
        /// See "STTypes.h" or README for more information.
        /// </summary>
        SeekWindowMs = 4,

        /// <summary>
        /// Time-stretch algorithm overlap length in milliseconds. When the chopped sound sequences 
        /// are mixed back together, to form a continuous sound stream, this parameter defines over 
        /// how long period the two consecutive sequences are let to overlap each other. 
        /// See "STTypes.h" or README for more information.
        /// </summary>
        OverlapMs = 5
    };
}