using System.ComponentModel;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient inner = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

IChatClient chat = new ChatClientBuilder(inner)
    .UseFunctionInvocation()
    .Build();

var tools = new List<AITool>
{
    AIFunctionFactory.Create(GetWeather),
    AIFunctionFactory.Create(ListEvents),
    AIFunctionFactory.Create(SendReminder),
};

var options = new ChatOptions { Tools = tools };

var messages = new List<ChatMessage>
{
    new(ChatRole.System, """
        You are a personal assistant. Use tools when the user asks about weather,
        their schedule, or wants to set reminders. Be concise.
        """),
    new(ChatRole.User, "What's the weather in Seattle today, do I have anything on my calendar, " +
                       "and remind me to bring an umbrella at 8am?"),
};

var response = await chat.GetResponseAsync(messages, options);
Console.WriteLine(response.Text);


[Description("Get current weather for a city.")]
static WeatherResult GetWeather(
    [Description("City name, e.g. 'Seattle'.")] string city)
    => new(city, 14, "partly cloudy with afternoon showers");

[Description("List the user's calendar events on a date (YYYY-MM-DD).")]
static IReadOnlyList<CalendarEvent> ListEvents(
    [Description("ISO date.")] string date)
    => new[]
    {
        new CalendarEvent("10:00", "Sprint planning"),
        new CalendarEvent("14:30", "Coffee with Sam"),
    };

[Description("Schedule a one-off reminder.")]
static string SendReminder(
    [Description("Local time HH:mm.")] string time,
    [Description("Short reminder text.")] string text)
    => $"Reminder set for {time}: {text}";

internal sealed record WeatherResult(string City, int TempC, string Conditions);
internal sealed record CalendarEvent(string Time, string Title);
