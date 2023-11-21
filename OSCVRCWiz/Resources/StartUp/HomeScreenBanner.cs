using OSCVRCWiz.Services.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OSCVRCWiz.Resources.StartUp.StartUp
{
    public class HomeScreenBanner
    {
        public static System.Threading.Timer rotationTimer;

        static int currentIndex = 0;

        static List<string> imageUrls = new List<string>();
        static List<string> websiteLinks = new List<string>();

        public static string websiteLink;

        private static object lockObject = new object();

        private static async Task LoadDataFromJsonFile(string jsonFileUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
               
                    HttpResponseMessage response = await httpClient.GetAsync(jsonFileUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonData = await response.Content.ReadAsStringAsync();

                        var data = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonData);

                        foreach (var entry in data)
                        {
                            if (entry.ContainsKey("image_url") && entry.ContainsKey("website_link"))
                            {
                                imageUrls.Add(entry["image_url"]);
                                websiteLinks.Add(entry["website_link"]);
                            }
                        }
                    }
                    else
                    {
                        OutputText.outputLog("Failed to retrieve banner JSON data. HTTP Status Code: " + response.StatusCode);
                    }
               
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog("An error occurred loading banner: " + ex.Message);
            }

        }

        public static async void initiateTimer()
        {
            try
            {
                if (VoiceWizardWindow.MainFormGlobal.rjToggleBannerOff.Checked == false)
                {

                    await LoadDataFromJsonFile("https://vrcwizard.github.io/TTSVWBanners/Banners.json");

                    try
                    {
                            Task.Run(() => VoiceWizardWindow.MainFormGlobal.pictureBox5.Load(imageUrls[currentIndex]));
                            websiteLink = websiteLinks[currentIndex];
                        
                    }
                    catch (Exception ex)
                    {
                        OutputText.outputLog($"Error Loading First Image {currentIndex}: {ex.Message}",Color.Red);
                    }



                    rotationTimer = new System.Threading.Timer(rotationtimertick);
                    rotationTimer.Change(15000, 0);
                }
            }
            catch (Exception ex)
            {
                OutputText.outputLog($"Home Banner Error: {ex.Message}", Color.Red);
            }
        }

        public static void stopTimer()
        {
            try
            {
                rotationTimer.Change(Timeout.Infinite, Timeout.Infinite);
                VoiceWizardWindow.MainFormGlobal.pictureBox5.Hide();
            }
            catch (Exception ex)
            {
                OutputText.outputLog($"Error Stopping Image Rotation: {ex.Message}", Color.Red);
            }
        }

        public static void rotationtimertick(object sender)
        {

            Thread t = new Thread(doRotationTimerTick);
            t.Start();
        }

        private static void doRotationTimerTick()
        {
           
            currentIndex++;

            if (currentIndex >= imageUrls.Count)
            {
                currentIndex = 0; // Start over if all images have been displayed
            }

            if (currentIndex < imageUrls.Count)
            {
                try
                {
                    if (VoiceWizardWindow.MainFormGlobal.pictureBox5.Visible == true)
                    {
                        // Update the PictureBox with the next image

                        Task.Run(() => VoiceWizardWindow.MainFormGlobal.pictureBox5.Load(imageUrls[currentIndex]));
                        websiteLink = websiteLinks[currentIndex];

                    }
                }

                catch (Exception ex)
                {
                    OutputText.outputLog($"Error Loading Image {currentIndex}: {ex.Message}", Color.Red);
                }

                // Update the web browser control with the corresponding link


            }
            rotationTimer.Change(15000, 0);
        }

        public static void previousBanner()
        {
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = imageUrls.Count-1; // Start over if all images have been displayed
            }
            if (currentIndex < imageUrls.Count)
            {
                // Update the PictureBox with the next image
                try
                {
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.pictureBox5.Load(imageUrls[currentIndex]));
                    websiteLink = websiteLinks[currentIndex];
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"Error Loading Previous Image {currentIndex}: {ex.Message}", Color.Red);
                }

                // Update the web browser control with the corresponding link
             
            }
            rotationTimer.Change(15000, 0);


        }
        public static void nextBanner()
        {
            currentIndex++;
            if (currentIndex >= imageUrls.Count)
            {
                currentIndex = 0; // Start over if all images have been displayed
            }

            if (currentIndex < imageUrls.Count)
            {
                // Update the PictureBox with the next image
                try
                {
                    Task.Run(() => VoiceWizardWindow.MainFormGlobal.pictureBox5.Load(imageUrls[currentIndex]));
                    websiteLink = websiteLinks[currentIndex];
                }
                catch (Exception ex)
                {
                    OutputText.outputLog($"Error Loading Next Image {currentIndex}: {ex.Message}");
                }

                // Update the web browser control with the corresponding link
            }
            rotationTimer.Change(15000, 0);

        }
    }
}
