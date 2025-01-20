using Mongodb_Boilerplate.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowSpecificOrigin",
//         builder =>
//         {
//             builder.WithOrigins("https://localhost:5173") // Specify the allowed origin
//                 .AllowAnyHeader() // Allow any headers (like Content-Type)
//                 .AllowAnyMethod(); // Allow any HTTP methods (GET, POST, etc.)
//         });
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        build =>
        {
            build.AllowAnyOrigin()    // Allow all origins
                .AllowAnyHeader()    // Allow any headers
                .AllowAnyMethod();   // Allow any HTTP methods
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton( sp => 
    new MongoRepository("mongodb://localhost:27017", "Bookstore"));
builder.Services.AddSingleton(sp => new MyJsonSerializer());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();
 
app.Run();

