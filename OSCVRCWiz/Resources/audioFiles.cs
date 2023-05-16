using NAudio.Wave;
using NAudio.Wave.Compression;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources
{
    public class audioFiles
    {
        public static void writeAudioToOutputMp3(MemoryStream stream)
        {


            // Write audio data to inputStream...

            // Reset the position of the stream to the beginning
            /*      stream.Position = 0;

                  // Create a WaveFileWriter to write the audio data to a WAV file
                  using (var outputStream = new WaveFileWriter("TextOut\\TTSVoiceWizard-output.wav", new WaveFormat(44100, 16, 2)))
                  {
                // Copy the audio data from the inputStream to the outputStream
                stream.CopyTo(outputStream);
                  }
            stream.Dispose();*/

            try
            {
                Mp3FileReader reader = new Mp3FileReader(stream);

                WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
                    
                  WaveFileWriter.CreateWaveFile("TextOut\\TTSVoiceWizard-output.wav", pcmStream);
                    
                
                reader.Dispose();
                pcmStream.Dispose();
                stream.Dispose();
            }
            catch (Exception ex) {
                OutputText.outputLog("[Output to Wave Error: " + ex.Message + "]", Color.Red);
            }



        }
        public static void writeAudioToOutputWav(MemoryStream stream)
        {


            // Write audio data to inputStream...

            // Reset the position of the stream to the beginning
            /*      stream.Position = 0;

                  // Create a WaveFileWriter to write the audio data to a WAV file
                  using (var outputStream = new WaveFileWriter("TextOut\\TTSVoiceWizard-output.wav", new WaveFormat(44100, 16, 2)))
                  {
                // Copy the audio data from the inputStream to the outputStream
                stream.CopyTo(outputStream);
                  }
            stream.Dispose();*/

            try
            {
                WaveFileReader reader = new WaveFileReader(stream);


                   WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
                    
                    WaveFileWriter.CreateWaveFile("TextOut\\TTSVoiceWizard-output.wav", pcmStream);
                    
                
                reader.Dispose();
                pcmStream.Dispose();
                stream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Output to Wave Error: " + ex.Message + "]", Color.Red);
            }



        }




    }
}
