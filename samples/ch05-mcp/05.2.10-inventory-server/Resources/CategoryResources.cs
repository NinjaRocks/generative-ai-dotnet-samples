using System.ComponentModel;
using Ch05.InventoryMcp.Data;
using ModelContextProtocol.Server;

namespace Ch05.InventoryMcp.Resources;

[McpServerResourceType]
public sealed class CategoryResources(InventoryDb db)
{
    [McpServerResource(UriTemplate = "inventory://categories", Name = "Inventory categories")]
    [Description("All available product categories.")]
    public IEnumerable<string> Categories() => db.Categories();
}
