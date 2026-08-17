import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
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
  private readonly cdr = inject(ChangeDetectorRef);

  todos: TodoItem[] = [];
  newTodoTitle = '';
  errorMessage = '';
  isLoading = false;
  isSaving = false;
  togglingIds = new Set<number>();
  deletingIds = new Set<number>();

  ngOnInit(): void {
    this.loadTodos(true);
  }

  loadTodos(showLoadingScreen = false): void {
    if (showLoadingScreen) {
      this.isLoading = true;
      this.cdr.detectChanges();
    }
    this.errorMessage = '';

    this.todoService.getAll()
      .pipe(
          finalize(() => {
            this.isLoading = false;
            this.cdr.detectChanges(); 
          })
        )
      .subscribe({
      next: (data: TodoItem[]) => {
        this.todos = data || [];
      },
      error: () => {
        this.errorMessage = 'Unable to load todos.';
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

    this.todoService.add({ title, isCompleted: false })
      .pipe(
        finalize(() => {
          this.isSaving = false;
          this.cdr.detectChanges(); 
        })
      )
      .subscribe({
       next: () => {
          // Clear the text box and reload the list of todos
          this.newTodoTitle = '';
          this.loadTodos(); 
        },
        error: (err) => {
          console.error('API Error:', err);
          this.errorMessage = 'Unable to create todo.';
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

    // Optimistically update the UI instantly
    this.todos = this.todos.map((item) =>
      item.id === todoId ? { ...item, isCompleted: nextValue } : item
    );
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.todoService.update(todoId, updatedTodo)
      .pipe(
        finalize(() => {
          this.togglingIds.delete(todoId);
          this.cdr.detectChanges(); 
        })
      )
      .subscribe({
        next: () => {
          // The UI is already updated, so we don't need to do anything here.
        },
        error: (err) => {
          console.error('Update failed:', err);
          // Rollback the UI if the server actually threw a real error
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

    // Optimistically remove the item from the UI
    this.todos = this.todos.filter((todo) => todo.id !== todoId);
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.todoService.delete(todoId)
      .pipe(
        finalize(() => {
          this.deletingIds.delete(todoId);
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: () => {
           // Success! 
        },
        error: (err) => {
          console.error('Delete failed:', err);
          // Rollback if it fails
          this.todos = previousTodos;
          this.errorMessage = 'Unable to delete todo.';
        }
      });
  }
}
