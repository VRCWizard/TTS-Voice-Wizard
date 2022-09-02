# TTS-Voice-Wizard
[![Discord](https://img.shields.io/discord/681732152517591048?label=Discord)](https://discord.gg/YjgR9SWPnW) 
![downloads](https://img.shields.io/github/downloads/VRCWizard/TTS-Voice-Wizard/total?label=Downloads) <br />
Use TTS Voice Wizard's accessibility features to improve your VRChat experience (it works outside of VRChat too!)<br />
* You can convert your Speech to Text and back to Speech through Microsoft Azure Voice Recognition and TTS.<br />
* You can send what you say as OSC messages to VRChat to be displayed on your avatar using KillFrenzyAvatarText!<br />
* You can translate what you say in one language to one of 20+ other support languages! (Have a language you want added? Join the discord linked below and let me know!) <br />
* There are over 50 different voices with various customization options so you can pick a voice that best suits you! <br />
* You can even let others know what you are listening to on Spotify by having a songs title, artist and progress appear above you! <br />

![Screenshot 2022-05-09 121718](https://user-images.githubusercontent.com/101527472/167462899-f954be86-4914-4d23-a38c-9b2b4259cffb.png)
# Demonstration Video


[![TTSVoiceWizard an OSC Speech to TTS App for VRChat](https://user-images.githubusercontent.com/101527472/181857099-15efc1ec-863d-4e1c-90aa-a5d8c432ca0b.png)](https://youtu.be/wBRUcx9EWes "TTSVoiceWizard an OSC Speech to TTS App for VRChat")


# Setup Tutorial Video

[![TTS Voice Wizard Setup Tutorial](https://user-images.githubusercontent.com/101527472/188194398-a3fe8745-b582-4319-af38-44363474090a.png)](https://www.youtube.com/watch?v=bGVs2ew08WY "TTS Voice Wizard Setup Tutorial")  <br />
This video just runs through the instructions on this page.

# Getting Started
- Download the latest version from [releases](https://github.com/VRCWizard/TTS-Voice-Wizard/releases) (it will be named something like TTSVoiceWizard-vx.x.x.zip, not the source code), unzip the folder and run the .exe file (recommended to use **"latest"** release, not pre-release) <br />


- It may ask you to install missing framework for .Net upon running the .exe file.<br />
![unknown3](https://user-images.githubusercontent.com/101527472/161798516-682fba28-e549-40fe-83c3-2f1e0c18fd2f.png)

- Download the and run x64 version for desktop apps.<br />

![download this one](https://user-images.githubusercontent.com/101527472/161798523-48efb29a-81a6-4ac5-acaf-45a33a857b73.png)

- If popup does not appear and application does not start use this link. https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime?utm_source=getdotnetcore&utm_medium=referral

# How to get your Microsoft Azure Key and Region
- For Speech Recognition and TTS to work you must have an Azure Subscription Key. <br />
[Free Azure Account](https://azure.microsoft.com/en-us/free/) (more releated info about free monthly limits after the first month avaliable in the [discord server](https://discord.gg/YjgR9SWPnW) #faq channel)<br />
or <br />
[Free Azure Account for students (no credit card required)](https://azure.microsoft.com/en-us/free/students/) (Completely free, have to renew every year)>

- After making your account you will need to create a speech service to get your Key and Region. You will enter this information into the "Provider" tab of the application. <br />
- **Follow this video to get your key and region information:**<br />
[![How to get your Key and Region](https://user-images.githubusercontent.com/101527472/181855148-a90bdabb-f997-4e5e-b574-d32b6bbe3035.png)](https://youtube.com/clip/Ugkxe7HlljnV9iwlI7AnAOx6YJSDus7K1GZF "How to get your Key and Region")

- **I am not responsible for any charges you recieve if you upgrade from a Free Azure Account! It is up to you to monitor your own usage if you are using a pay-as-you-go azure account** <br />
    - [Spending Limits](https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/spending-limit)  <br />
    - [Avoid Charges](https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/avoid-charges-free-account) <br />
    - [Azure Speech Service Pricing and Free Monthly Limits](https://azure.microsoft.com/en-us/pricing/details/cognitive-services/speech-services/) <br />
(This program using Speech to Text Standard, Text to Speech Neural, and Speech Translation Standard)
    - [Monitor Usage](https://docs.microsoft.com/en-us/answers/questions/643390/how-to-see-text-to-speech-usage.html) (the location to see your speech service usage can be hard to find, this post should help!)
    - [Budgets and Alerts](https://docs.microsoft.com/en-us/azure/cost-management-billing/costs/cost-mgt-alerts-monitor-usage-spending?WT.mc_id=Portal-Microsoft_Azure_CostManagement)

- **Don't want to use Microsoft Azure? Can't set up an account? No credit card to make account? No school email?:** 
    -  Refer to the "Windows Built-In System Speech" section

-  Your key and region go in the "Microsoft Azure Cognitive Service" tab located in "Settings"
    -  Make sure to click the change button for both key and region
![image](https://user-images.githubusercontent.com/101527472/188167519-2861d865-6e76-41d9-b6c2-965993b553e7.png)


# How to output TTS through microphone?
- Upon clicking the TTS button you should be able to hear the TTS in the Sara voice by default through your speakers

- Download a virtual audio cable, you can find one here (needed to play audio through microphone) https://vb-audio.com/Cable/


- To be able to hear the TTS while outputing it though the microphone checkmark "listen to this device" for the virtual cable.
Control Panel > Sound > Recording > Select the Virtual Cable Output > Properties > Listen
(known issue, on computer restart listen to this device may not work. To fix this uncheck/apply and then recheck/apply changes.)
![Screenshot 2022-03-15 192241](https://user-images.githubusercontent.com/101527472/158493212-8b1db84b-bf10-45ae-bca4-71c858113bb9.jpg)

- **This step is for Azure, for System speech refer to this** https://github.com/VRCWizard/TTS-Voice-Wizard/blob/main/README.md#windows-built-in-system-speech-ignore-if-using--microsoft-azure-key
- You will then need change the Apps **Output Device**  to the **Virtual Cable** which will allow you to play the TTS though your microphone. 
![image](https://user-images.githubusercontent.com/101527472/188167891-e19cbb7f-fa17-4bc9-8934-f23f9f91ca5e.png)

- Your microphone/input device for TTSVoiceWizard should be whatever microphone you normally use!
- In the application you want the audio to play in like VRChat or Discord you will set your microphone to the vitrual cable!

# Windows Built-In System Speech (Ignore if using  Microsoft Azure Key)
- **If using a microsoft azure key you should ignore these steps**
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
- You can now use the new VRChat Chatboxes by default <br />
    -  Make sure to turn on OSC from the VRChat radial menu <br />
    -  Currently you must opt into vrchats beta to use this feature <br />
    
    

- Another method which has text output on your avatar in VRChat is [Killfrenzy96's Avatar Text Displayer](https://github.com/killfrenzy96/KillFrenzyAvatarText/) <br />
**Download KillFrenzy Avatar Text (KAT) latest release (supports 4, 8, or 16 sync parameters currently)**
    - Make sure to turn on "Send Text to VRChat with KAT" in TTSVoiceWizard Settings > Text Output 
    - If it isn't working after uploading your avatar, you may have to delete your %appdata%..\LocalLow\VRChat\OSC files when reuploading an avatar with new parameters
    - If you would like to add more functionaility to Killfrenzy Avatar Text. Frosty's Billboard is a KillFrenzy Avatar Text addon, it adds more functionality to the KAT such as making KAT grabbable and poseable. Download it from the "asset-showcase" channel in the [discord server](https://discord.gg/YjgR9SWPnW)
    ![VRChat_1920x1080_2022-08-20_16-26-38 130](https://user-images.githubusercontent.com/101527472/185766796-7ff16a81-a00b-42f2-8340-29e85e1387fe.png)


# Can I set hotkey on my controllers?
- This feature is not avaliable as apart of this ATM but you can use this handy program! <br />
    - [App to bind key combinations to your VR controller](https://github.com/BOLL7708/OpenVR2Key) <br />
- Make sure TTSVoiceWizard is not running then set a button to "ctrl + g" (if app is running it will steal the input)<br />

# How to enable Spotify integration
[Spotify integration setup guide](https://github.com/VRCWizard/TTS-Voice-Wizard/blob/main/Extra%20Guides/Spotify%20Setup.md)

# Need Help / Have Questions / Wanna make suggestions?
[Join the discord server](https://discord.gg/YjgR9SWPnW) <br />

# Socials
[Follow me on Twitter](https://twitter.com/Wizard_VR) <br />
[Subscribe to my Youtube](https://www.youtube.com/channel/UC5e7eigqyhxL6JaS6U4pGvg) <br />


# Donate
- Leave me a Github Star (it's free) or <br />

<a href='https://ko-fi.com/ttsvoicewizard' target='_blank'><img height='35' style='border:0px;height:46px;' src='https://az743702.vo.msecnd.net/cdn/kofi3.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /> <br />



