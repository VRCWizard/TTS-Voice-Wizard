using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSCVRCWiz.Settings
{
    public class Import_Export
    {
        public static string importFile(string path)
        {
            try
            {
                string contents = "";
                using (FileStream stream = new FileStream(path, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        contents = reader.ReadToEnd();
                    }
                }
                return contents;

            }
            catch (Exception ex)
            {
                OutputText.outputLog("[Text File Reader Error: This error occured while attempting to read the text file: " + ex.Message + "]");
                return "";
            }
        }


        public static void ExportList(string folderPath, string listType, string data)
        {
            string fileName = "TTSVoiceWiz_Export_" + listType + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string filePath = Path.Combine(folderPath, fileName);

            File.WriteAllText(filePath, data);

            //Process.Start("explorer.exe", folderPath);

        }
    }
}
