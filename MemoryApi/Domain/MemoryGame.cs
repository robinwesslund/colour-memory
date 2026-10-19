namespace MemoryApi.Domain;

public sealed class MemoryGame
{
    private readonly Card[] _cards;
    private readonly List<Card> _faceUp = new(2);

    public Guid Id { get; } = Guid.NewGuid();
    public int Moves { get; private set; }
    public IReadOnlyList<Card> Cards => _cards;
    public bool IsWon => _cards.All(card => card.State == CardState.Matched);

    public MemoryGame(IReadOnlyList<string> symbols, Random rng)
    {
        var shuffledSymbols = symbols.Concat(symbols).ToArray();
        rng.Shuffle(shuffledSymbols);
        _cards = shuffledSymbols
            .Select((symbol, index) => new Card(index, symbol))
            .ToArray();
    }

    public void Flip(int index)
    {
        if (index < 0 || index >= _cards.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Card index must be 0-15.");

        HidePreviousMismatch();

        var card = _cards[index];
        if (card.State != CardState.Hidden)
            throw new InvalidOperationException("Card is not face down."); // not really realistic unless manupulating request 

        card.State = CardState.Revealed;
        _faceUp.Add(card);

        if (_faceUp.Count == 2)
            ResolveFlippedPair();
    }

    private void HidePreviousMismatch()
    {
        if (_faceUp.Count != 2) return;

        foreach (var card in _faceUp)
            card.State = CardState.Hidden;
        _faceUp.Clear();
    }

    private void ResolveFlippedPair()
    {
        Moves++;
        if (_faceUp[0].Symbol != _faceUp[1].Symbol) return;

        foreach (var card in _faceUp)
            card.State = CardState.Matched;
        _faceUp.Clear();
    }
}
