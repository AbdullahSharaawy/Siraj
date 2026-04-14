import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../Services/Authentication';

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

  async onSubmit(): Promise<void> {
    this.apiError = '';
    if (!this.validateAll()) return;

    this.isLoading = true;
    try {
      // Backend Login endpoint accepts userName OR email — we pass whatever the user typed
      const token = await this.authService.signIn(this.email, this.password, this.rememberMe);
      this.authService.saveToken(token, this.rememberMe);
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
}