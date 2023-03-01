using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SharpTalk;
using System.Diagnostics;
using SharpTalk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MoonbaseAlpha
{
    [ApiController]
    [Route("[controller]")]
    public class AudioController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post(string voice, string text)
        {
            //   Console.WriteLine("Sending synthesized audio");
            //   Debug.WriteLine("Sending synthesized audio");

            var tts = new FonixTalkEngine();
           
              
                   switch (voice)
                   {
                       case "Betty": tts.Voice = TtsVoice.Betty; break;
                       case "Dennis": tts.Voice = TtsVoice.Dennis; break;
                       case "Frank": tts.Voice = TtsVoice.Frank; break;
                       case "Harry": tts.Voice = TtsVoice.Harry; break;
                       case "Kit": tts.Voice = TtsVoice.Kit; break;
                       case "Paul": tts.Voice = TtsVoice.Paul; break;
                       case "Rita": tts.Voice = TtsVoice.Rita; break;
                       case "Ursula": tts.Voice = TtsVoice.Ursula; break;
                       case "Wendy": tts.Voice = TtsVoice.Wendy; break;
                       default: break;
                   }

                //////////////////// //   tts.Speak(phrase); //ONLY WORKS IF PROJECT > PROPERTIES > BUILD > PLATFORM TARGET  is set to x86 due to the FonixTalk.dll being 32 bit only
                MemoryStream memoryStream = new MemoryStream();
                 tts.SpeakToStream(memoryStream, text);
                 tts.Dispose();

                 memoryStream.Flush();
                 memoryStream.Seek(0, SeekOrigin.Begin);

                 var bytes = memoryStream.ToArray();
                 string audio = Convert.ToBase64String(bytes); 

                //  string audio = "Your audio response here";
               // string audio = voice+text;
            //   Console.WriteLine("Synthesis Successful");
          
            return Ok(audio);
        }
    }
}
