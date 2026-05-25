import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './services/auth.interceptor'; // <-- Updated to your services folder

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // This injects the interceptor into every HTTP request globally!
    provideHttpClient(withInterceptors([authInterceptor])) 
  ]
};