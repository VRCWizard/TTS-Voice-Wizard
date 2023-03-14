using OSCVRCWiz.Text;

namespace OSCVRCWiz
{
    internal static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            try
            {
            ApplicationConfiguration.Initialize();
             }
            catch (Exception ex)
            {
                var errorMsg = ex.Message + "\n" + ex.TargetSite + "\n\nStack Trace:\n" + ex.StackTrace;
                try
                {
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;

                }
                catch { }

                try
                {
                    
                        File.WriteAllTextAsync(@"CrashDump\CrashReport"+ DateTime.Now.ToString("-MM-dd_h-mm-ss") + ".txt", errorMsg);
                    
                }
                catch (Exception exx)
                {
                    MessageBox.Show("[Error Writing to Crash File: " + exx.Message + ". Crash file could not be accessed try moving tts voice wizard folder to a different location.]");
                }
                MessageBox.Show("A configuration initalization error occurred. Please join the discord and put a copy of the latest CrashReport.txt (located in the CrashDump folder) in #tts-voice-wizard-bugs with a explaination of what you were doing when it crashed");

            }

            try
            {
                Application.Run(new VoiceWizardWindow());
            }
            catch (Exception ex)
            {
               var errorMsg =  ex.Message + "\n"+ ex.TargetSite+ "\n\nStack Trace:\n" + ex.StackTrace;

                try 
                { 
                    errorMsg += "\n\n" + ex.InnerException.Message + "\n" + ex.InnerException.TargetSite + "\n\nStack Trace:\n" + ex.InnerException.StackTrace;
                
                }
                catch { }
               

               try
                {

                    File.WriteAllTextAsync(@"CrashDump\CrashReport" + DateTime.Now.ToString("-MM-dd_h-mm-ss") + ".txt", errorMsg);

                }
                catch (Exception exx)
                {
                    MessageBox.Show("[Error Writing to Crash File: " + exx.Message + ". Crash file could not be created try moving tts voice wizard folder to a different location.]");
                }
                MessageBox.Show("A application error occurred. Please join the discord and put a copy of the latest CrashReport.txt (located in the CrashDump folder) in #tts-voice-wizard-bugs with a explaination of what you were doing when it crashed");

                

            } 

          
               

        }
    }
}