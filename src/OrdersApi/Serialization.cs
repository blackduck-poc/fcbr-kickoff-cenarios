using Newtonsoft.Json;

namespace OrdersApi;

public static class Serialization
{
    // Type name handling enabled for untrusted input.
    private static readonly JsonSerializerSettings Settings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public static object? Deserialize(string json) => JsonConvert.DeserializeObject(json, Settings);
}
