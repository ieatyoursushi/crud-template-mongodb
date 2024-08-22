using Microsoft.AspNetCore.Mvc;
using Mongodb_Boilerplate.Models;
using Mongodb_Boilerplate.Services;
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
    //refactor code to have type in mind
    [HttpGet("{collectionName}/{id}")]
    public async Task<IActionResult> GetDocumentById(string collectionName, string id)
    {
       IDocument bookDocument = await database.GetDocumentByIdAsync<Book>(collectionName, id);
        if (bookDocument is null)
        {
            return NotFound();
        }
        return Ok(bookDocument);
    }
}