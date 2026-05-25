import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Make sure this matches your C# backend route!
  private apiUrl = 'https://portfolio-website-2prc.onrender.com/api/auth'; 

  constructor(private http: HttpClient, private router: Router) {}

  /**
   * Sends credentials to the C# backend and stores the JWT token if successful.
   */
  login(username: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { username, password }).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem('adminToken', response.token);
        }
      })
    );
  }

  /**
   * Destroys the token and kicks the user back to the public site.
   */
  logout(): void {
    localStorage.removeItem('adminToken');
    this.router.navigate(['/']);
  }

  /**
   * Retrieves the current token.
   */
  getToken(): string | null {
    return localStorage.getItem('adminToken');
  }

  /**
   * Checks if the user is currently authenticated (has a token).
   */
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
