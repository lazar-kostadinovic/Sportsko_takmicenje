using Neo4jClient;
using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RedisService>(_ => new RedisService("localhost:6379"));
builder.Services.AddSingleton<LeaderboardService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var neo4jSettings = builder.Configuration.GetSection("Neo4jSettings").Get<Neo4jSettings>();
builder.Services.AddSingleton(new Neo4jService(neo4jSettings.Uri, neo4jSettings.Username, neo4jSettings.Password));

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
