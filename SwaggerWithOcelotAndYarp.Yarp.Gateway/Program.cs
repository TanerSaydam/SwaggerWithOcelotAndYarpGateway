using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Yarp Gateway", Version = "v1" });
    c.AddServer(new OpenApiServer { Url = "http://localhost:5090" });
});

builder.Services.AddCors();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("http://localhost:5001/swagger/v1/swagger.json", "Order API v1");
    c.SwaggerEndpoint("http://localhost:5002/swagger/v1/swagger.json", "Catalog API v1");
});



app.MapReverseProxy();

app.MapGet("/", () => "Hello World!");

app.Run();
