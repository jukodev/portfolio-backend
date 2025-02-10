using portfolio_backend.Middlewares;
using portfolio_backend.Services;
using portfolio_backend.Utils;

var cors = "_allowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(dotenv);

var config =
    new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy(cors, builder =>
    {
        builder.WithOrigins(["https://sharkys.de","http://localhost:5173"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WebScraper>();
builder.Services.AddSingleton<ProxyService>();
// builder.Services.AddHostedService<TimedWebScraper>(); disabled for now

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.Run();
