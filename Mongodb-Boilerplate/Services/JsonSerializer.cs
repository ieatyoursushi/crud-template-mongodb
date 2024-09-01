namespace Mongodb_Boilerplate.Services;
using System.Text.Json;
public class MyJsonSerializer
{
    private readonly JsonSerializerOptions _options;

    public MyJsonSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj);
    }

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }
    
}