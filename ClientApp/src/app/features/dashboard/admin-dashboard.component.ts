import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { BlogApiService } from '../../core/services/blog-api.service';
import { Category, Post, UserSummary } from '../../core/models/post.models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatTabsModule,
    MatIconModule,
  ],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent implements OnInit {
  readonly categories = signal<Category[]>([]);
  readonly posts = signal<Post[]>([]);
  readonly users = signal<UserSummary[]>([]);
  readonly displayedCategoryColumns = ['name', 'description', 'actions'];
  readonly displayedPostColumns = ['title', 'author', 'category', 'created', 'actions'];
  readonly displayedUserColumns = ['fullName', 'email', 'roles'];
  readonly editingCategoryId = signal<number | null>(null);

  readonly categoryForm = this.fb.group({
    name: ['', Validators.required],
    description: [''],
  });

  constructor(private fb: FormBuilder, private blogApiService: BlogApiService) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadPosts();
    this.loadUsers();
  }

  saveCategory() {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const payload = this.categoryForm.getRawValue();
    const request = this.editingCategoryId()
      ? this.blogApiService.updateCategory(this.editingCategoryId()!, payload)
      : this.blogApiService.createCategory(payload);

    request.subscribe({
      next: () => {
        this.resetCategoryForm();
        this.loadCategories();
      },
    });
  }

  editCategory(category: Category) {
    this.editingCategoryId.set(category.id);
    this.categoryForm.patchValue({
      name: category.name,
      description: category.description ?? '',
    });
  }

  deleteCategory(category: Category) {
    if (!confirm(`${category.name} kategorisi silinecek. Onaylıyor musunuz?`)) {
      return;
    }

    this.blogApiService.deleteCategory(category.id).subscribe({
      next: () => this.loadCategories(),
    });
  }

  deletePost(post: Post) {
    if (!confirm(`${post.title} gönderisi silinecek. Onaylıyor musunuz?`)) {
      return;
    }

    this.blogApiService.deletePost(post.id).subscribe({
      next: () => this.loadPosts(),
    });
  }

  resetCategoryForm() {
    this.categoryForm.reset();
    this.editingCategoryId.set(null);
  }

  private loadCategories() {
    this.blogApiService.getCategories().subscribe({
      next: (categories) => this.categories.set(categories),
    });
  }

  private loadPosts() {
    this.blogApiService.getPosts().subscribe({
      next: (posts) => this.posts.set(posts),
    });
  }

  private loadUsers() {
    this.blogApiService.getUsers().subscribe({
      next: (users) => this.users.set(users),
    });
  }
}
