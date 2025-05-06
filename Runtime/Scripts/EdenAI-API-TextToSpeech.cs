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
    }
}
