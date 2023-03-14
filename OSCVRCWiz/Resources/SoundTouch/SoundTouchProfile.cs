//from naudio/varispeed-sample repo
namespace VarispeedDemo.SoundTouch
{
    internal class SoundTouchProfile
    {
        public bool UseTempo { get; set; }
        public bool UseAntiAliasing { get; set; }
        public bool UseQuickSeek { get; set; }

        public SoundTouchProfile(bool useTempo, bool useAntiAliasing)
        {
            UseTempo = useTempo;
            UseAntiAliasing = useAntiAliasing;
        }
    }
}