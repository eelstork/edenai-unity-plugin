using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace EdenAI
{
    public partial class EdenAIApi
    {

        public async Task<ChatResponse> SendChatRequest(string provider, string text, string chatBotGlobalAction = null,
            List<ChatMessage> previousHistory = null, string model = null, int max_tokens = 1000)
        {
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
            ChatResponse[] obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
            return obj[0];
        }
    }
}
