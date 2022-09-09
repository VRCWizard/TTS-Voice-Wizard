# How to setup Spotify Feature 
-  Go to the Spotify developer dashboard https://developer.spotify.com/dashboard/
-  Create a new app
-  add the correct redirect URI http://localhost:5000/callback
![image](https://user-images.githubusercontent.com/101527472/184249336-b0c075c3-6a71-4b6f-b60b-0bd6ce012af7.png)
![image](https://user-images.githubusercontent.com/101527472/184249358-79ef66c1-890a-46ab-84ea-db3ec70d872f.png)
- save the change
- you can now copy your Client ID into TTS Voice Wizard and click the Connect Spotify Button (The Client ID in this picture will not work, unless I were to add your email to the "users and access" tab)
![image](https://user-images.githubusercontent.com/101527472/184249500-e217f021-1473-4056-8476-d19cb2e16af8.png)
![image](https://user-images.githubusercontent.com/101527472/184249619-0c284fc5-b8cd-41cd-9c15-b5d3889eb442.png)
-  You can then use KAT https://github.com/VRCWizard/TTS-Voice-Wizard#killfrenzy-avatar-text to display the text (shown in the section below) or the new VRChat Chatbox
![image](https://user-images.githubusercontent.com/101527472/184250055-0ce6dbf1-b474-440e-bff6-91c0805059b8.png)
![image](https://user-images.githubusercontent.com/101527472/184250129-65706fdc-ae58-4f32-a4ef-84308c9f4b87.png)

-  Make sure you turned on OSC from the VRChat radial menu!!!!!!!!!!!!!!!!!!! <br />
![EnableOSC (1)](https://user-images.githubusercontent.com/101527472/189431342-18dfecda-df3b-40c0-be66-6ecb56107543.gif) <br />
(Gif taken from Killfrenzy Avatar Text setup guide) <br />

-  Note: If you did all of this and the Spotify text does not appear then you must go to %appdata%..\LocalLow\VRChat and delete the OSC folder <br />
-  (Close VRChat, delete the OSC folder then restart VRChat is the proper order of operations)
![image](https://user-images.githubusercontent.com/101527472/189431265-c3005a90-8f0b-49b9-88b6-3300f4e4a465.png) <br />



-  Example with VRChat Chatbox ![image](https://user-images.githubusercontent.com/101527472/185652165-31caebd6-75fe-4bfb-be86-092b12b7ceea.png)





# How to setup Spotify Feature (Legacy)
-  The Spotify feature is used to output the name of the song you are listening to in game. This feature currently has limited access. To enable this feature you must join the discord server and DM Wizard through discord to request access.
-  Spotify Setting are located in the addon tab. Once you have recieved access click the "Connect Spotify" button to connect your spotify.
- Once that's all working if you havent already you must install KAT on your avatar to see the text in game https://github.com/VRCWizard/TTS-Voice-Wizard#killfrenzy-avatar-text (You can now use VRChats built in Chatbox feature instead)
-  Emoji addon currently only works with Spotify and heartbeat, it replaces some text with a heart or the Spotify symbol for example.
    - How to add emojis: in unity replace the KAT texture with the png file provided in the "KAT Emoji Texture Sheet" folder and enable emojis in the app through the addon tab)
    - Example with Killfrenzy Avatar Text (With Emojis on)
    ![image](https://user-images.githubusercontent.com/101527472/182697581-161c3458-0f75-4ca5-8523-af8f32aab7f3.png)


