import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('adminToken');

  // If they have a token, let them in
  if (token) {
    return true;
  }

  // Otherwise, kick them back to the auth component
  router.navigate(['/login']);
  return false;
};