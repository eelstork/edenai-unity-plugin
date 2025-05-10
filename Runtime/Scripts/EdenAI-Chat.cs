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

        [JsonProperty(PropertyName = "media_type", NullValueHandling = NullValueHandling.Ignore)]
        public ImageUrl MediaType { get; set; }

        [JsonProperty("media_base64", NullValueHandling = NullValueHandling.Ignore)]
        public string MediaBase64 { get; set; }

        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public MediaBase64 MediaBase64Content { get; set; }

        // -------------------------------------------------------

        public static MessageContent FromText(string arg)
        => new MessageContent{ Type = "text", Text = arg };

        public static MessageContent FromImageUrl(string arg)
        => new MessageContent{
            Type = "image_url",
            ImageUrl = new ImageUrl { Url = arg }
        };

        public static MessageContent FromImagePath(string arg){
            var base64Image = ImageUtils.EncodeImageToBase64(arg);
            return new MessageContent{
                Type = "media_base64",
                MediaBase64Content = new MediaBase64{
                    Base64 = base64Image, MediaType = "image/png"
                }
            };
        }

    }

    public class MediaBase64
    {
        [JsonProperty("media_base64")]
        public string Base64 { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; } // "image/png", "image/jpeg"
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
        [JsonProperty("providers")]
        public string Providers { get; set; }

        [JsonProperty("response_as_dict")]
        public bool ResponseAsDict = true;

        [JsonProperty("show_original_response")]
        public bool ShowOriginalResponse = false;

        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonProperty("messages")]
        public List<ChatMessage> Messages { get; set; }

        // Add previousHistory as an optional field
        //[JsonProperty("previous_history", NullValueHandling = NullValueHandling.Ignore)]
        //public List<ChatMessage> PreviousHistory { get; set; }

        [JsonProperty("chatbot_global_action", NullValueHandling = NullValueHandling.Ignore)]
        public string ChatbotGlobalAction { get; set; }

        [JsonProperty("settings", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Settings { get; set; }

        public ChatRequest(
            string provider,
            List<ChatMessage> messages,
            //List<ChatMessage> previousHistory = null,
            string chatbotGlobalAction = null,
            Dictionary<string, string> settings = null,
            int maxTokens = 1000
        ){
            Providers = provider;
            Messages = messages;
            //PreviousHistory = previousHistory; // Assign the previous history
            ChatbotGlobalAction = chatbotGlobalAction;
            Settings = settings;
            MaxTokens = maxTokens;
        }
    }

}
