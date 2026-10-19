using System.Text.Json.Serialization;
using MemoryApi.Endpoints;
using MemoryApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGameStore, InMemoryGameStore>();

builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();

app.MapGameEndpoints();

app.Run();
