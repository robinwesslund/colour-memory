import { Component, OnInit, inject, signal } from '@angular/core';
import { GameService } from './game.service';
import { Game } from './game.model';

const MISMATCH_DELAY_MS = 1000;

@Component({
  selector: 'app-board',
  standalone: true,
  template: `
    <header>
      <h1>Memory</h1>
      <p class="moves">Moves: {{ game()?.moves ?? 0 }}</p>
      <button class="new" (click)="newGame()">New game</button>
    </header>

    @if (error()) {
      <p class="error" role="alert">{{ error() }}</p>
    }

    <div class="grid">
      @for (card of game()?.cards; track card.index) {
        <button
          class="card"
          [class.revealed]="card.state !== 'Hidden'"
          [class.matched]="card.state === 'Matched'"
          [disabled]="busy() || card.state !== 'Hidden'"
          [attr.aria-label]="card.state === 'Hidden' ? 'Face-down card ' + (card.index + 1) : card.symbol"
          (click)="flip(card.index)">
          <span class="face back"></span>
          <span class="face front">{{ card.symbol }}</span>
        </button>
      }
    </div>

    @if (game()?.isWon) {
      <p class="won">All pairs found in {{ game()!.moves }} moves.</p>
    }
  `,
  styles: [`
    :host { display: block; max-width: 420px; margin: 2rem auto; padding: 0 1rem; font-family: system-ui, sans-serif; }
    header { display: flex; align-items: baseline; gap: 1rem; margin-bottom: 1rem; }
    h1 { margin: 0; font-size: 1.6rem; flex: 1; }
    .moves { margin: 0; color: #555; }
    .new { padding: .4rem .8rem; cursor: pointer; }
    .grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }
    .card { position: relative; aspect-ratio: 1; padding: 0; border: 0; background: none; cursor: pointer;
            perspective: 600px; font-size: 2rem; }
    .card:disabled { cursor: default; }
    .card:focus-visible { outline: 3px solid #e0a100; outline-offset: 2px; border-radius: 10px; }
    .face { position: absolute; inset: 0; display: grid; place-items: center; border-radius: 10px;
            backface-visibility: hidden; transition: transform .35s; }
    .back  { background: #1f5f5b; }
    .front { background: #fffaf0; border: 2px solid #1f5f5b; transform: rotateY(180deg); }
    .revealed .back  { transform: rotateY(180deg); }
    .revealed .front { transform: rotateY(0); }
    .matched .front { background: #d9efd5; border-color: #3b8a3b; }
    .won { margin-top: 1rem; font-weight: 600; }
    .error { color: #b00020; }
    @media (prefers-reduced-motion: reduce) { .face { transition: none; } }
  `]
})
export class BoardComponent implements OnInit {
  private svc = inject(GameService);

  game = signal<Game | null>(null);
  busy = signal(false);
  error = signal<string | null>(null);

  ngOnInit() { 
    this.newGame(); 
  }

  newGame() {
    this.error.set(null);
    this.busy.set(false);
    this.svc.create().subscribe({
      next: g => this.game.set(g),
      error: () => this.error.set('Cannot reach the server. Is the API running on http://localhost:5000?')
    });
  }

  flip(index: number) {
    const current = this.game();
    if (!current || this.busy()) return;

    this.svc.flip(current.id, index).subscribe({
      next: g => {
        this.game.set(g);
        this.error.set(null);

        // Two revealed cards that did not match: show them briefly, then hide locally.
        // The server hides them lazily on the next flip, so both sides agree.
        if (g.cards.filter(c => c.state === 'Revealed').length === 2) {
          this.busy.set(true);
          setTimeout(() => {
            this.game.update(prev => prev && ({
              ...prev,
              cards: prev.cards.map(c =>
                c.state === 'Revealed' ? { ...c, state: 'Hidden' as const, symbol: null } : c)
            }));
            this.busy.set(false);
          }, MISMATCH_DELAY_MS);
        }
      },
      error: e => this.error.set(e?.error?.error ?? 'Something went wrong.')
    });
  }
}
