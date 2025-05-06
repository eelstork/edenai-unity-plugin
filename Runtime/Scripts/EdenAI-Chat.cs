using System.Collections;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EdenAI
{
    [Serializable]
    public class ChatResponse
    {
        public string status { get; set; }
        public string provider { get; set; }
        public string model { get; set; }
        public string generated_text { get; set; }
        public List<ChatMessage> message { get; set; }
        public double cost { get; set; }
    }

    [Serializable]
    public class ChatMessage
    {
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        // For backward compatibility, we'll keep Message but make it optional
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        // New content property that can handle multiple content types
        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageContent> Content { get; set; }
    }

    [Serializable]
    public class MessageContent
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }  // "text" or "image_url"

        [JsonProperty(PropertyName = "text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "image_url", NullValueHandling = NullValueHandling.Ignore)]
        public ImageUrl ImageUrl { get; set; }
    }

    [Serializable]
    public class ImageUrl
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
    
    [Serializable]
    public class ChatRequest
    {
        public string providers { get; set; }
        public bool response_as_dict = false;
        public bool show_original_response = false;
        public int max_tokens { get; set; }
        public string text { get; set; }
        public List<ChatMessage> previous_history { get; set; }
        public string ChatBotGlobalAction { get; set; }
        public Dictionary<string, string> settings { get; set; }

        public ChatRequest(string provider, string text, string chatBotGlobalAction = null,
            List<ChatMessage> previousHistory = null, Dictionary<string,string> settings = null,
            int max_tokens = 1000)
        {
            this.max_tokens = max_tokens;
            this.providers = provider;
            this.text = text;
            this.previous_history = previousHistory;
            this.ChatBotGlobalAction = chatBotGlobalAction;
            this.settings = settings;
        }
    }
    }
