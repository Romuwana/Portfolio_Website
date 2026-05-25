import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service'; // <-- Import the new service!

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './auth.html',
  styleUrl: './auth.css'
})
export class AuthComponent {
  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  // Inject the AuthService instead of HttpClient
  constructor(private authService: AuthService, private router: Router) {}

  onLogin(): void {
    if (!this.username || !this.password) {
      this.errorMessage = 'System Exception: Credentials required.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Use the clean service method
    this.authService.login(this.username, this.password).subscribe({
      next: () => {
        // The service already saved the token, just navigate!
        this.router.navigate(['/admin']);
      },
      error: (err) => {
        console.error('Login Error:', err);
        if (err.status === 401) {
          this.errorMessage = 'Access Denied: Invalid credentials.';
        } else {
          this.errorMessage = 'System Error: Cannot reach the authentication server.';
        }
        this.isLoading = false;
      }
    });
  }
}