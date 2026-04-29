using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

void Show(string key)
{
    var value = config[key];
    var masked = string.IsNullOrEmpty(value)
        ? "(unset)"
        : value.Length <= 8 ? new string('*', value.Length) : value[..3] + new string('*', value.Length - 6) + value[^3..];
    Console.WriteLine($"  {key,-32} = {masked}");
}

Console.WriteLine("Resolved configuration values (sources merged in order: appsettings -> user-secrets -> env):");
Show("OpenAI:ApiKey");
Show("AzureOpenAI:Endpoint");
Show("AzureOpenAI:Key");
Show("Anthropic:ApiKey");
Show("GitHub:Token");

Console.WriteLine();
Console.WriteLine("Add a secret with:");
Console.WriteLine("  dotnet user-secrets set \"OpenAI:ApiKey\" \"sk-...\" --project samples/ch01-foundations/01.4-secrets-and-config");
