using MemoryApi.Domain;

namespace MemoryApi.Services;

public interface IGameStore
{
    void Save(MemoryGame game);
    MemoryGame? Get(Guid id);
}
