import { Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { loginGuard } from './guards/login.guard';
import { ChatComponent } from './chat/chat.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: 'register', loadComponent: () => import('./register/register.component').then(m => m.RegisterComponent), canActivate: [loginGuard] },
    { path: 'login', loadComponent: () => import('./login/login.component').then(m => m.LoginComponent), canActivate: [loginGuard] },
    { path: 'chat', loadComponent: () => import('./chat/chat.component').then(m => m.ChatComponent), canActivate: [authGuard] },
    { path: '**', redirectTo: 'chat', pathMatch: 'full' }
];
