using NAudio.Wave;
using NAudio.Wave.Compression;
using OSCVRCWiz.Services.Text;
using Swan.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources.Audio
{
    public enum AudioFormat
    {
        Mp3,
        Raw,
        Wav
    }

    public class AudioToFile
    {
        public static void WriteAudioToOutput(MemoryStream stream, AudioFormat format,bool uniqueNames)
        {
            try
            {
                DateTime timestamp = DateTime.Now;
                string timestampString = "";
                if (uniqueNames)
                {
                    timestampString = timestamp.ToString("-yyyyMMdd_HHmmss");
                }

                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string relativePath = "Output\\AudioOutput\\TTSVoiceWizard" + timestampString + ".wav";
                string fullPath = Path.Combine(basePath, relativePath);


                

                switch (format)
                {
                    case AudioFormat.Mp3:
                        using (Mp3FileReader mp3Reader = new Mp3FileReader(stream))
                        {
                            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader))
                            {
                                WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                                OutputText.outputLog("[Audio File Saved]");
                            }
                        }
                        break;

                    case AudioFormat.Raw:
                        using (RawSourceWaveStream rawReader = new RawSourceWaveStream(stream, new WaveFormat(11000, 16, 1))) 
                        {

                            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(rawReader))
                            {
                                WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                                OutputText.outputLog("[Audio File Saved]");
                            }
                        }
                        break;

                    case AudioFormat.Wav:
                        using (WaveFileReader wavReader = new WaveFileReader(stream))
                        {
                            using (var mediaStream = new StreamMediaFoundationReader(stream))//this fixes issue with new API conversion formats
                            {
                                WaveFileWriter.CreateWaveFile(fullPath, mediaStream);
                                OutputText.outputLog("[Audio File Saved]");
                            }
                        }
                        /*using (WaveFileReader wavReader = new WaveFileReader(stream))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(wavReader);
                            WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                            OutputText.outputLog("[Audio File Saved]");
                        }*/
                        break;

                    default:
                        throw new ArgumentException("Invalid output format specified.");
                }
                stream.Dispose();
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Output to Wave Error: " + ex.Message + ex.StackTrace + "]", Color.Red);
            }
        }
    }
}
