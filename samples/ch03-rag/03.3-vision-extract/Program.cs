using System.ComponentModel;
using Microsoft.Extensions.AI;
using OpenAI;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: dotnet run -- <path-to-receipt-image>");
    return 1;
}

var imagePath = args[0];
if (!File.Exists(imagePath))
{
    Console.Error.WriteLine($"File not found: {imagePath}");
    return 1;
}

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("Set OPENAI_API_KEY.");

IChatClient chat = new OpenAIClient(apiKey)
    .GetChatClient("gpt-4o-mini")
    .AsIChatClient();

byte[] bytes = await File.ReadAllBytesAsync(imagePath);
string mediaType = Path.GetExtension(imagePath).ToLowerInvariant() switch
{
    ".jpg" or ".jpeg" => "image/jpeg",
    ".png" => "image/png",
    ".webp" => "image/webp",
    _ => throw new InvalidOperationException("Use a jpg/png/webp image."),
};

var message = new ChatMessage(ChatRole.User,
[
    new TextContent("Extract the receipt fields from this image."),
    new DataContent(bytes, mediaType),
]);

var response = await chat.GetResponseAsync<Receipt>([message]);

if (response.TryGetResult(out var receipt))
{
    Console.WriteLine($"Vendor:   {receipt.Vendor}");
    Console.WriteLine($"Date:     {receipt.DateIso}");
    Console.WriteLine($"Total:    {receipt.Total} {receipt.Currency}");
    Console.WriteLine($"Items:");
    foreach (var item in receipt.LineItems)
    {
        Console.WriteLine($"  - {item.Name,-30}  {item.UnitPrice:F2} x {item.Quantity}");
    }
}
else
{
    Console.Error.WriteLine("Model returned malformed JSON. Raw text:");
    Console.Error.WriteLine(response.Text);
    return 1;
}

return 0;

internal sealed record Receipt(
    [property: Description("Vendor / store name as printed.")] string Vendor,
    [property: Description("Receipt date as ISO 8601 (YYYY-MM-DD).")] string DateIso,
    [property: Description("Grand total amount.")] decimal Total,
    [property: Description("ISO 4217 currency code, e.g. 'USD'.")] string Currency,
    IReadOnlyList<ReceiptLine> LineItems);

internal sealed record ReceiptLine(string Name, decimal UnitPrice, int Quantity);
