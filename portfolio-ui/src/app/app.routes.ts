import { Routes } from '@angular/router';
import { PortfolioComponent } from './components/portfolio/portfolio';
import { AuthComponent  } from './components/auth/auth'; // <-- Updated to your auth folder
import { AdminComponent } from './components/admin/admin';
import { authGuard } from './services/auth.guard'; // <-- Updated to your services folder

export const routes: Routes = [
  { path: '', component: PortfolioComponent },
  { path: 'login', component: AuthComponent  }, // Routes /login to your auth component
  
  // The Guard is applied here!
  { path: 'admin', component: AdminComponent, canActivate: [authGuard] },
  
  // Catch-all route for bad URLs
  { path: '**', redirectTo: '' } 
];