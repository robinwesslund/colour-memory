export type CardState = 'Hidden' | 'Revealed' | 'Matched';

export interface Card {
  index: number;
  state: CardState;
  symbol: string | null; // face down is null
}

export interface Game {
  id: string;
  moves: number;
  isWon: boolean;
  cards: Card[];
}
