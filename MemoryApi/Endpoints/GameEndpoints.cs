using MemoryApi.Domain;
using MemoryApi.Dtos;
using MemoryApi.Services;

namespace MemoryApi.Endpoints;

// minimalistic api for simplicitys sake here, so opted for no controllers 
public static class GameEndpoints
{
    private static readonly string[] Symbols =
        { "🍎", "🍌", "🍇", "🍒", "🍋", "🍉", "🥝", "🍑" };

    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/games", (IGameStore store) =>
        {
            var game = new MemoryGame(Symbols, Random.Shared);
            store.Save(game);
            return Results.Created($"/api/games/{game.Id}", GameDto.BuildFromMemory(game));
        });

        endpoints.MapGet("/api/games/{id:guid}", (Guid id, IGameStore store) =>
            store.Get(id) is { } game ? Results.Ok(GameDto.BuildFromMemory(game)) : Results.NotFound());

        endpoints.MapPost("/api/games/{id:guid}/flip/{index:int}", (Guid id, int index, IGameStore store) =>
        {
            if (store.Get(id) is not { } game) return Results.NotFound();

            try
            {
                game.Flip(index);
                return Results.Ok(GameDto.BuildFromMemory(game));
            }
            catch (Exception exception) when (exception is ArgumentOutOfRangeException or InvalidOperationException)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
        });

        return endpoints;
    }
}
