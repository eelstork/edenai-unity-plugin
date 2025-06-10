using System;
using System.Collections.Generic;

namespace EdenAI_LLM{
public class ChatResponseLLM
{
    public string Id { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public string Object { get; set; }
    public string SystemFingerprint { get; set; }
    public List<Choice> Choices { get; set; }
    public long ProviderTime { get; set; }
    public object EdenaiTime { get; set; }
    public Usage Usage { get; set; }
    public double Cost { get; set; }
}

public class Choice
{
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public Message Message { get; set; }
}

public class Message
{
    public string Content { get; set; }
    public string Role { get; set; }
    public object ToolCalls { get; set; }
    public object FunctionCall { get; set; }
}

public class Usage
{
    public int CompletionTokens { get; set; }
    public int PromptTokens { get; set; }
    public int TotalTokens { get; set; }
    public object CompletionTokensDetails { get; set; }
    public PromptTokensDetails PromptTokensDetails { get; set; }
    public int CacheCreationInputTokens { get; set; }
    public int CacheReadInputTokens { get; set; }
}

public class PromptTokensDetails
{
    public object AudioTokens { get; set; }
    public int CachedTokens { get; set; }
}
}
