using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using Mongodb_Boilerplate.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongodb_Boilerplate.Models;

public class Book : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    //[BsonElement("Id", IsReadOnly = true)]
    public string? Id { get; set; } //id will be generated by mongodb
    
    [JsonPropertyName("name")]
    public string BookName { get; set; } = null!;

    public decimal Price { get; set; }
    
    public string Category { get; set; } = null!;

    public string Author { get; set; } = null!;
}