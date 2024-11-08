using System.Text;
using System.Text.Json.Nodes;

namespace OpenAIChatConsole;

public class OpenAiConnector : IDisposable
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private const string Model = "gpt-4o";
    private const int Temperature = 1;
    private const int MaxTokens = 2048;
    private const int TopP = 1;
    private const int FrequencyPenalty = 0;
    private const int PresencePenalty = 0;
    
    public OpenAiConnector(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        ConfigureClient();
    }

    public async Task<string?> Prompt(string message, string responseFormat = "text")
    {
        var payload = new StringContent(CreatePayload(message, responseFormat), Encoding.UTF8, "application/json");
        var result = await _httpClient.PostAsync("/v1/chat/completions", payload);
        var response = ParseResponse(await result.Content.ReadAsStringAsync());

        return response;
    }

    private void ConfigureClient()
    {
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);
        _httpClient.BaseAddress = new Uri("https://api.openai.com");
    }

    private string ParseResponse(string response)
    {
        var responseObject = JsonNode.Parse(response);
        return responseObject?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "Sorry, I don't have a response for you at this time";
    }

    private string CreatePayload(string message, string responseFormat)
    {
        JsonObject payload = new JsonObject();
        JsonObject responseFormatObject = new JsonObject();
        JsonObject messageObject = new JsonObject();
        JsonObject messageContentObject = new JsonObject();
        JsonArray messageContentArray = new JsonArray();
        JsonArray messageArray = new JsonArray();
        
        messageContentObject.Add("type", "text");
        messageContentObject.Add("text", message);
        messageContentArray.Add(messageContentObject);
        
        messageObject.Add("role", "user");
        messageObject.Add("content", messageContentArray);
        messageArray.Add(messageObject);
        payload.Add("messages", messageArray);
        
        responseFormatObject.Add("type", responseFormat);
        payload.Add("response_format", responseFormatObject);

        payload.Add("model", Model);
        payload.Add("temperature", Temperature);
        payload.Add("max_tokens", MaxTokens);
        payload.Add("top_p", TopP);
        payload.Add("frequency_penalty", FrequencyPenalty);
        payload.Add("presence_penalty", PresencePenalty);
        return payload.ToJsonString();
    }


    public void Dispose()
    {
        _httpClient.Dispose();
    }
}