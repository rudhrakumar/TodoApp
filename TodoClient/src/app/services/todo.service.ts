import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TodoItem } from '../models/todo.item';

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  private readonly apiUrl = 'http://localhost:5001/todo';

  constructor(private http: HttpClient) {}

  getAll(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(`${this.apiUrl}`);
  }

  add(todo: TodoItem): Observable<any> {
    return this.http.post(this.apiUrl, todo, { responseType: 'text'});
  }

  update(id: number, todo: TodoItem): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, todo, { responseType: 'text'});
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { responseType: 'text' });
  }
}
