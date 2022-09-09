# How to output TTS through microphone?
- Upon clicking the TTS button you should be able to hear the TTS in the Sara voice by default through your speakers

- Download a virtual audio cable, you can find one here (needed to play audio through microphone) https://vb-audio.com/Cable/


- To be able to hear the TTS while outputing it though the microphone checkmark "listen to this device" for the virtual cable.
Control Panel > Sound > Recording > Select the Virtual Cable Output > Properties > Listen
(known issue, on computer restart listen to this device may not work. To fix this uncheck/apply and then recheck/apply changes.)
![Screenshot 2022-03-15 192241](https://user-images.githubusercontent.com/101527472/158493212-8b1db84b-bf10-45ae-bca4-71c858113bb9.jpg)



- Your microphone/input device for TTSVoiceWizard should be whatever microphone you normally use!
- In the application you want the audio to play in like VRChat or Discord you will set your microphone to the vitrual cable!

# Windows Built-In System Speech (Ignore if using Microsoft Azure Key)
- If you don't want to setup / use the Azure Conginitive Speech Services you can use your windows build in speech to text and text to speech engine. <br />
- Whats the difference you may ask?
    -  Azure can cost money if used too often as explained above in the "How to get your Microsoft Azure Key and Region" section
    -  Azure has better TTS voices and Speech recognition ability as demonstrated in the video above
- Setup: 
    - You will then need to change the Apps **output device**  to the **virtual cable** which will allow you to play the System Speech TTS though your microphone.
    - System Speech will always use default microphone. You can set this through control panel or for this app specifically through window sound settings
        - WINDOWS 10- Settings > Sound > App volume and device preferences<br />
        - WINDOWS 11- Settings > Sound > Volume Mixer<br />
        (find TTS voice wizard and change the input device to your microphone)<br />
   ![image](https://user-images.githubusercontent.com/101527472/188168225-c10efdbe-a163-4957-a485-8e7bf25de4dd.png)
   
# How to get Text in VRChat
[Text for VRChat Setup Guide](https://github.com/VRCWizard/TTS-Voice-Wizard/blob/main/Extra%20Guides/Text%20Setup.md)

