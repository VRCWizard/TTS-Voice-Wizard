using AutoUpdaterDotNET;
using Octokit;
using OSCVRCWiz.Services.Text;
using System.Diagnostics;

namespace OSCVRCWiz.Resources.StartUp
{
    public class Updater
    {

        public static string currentVersion = "1.7.7";
        public static string updateXMLName = "https://github.com/VRCWizard/TTS-Voice-Wizard/releases/latest/download/AutoUpdater-x64.xml";

        public static async void getGithubInfo()
        {
            try
            {

                var githubClient = new GitHubClient(new ProductHeaderValue("TTS-Voice-Wizard"));


                var release = githubClient.Repository.Release.GetLatest("VRCWizard", "TTS-Voice-Wizard").Result;

               /* var releases = githubClient.Repository.Release.GetAll("VRCWizard", "TTS-Voice-Wizard"); //can be used for grabbing pre-releases
                var latest = (await releases)[0];
                Debug.WriteLine("The latest release is tagged at {0} and is named {1}",latest.TagName, latest.Name); */

                //   System.Diagnostics.Debug.WriteLine(release.TagName.ToString());
                string releaseText = release.TagName.ToString();
                releaseText = releaseText.Replace("v", "");
                Version latestGitHubVersion = new Version(releaseText);
                System.Diagnostics.Debug.WriteLine(releaseText);

                Version localVersion = new Version(currentVersion);

                int versionComparison = localVersion.CompareTo(latestGitHubVersion);
                // var ot = new OutputText();
                if (versionComparison < 0)
                {
                    //The version on GitHub is more up to date than this local release.
                    OutputText.outputLog("[The version on GitHub (" + releaseText + ") is more up to date than the current version (" + currentVersion + "). Click the yellow update button to auto update or grab the new release from the Github https://github.com/VRCWizard/TTS-Voice-Wizard/releases ]");
                    VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                    {
                        VoiceWizardWindow.MainFormGlobal.navbarUpdates.Visible = true;
                    });
                }
                else if (versionComparison > 0)
                {
                    //This local version is greater than the release version on GitHub.
                    OutputText.outputLog("[The current version (" + currentVersion + ") is greater than the release version on GitHub (" + releaseText + "). You are on a pre-release/development build]");
                }
                else
                {
                    //This local Version and the Version on GitHub are equal.
                    OutputText.outputLog("[The current version (" + currentVersion + ") and the version on GitHub (" + releaseText + ") are equal. Your program is up to date]");
                }
                VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                {
                    VoiceWizardWindow.MainFormGlobal.versionLabel.Text = "v" + currentVersion;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with Github info: " + ex.Message + ". Check your Internet Connection.");
            }



        }
        public static void UpdateButtonClicked()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativePath = @"updates";

            string fullPath = Path.Combine(basePath, relativePath);


            AutoUpdater.Start(updateXMLName);
            AutoUpdater.InstalledVersion = new Version(currentVersion);
            AutoUpdater.DownloadPath = fullPath;
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.ShowRemindLaterButton = false;
        }
    }
}
