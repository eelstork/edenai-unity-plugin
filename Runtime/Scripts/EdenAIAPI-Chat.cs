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

        const int MaxLength = 100000;

        public async Task<ChatResponse> SendChatRequest(string provider, string text, string chatBotGlobalAction = null,
            List<ChatMessage> previousHistory = null, string model = null, int max_tokens = 1000)
        {
            if(previousHistory.Count > 0){
                var n = previousHistory.Count;
                if(previousHistory[n-1].Message == text){
                    //ebug.LogWarning($"Remove dupe at end: {text}");
                    previousHistory.RemoveAt(n - 1);
                }
            }
            var len = text.Length;
            if(len > MaxLength){
                Debug.LogWarning($"{len} exceeds max chars in chat request (max: {MaxLength})... {text}");
                //throw new ArgEx($"{len} exceeds max chars in chat request (max: {MaxLength})... {text}");
                text = text.Substring(0, 100000);
            }
            string url = "https://api.edenai.run/v2/text/chat";
            var settings = new Dictionary<string, string>();
            if (model != null) provider = provider + "/" + model;
            else settings = null;
            //
            var payload = new ChatRequest(
                provider: provider,
                text: text,
                chatBotGlobalAction: chatBotGlobalAction,
                previousHistory: previousHistory,
                max_tokens: max_tokens,
                settings: settings
            );
            string responseText = await SendHttpRequestAsync(url, HttpMethod.Post, payload);
            //ebug.Log(responseText);
            if(responseText.Contains("violation")){
                //ebug.Log("RAISING");
                throw new ArgEx(responseText);
            }
            //ebug.Log("HEADS DOWN");
            var obj = JsonConvert.DeserializeObject<ChatResponse[]>(responseText);
            var cri = 0; foreach(var k in obj){
                if(k.cost > 0){
                //    throw new Ex($"Thank you for everything, the free lunch is over ({cri})");
                }
                cri ++;
            }
            return obj[0];
        }
    }
}
