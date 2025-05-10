using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ArgEx = System.ArgumentException;
using Ex = System.Exception;

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
        var messageContent = new List<MessageContent>();
        if (!string.IsNullOrWhiteSpace(text)){
            messageContent.Add(new MessageContent{
                Type = "text", Text = text
            });
        }
        if (!string.IsNullOrWhiteSpace(imageUrl)){
            messageContent.Add(new MessageContent{
                Type = "image_url",
                ImageUrl = new ImageUrl { Url = imageUrl }
            });
        }

        if (!string.IsNullOrWhiteSpace(imagePath)){
            var base64Image = ImageUtils.EncodeImageToBase64(imagePath);
            messageContent.Add(new MessageContent{
                Type = "media_base64",
                MediaBase64Content = new MediaBase64
                {
                    Base64 = base64Image,
                    MediaType = "image/png" // Optional: detect dynamically
                }
            });
        }

        if (messageContent.Count == 0){
            throw new ArgEx("At least one of text, imageUrl, or imagePath must be provided.");
        }
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
        var url = "https://api.edenai.run/v2/text/chat";
        var settings = new Dictionary<string, string>();
        if (model != null) provider = provider + "/" + model;
        else settings = null;
        // Create the message with content instead of text
        var message = new ChatMessage
        {
            Role = "user",
            Content = content,
            Message = "Image"
        };
        var messages = previousHistory != null
            ? new List<ChatMessage>(previousHistory) { message }
            : new List<ChatMessage> { message };
        var payload = new ChatRequest(
            provider: provider,
            messages: messages, // The new content for the chat
            //previousHistory: updatedHistory, // Your previous conversation context
            chatbotGlobalAction: chatBotGlobalAction,
            settings: settings,
            maxTokens: max_tokens
        );

        var responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
        if (responseText.Contains("violation")){
            throw new ArgEx(responseText);
        }
        var obj = JsonConvert
            .DeserializeObject<ChatResponse[]>(responseText);
        return obj[0];
    }

} // end-class
} // end-namespace
