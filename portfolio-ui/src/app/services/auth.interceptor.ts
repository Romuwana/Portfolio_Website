import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('adminToken');
  const router = inject(Router);

  // Clone the request to add the authentication header
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  // Send the request and listen for security errors
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // If the C# API rejects the token (Expired or Invalid)
      if (error.status === 401) {
        localStorage.removeItem('adminToken');
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};