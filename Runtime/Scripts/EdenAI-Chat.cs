using System.Collections;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Dic = System.Collections.Generic.Dictionary<System.String, System.String>;

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

        override public string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);

    }

    [Serializable]
    public class ChatMessage
    {
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }
        [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageContent> Content { get; set; }

        override public string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);

    }

    [Serializable]
    public class MessageContent
    {
        public string type { get; set; }  // "text" or "image_url"
        // NOTE - for LLM endpoint
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> content { get; set; }

        // public static MessageContent FromText(string text){
        //     return new MessageContent{
        //         type = "text",
        //         text = "the quick brown fox"
        //         //text = text
        //         //content = new Dic{{"text", text}}
        //     };
        // }

        public static MessageContent OldStyle(string text){
            return new MessageContent{
                type = "text",
                text = text
            };
        }

        public static MessageContent FromText(string text){
            return new MessageContent{
                type = "text",
                //content = new Dic{{"text", "The quick brown fox"}}
                content = new Dic{{"text", text}}
            };
        }

        // TODO - if the url does not exist... service may get stuck
        public static MessageContent FromImageUrl(string imageUrl){
            return new MessageContent{
                type = "media_url",
                content = new Dic{
                    {"media_url", imageUrl},
                    {"media_type", "image/jpeg"}
                }
            };
        }

        public static MessageContent FromImagePath(string imagePath){
            // NOTE - afaik this happens on reload...
            // Normally the upstream client message should catch this
            if(!System.IO.File.Exists(imagePath)){
                UnityEngine.Debug.LogWarning($"[EdenAI-Chat] trying to encode {imagePath}, however the image was not found");
                return FromText(imagePath + " (file no longer exists)");
            }
            var base64Image = ImageUtils.EncodeImageToBase64(imagePath);
            return new MessageContent{
                type = "media_base64",
                content = new Dic{
                    {"media_base64", base64Image},
                    {"media_type", "image/png"},
                }
            };
        }

    }

    // public class MediaBase64
    // {
    //     [JsonProperty("media_base64")]
    //     public string Base64 { get; set; }
    //
    //     [JsonProperty("media_type")]
    //     public string MediaType { get; set; } // "image/png", "image/jpeg"
    // }

    // [Serializable]
    // public class ImageUrl
    // {
    //     [JsonProperty(PropertyName = "url")]
    //     public string Url { get; set; }
    // }

    [Serializable]
    public class ChatRequest
    {
        [JsonProperty("providers")]
        public string Providers { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("response_as_dict")]
        public bool ResponseAsDict = false;

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
            string model = null,
            Dictionary<string, string> settings = null,
            int maxTokens = 1000
        ){
            Providers = provider;
            Model = model;
            Messages = messages;
            //PreviousHistory = previousHistory; // Assign the previous history
            ChatbotGlobalAction = chatbotGlobalAction;
            Settings = settings;
            MaxTokens = maxTokens;
        }
    }

}
