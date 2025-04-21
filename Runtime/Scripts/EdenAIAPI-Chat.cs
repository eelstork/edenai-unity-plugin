using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using ArgEx = System.ArgumentException;
using Ex = System.Exception;

namespace EdenAI
{
public partial class EdenAIApi
{

    const int MaxLength = 128000;

    public async Task<ChatResponse> SendChatRequest(
        string provider,
        string text,
        string chatBotGlobalAction = null,
        List<ChatMessage> previousHistory = null,
        string model = null,
        int max_tokens = 1000
    ){
        // Existing duplicate check
        while (previousHistory?.Count > 0 && previousHistory[^1].Message == text){
            Debug.Log("Remove 1 duplicate message");
            var n = previousHistory.Count;
            previousHistory.RemoveAt(n - 1);
        }
        if(previousHistory?.Count > 0){
            Debug.Log($"INCOMING {text}");
            Debug.Log($"LAST IN  {previousHistory[^1].Message} ");
        }
        var len = text.Length;
        if (len > MaxLength)
        {
            Debug.LogWarning($"{len} exceeds max chars in chat request (max: {MaxLength})... {text}");
            text = text.Substring(0, MaxLength);
        }

        // Create the message (using Message for backward compatibility)
        var message = new ChatMessage{ Role = "user", Message = text };

        // Add to history if provided
        //var updatedHistory = previousHistory != null ?
        //new List<ChatMessage>(previousHistory) { message } :
        //new List<ChatMessage> { message };

    string url = "https://api.edenai.run/v2/text/chat";
    var settings = new Dictionary<string, string>();
    if (model != null) provider = provider + "/" + model;
    else settings = null;

    var payload = new ChatRequest(
        provider: provider,
        text: text,
        chatBotGlobalAction: chatBotGlobalAction,
        previousHistory: previousHistory,
        max_tokens: max_tokens,
        settings: settings
    );

    string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);

    if (responseText.Contains("violation"))
    {
        throw new ArgEx(responseText);
    }

    var obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
    return obj[0];
}

// HELPERS ------------------------------------------------

    public async Task<ChatResponse> SendChatWithImageRequest(
    string provider,
    string text,
    string imageUrl,
    string chatBotGlobalAction = null,
    List<ChatMessage> previousHistory = null,
    string model = null,
    int max_tokens = 1000)
{
    var messageContent = new List<MessageContent>
    {
        new MessageContent { Type = "text", Text = text },
        new MessageContent { Type = "image_url", ImageUrl = new ImageUrl { Url = imageUrl } }
    };

    return await SendMultiModalChatRequest(
        provider,
        messageContent,
        chatBotGlobalAction,
        previousHistory,
        model,
        max_tokens
    );
}

private async Task<ChatResponse> SendMultiModalChatRequest(
    string provider,
    List<MessageContent> content,
    string chatBotGlobalAction = null,
    List<ChatMessage> previousHistory = null,
    string model = null,
    int max_tokens = 1000)
{
    Debug.Log("Sending multimodal chat request");
    // Handle duplicate message check
    if (previousHistory?.Count > 0)
    {
        var lastMessage = previousHistory[previousHistory.Count - 1];
        if (lastMessage.Content != null && content != null &&
            lastMessage.Content.Count == content.Count)
        {
            bool isDuplicate = true;
            for (int i = 0; i < content.Count; i++)
            {
                if (lastMessage.Content[i].Type != content[i].Type ||
                    (content[i].Type == "text" && lastMessage.Content[i].Text != content[i].Text) ||
                    (content[i].Type == "image_url" && lastMessage.Content[i].ImageUrl.Url != content[i].ImageUrl.Url))
                {
                    isDuplicate = false;
                    break;
                }
            }
            if (isDuplicate)
            {
                previousHistory.RemoveAt(previousHistory.Count - 1);
            }
        }
    }

    string url = "https://api.edenai.run/v2/text/chat";
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

    // Add to history if provided
    // var updatedHistory = previousHistory != null ?
    //     new List<ChatMessage>(previousHistory) { message } :
    //     new List<ChatMessage> { message };

    var payload = new ChatRequest(
        provider: provider,
        text: null, // Will be ignored when content is present
        chatBotGlobalAction: chatBotGlobalAction,
        previousHistory: previousHistory,
        max_tokens: max_tokens,
        settings: settings
    );

    string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);

    if (responseText.Contains("violation"))
    {
        throw new ArgEx(responseText);
    }

    var obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
    return obj[0];
}

} // end-class

} // end-namespace
