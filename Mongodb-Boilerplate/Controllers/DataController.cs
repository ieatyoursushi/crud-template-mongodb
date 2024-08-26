using System.Diagnostics;
using System.Reflection;
using Amazon.Runtime.Documents;
using Microsoft.AspNetCore.Mvc;
using Mongodb_Boilerplate.Models;
using Mongodb_Boilerplate.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongodb_Boilerplate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : Controller
{
    private readonly MongoRepository database;

    public DataController(MongoRepository database)
    {
        this.database = database;
    }

    [HttpGet("{collectionName}")]
    // public async Task<IActionResult> GetAllDocuments(string collectionName)
    // {
    //     
    // }
    //refactor code to have type in mind
    [HttpGet("searchType/{documentType}/{collectionName}/{id}")]
    public async Task<IActionResult> GetDocumentByIf(string documentType,string collectionName, string id)
    {
        Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
        if (bsonDocumentType == null)
        {
            return BadRequest("Invalid Document/model-class type");
        }
        MethodInfo method = typeof(MongoRepository).GetMethod("GetDocumentByIdAsync").MakeGenericMethod(bsonDocumentType);
        var task = (Task)method.Invoke(database, new object?[] { collectionName, id });
        await task.ConfigureAwait(false);
        PropertyInfo? resultProperty = task.GetType().GetProperty("Result");
        var document =  (IDocument)resultProperty.GetValue(task);
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

    
    
    
    [HttpGet("{fieldName}/{fieldValue}/{collectionName}")]
    public async Task<IActionResult> GetDocumentByQuerySearch(string fieldName, string fieldValue,
        string collectionName)
    {
        List<BsonDocument> document = await database.GetDocumentsByIdAsync(fieldName, fieldValue, collectionName);
        if (document is null)
        {
            return NotFound();
        }
        return Ok(document);
    }
}