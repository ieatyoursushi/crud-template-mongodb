using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Mongodb_Boilerplate.Models;

namespace Mongodb_Boilerplate.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

public class MongoRepository
{
    private readonly IMongoDatabase _database;

    public MongoRepository(string connectionString, string databaseName)
    {
        MongoClient client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    //getDocument method
    public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionName)
    where T: IDocument
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.Find(_ => true).ToListAsync();
    }
    public async Task<T> GetDocumentByIdAsync<T>(string collectionName, string documentId)
    where T: IDocument {
        var collection =_database.GetCollection<T>(collectionName);
        return await collection.Find(x => x.Id == documentId).FirstOrDefaultAsync();
    }
    
    
    //tenary type-loose document retrieving method in progress
    public async Task<List<BsonDocument>> GetDocumentsByIdAsync(string fieldName, string fieldValue, string collectionName)
    {
        var filter = new BsonDocument(fieldName, fieldName=="Id" ? new ObjectId(fieldValue) : BsonValue.Create(fieldValue));
        var collection = _database.GetCollection<BsonDocument>(collectionName);
        var documents = await collection.Find(filter).ToListAsync();
        return documents;
    }
}