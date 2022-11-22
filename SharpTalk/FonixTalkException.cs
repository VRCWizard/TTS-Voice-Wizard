using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTalk
{
    /// <summary>
    /// Contains information related to errors thrown by the TTS engine.
    /// </summary>
    public sealed class FonixTalkException : Exception
    {
        internal FonixTalkException(MMRESULT code) : base(GetMessage(code))
        {            
        }

        internal FonixTalkException(string message) : base(message)
        {
        }

        private static string GetMessage(MMRESULT code)
        {
            switch(code)
            {
                case MMRESULT.MMSYSERR_INVALPARAM:
                    return "An invalid parameter was passed to the function.";
                case MMRESULT.MMSYSERR_INVALHANDLE:
                    return "The associated handle is invalid. Did you dispose it?";
                case MMRESULT.MMSYSERR_ERROR:
                    return "The function returned a generic error. Please check that you are using the functions correctly.";
                case MMRESULT.MMSYSERR_NOERROR:
                    return "The function did not throw an error. If you are seeing this, Berkin was obviously high while coding.";
                case MMRESULT.MMSYSERR_NOMEM:
                    return "There was insufficnent memory available to allocate the requested resources.";
                case MMRESULT.MMSYSERR_ALLOCATED:
                    return "The requested resources are already in use somewhere else.";
                case MMRESULT.WAVERR_BADFORMAT:
                    return "Wave output device does not support request format.";
                case MMRESULT.MMSYSERR_BADDEVICEID:
                    return "Device ID out of range.";
                case MMRESULT.MMSYSERR_NODRIVER:
                    return "No wave output device present.";
                default:
                    return code.ToString();
            }
        }
    }
}
