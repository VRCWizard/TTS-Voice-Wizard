using NAudio.Wave;
using NAudio.Wave.Compression;
using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


                WaveStream pcmStream;

                switch (format)
                {
                    case AudioFormat.Mp3:
                        using (Mp3FileReader mp3Reader = new Mp3FileReader(stream))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
                            WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                            OutputText.outputLog("[Audio File Saved]");
                        }
                        break;

                    case AudioFormat.Raw:
                        using (RawSourceWaveStream rawReader = new RawSourceWaveStream(stream, new WaveFormat(11000, 16, 1)))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(rawReader);
                            WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                            OutputText.outputLog("[Audio File Saved]");
                        }
                        break;

                    case AudioFormat.Wav:
                        using (WaveFileReader wavReader = new WaveFileReader(stream))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(wavReader);
                            WaveFileWriter.CreateWaveFile(fullPath, pcmStream);
                            OutputText.outputLog("[Audio File Saved]");
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid output format specified.");
                }

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
