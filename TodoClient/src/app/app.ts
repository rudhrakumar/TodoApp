import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs';
import { TodoItem } from './models/todo.item';
import { TodoService } from './services/todo.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly todoService = inject(TodoService);

  todos: TodoItem[] = [];
  newTodoTitle = '';
  errorMessage = '';
  isLoading = false;
  isSaving = false;
  togglingIds = new Set<number>();
  deletingIds = new Set<number>();

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.todoService.getAll().subscribe({
      next: (data: TodoItem[]) => {
        this.todos = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load todos.';
        this.isLoading = false;
      }
    });
  }

  addTodo(): void {
    const title = this.newTodoTitle.trim();
    if (!title || this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const optimisticTodo: TodoItem = {
      id: Math.max(0, ...this.todos.map((todo) => todo.id ?? 0)) + 1,
      title,
      isCompleted: false,
    };

    this.todos = [...this.todos, optimisticTodo];
    this.newTodoTitle = '';

    this.todoService.add({ title, isCompleted: false }).subscribe({
      next: () => {
        this.isSaving = false;
        this.loadTodos();
      },
      error: () => {
        this.todos = this.todos.filter((todo) => todo.id !== optimisticTodo.id);
        this.isSaving = false;
        this.errorMessage = 'Unable to create todo.';
      },
      complete: () => {
        this.isSaving = false;
      }
    });
  }

  toggleTodo(todo: TodoItem): void {
    if (todo.id === undefined || this.togglingIds.has(todo.id)) {
      return;
    }

    const todoId = todo.id;
    const nextValue = !todo.isCompleted;
    const previousTodos = [...this.todos];
    const updatedTodo = { ...todo, isCompleted: nextValue };
    this.togglingIds.add(todoId);

    this.todos = this.todos.map((item) =>
      item.id === todoId ? { ...item, isCompleted: nextValue } : item
    );
    this.errorMessage = '';

    this.todoService.update(todoId, updatedTodo)
      .pipe(
        finalize(() => {
          this.togglingIds.delete(todoId);
        })
      )
      .subscribe({
        next: () => {
          this.todos = this.todos.map((item) =>
            item.id === todoId ? { ...item, isCompleted: nextValue } : item
          );
        },
        error: () => {
          this.todos = previousTodos;
          this.errorMessage = 'Unable to update todo.';
        }
      });
  }

  deleteTodo(id?: number): void {
    if (id === undefined || this.deletingIds.has(id)) {
      return;
    }

    const todoId = id;
    this.deletingIds.add(todoId);
    const previousTodos = [...this.todos];

    this.todos = this.todos.filter((todo) => todo.id !== todoId);
    this.errorMessage = '';

    this.todoService.delete(todoId)
      .pipe(
        finalize(() => {
          this.deletingIds.delete(todoId);
        })
      )
      .subscribe({
        next: () => {
          this.todos = this.todos.filter((todo) => todo.id !== todoId);
        },
        error: () => {
          this.todos = previousTodos;
          this.errorMessage = 'Unable to delete todo.';
        }
      });
  }
}
