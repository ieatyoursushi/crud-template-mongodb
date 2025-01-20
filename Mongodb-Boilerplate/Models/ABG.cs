using System.Text.Json.Serialization;
using Mongodb_Boilerplate.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongodb_Boilerplate.Models;

public class ABG : IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public string Name { get; set; }
    
    public string HairColor { get; set; }
    
    public string LinkedIn { get; set; }
}