import { Injectable, computed, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';

const ACCESS_TOKEN_KEY = 'devdigest.accessToken';
const REFRESH_TOKEN_KEY = 'devdigest.refreshToken';
const USER_KEY = 'devdigest.user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private readonly userSignal = signal<AuthResponse | null>(this.restoreUser());

  readonly currentUser = computed(() => this.userSignal());
  readonly isAuthenticated = computed(() => !!this.userSignal());
  readonly isAdmin = computed(() => this.userSignal()?.role === 'Admin');

  constructor(private http: HttpClient) {}

  login(request: LoginRequest) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
      .pipe(tap((response) => this.persistSession(response)));
  }

  register(request: RegisterRequest) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/register`, request)
      .pipe(tap((response) => this.persistSession(response)));
  }

  refreshToken(refreshToken: string) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/refresh`, { refreshToken })
      .pipe(tap((response) => this.persistSession(response)));
  }

  logout() {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this.userSignal.set(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private persistSession(response: AuthResponse) {
    localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(response));
    this.userSignal.set(response);
  }

  private restoreUser(): AuthResponse | null {
    const stored = localStorage.getItem(USER_KEY);
    if (!stored) {
      return null;
    }

    try {
      return JSON.parse(stored) as AuthResponse;
    } catch {
      return null;
    }
  }
}
