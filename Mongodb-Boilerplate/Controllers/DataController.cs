using System.Collections;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Mongodb_Boilerplate.Models;
using Mongodb_Boilerplate.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Mongodb_Boilerplate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : Controller
{
    private readonly MongoRepository _database;
    private readonly MyJsonSerializer _jsonSerializer;
    public DataController(MongoRepository database, MyJsonSerializer jsonSerializer)
    {
        _database = database;
        _jsonSerializer = jsonSerializer;
    }

    [HttpGet("{documentType}/{collectionName}")]
     public async Task<IActionResult> GetAllDocuments(string documentType, string collectionName)
     {
         Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
         if (bsonDocumentType == null)
         {
             return BadRequest("Invalid Document/model-class type");
         } 
         var documents = await GenericTypeConverter.GenericTypeConversionAsync<MongoRepository>(_database,"GetAllDocumentsAsync", bsonDocumentType, new object[] { bsonDocumentType, collectionName }).ConfigureAwait(false) as IList;
         if (documents == null)
         {
             return NotFound();
         }
         return Ok(documents);
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
        var document =  await GenericTypeConverter.GenericTypeConversionAsync<MongoRepository>(_database,"GetDocumentByIdAsync", bsonDocumentType, new object?[]{collectionName, id});
        if (document == null)
        {
            return NotFound();
        }
        return Ok(document);
    }

    [HttpGet("/delete/{documentType}/{collectionName}/{id}")]
    public async Task<IActionResult> DeleteDocument(string documentType, string collectionName, string id)
    {
        Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
        if (bsonDocumentType == null)
        {
            return BadRequest("Invalid Document/model-class type");
        }
        var document = await GenericTypeConverter.GenericTypeConversionAsync<MongoRepository>(_database, "GetDocumentByIdAsync", bsonDocumentType, new object[] { collectionName, id });
        if (document == null)
        {
            return NotFound();
        }

        await GenericTypeConverter.GenericTypeConversionAsync<MongoRepository>(_database, "DeleteDocumentByIdAsync", bsonDocumentType, new object[] { collectionName, id });
        return NoContent();
    }
    
    [HttpPost("post/{documentType}/{collectionName}")]
    //change to accept any type
    public async Task<IActionResult> Post(string documentType, string collectionName, [FromBody] JsonElement content)
    {
        Type bsonDocumentType = Type.GetType($"Mongodb_Boilerplate.Models.{documentType}");
        if (bsonDocumentType == null)
        {
            return BadRequest("Invalid Document/model-class type");
        }
        string jsonString = content.GetRawText();
        //var newBook = JsonSerializer.Deserialize<Book>(jsonString);
        var newBook = GenericTypeConverter.GenericTypeConversion<MyJsonSerializer>(_jsonSerializer,"Deserialize", bsonDocumentType,
                new object[] { jsonString }) as IDocument;
        await GenericTypeConverter.GenericTypeConversionAsync<MongoRepository>(_database, "PostDocumentAsync",
            bsonDocumentType, new object[] { collectionName, newBook });
        
        return CreatedAtAction(
            nameof(GetDocumentByIf),
            new { documentType = "Book", collectionName, id = newBook.Id },
            newBook
        );
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