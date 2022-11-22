using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SharpTalk
{
    [Flags]
    internal enum DeviceOptions : uint
    {
        OwnAudioDevice =         0x00000001,
        ReportOpenError =        0x00000002,
        UseSapi5AudioDevice =    0x40000000,
        DoNotUseAudioDevice =    0x80000000
    }
}
