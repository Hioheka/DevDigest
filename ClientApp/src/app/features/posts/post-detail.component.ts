import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BlogApiService } from '../../core/services/blog-api.service';
import { Post, Comment } from '../../core/models/post.models';
import { AuthService } from '../../core/services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.scss',
})
export class PostDetailComponent implements OnInit, OnDestroy {
  readonly post = signal<Post | null>(null);
  readonly comments = signal<Comment[]>([]);
  readonly loading = signal<boolean>(false);
  readonly canComment = this.authService.isAuthenticated;
  private subscription?: Subscription;

  readonly form = this.fb.group({
    content: ['', [Validators.required, Validators.minLength(3)]],
  });

  constructor(
    private route: ActivatedRoute,
    private blogApiService: BlogApiService,
    private fb: FormBuilder,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.subscription = this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (id) {
        this.loadPost(id);
        this.loadComments(id);
      }
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  submitComment() {
    if (!this.form.valid || !this.post()) {
      return;
    }

    this.blogApiService
      .addComment(this.post()!.id, { content: this.form.value.content ?? '' })
      .subscribe({
        next: (comment) => {
          this.comments.set([comment, ...this.comments()]);
          this.form.reset();
        },
      });
  }

  private loadPost(id: number) {
    this.loading.set(true);
    this.blogApiService.getPost(id).subscribe({
      next: (post) => {
        this.post.set(post);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private loadComments(id: number) {
    this.blogApiService.getComments(id).subscribe({
      next: (comments) => this.comments.set(comments),
    });
  }
}
