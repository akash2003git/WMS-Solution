import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';
import { ResetPasswordRequest } from '../models/reset-password.model';
import { ApiResponse } from '../../../core/models/api-response.model';
import { CurrentUser } from '../../../core/models/current-user.model';
import { StorageService } from '../../../core/services/storage';
import { jwtDecode } from 'jwt-decode';
import { JwtPayload } from '../../../core/models/jwt-payload.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly storageService = inject(StorageService);

  private readonly apiUrl = `${environment.apiUrl}/Auth`;

  private readonly currentUserSubject =
    new BehaviorSubject<CurrentUser | null>(
      this.initializeUser()
    );

  private initializeUser(): CurrentUser | null {
    const token = this.storageService.getToken();

    const user = this.storageService.getUser<CurrentUser>();
    if (!token || !user) {
      return null;
    }

    if (this.isTokenExpired(token)) {
      this.logout();
      return null;
    }

    return user;
  }

  currentUser$ = this.currentUserSubject.asObservable();

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiUrl}/login`,
      request
    )
  }

  logout(): void {
    this.storageService.clear();
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return this.storageService.getToken();
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) {
      return false;
    }

    const isExpired = this.isTokenExpired(token);
    if (isExpired) {
      this.logout();
      return false;
    }

    return true;
  }

  getCurrentUser(): CurrentUser | null {
    return this.currentUserSubject.value;
  }

  isTokenExpired(token: string): boolean {
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      if (!decoded.exp) {
        return true;
      }
      const expiration = decoded.exp * 1000;
      return Date.now() > expiration;
    } catch {
      return true;
    }
  }

  resetPassword(request: ResetPasswordRequest) {
    return this.http.post<ApiResponse<string>>(
      `${this.apiUrl}/reset-password`,
      request
    );
  }

  setSession(response: LoginResponse): void {
    const user: CurrentUser = {
      userId: response.userId,
      username: response.username,
      role: response.role,
      token: response.token
    };

    this.storageService.setToken(response.token);
    this.storageService.setUser(user);
    this.currentUserSubject.next(user);
  }
}
