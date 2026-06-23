import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { firstValueFrom } from 'rxjs';
import { AdminRole, ADMIN_ROLE_RANK, UserRole } from '../../../shared/utils/roles';

export interface RegisterPayload {
  userName: string;
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface RegisterResponse {
  message?: string;
}

interface LoginResponse {
  token: string;
}

interface TokenClaims {
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string | string[];
  role?: string | string[];
  orgId?: string;
  exp?: number;
}

const API_BASE = 'http://localhost:5277';
const TOKEN_KEY  = 'auth_token';

// Service

/**
 * Auth domain's data-access. This is the ONLY place that should talk to
 * the auth API or read/write the token. Other domains that need to know
 * "is the user signed in" / "what role do they have" go through these
 * read methods on the barrel-exported AuthService, rather than each
 * domain inventing its own session check.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {

  private http       = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  // Auth API calls

  /**
   * POST /api/User/login
   * Accepts email OR username — the backend handles both.
   * Returns the raw JWT string.
   */
  async signIn(emailOrUsername: string, password: string, rememberMe: boolean): Promise<string> {
    const body = { userName: emailOrUsername, password, rememberMe };
    const res  = await firstValueFrom(
      this.http.post<LoginResponse>(`${API_BASE}/api/User/login`, body)
    );
    return res.token;
  }

  /**
   * POST /api/User/register
   * On success the backend sends a confirmation email; returns { message }.
   */
  async register(payload: RegisterPayload): Promise<RegisterResponse> {
    return firstValueFrom(
      this.http.post<RegisterResponse>(`${API_BASE}/api/User/register`, {
        userName:        payload.userName,
        fullName:        payload.fullName,
        email:           payload.email,
        password:        payload.password,
        confirmPassword: payload.confirmPassword,
        // phoneNumber and address are optional — add fields to RegisterPayload
        // and pass them here if you add those inputs to the sign-up form later
      })
    );
  }

  // Token storage (SSR-safe)

  /**
   * Persists the JWT.
   * rememberMe=true  -> localStorage  (survives tab close)
   * rememberMe=false -> sessionStorage (cleared on tab close)
   * Both are no-ops on the server (SSR guard).
   */
  saveToken(token: string, rememberMe: boolean): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const storage = rememberMe ? localStorage : sessionStorage;
    storage.setItem(TOKEN_KEY, token);
  }

  /** Returns the stored JWT, or null if not signed in / on server. */
  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(TOKEN_KEY) ?? sessionStorage.getItem(TOKEN_KEY);
  }

  /** Clears the token from both storages (sign-out). */
  clearToken(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.removeItem(TOKEN_KEY);
    sessionStorage.removeItem(TOKEN_KEY);
  }

  /** True if a token is present and not expired. */
  isSignedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    const claims = this.decodeClaims(token);
    if (!claims?.exp) return true; // no exp claim -> treat as valid
    return claims.exp * 1000 > Date.now();
  }

  // Role accessors

  /**
   * Donor vs Worker, read from the decoded token claims.
   * Returns null when not signed in or the role is an admin role.
   */
  getUserRole(): UserRole | null {
    const roles = this.getRawRoles();
    if (roles.includes(UserRole.Worker)) return UserRole.Worker;
    if (roles.includes(UserRole.Donor))  return UserRole.Donor;
    return null;
  }

  /**
   * Returns the admin tier (SubAdmin / SuperAdminOfOrg / UltraSuperAdmin)
   * if this account has one, or null for regular donors/workers.
   */
  getAdminRole(): AdminRole | null {
    const roles = this.getRawRoles();
    // Check from highest to lowest so we return the most privileged tier
    for (const tier of Object.keys(ADMIN_ROLE_RANK).sort(
      (a, b) => ADMIN_ROLE_RANK[b as AdminRole] - ADMIN_ROLE_RANK[a as AdminRole]
    ) as AdminRole[]) {
      if (roles.includes(tier)) return tier;
    }
    return null;
  }

  /**
   * For SuperAdminOfOrg / SubAdmin, which org they administer.
   * Null for UltraSuperAdmin (administers all) or non-admins.
   */
  getAdministeredOrgId(): string | null {
    const token = this.getToken();
    if (!token) return null;
    return this.decodeClaims(token)?.orgId ?? null;
  }

  // Private helpers

  /**
   * Decodes the JWT payload without verifying the signature
   * (verification happens on the backend on every request).
   */
  private decodeClaims(token: string): TokenClaims | null {
    try {
      const payload = token.split('.')[1];
      const json    = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      return JSON.parse(json) as TokenClaims;
    } catch {
      return null;
    }
  }

  /** Extracts all role strings from the token as a flat array. */
  private getRawRoles(): string[] {
    const token = this.getToken();
    if (!token) return [];
    const claims = this.decodeClaims(token);
    if (!claims) return [];

    // ASP.NET Identity can use either claim URI or short "role" key
    const raw =
      claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      claims['role'] ??
      [];

    return Array.isArray(raw) ? raw : [raw];
  }
}