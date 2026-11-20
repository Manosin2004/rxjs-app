import { Component, inject } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { HttpClient, HttpParams } from '@angular/common/http';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { catchError, combineLatest, debounceTime, distinctUntilChanged, map, of, startWith, switchMap } from 'rxjs';

interface Movie {
  id: number;
  title: string;
  genre: string;
  year: number;
  rating: number;
}

@Component({
  selector: 'app-root',
  imports: [AsyncPipe, ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  private http = inject(HttpClient);
  private api = 'http://localhost:7001/api';

  genres = ['All', 'Action', 'Drama', 'Crime', 'Sci-Fi', 'Thriller'];
  ratings = [
    { label: 'Any rating', value: 0 },
    { label: '8.0+', value: 8 },
    { label: '8.5+', value: 8.5 },
    { label: '9.0+', value: 9 },
  ];

  search = new FormControl('', { nonNullable: true });
  genre = new FormControl('All', { nonNullable: true });
  rating = new FormControl(0, { nonNullable: true });

  movies$ = combineLatest([
    this.search.valueChanges.pipe(startWith(''), debounceTime(300), distinctUntilChanged()),
    this.genre.valueChanges.pipe(startWith('All')),
    this.rating.valueChanges.pipe(startWith(0)),
  ]).pipe(
    switchMap(([term, genre, rating]) => {
      const t = term.trim();
      const req = t
        ? this.http
            .get<Movie[]>(`${this.api}/SearchMovies?search=${encodeURIComponent(t)}`)
            .pipe(
              map((list) =>
                list.filter((m) => (genre === 'All' || m.genre === genre) && m.rating >= rating)
              )
            )
        : this.http.get<Movie[]>(`${this.api}/GetMovieRecommendations`, {
            params: genre === 'All'
              ? new HttpParams().set('rating', rating)
              : new HttpParams().set('rating', rating).set('genre', genre),
          });
      return req.pipe(catchError(() => of([] as Movie[])), startWith(null));
    })
  );
}