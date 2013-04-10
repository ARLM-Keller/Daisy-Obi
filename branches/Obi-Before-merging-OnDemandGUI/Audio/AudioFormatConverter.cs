using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using AudioLib;

namespace Obi.Audio
{
    public class AudioFormatConverter
    {
        public static string[] ConvertFiles(string[] fileName, Presentation presentation) 
        {
            
            int numberOfFiles = fileName.Length;
            string convertedFile = null;
            string[] listOfConvertedFiles = new string[numberOfFiles];
           
            
            for (int i = 0; i < numberOfFiles; i++)
            {
                convertedFile = ConvertedFile(fileName[i], presentation);
                listOfConvertedFiles[i] = convertedFile;
            }
            return listOfConvertedFiles;
        }
        public static string ConvertedFile(string filePath, Presentation pres)
        {
            AudioLib.WavFormatConverter audioConverter = new WavFormatConverter(true);
            int samplingRate = (int)pres.DataManager.getDefaultPCMFormat().getSampleRate();
            int channels = pres.DataManager.getDefaultPCMFormat().getNumberOfChannels();
            int bitDepth = pres.DataManager.getDefaultPCMFormat().getBitDepth();
            string directoryPath = ((urakawa.media.data.FileDataProviderManager)pres.getDataProviderManager()).getDataFileDirectoryFullPath();
            string convertedFile = null;
            try
            {
                if (filePath.EndsWith(".wav"))
                {
                    Stream wavStream = null;
                    wavStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    urakawa.media.data.audio.PCMDataInfo newFilePCMInfo = urakawa.media.data.audio.PCMDataInfo.parseRiffWaveHeader(wavStream);
                    if (wavStream != null) wavStream.Close();
                    if (newFilePCMInfo.getSampleRate() == samplingRate && newFilePCMInfo.getNumberOfChannels() == channels && newFilePCMInfo.getBitDepth() == bitDepth)
                    {
                        convertedFile = filePath;
                    }
                    else
                        convertedFile = audioConverter.ConvertSampleRate(filePath, directoryPath, channels, samplingRate, bitDepth);
                }
                else if (filePath.EndsWith(".mp3"))
                {
                    convertedFile = audioConverter.UnCompressMp3File(filePath, directoryPath, channels, samplingRate, bitDepth);
                }
              
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return convertedFile;
        }

    }
}