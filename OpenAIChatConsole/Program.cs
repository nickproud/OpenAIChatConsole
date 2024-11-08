using OpenAIChatConsole;

var apiKey = "[OPENAI_API_KEY]";
var openAiConnector = new OpenAiConnector(apiKey);

bool stayOpen = true;

while (stayOpen)
{
    var message = Console.ReadLine();
    if (message != null)
    {
        var response = await openAiConnector.Prompt(message);
        Console.WriteLine(response);
    }
}