import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '/home/mohamed/Documents/TheCharity/Website/FrontEnd/src/app/Services/Authentication';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sign-in',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './sign-in.html',
  styleUrl: './sign-in.css',
})
export class SignIn {

  email        = '';
  password     = '';
  rememberMe   = false;
  showPassword = false;
  isLoading    = false;

  emailError    = '';
  passwordError = '';
  apiError      = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  // Validation
  validateEmail(): void {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    this.emailError = !this.email
      ? 'Email is required.'
      : !emailRegex.test(this.email)
      ? 'Please enter a valid email address.'
      : '';
  }

  private validateAll(): boolean {
    this.validateEmail();
    this.passwordError = !this.password
      ? 'Password is required.'
      : this.password.length < 6
      ? 'Password must be at least 6 characters.'
      : '';
    return !this.emailError && !this.passwordError;
  }

  // Email / password submit
  async onSubmit(): Promise<void> {
    this.apiError = '';
    if (!this.validateAll()) return;

    this.isLoading = true;
    try {
      const token = await this.authService.signIn(this.email, this.password);
      if (this.rememberMe) {
        localStorage.setItem('auth_token', token);
      } else {
        sessionStorage.setItem('auth_token', token);
      }
      this.router.navigate(['/dashboard']);
    } catch (err: any) {
      this.apiError =
        err?.error?.message ||
        err?.message ||
        'Sign-in failed. Please check your credentials and try again.';
    } finally {
      this.isLoading = false;
    }
  }

  // Google OAuth
  loginWithGoogle(): void {
    const returnUrl = encodeURIComponent(window.location.href);
    window.location.href =
      `/api/ExternalLogin/external-login?provider=Google&returnUrl=${returnUrl}`;
  }
}