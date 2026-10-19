using System.Collections.Concurrent;
using MemoryApi.Domain;

namespace MemoryApi.Services;

// using concurrecent dicionary for simple in memory storage. this would be replaced with a database in a real app 
public sealed class InMemoryGameStore : IGameStore
{
    private readonly ConcurrentDictionary<Guid, MemoryGame> _games = new();

    public void Save(MemoryGame game) => _games[game.Id] = game;

    public MemoryGame? Get(Guid id) => _games.TryGetValue(id, out var game) ? game : null;
}
