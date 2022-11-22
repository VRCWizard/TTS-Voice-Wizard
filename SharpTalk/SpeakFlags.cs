using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTalk
{
    [Flags]
    internal enum SpeakFlags : uint
    {
        Normal = 0,
        Force = 1
    }
}
