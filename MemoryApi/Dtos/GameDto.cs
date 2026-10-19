using MemoryApi.Domain;

namespace MemoryApi.Dtos;

public sealed record GameDto(Guid Id, int Moves, bool IsWon, IReadOnlyList<CardDto> Cards)
{
    // return a copy of the game and hide the symbols if it's faced down
    public static GameDto BuildFromMemory(MemoryGame game) => new(
        game.Id,
        game.Moves,
        game.IsWon,
        game.Cards.Select(card => new CardDto(
                card.Index,
                card.State,
                card.State == CardState.Hidden ? null : card.Symbol))
            .ToList());
}
