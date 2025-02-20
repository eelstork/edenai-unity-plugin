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
