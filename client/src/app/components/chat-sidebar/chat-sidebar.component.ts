import { Component, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { A, c } from "../../../../node_modules/@angular/cdk/a11y-module.d-DBHGyKoh";
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-chat-sidebar',
  imports: [MatIconModule, MatMenuModule],
  templateUrl: './chat-sidebar.component.html',
  styles: ``
})
export class ChatSidebarComponent {

  authService = inject(AuthService);
  router = inject(Router);

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
