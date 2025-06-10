using System; using System.Collections.Generic;
using System.IO; using System.Net.Http;
using System.Threading.Tasks; using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

namespace EdenAI{

    [Serializable]
    public class Key{
        public string api_key { get; set; }
    }

    public partial class EdenAIApi{
        //static bool log_queries = true;
        private string _apiKey;
        private static readonly HttpClient _httpClient = new HttpClient();

        Action<string> log, logw, log_err;

        public EdenAIApi(
            Action<string> log, Action<string> logw, Action<string> log_err, string apiKey = default
        ){
            this.log = log; this.logw = logw; this.log_err = log_err;
            _httpClient.Timeout = TimeSpan.FromSeconds(200);
            if (!string.IsNullOrEmpty(apiKey)){
                this._apiKey = apiKey;
            }else{
                // Try to load from Unity Editor Preferences
                this._apiKey = EditorPrefs.GetString("EdenAI_API_Key", "");

                if (string.IsNullOrEmpty(this._apiKey))
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
                        throw new Exception("API Key is null and auth.json does not exist. Please set the API Key in the Unity Editor Preferences (EdenAI_API_Key) or create an auth.json file.");
                    }
                }
            }
        }

        private void AddHeaders(HttpRequestMessage request){
            if (string.IsNullOrEmpty(this._apiKey)){
                throw new Exception("Missing API Key");
            }
            request.Headers.Add("Authorization", "Bearer " + this._apiKey);
        }

        private async Task<string> SendHttpRequestAsync(
            string url, HttpMethod method, object payload
        ){
            Savings.CheckCost();
            if(Stopper.should_stop){
                throw new Savings("Stopped");
            }
            string jsonPayload = JsonConvert.SerializeObject(payload, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            AddHeaders(request);

            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            request.Content = content;
            string prettyJson = JsonConvert.SerializeObject(payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            log($"CONTENT\n{prettyJson}");

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            string responseText = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode){
                Debug.LogError(responseText);
                response.EnsureSuccessStatusCode();
            }
            var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseText), Formatting.Indented);
            log($"RESPONSE\n{formattedJson}");
            return responseText;

        }

    }
}
