import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../Services/Authentication';

@Component({
  selector: 'app-sign-up',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './sign-up.html',
  styleUrl: './sign-up.css',
})
export class SignUp {

  email           = '';
  username        = '';
  fullName        = '';
  password        = '';
  confirmPassword = '';
  rememberMe      = false;
  showPassword    = false;
  showConfirm     = false;
  isLoading       = false;

  emailError           = '';
  usernameError        = '';
  passwordError        = '';
  confirmPasswordError = '';
  apiError             = '';
  successMessage       = '';

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

  validateUsername(): void {
    this.usernameError = !this.username
      ? 'Username is required.'
      : this.username.length < 3
      ? 'Username must be at least 3 characters.'
      : '';
  }

  validateConfirmPassword(): void {
    this.confirmPasswordError = !this.confirmPassword
      ? 'Please confirm your password.'
      : this.confirmPassword !== this.password
      ? 'Passwords do not match.'
      : '';
  }

  private validateAll(): boolean {
    this.validateEmail();
    this.validateUsername();
    this.passwordError = !this.password
      ? 'Password is required.'
      : this.password.length < 6
      ? 'Password must be at least 6 characters.'
      : '';
    this.validateConfirmPassword();
    return !this.emailError && !this.usernameError && !this.passwordError && !this.confirmPasswordError;
  }

  async onSubmit(): Promise<void> {
    this.apiError = '';
    this.successMessage = '';
    if (!this.validateAll()) return;

    this.isLoading = true;
    try {
      const response = await this.authService.register({
        userName: this.username,
        fullName: this.fullName || this.username,
        email: this.email,
        password: this.password,
        confirmPassword: this.confirmPassword,
      });

      this.successMessage = response.message ?? 'Registration successful! Please check your email to confirm your account.';
      setTimeout(() => this.router.navigate(['/sign-in']), 3000);
    } catch (err: any) {
      const errors = err?.error?.errors;
      this.apiError = errors
        ? Object.values(errors).flat().join(' ')
        : err?.error?.message || 'Registration failed. Please try again.';
    } finally {
      this.isLoading = false;
    }
  }
}