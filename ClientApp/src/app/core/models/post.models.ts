export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface Post {
  id: number;
  title: string;
  content: string;
  coverImageUrl?: string | null;
  categoryId: number;
  categoryName: string;
  authorId: string;
  authorEmail: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface PostRequest {
  title: string;
  content: string;
  categoryId: number;
  coverImageUrl?: string | null;
}

export interface Comment {
  id: number;
  content: string;
  blogPostId: number;
  authorId: string;
  authorEmail: string;
  createdAt: string;
}

export interface CommentRequest {
  content: string;
}

export interface UserSummary {
  id: string;
  email: string;
  fullName: string;
  roles: string[];
}
