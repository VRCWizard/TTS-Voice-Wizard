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

# Can I set hotkey on my controllers?
- This feature is not avaliable as apart of this ATM but you can use this handy program! <br />
    - [App to bind key combinations to your VR controller](https://github.com/BOLL7708/OpenVR2Key) <br />
- Make sure TTSVoiceWizard is not running then set a button to "ctrl + g" (if app is running it will steal the input)<br />
