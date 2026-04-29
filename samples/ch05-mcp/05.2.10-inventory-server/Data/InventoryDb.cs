namespace Ch05.InventoryMcp.Data;

public sealed record Product(string Sku, string Name, string Category, int Stock, decimal Price);

public sealed class InventoryDb
{
    private readonly List<Product> _products =
    [
        new("ECC-100", "Wireless earbuds", "audio", 42, 79.99m),
        new("ECC-200", "Bone conduction headphones", "audio", 12, 119.00m),
        new("KBD-310", "Mechanical keyboard", "input", 8, 145.00m),
        new("MSE-410", "Vertical ergonomic mouse", "input", 0, 65.50m),
        new("STN-510", "Aluminum laptop stand", "accessories", 24, 39.95m),
    ];

    public IEnumerable<Product> All() => _products;

    public IEnumerable<Product> Search(string query) =>
        _products.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Sku.Contains(query, StringComparison.OrdinalIgnoreCase));

    public Product? Get(string sku) =>
        _products.FirstOrDefault(p => p.Sku.Equals(sku, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<string> Categories() =>
        _products.Select(p => p.Category).Distinct();
}
