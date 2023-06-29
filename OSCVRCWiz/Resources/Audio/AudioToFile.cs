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
        public static void WriteAudioToOutput(MemoryStream stream, AudioFormat format)
        {
            try
            {
                WaveStream pcmStream;

                switch (format)
                {
                    case AudioFormat.Mp3:
                        using (Mp3FileReader mp3Reader = new Mp3FileReader(stream))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(mp3Reader);
                            WaveFileWriter.CreateWaveFile("Output\\AudioOutput\\TTSVoiceWizard-output.wav", pcmStream);
                        }
                        break;

                    case AudioFormat.Raw:
                        using (RawSourceWaveStream rawReader = new RawSourceWaveStream(stream, new WaveFormat(11000, 16, 1)))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(rawReader);
                            WaveFileWriter.CreateWaveFile("Output\\AudioOutput\\TTSVoiceWizard-output.wav", pcmStream);
                        }
                        break;

                    case AudioFormat.Wav:
                        using (WaveFileReader wavReader = new WaveFileReader(stream))
                        {
                            pcmStream = WaveFormatConversionStream.CreatePcmStream(wavReader);
                            WaveFileWriter.CreateWaveFile("Output\\AudioOutput\\TTSVoiceWizard-output.wav", pcmStream);
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
