var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
var app = builder.Build();
app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/get-all", () =>
{
    var list = new List<string>()
    {
        "Catalog1",
        "Catalog1"
    };

    return list;
});

app.Run();
