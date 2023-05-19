using NAudio.Wave;
using NAudio.Wave.Compression;
using OSCVRCWiz.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources
{
    public class audioFiles
    {
        public static void writeAudioToOutputMp3(MemoryStream stream)
        {


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
        public static void writeAudioToOutputRaw(MemoryStream stream)
        {


            try
            {
                RawSourceWaveStream reader = new RawSourceWaveStream(stream, new WaveFormat(11000, 16, 1));

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
        public static void writeAudioToOutputWav(MemoryStream stream)
        {


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
