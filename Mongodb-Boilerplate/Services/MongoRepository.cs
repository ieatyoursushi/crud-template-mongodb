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
    private IMongoCollection<T> GetCollection<T>(string collectionName)
    where T: IDocument{
        return _database.GetCollection<T>(collectionName);
    }
    //getDocument method
    public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionName)
    where T: IDocument
    {
        var colleciton = GetCollection<T>(collectionName);
        return await colleciton.Find(_ => true).ToListAsync();
    }
    public async Task<T> GetDocumentByIdAsync<T>(string collectionName, string documentId)
    where T: IDocument {
        var collection = GetCollection<T>(collectionName);
        return await collection.Find(x => x.Id == documentId).FirstOrDefaultAsync();
    }
    
}