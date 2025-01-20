using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
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
    public async Task<IList> GetAllDocumentsAsync<T>(Type documentType, string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var listAsync = await collection.Find(_ => true).ToListAsync();
        return listAsync;
    }
    public async Task<T> GetDocumentByIdAsync<T>(string collectionName, string documentId)
    where T: IDocument {
        var collection =_database.GetCollection<T>(collectionName);
        return await collection.Find(x => x.Id == documentId).FirstOrDefaultAsync();
    }

    public async Task DeleteDocumentByIdAsync<T>(string collectionName, string documentId) 
        where T : IDocument
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.DeleteOneAsync(x => x.Id == documentId);
    }

    public async Task PostDocumentAsync<T>(string collectionName, T document)
    where T : IDocument
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.InsertOneAsync(document);
    }

    public async Task UpdateDocumentAsync<T>(string collectionName, string documentId, T document)
    where T : IDocument
    {
        document.Id = documentId;
        var collection = _database.GetCollection<T>(collectionName);
        await collection.ReplaceOneAsync(x => x.Id == documentId, document);
    }
    //type-loose document retrieving method in progress
    public async Task<List<BsonDocument>> GetDocumentsByIdAsync(string fieldName, string fieldValue, string collectionName)
    {
        var filter = new BsonDocument(
            fieldName,
            fieldName=="Id" ? new ObjectId(fieldValue) : BsonValue.Create(fieldValue)
        );
        
        var collection = _database.GetCollection<BsonDocument>(collectionName);
        var documents = await collection.Find(filter).ToListAsync();
        return documents;
    }
}