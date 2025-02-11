using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

namespace EdenAI
{
    [Serializable]
    public class Key
    {
        public string api_key { get; set; }
    }

    public class EdenAIApi
    {
        private string _apiKey;
        private static readonly HttpClient _httpClient = new HttpClient();

        public EdenAIApi(string apiKey = default)
        {
            if (!string.IsNullOrEmpty(apiKey))
            {
                this._apiKey = apiKey;
            }
            else
            {
                var userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var authPath = $"{userPath}/.edenai/auth.json";
                if (File.Exists(authPath))
                {
                    var json = File.ReadAllText(authPath);
                    Key auth = JsonConvert.DeserializeObject<Key>(json);
                    this._apiKey = auth.api_key;
                }
                else
                {
                    throw new Exception("API Key is null and auth.json does not exist.");
                }
            }
        }

        private void AddHeaders(HttpRequestMessage request)
        {
            if (string.IsNullOrEmpty(this._apiKey))
            {
                throw new Exception("Missing API Key");
            }
            request.Headers.Add("Authorization", "Bearer " + this._apiKey);
        }

        private async Task<string> SendHttpRequestAsync(string url, HttpMethod method, object payload)
        {
            string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            AddHeaders(request);

            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = content;

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError(responseText);
                throw new Exception(responseText);
            }
            return responseText;
        }

        public async Task<TextToSpeechResponse> SendTextToSpeechRequest(string provider, string text, string audioFormat,
            TextToSpeechOption option, string language, int? rate = null, int? pitch = null, int? volume = null, string voiceModel = null)
        {
            string url = "https://api.edenai.run/v2/audio/text_to_speech";
            var settings = voiceModel != null ? new Dictionary<string, string> { { provider, voiceModel } } : null;
            var payload = new TextToSpeechRequest(provider, text, audioFormat, option, language, rate, pitch, volume, settings);

            string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
            TextToSpeechResponseJson[] response = JsonConvert.DeserializeObject<TextToSpeechResponseJson[]>(responseText);

            return new TextToSpeechResponse
            {
                status = response[0].status,
                provider = response[0].provider,
                cost = response[0].cost,
                voice_type = response[0].voice_type,
                audio_base64 = response[0].audio
            };
        }

        public async Task<ChatResponse> SendChatRequest(string provider, string text, string chatBotGlobalAction = null,
            List<ChatMessage> previousHistory = null, string model = null)
        {
            string url = "https://api.edenai.run/v2/text/chat";
            var settings = model != null ? new Dictionary<string, string> { { provider, model } } : null;
            var payload = new ChatRequest(provider, text, chatBotGlobalAction, previousHistory, settings);

            string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
            ChatResponse[] obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
            return obj[0];
        }

        public async Task<YodaResponse> SendYodaRequest(string projectID, string query, List<Dictionary<string, string>> history = null,
            int? k = null, string llmModel = null, string llmProvider = null)
        {
            string url = $"https://api.edenai.run/v2/aiproducts/askyoda/{projectID}/ask_llm";
            var payload = new YodaRequest(query, history, k, llmModel, llmProvider);

            string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
            return JsonConvert.DeserializeObject<YodaResponse>(responseText);
        }
    }
}
