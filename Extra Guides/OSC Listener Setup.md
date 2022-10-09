 
 # How to connect XSOverlay battery life OSC data 
 -  Option to show battery life of Controllers (not for quest controllers) and tracker battery life. 
      -  MUST HAVE XSOVERLAY to use https://store.steampowered.com/app/1173510/XSOverlay/
      -  Battery Updates appear in the log and can be shown with the Spotify Song Display Integration (can't be output on their own ATM , must be listening to a song)
      -  Spotify Output Text Variables
          -   {averageTrackerBattery}
          -   {leftControllerBattery}
          -   {rightControllerBattery}
          -   {averageControllerBattery}
  -  How to connect XSOverlay battery life data 
  Change the port that XSOverlay sends to from VRChat (9000) to TTS Voice Wizard (4026 or what ever you changed to receive port to)
  Press the Activate OSC Listener Button once
![image](https://user-images.githubusercontent.com/101527472/193479576-d97fdd43-74a9-4286-9c68-ae6ae12c1bf4.png)
![image](https://user-images.githubusercontent.com/101527472/193479591-6b46d2a5-625a-4b37-bfa5-3e0a0ca28d83.png)
![image](https://user-images.githubusercontent.com/101527472/193479900-c21c0ad3-0ce9-4754-a1ce-d25fa139a2b9.png)
-  That's it, now when you have XSOverlay open it will send OSC messages to TTS Voice Wizard. To have the messages appear in-game this feature must be used in conjunction with the Spotify Integration feature.


Example Images
![image](https://user-images.githubusercontent.com/101527472/193480329-0fa27ce3-56de-4d42-aedd-6c4e33affada.png)
Quest controller battery life is not supported with XSOverlay as shown in this image using a Quest 2

![image](https://user-images.githubusercontent.com/101527472/193480697-728af0f1-5a6d-459c-82a1-0f40cc9c08ea.png)


![VRChat_2022-10-02_18-12-00 442_1920x1080](https://user-images.githubusercontent.com/101527472/193480633-65cca716-227b-4248-b7b6-1bfce1ab8755.png)


# How to connect HRtoVRChat_OSC heartrate OSC data 
- https://github.com/200Tigersbloxed/HRtoVRChat_OSC
- only thing you would change in the setup process is for port in HRtoVRChat_OSC it should be changed to TTS Voice Wizard (4026 or what ever you changed to receive port to)
