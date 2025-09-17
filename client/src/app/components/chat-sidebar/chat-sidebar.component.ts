import { Component, inject, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { A, c } from "../../../../node_modules/@angular/cdk/a11y-module.d-DBHGyKoh";
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { TitleCasePipe } from '@angular/common';
import { ChatService } from '../../services/chat.service';
import { User } from '../../Models/user';


@Component({
  selector: 'app-chat-sidebar',
  imports: [MatIconModule, MatMenuModule, TitleCasePipe],
  templateUrl: './chat-sidebar.component.html',
  styles: ``
})
export class ChatSidebarComponent implements OnInit {

  authService = inject(AuthService);
  router = inject(Router);
  chatService = inject(ChatService);

  loggedUser = this.authService.currentLoggedUser;
  onlineUsers = this.chatService.onlineUsers;

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
    this.chatService.disconect();
  }

  openChatWindow(user: User){
    this.chatService.currentOppendChat.set(user);
  }
  
  ngOnInit(): void {
   this.chatService.startConnection(this.authService.getAccessToken()!); 
  }
}
