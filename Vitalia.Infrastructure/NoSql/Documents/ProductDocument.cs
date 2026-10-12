using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vitalia.Infrastructure.NoSql.Documents;

public class ProductDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("productId")]
    public int ProductId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [BsonElement("updatedAt")]
    public string UpdatedAt { get; set; } = string.Empty;

    [BsonElement("category")]
    public ProductCategoryDocument Category { get; set; } = new();
}

public class ProductCategoryDocument
{
    [BsonElement("categoryId")]
    public int CategoryId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string? Description { get; set; }
}