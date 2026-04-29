using System.ComponentModel;
using Ch05.InventoryMcp.Data;
using ModelContextProtocol.Server;

namespace Ch05.InventoryMcp.Tools;

[McpServerToolType]
public sealed class InventoryTools(InventoryDb db)
{
    [McpServerTool, Description("Search the inventory by name, SKU, or category.")]
    public IEnumerable<Product> Search(
        [Description("Free-text query.")] string query)
        => db.Search(query);

    [McpServerTool, Description("Get a product by SKU.")]
    public Product? Get(
        [Description("Product SKU, e.g. ECC-100.")] string sku)
        => db.Get(sku);

    [McpServerTool, Description("Check stock for a SKU. Returns 0 if not found.")]
    public int Stock(
        [Description("Product SKU.")] string sku)
        => db.Get(sku)?.Stock ?? 0;
}
