import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
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
  editTitle = '';
  editingId: number | null = null;
  errorMessage = '';
  isLoading = false;

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.todoService.getAll().subscribe({
      next: (items) => {
        this.todos = items;
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
    if (!title) {
      return;
    }

    const todo: TodoItem = {
      title,
      isCompleted: false
    };

    this.todoService.add(todo).subscribe({
      next: (createdTodo) => {
        this.todos.unshift(createdTodo);
        this.newTodoTitle = '';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Unable to create todo.';
      }
    });
  }

  toggleTodo(todo: TodoItem): void {
    if (todo.id === undefined) {
      return;
    }

    const updatedTodo = { ...todo, isCompleted: !todo.isCompleted };

    this.todoService.update(todo.id, updatedTodo).subscribe({
      next: () => {
        todo.isCompleted = updatedTodo.isCompleted;
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Unable to update todo.';
      }
    });
  }

  startEditing(todo: TodoItem): void {
    this.editingId = todo.id ?? null;
    this.editTitle = todo.title ?? '';
  }

  saveEdit(todo: TodoItem): void {
    const title = this.editTitle.trim();
    if (!todo.id || !title) {
      return;
    }

    const updatedTodo = { ...todo, title };

    this.todoService.update(todo.id, updatedTodo).subscribe({
      next: () => {
        todo.title = title;
        this.editingId = null;
        this.editTitle = '';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Unable to update todo.';
      }
    });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.editTitle = '';
  }

  deleteTodo(id?: number): void {
    if (id === undefined) {
      return;
    }

    this.todoService.delete(id).subscribe({
      next: () => {
        this.todos = this.todos.filter((todo) => todo.id !== id);
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'Unable to delete todo.';
      }
    });
  }
}
