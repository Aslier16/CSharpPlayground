using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSharpPlayground.Models;

public class DeepseekRequest
{
    public async Task<string> SendRequest(string userContent, string token)
    {
        var client = new HttpClient();
        var drequest = new HttpRequestMessage(HttpMethod.Post, "https://api.deepseek.com/chat/completions");
        drequest.Headers.Add("Accept", "application/json");
        drequest.Headers.Add("Authorization", $"Bearer {token}");

        var chatRequest = new ChatRequest
        {
            messages = new List<RequestMessage>
            {
                new RequestMessage { role = "system", content = "You are a helpful assistant" },
                new RequestMessage { role = "user", content = userContent }
            },
            model = "deepseek-chat",
            frequency_penalty = 0,
            max_tokens = 2048,
            presence_penalty = 0,
            response_format = new ResponseFormat { type = "text" },
            stop = null,
            stream = false,
            stream_options = null,
            temperature = 1,
            top_p = 1,
            tools = null,
            tool_choice = "none",
            logprobs = false,
            top_logprobs = null
        };

        var json = JsonSerializer.Serialize(chatRequest, new JsonSerializerOptions { IgnoreNullValues = true });
        drequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
        var dresponse = await client.SendAsync(drequest);
        dresponse.EnsureSuccessStatusCode();
        return await dresponse.Content.ReadAsStringAsync();
    }
}

public class ChatRequest
{
    public List<RequestMessage> messages { get; set; }
    public string model { get; set; }
    public double frequency_penalty { get; set; }
    public int max_tokens { get; set; }
    public double presence_penalty { get; set; }
    public ResponseFormat response_format { get; set; }
    public object stop { get; set; }
    public bool stream { get; set; }
    public object stream_options { get; set; }
    public double temperature { get; set; }
    public double top_p { get; set; }
    public object tools { get; set; }
    public string tool_choice { get; set; }
    public bool logprobs { get; set; }
    public object top_logprobs { get; set; }
}

public class ResponseFormat
{
    public string type { get; set; }
}

public class RequestMessage
{
    public string content { get; set; }
    public string role { get; set; }
}

public class Response
{
    public string id { get; set; }
    public Choice[] choices { get; set; }
    public int created { get; set; }
    public string model { get; set; }
}

public class Choice
{
    public string finish_reason { get; set; }
    public int index { get; set; }
    public ResponseMessage message { get; set; }
    public int created { get; set; }
    public string model { get; set; }
}

public class ResponseMessage
{
    public string content { get; set; }
    public string reasoning_content { get; set; }
    public string role { get; set; }
}

