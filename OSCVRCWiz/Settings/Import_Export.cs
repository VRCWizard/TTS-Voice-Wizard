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

                string basePath = AppDomain.CurrentDomain.BaseDirectory;

                string relativePath = path;

                string absPath = Path.Combine(basePath, relativePath);

                using (FileStream stream = new FileStream(absPath, System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string absFolderPath = Path.Combine(basePath, folderPath);


            string fileName = "TTSVoiceWiz_Export_" + listType + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string filePath = Path.Combine(absFolderPath, fileName);

            File.WriteAllText(filePath, data);

            //Process.Start("explorer.exe", folderPath);

        }
    }
}
