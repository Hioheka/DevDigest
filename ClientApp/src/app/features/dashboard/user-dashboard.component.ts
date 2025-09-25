import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { BlogApiService } from '../../core/services/blog-api.service';
import { Post, Category } from '../../core/models/post.models';

@Component({
  selector: 'app-user-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './user-dashboard.component.html',
  styleUrl: './user-dashboard.component.scss',
})
export class UserDashboardComponent implements OnInit {
  readonly posts = signal<Post[]>([]);
  readonly categories = signal<Category[]>([]);
  readonly editingPostId = signal<number | null>(null);
  readonly displayedColumns = ['title', 'category', 'created', 'actions'];

  readonly form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    content: ['', [Validators.required, Validators.minLength(10)]],
    categoryId: ['', Validators.required],
    coverImageUrl: [''],
  });

  constructor(private fb: FormBuilder, private blogApiService: BlogApiService) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadPosts();
  }

  savePost() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload = {
      title: raw.title ?? '',
      content: raw.content ?? '',
      categoryId: Number(raw.categoryId),
      coverImageUrl: raw.coverImageUrl ?? undefined,
    };
    const request = this.editingPostId()
      ? this.blogApiService.updatePost(this.editingPostId()!, payload)
      : this.blogApiService.createPost(payload);

    request.subscribe({
      next: () => {
        this.resetForm();
        this.loadPosts();
      },
    });
  }

  editPost(post: Post) {
    this.editingPostId.set(post.id);
    this.form.patchValue({
      title: post.title,
      content: post.content,
      categoryId: post.categoryId,
      coverImageUrl: post.coverImageUrl ?? '',
    });
  }

  deletePost(post: Post) {
    if (!confirm(`${post.title} silinecek. Onaylıyor musunuz?`)) {
      return;
    }

    this.blogApiService.deletePost(post.id).subscribe({
      next: () => this.loadPosts(),
    });
  }

  cancelEdit() {
    this.resetForm();
  }

  private resetForm() {
    this.editingPostId.set(null);
    this.form.reset();
  }

  private loadPosts() {
    this.blogApiService.getMyPosts().subscribe({
      next: (posts) => this.posts.set(posts),
    });
  }

  private loadCategories() {
    this.blogApiService.getCategories().subscribe({
      next: (categories) => this.categories.set(categories),
    });
  }
}
