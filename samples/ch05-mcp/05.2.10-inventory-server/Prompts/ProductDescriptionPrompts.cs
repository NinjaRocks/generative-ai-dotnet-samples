using System.ComponentModel;
using ModelContextProtocol.Server;

namespace Ch05.InventoryMcp.Prompts;

[McpServerPromptType]
public sealed class ProductDescriptionPrompts
{
    [McpServerPrompt, Description("Generate a marketing-style product description.")]
    public static string Describe(
        [Description("Product name to describe.")] string productName,
        [Description("Tone: friendly | technical | terse.")] string tone = "friendly")
        => $"""
            Write a {tone} 60-word product description for: {productName}.
            Lead with the customer benefit, end with a call to action.
            """;
}
