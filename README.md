# TTS-Voice-Wizard-Beta
[![Discord](https://img.shields.io/discord/681732152517591048?label=Discord)](https://discord.gg/YjgR9SWPnW) 
![downloads](https://img.shields.io/github/downloads/VRCWizard/TTS-Voice-Wizard/total?label=Downloads) <br />
Use TTS Voice Wizard's accessibily features to improve your VRChat experience (it works outside of VRChat too!)<br />
* You can convert your Speech to Text and back to Speech through Microsoft Azure Voice Recognition and TTS.<br />
* You can send what you say as OSC messages to VRChat to be displayed on your avatar using KillFrenzyAvatarText!<br />
* You can translate what you say in one language to one of 20+ other support languages! (Have a language you want added? Join the discord linked below and let me know!) <br />
* There are over 50 different voices with various customization options so you can pick a voice that best suits you! <br />

![Screenshot 2022-05-09 121718](https://user-images.githubusercontent.com/101527472/167462899-f954be86-4914-4d23-a38c-9b2b4259cffb.png)


# Getting Started
- Download the latest version from releases and run the .exe file (recommended to use **"latest"** release, not pre-release) <br />
https://github.com/VRCWizard/TTS-Voice-Wizard/releases <br />

- It may ask you to install missing framework for .Net upon running the .exe file.<br />
![unknown3](https://user-images.githubusercontent.com/101527472/161798516-682fba28-e549-40fe-83c3-2f1e0c18fd2f.png)

- Download the x64 version for desktop apps.<br />

![download this one](https://user-images.githubusercontent.com/101527472/161798523-48efb29a-81a6-4ac5-acaf-45a33a857b73.png)

- If popup does not appear and application does not start use this link. https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime?utm_source=getdotnetcore&utm_medium=referral

# Microsoft Azure Subscription Key
- For Speech Recognition and TTS to work you must have an Azure Subscription Key. <br />
https://azure.microsoft.com/en-us/free/ <br />
or <br />
https://azure.microsoft.com/en-us/free/students/ (no credit card required)<br />


- After making your account you will need to create a speech service to get your Key and Region. You will enter this information into the "Provider" tab of the application. <br />
- **Follow this video to get your key and region information:** https://youtube.com/clip/Ugkxe7HlljnV9iwlI7AnAOx6YJSDus7K1GZF <br />


- **I am not responsible for any charges you recieve if you upgrade from a Free Azure Account! It is up to you to monitor your own usage if you are using a pay-as-you-go azure account** <br />
    - **Spending Limits:**: https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/spending-limit  <br />
    - **Avoid Charges:** https://docs.microsoft.com/en-us/azure/cost-management-billing/manage/avoid-charges-free-account <br />
    - **Azure Speech Service Pricing and Free Monthly Limits:**  https://azure.microsoft.com/en-us/pricing/details/cognitive-services/speech-services/ <br />
(This program using Speech to Text Standard, Text to Speech Neural, and Speech Translation Standard)
    - **Monitor Usage:** https://docs.microsoft.com/en-us/answers/questions/643390/how-to-see-text-to-speech-usage.html (the location to see your speech service usage can be hard to find, the post should help!)

- **Don't want to use Microsoft Azure? Can't set up an account? No credit card to make account? No school email?:** Then try out the lite version of the app that uses windows built in TTS and voice recognition engine https://github.com/VRCWizard/TTS-Voice-Wizard-Lite<br />


![Screenshot 2022-05-09 121758wa](https://user-images.githubusercontent.com/101527472/167463062-909190bb-eec0-42f3-a985-7e7c714782bb.png)


# How to output TTS though microphone?
- Upon clicking the TTS button you should be able to hear the TTS in the Sara voice by default (The app must have outputted audio for you to select it in volume mixer)

- Download a virtual audio cable, you can find one here https://vb-audio.com/Cable/


- To be able to hear the TTS while outputing it though the microphone checkmark "listen to this device" for the virtual cable.
Control Panel > Sound > Recording > Select the Virtual Cable Output > Properties > Listen
(known issue, on computer restart listen to this device may not work. To fix this uncheck/apply and then recheck/apply changes.)
![Screenshot 2022-03-15 192241](https://user-images.githubusercontent.com/101527472/158493212-8b1db84b-bf10-45ae-bca4-71c858113bb9.jpg)

- You will then need change the Apps **Output Device**  to the **Virtual Cable** which will allow you to play the TTS though your microphone.
![image](https://user-images.githubusercontent.com/101527472/170846684-cd7d01f7-f2fa-4a05-aea6-ec8653d3a58d.png)

- Your microphone/input device for TTSVoiceWizard should be whatever microphone you normally use!
- In the application you want the audio to play in like VRChat or Discord you will set your microphone to the vitrual cable!
# KillFrenzy Avatar Text
- To have text output on your avatar in vrchat you must install killfrenzy96's Avatar Text Displayer <br />
https://github.com/killfrenzy96/KillFrenzyAvatarText/ <br />
**Download KillFrenzy Avatar Text (KAT) latest release (supports 4, 8, or 16 sync parameters currently)**

# Can I set hotkey on my controllers?
- This feature is not avaliable as apart of this ATM but you can use this handy program! <br />
https://github.com/BOLL7708/OpenVR2Key <br />
- Make sure TTSVoiceWizard is not running then set a button to "ctrl + g" (if app is running it will steal the input)<br />


# Need Help / Have Questions / Wanna make suggestions?
- Join the discord server <br />
https://discord.gg/YjgR9SWPnW <br />

# Donate
- Leave me a Github Star (it's free) or <br />

<a href='https://ko-fi.com/ttsvoicewizard' target='_blank'><img height='35' style='border:0px;height:46px;' src='https://az743702.vo.msecnd.net/cdn/kofi3.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /> <br />



