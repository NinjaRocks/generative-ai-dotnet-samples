using System.ComponentModel;
using Microsoft.Extensions.AI;
using OpenAI;

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient chat = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

var input = """
    Booking confirmation: Anna Lee booked seat 14C on flight BA0287 from London (LHR)
    to San Francisco (SFO) on 2026-05-12, departing 14:55. Confirmation code: BA-9X7Q2.
    """;

ChatResponse<FlightBooking> response = await chat.GetResponseAsync<FlightBooking>(
    [new ChatMessage(ChatRole.User, $"Extract the booking details from:\n\n{input}")]);

if (response.TryGetResult(out var booking))
{
    Console.WriteLine($"Passenger:    {booking.Passenger}");
    Console.WriteLine($"Flight:       {booking.FlightNumber}");
    Console.WriteLine($"Origin:       {booking.OriginCode}");
    Console.WriteLine($"Destination:  {booking.DestinationCode}");
    Console.WriteLine($"Date:         {booking.DateIso}");
    Console.WriteLine($"Seat:         {booking.Seat}");
    Console.WriteLine($"Confirmation: {booking.ConfirmationCode}");
}
else
{
    Console.Error.WriteLine("Model returned malformed JSON. Raw text:");
    Console.Error.WriteLine(response.Text);
}

internal sealed record FlightBooking(
    [property: Description("Full passenger name as printed on the ticket.")] string Passenger,
    [property: Description("Flight number, e.g. BA0287.")] string FlightNumber,
    [property: Description("Origin airport IATA code, e.g. LHR.")] string OriginCode,
    [property: Description("Destination airport IATA code, e.g. SFO.")] string DestinationCode,
    [property: Description("Departure date as ISO 8601 (YYYY-MM-DD).")] string DateIso,
    [property: Description("Seat assignment, e.g. 14C.")] string Seat,
    [property: Description("Booking confirmation code.")] string ConfirmationCode);
