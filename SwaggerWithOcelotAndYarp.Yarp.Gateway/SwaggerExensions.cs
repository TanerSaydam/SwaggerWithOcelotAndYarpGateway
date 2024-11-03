using Microsoft.Extensions.Options;
using System.Text.Json;

namespace SwaggerWithOcelotAndYarp.Yarp.Gateway;

public static class SwaggerExensions
{
    public static Dictionary<string, string> SwaggerFiles = new();
    public static IReverseProxyBuilder AddSwagger(this IReverseProxyBuilder builder)
    {
        SwaggerFiles = new();

        var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
        var options = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<ReverseProxy>>();
        ReverseProxy reverseProxy = options.Value;

        // Swagger bilgilerini yakala
        foreach (var cluster in reverseProxy.Clusters)
        {
            foreach (var destination in cluster.Value.Destinations)
            {
                if (destination.Value.Swagger is not null)
                {
                    GetSwaggerJsonFile(httpClient, destination.Value.Address, destination.Value.Swagger.Prefix, destination.Value.Swagger.FileName);
                    SwaggerFiles.Add(destination.Value.Swagger.SwaggerName, "/" + destination.Value.Swagger.FileName + ".json");
                }
            }
        }


        return builder;
    }

    private static void GetSwaggerJsonFile(HttpClient httpClient, string endpoint, string prefix, string fileName)
    {
        var message = httpClient.GetAsync($"{endpoint}/swagger/v1/swagger.json").GetAwaiter().GetResult();
        var content = message.Content.ReadFromJsonAsync<JsonElement>().GetAwaiter().GetResult();

        // Create a mutable copy of the paths
        var updatedPaths = new Dictionary<string, JsonElement>();

        // Iterate through the original paths and update them
        foreach (var path in content.GetProperty("paths").EnumerateObject())
        {
            // Add the /order prefix to each path
            var newPath = $"/{prefix}{path.Name}";
            updatedPaths[newPath] = path.Value;
        }

        // Create a new JSON object with the updated paths
        var updatedSwagger = new
        {
            openapi = content.GetProperty("openapi").GetString(),
            info = content.GetProperty("info"),
            paths = updatedPaths,
            components = content.GetProperty("components")
        };

        // Serialize the updated content to JSON
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true // Format the JSON to be more readable
        };

        var jsonString = JsonSerializer.Serialize(updatedSwagger, jsonOptions);

        // File path for saving the updated JSON
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"{fileName}.json");

        // Write the JSON content to the file
        File.WriteAllTextAsync(filePath, jsonString).GetAwaiter().GetResult();
    }
}

public class ReverseProxy
{
    public Dictionary<string, Route> Routes { get; set; } = default!;
    public Dictionary<string, Cluster> Clusters { get; set; } = default!;
}

public class Route
{
    public string ClusterId { get; set; } = default!;
    public Match Match { get; set; } = default!;
    public List<Transform> Transforms { get; set; } = default!;
}

public class Match
{
    public string Path { get; set; } = default!;
}

public class Transform
{
    public string PathRemovePrefix { get; set; } = default!;
}

public class Cluster
{
    public Dictionary<string, Destination> Destinations { get; set; } = default!;
}

public class Destination
{
    public string Address { get; set; } = default!;
    public Swagger Swagger { get; set; } = default!;
}

public class Swagger
{
    public string FileName { get; set; } = default!;
    public string Prefix { get; set; } = default!;
    public string SwaggerName { get; set; } = default!;
}