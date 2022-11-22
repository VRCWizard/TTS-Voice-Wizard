using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTalk
{
    internal enum TTSMessageType : uint
    {
        Buffer = 0,
        IndexMarker = 1,
        Status = 2,
        Visual = 3
    }
}
