import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatButtonModule } from '@angular/material/button';
import { BlogApiService } from '../../core/services/blog-api.service';
import { Post } from '../../core/models/post.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatChipsModule, MatButtonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  readonly posts = signal<Post[]>([]);
  readonly loading = signal<boolean>(false);
  constructor(private blogApiService: BlogApiService) {}

  ngOnInit(): void {
    this.fetchPosts();
  }

  fetchPosts() {
    this.loading.set(true);
    this.blogApiService.getPosts().subscribe({
      next: (posts) => {
        this.posts.set(posts);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
