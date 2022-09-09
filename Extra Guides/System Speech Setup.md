# Windows Built-In System Speech (Ignore if using Microsoft Azure Key)
- If you don't want to setup / use the Azure Conginitive Speech Services you can use your windows build in speech to text and text to speech engine. <br />
- Whats the difference you may ask?
    -  Azure can cost money if used too often as explained above in the "How to get your Microsoft Azure Key and Region" section
    -  Azure has better TTS voices and Speech recognition ability as demonstrated in the video above
- Setup: 
    -  First you will need to [download and set up a virtual cable](https://github.com/VRCWizard/TTS-Voice-Wizard/blob/main/Extra%20Guides/Virtual%20Cable%20Setup.md)
    - You will then need to change the Apps **output device**  to the **virtual cable** which will allow you to play the System Speech TTS though your microphone.
    - System Speech will always use default microphone. You can set this through control panel or for this app specifically through window sound settings
        - WINDOWS 10- Settings > Sound > App volume and device preferences<br />
        - WINDOWS 11- Settings > Sound > Volume Mixer<br />
        (find TTS voice wizard and change the input device to your microphone)<br />
   ![image](https://user-images.githubusercontent.com/101527472/188168225-c10efdbe-a163-4957-a485-8e7bf25de4dd.png)
   
# How to get Text in VRChat
-  [Text for VRChat Setup Guide](https://github.com/VRCWizard/TTS-Voice-Wizard/blob/main/Extra%20Guides/Text%20Setup.md)

# Can I set hotkey on my controllers?
- This feature is not avaliable as apart of this ATM but you can use this handy program! <br />
    - [App to bind key combinations to your VR controller](https://github.com/BOLL7708/OpenVR2Key) <br />
- Make sure TTSVoiceWizard is not running then set a button to "ctrl + g" (if app is running it will steal the input)<br />

