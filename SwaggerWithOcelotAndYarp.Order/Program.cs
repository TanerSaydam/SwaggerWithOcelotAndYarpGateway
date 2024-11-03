var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => "Hello order world!");

app.MapGet("/api/get-all", () =>
{
    var list = new List<string>()
    {
        "Order1",
        "Order2"
    };

    return list;
}).WithTags("orders");

app.MapControllers();

app.Run();
