import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export interface LoginRequest {
  userName: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterRequest {
  userName: string;
  fullName: string;
  email: string;
  phoneNumber?: string;
  password: string;
  confirmPassword: string;
  address?: string;
}

export interface LoginResponse {
  token: string;
  message?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = 'http://localhost:5277/api/User';

  constructor(private http: HttpClient) {}

  async signIn(userName: string, password: string, rememberMe = false): Promise<string> {
    const body: LoginRequest = { userName, password, rememberMe };
    const response = await firstValueFrom(
      this.http.post<LoginResponse>(`${this.baseUrl}/login`, body)
    );
    return response.token;
  }

  async register(data: RegisterRequest): Promise<{ message: string }> {
    return firstValueFrom(
      this.http.post<{ message: string }>(`${this.baseUrl}/register`, data)
    );
  }

  saveToken(token: string, remember: boolean): void {
    if (remember) {
      localStorage.setItem('auth_token', token);
    } else {
      sessionStorage.setItem('auth_token', token);
    }
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token') ?? sessionStorage.getItem('auth_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem('auth_token');
    sessionStorage.removeItem('auth_token');
  }
}