import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Game } from './game.model';

@Injectable({ providedIn: 'root' })
export class GameService {
  private http = inject(HttpClient);
  private base = '/api/games'; // localhost proxy

  create(): Observable<Game> {
    return this.http.post<Game>(this.base, {});
  }

  flip(id: string, index: number): Observable<Game> {
    return this.http.post<Game>(`${this.base}/${id}/flip/${index}`, {});
  }
}
