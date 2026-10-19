using MemoryApi.Domain;
using MemoryApi.Dtos;
using MemoryApi.Services;
using Xunit;

namespace MemoryApi.Tests;

public sealed class MemoryGameTests
{
    private static readonly string[] Symbols =
        ["apple", "banana", "grape", "cherry", "lemon", "melon", "kiwi", "peach"];

    [Fact]
    public void NewGame_CreatesSixteenHiddenCardsInEightPairs()
    {
        var game = new MemoryGame(Symbols, new Random(1));

        Console.WriteLine(string.Join(
            Environment.NewLine,
            game.Cards.Select(card => $"{card.Index}: {card.Symbol} ({card.State})")));

        Assert.Equal(16, game.Cards.Count);
        Assert.Equal(Enumerable.Range(0, 16), game.Cards.Select(card => card.Index));
        Assert.All(game.Cards, card => Assert.Equal(CardState.Hidden, card.State));
        Assert.All(game.Cards.GroupBy(card => card.Symbol), pair => Assert.Equal(2, pair.Count()));
        Assert.Equal(0, game.Moves);
        Assert.False(game.IsWon);
    }

    [Fact]
    public void FlippingMatchingPair_MarksCardsMatchedAndIncrementsMoves()
    {
        var game = new MemoryGame(Symbols, new Random(2));
        var matchingPair = game.Cards.GroupBy(card => card.Symbol).First().ToArray();

        game.Flip(matchingPair[0].Index);
        game.Flip(matchingPair[1].Index);

        Assert.All(matchingPair, card => Assert.Equal(CardState.Matched, card.State));
        Assert.Equal(1, game.Moves);
        Assert.False(game.IsWon);
    }

    [Fact]
    public void FlippingMismatchedCards_HidesThemOnTheNextFlip()
    {
        var game = new MemoryGame(Symbols, new Random(3));
        var firstCard = game.Cards[0];
        var secondCard = game.Cards.First(card => card.Symbol != firstCard.Symbol);
        var thirdCard = game.Cards.First(card =>
            card.Index != firstCard.Index && card.Index != secondCard.Index);

        game.Flip(firstCard.Index);
        game.Flip(secondCard.Index);
        game.Flip(thirdCard.Index);

        Assert.Equal(CardState.Hidden, firstCard.State);
        Assert.Equal(CardState.Hidden, secondCard.State);
        Assert.Equal(CardState.Revealed, thirdCard.State);
        Assert.Equal(1, game.Moves);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(16)]
    public void Flip_WithIndexOutsideTheBoard_Throws(int index)
    {
        var game = new MemoryGame(Symbols, new Random(4));

        Assert.Throws<ArgumentOutOfRangeException>(() => game.Flip(index));
    }

    [Fact]
    public void Flip_RejectsACardThatIsAlreadyRevealed()
    {
        var game = new MemoryGame(Symbols, new Random(5));
        game.Flip(0);

        Assert.Throws<InvalidOperationException>(() => game.Flip(0));
    }

    [Fact]
    public void FlippingEveryPair_WinsTheGame()
    {
        var game = new MemoryGame(Symbols, new Random(6));

        foreach (var pair in game.Cards.GroupBy(card => card.Symbol))
        {
            var cards = pair.ToArray();
            game.Flip(cards[0].Index);
            game.Flip(cards[1].Index);
        }

        Assert.Equal(8, game.Moves);
        Assert.True(game.IsWon);
        Assert.All(game.Cards, card => Assert.Equal(CardState.Matched, card.State));
    }

    [Fact]
    public void BuildFromMemory_HidesSymbolsOnHiddenCards()
    {
        var game = new MemoryGame(Symbols, new Random(7));
        var revealedCard = game.Cards[0];
        game.Flip(revealedCard.Index);

        var dto = GameDto.BuildFromMemory(game);

        Assert.All(dto.Cards.Where(card => card.State == CardState.Hidden),
            card => Assert.Null(card.Symbol));
        Assert.Equal(revealedCard.Symbol, dto.Cards[revealedCard.Index].Symbol);
        Assert.False(dto.IsWon);
    }

    [Fact]
    public void GameStore_ReturnsSavedGameAndNullForUnknownId()
    {
        var store = new InMemoryGameStore();
        var game = new MemoryGame(Symbols, new Random(8));

        store.Save(game);

        Assert.Same(game, store.Get(game.Id));
        Assert.Null(store.Get(Guid.NewGuid()));
    }
}
