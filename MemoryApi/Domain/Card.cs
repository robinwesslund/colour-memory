namespace MemoryApi.Domain;

public sealed class Card
{
    public Card(int index, string symbol) => (Index, Symbol) = (index, symbol);

    public int Index { get; }
    public string Symbol { get; }
    public CardState State { get; internal set; } = CardState.Hidden;
}
