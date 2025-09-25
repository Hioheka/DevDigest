import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import {
  Category,
  Comment,
  CommentRequest,
  Post,
  PostRequest,
  UserSummary,
} from '../models/post.models';

@Injectable({ providedIn: 'root' })
export class BlogApiService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getCategories() {
    return this.http.get<Category[]>(`${this.apiUrl}/categories`);
  }

  createCategory(request: Partial<Category>) {
    return this.http.post<Category>(`${this.apiUrl}/categories`, request);
  }

  updateCategory(id: number, request: Partial<Category>) {
    return this.http.put<Category>(`${this.apiUrl}/categories/${id}`, request);
  }

  deleteCategory(id: number) {
    return this.http.delete(`${this.apiUrl}/categories/${id}`);
  }

  getPosts() {
    return this.http.get<Post[]>(`${this.apiUrl}/posts`);
  }

  getMyPosts() {
    return this.http.get<Post[]>(`${this.apiUrl}/posts/my`);
  }

  getPost(id: number) {
    return this.http.get<Post>(`${this.apiUrl}/posts/${id}`);
  }

  createPost(request: PostRequest) {
    return this.http.post<Post>(`${this.apiUrl}/posts`, request);
  }

  updatePost(id: number, request: PostRequest) {
    return this.http.put<Post>(`${this.apiUrl}/posts/${id}`, request);
  }

  deletePost(id: number) {
    return this.http.delete(`${this.apiUrl}/posts/${id}`);
  }

  getComments(postId: number) {
    return this.http.get<Comment[]>(`${this.apiUrl}/posts/${postId}/comments`);
  }

  addComment(postId: number, request: CommentRequest) {
    return this.http.post<Comment>(`${this.apiUrl}/posts/${postId}/comments`, request);
  }

  deleteComment(postId: number, commentId: number) {
    return this.http.delete(`${this.apiUrl}/posts/${postId}/comments/${commentId}`);
  }

  getUsers() {
    return this.http.get<UserSummary[]>(`${this.apiUrl}/users`);
  }
}
