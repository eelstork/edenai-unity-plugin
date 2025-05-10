using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ArgEx = System.ArgumentException;
using Ex = System.Exception;
using Dic = System.Collections.Generic.Dictionary<System.String, System.String>;

namespace EdenAI{
public partial class EdenAIApi{

    const int MaxLength = 128000;

    public async Task<ChatResponse> SendChatRequest(
        string provider,
        string text = null,
        string imageUrl = null,
        string imagePath = null,
        string chatBotGlobalAction = null,
        List<ChatMessage> previousHistory = null,
        string model = null, int max_tokens = 1000
    ){
        UnityEngine.Debug.Log($"History size {previousHistory.Count}");
        var messageContent = new List<MessageContent>();
        if (!string.IsNullOrWhiteSpace(text)){
            messageContent.Add(new MessageContent{
                type = "text",
                content = new Dic{{"text", text}}
            });
        }
        if (!string.IsNullOrWhiteSpace(imageUrl)){
            messageContent.Add(new MessageContent{
                type = "media_url",
                content = new Dic{{"media_url", imageUrl}}
            });
        }
        if (!string.IsNullOrWhiteSpace(imagePath)){
            var base64Image = ImageUtils.EncodeImageToBase64(imagePath);
            messageContent.Add(new MessageContent{
                type = "media_base64",
                content = new Dic{
                    {"media_base64", base64Image},
                    {"media_type", "image/png"},
                }
            });
        }
        if (messageContent.Count == 0 && previousHistory.Count == 0){
            throw new ArgEx("At least one of text, imageUrl, or imagePath must be provided.");
        }
        if(messageContent.Count == 0) messageContent = null;
        return await SendMultiModalChatRequest(
            provider, messageContent, chatBotGlobalAction,
            previousHistory, model, max_tokens
        );
    }

    // Multimodal chat -----------------------------------------

    private async Task<ChatResponse> SendMultiModalChatRequest(
        string provider, List<MessageContent> content,
        string chatBotGlobalAction = null,
        List<ChatMessage> previousHistory = null,
        string model = null,
        int max_tokens = 1000
    ){
        log("Sending multimodal chat request");
        var url = "https://api.edenai.run/v2/multimodal/chat";
        var settings = new Dictionary<string, string>();
        if (model != null) provider = provider + "/" + model;
        else settings = null;
        // Create the message with content instead of text
        List<ChatMessage> messages = previousHistory;
        if(content != null){
            throw new ArgEx("Not supported right now");
            // var message = new ChatMessage
            // {
            //     Role = "user",
            //     Content = content,
            //     Message = "Image"
            // };
            // messages = previousHistory != null
            //     ? new List<ChatMessage>(previousHistory) { message }
            //     : new List<ChatMessage> { message };
        }
        if(messages.Count < 1) throw new System.Exception("NO MESSAGES");
        var payload = new ChatRequest(
            provider: provider,
            messages: messages, // The new content for the chat
            //previousHistory: updatedHistory, // Your previous conversation context
            chatbotGlobalAction: chatBotGlobalAction,
            settings: settings,
            maxTokens: max_tokens
        );
        string prettyPayload = JsonConvert.SerializeObject(payload, Formatting.Indented);
        UnityEngine.Debug.Log(prettyPayload); // Or use a logger
        var responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
        UnityEngine.Debug.Log(responseText);
        try{
            var obj = JsonConvert
                .DeserializeObject<ChatResponse[]>(responseText);
            return obj[0];
        }catch(Exception e){
            if (responseText.Contains("violation")){
                throw new ArgEx(responseText);
            }else{
                throw;
            }
        }
    }

} // end-class
} // end-namespace
