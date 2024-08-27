using System.Collections;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Mongodb_Boilerplate.Services;
using MongoDB.Bson;

namespace Mongodb_Boilerplate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : Controller
{
    private readonly MongoRepository _database;

    public DataController(MongoRepository database)
    {
        this._database = database;
    }

    [HttpGet("{documentType}/{collectionName}")]
     public async Task<IActionResult> GetAllDocuments(string documentType, string collectionName)
     {
         Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
         if (bsonDocumentType == null)
         {
             return BadRequest("Invalid Document/model-class type");
         } 
         var documents = await GetDocument("GetAllDocumentsAsync", bsonDocumentType, new object[] { bsonDocumentType, collectionName }).ConfigureAwait(false);
         if (documents == null)
         {
             return NotFound();
         }
 
         //converts IDocuments list to readable BsonDocumentType list (ex: List<Book>) 
         //so each object returned as json in the list has all properties intead of just Id from the List<IDocuments>
         //tldr: snippet converts List<IDocument> -> List<ModelType> from documentType
         Type listType = typeof(List<>).MakeGenericType(bsonDocumentType);
         var concreteDocuments = (IList)Activator.CreateInstance(listType);
         foreach (var document in (IEnumerable)documents)
         {
             concreteDocuments.Add(document);
         }
         //logic flaw: GetAllDocumentsAsync in MongoRepository needs to return List<T> instead of List<IDocument>;
         return Ok(concreteDocuments);
     }
    //refactor code to have type in mind
    [HttpGet("search/{documentType}/{collectionName}/{id}")]
    public async Task<IActionResult> GetDocumentByIf(string documentType,string collectionName, string id)
    {
        Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
        if (bsonDocumentType == null)
        {
            return BadRequest("Invalid Document/model-class type");
        }
        var document =  await GetDocument("GetDocumentByIdAsync", bsonDocumentType, new object?[]{collectionName, id});
        if (document == null)
        {
            return NotFound();
        }
        return Ok(document);
        //explicit inline type search
        // IDocument bookDocument = await database.GetDocumentByIdAsync<Book>(collectionName, id);
        // if (bookDocument is null)
        // {
        //     return NotFound();
        // }
        // return Ok(bookDocument);
    }
    //generic method,type for the generic method, object of {params}
    [HttpGet]
    public virtual async Task<object?> GetDocument(string databaseMethod,Type bsonDocumentType, [FromQuery] object[] parameters)
    {
        MethodInfo method = typeof(MongoRepository).GetMethod(databaseMethod).MakeGenericMethod(bsonDocumentType);
        var task = (Task)method.Invoke(_database, parameters);
        await task.ConfigureAwait(false);
        PropertyInfo? resultProperty = task.GetType().GetProperty("Result");
        var document = resultProperty.GetValue(task);

        if (document is IDocument)
            return document;
        if (document is IEnumerable<IDocument> documents)
            return documents.ToList();
        return null;
    }


    [HttpGet("{fieldName}/{fieldValue}/{collectionName}")]
    public async Task<IActionResult> GetDocumentByQuerySearch(string fieldName, string fieldValue,
        string collectionName)
    {
        List<BsonDocument> document = await _database.GetDocumentsByIdAsync(fieldName, fieldValue, collectionName);
        if (document is null)
        {
            return NotFound();
        }
        return Ok(document);
    }
}

// IDocument doc = (IDocument)(Task)typeof(MongoRepository).GetMethod("GetDocumentByIdAsync").MakeGenericMethod(bsonDocumentType).Invoke(database, new object?[] { collectionName, id }).GetType().GetProperty("Result").GetValue((Task)typeof(MongoRepository).GetMethod("GetDocumentByIdAsync").MakeGenericMethod(Type.GetType($"Mongodb_Boilerplate.Models.{documentType}")).Invoke(database, new object?[] { collectionName, id }));