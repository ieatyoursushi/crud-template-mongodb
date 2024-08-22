using Mongodb_Boilerplate.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Mongodb_Boilerplate.Services;

public interface IDocument
{
    [BsonRepresentation(BsonType.ObjectId)] 
    string? Id { get; set; }
}