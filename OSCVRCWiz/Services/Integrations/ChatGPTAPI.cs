
using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using OSCVRCWiz.Services.Text;

namespace OSCVRCWiz.Services.Integrations
{
    public class ChatGPTAPI
    {
        
        private static ChatGpt OfficialBot=null;

        //private static ChatGptUnofficial UnofficialBot=null;
        public static ChatGptConversation chatSession;

       // private static int maxMessages = 16;
        public static int messagesInHistory = 0;

        public static string ChatGPTMode = "";
        public static void OfficialBotSetAPIKey(string key,string model)
        {

            OfficialBot = new ChatGpt(key, new ChatGptOptions
            {
                Model = model,
            });

            chatSession = new ChatGptConversation();

        }


        public static async Task<string> GPTResponse(string input)
        {
            string response = "";
            try
            {
                if (ChatGPTMode == "")
                {
                    string key = VoiceWizardWindow.MainFormGlobal.textBoxChatGPT.Text.ToString();
                    string model = VoiceWizardWindow.MainFormGlobal.textBoxGPTModel.Text.ToString();
                    if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(model))
                    {
                        OutputText.outputLog("[ChatGPT Error: Key or model text field is blank]", Color.Red);
                        return response;
                    }
                    else
                    {

                    
    
                            ChatGPTAPI.ChatGPTMode = "Key";
                            ChatGPTAPI.OfficialBotSetAPIKey(key, model);
                            OutputText.outputLog("[ChatGPT loaded with API key]", Color.Green);

                      
                    }

                }
                if (ChatGPTMode == "Key")
                {
                    if (VoiceWizardWindow.MainFormGlobal.rjToggleGPTUsePrompt.Checked)
                    {
                        VoiceWizardWindow.MainFormGlobal.Invoke((MethodInvoker)delegate ()
                        {
                            input = VoiceWizardWindow.MainFormGlobal.richTextBoxGPTPrompt.Text.ToString().Trim() + ": " + input;
                        });
                    }
                    

                    if (VoiceWizardWindow.MainFormGlobal.rjToggleUseContextWithGPT.Checked)
                    {
                        response = await OfficialBot.Ask(input, chatSession.Id);
                       // OutputText.outputLog("session: "+chatSession.ToString());

                        ChatGptMessage messageUser = new ChatGptMessage();
                        messageUser.Role = "user";
                        messageUser.Content = input;
                        chatSession.Messages.Add(messageUser);


                        ChatGptMessage messageAssistant = new ChatGptMessage();
                        messageAssistant.Role = "assistant";
                        messageAssistant.Content = response;
                        chatSession.Messages.Add(messageAssistant);


                        //OutputText.outputLog(OfficialBot.Conversations.Count().ToString());
                        //  OutputText.outputLog("Conversation ID: "+ chatSession.Id.ToString());
                        //    OutputText.outputLog("Conversation Message Count: "+ chatSession.Messages.Count.ToString());

                        int messageCount = chatSession.Messages.Count;
                        int maxMessages = Int16.Parse(VoiceWizardWindow.MainFormGlobal.textBoxChatGPTMaxHistory.Text);


                        if (maxMessages < 6)
                        {
                            OutputText.outputLog("Max Message Context cannot be less than 6, the context was " + maxMessages, Color.DarkOrange);
                            //OutputText.outputLog("To turn off message context turn off this feature");
                            maxMessages = 6;
                        }
                        if (maxMessages % 2 != 0)
                        {
                            OutputText.outputLog("Message Context cannot be odd the max context was " + maxMessages, Color.DarkOrange);
                            maxMessages += 1;
                        }
                        // OutputText.outputLog("Max message context set to: " + maxMessages);

                        if (messageCount >= maxMessages)
                        {
                            int half = messageCount / 2;
                            if (half % 2 == 0) // Even number of messages
                            {
                                int userIndex = half;
                                int assistantIndex = half + 1;

                                chatSession.Messages.RemoveAt(assistantIndex);
                                chatSession.Messages.RemoveAt(userIndex);
                              //  OutputText.outputLog($"removed user message at {userIndex} and assistant message at {assistantIndex}");
                            }
                            else // Odd number of messages
                            {
                                int assistantIndex = half;
                                int userIndex = half - 1;

                                chatSession.Messages.RemoveAt(assistantIndex); //remove assistant index first always
                                chatSession.Messages.RemoveAt(userIndex);

                               // OutputText.outputLog($"removed user message at {userIndex} and assistant message at {assistantIndex}");

                            }

                            // messagesInHistory = chatSession.Messages.Count;

                        }
                      //  OutputText.outputLog("Conversation Message Count: " + chatSession.Messages.Count.ToString());
                    }
                    else
                    {
                        response = await OfficialBot.Ask(input);
                    }

               }

                
            }
            catch (Exception ex)
            {
                OutputText.outputLog("[ChatGPT Error: " + ex.Message + "]", Color.Red);
            }
            return response;
        }


    }
}
