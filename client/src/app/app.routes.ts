import { Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { loginGuard } from './guards/login.guard';
import { ChatComponent } from './chat/chat.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
    { path: 'register', component: RegisterComponent, canActivate: [loginGuard] },
    { path: 'login', component: LoginComponent, canActivate: [loginGuard] },
    { path: 'chat', component: ChatComponent, canActivate: [authGuard] },
    { path: '**', redirectTo: 'chat', pathMatch: 'full' }
];
