import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ApiResponse } from '../Models/api-response';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [MatFormFieldModule, MatInputModule, FormsModule, MatIconModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  email!: string;
  password!: string;

  private authService = inject(AuthService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  hide = signal(true);

  togglePasswordVisibility(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  login() {
    this.authService.login(this.email, this.password).subscribe({
      next: () => {
        this.authService.user().subscribe();
        this.snackBar.open("Login efetuado com sucesso", 'Close', { duration: 3000 });
      },
      error: (error) => {
        let err = error.error as ApiResponse<string>;
        this.snackBar.open(err.error, 'Close', { duration: 3000 });
      },
      complete: () => {
        this.router.navigate(['/chat']);
      }
    });
  }
}
