using Microsoft.OpenApi.Models;
using SwaggerWithOcelotAndYarp.Yarp.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Yarp Gateway", Version = "v1" });
});

builder.Services.AddHttpClient(); // HttpClient servisini ekleyin
builder.Services.AddCors();

builder.Services.AddHttpClient();

builder.Services.Configure<ReverseProxy>(builder.Configuration.GetSection("ReverseProxy"));

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddSwagger()
    ;

var app = builder.Build();

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Yarp Gateway v1");
    foreach (var item in SwaggerExensions.SwaggerFiles)
    {
        c.SwaggerEndpoint(item.Value, item.Key);
    }
});

app.MapReverseProxy();
app.MapGet("/", () => "Hello World!");

app.Run();
