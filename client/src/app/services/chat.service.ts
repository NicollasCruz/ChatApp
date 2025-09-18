import { inject, Injectable, signal } from '@angular/core';
import { User } from '../Models/user';
import { AuthService } from './auth.service';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { Message } from '../Models/Message';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private authService = inject(AuthService);
  private hubUrl = 'http://localhost:5000/hubs/chat';
  onlineUsers = signal<User[]>([]);

  currentOppendChat = signal<User | null>(null);

  chatMessages = signal<Message[] | null>(null);

  isLoading = signal<boolean>(true);

  private hubConnection?: HubConnection;

  startConnection(token: string, senderId?: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}?senderId=${senderId || ''}`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
      })
      .catch((err) => console.log('Error while starting connection: ' + err));

    this.hubConnection.on('OnlineUsers', (users: User[]) => {
      this.onlineUsers.update(() =>
        users.filter(
          (user) =>
            user.userName !== this.authService.currentLoggedUser?.userName
        )
      );
    });

    this.hubConnection!.on("ReceiveMessageList", (message) => {
      this.chatMessages.update(messages => [...message, messages]);
      this.isLoading.update(() => false);
    });
  }

  loadMessages(pageNumber: number){
    this.hubConnection?.invoke("LoadMessages", this.currentOppendChat()?.id, pageNumber)
    .then(res => console.log(res))
    .catch()
    .finally(() => this.isLoading.update(() => false));
  }



  disconect() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((err) => console.log(err));
    }
  }

  status(userName: string) : string{
    const currentChatUser = this.currentOppendChat();
    if(!currentChatUser)
       return 'offline';
    
    const onlineUser = this.onlineUsers().find(u => u.userName === userName);

    return onlineUser?.isTyping ? "Digitando..." : this.isUserOnline();
  }

  isUserOnline(): string {
    let onlineUser = this.onlineUsers().find(
      (u) => u.userName === this.currentOppendChat()?.userName
    );
    return onlineUser?.isOnline ? 'online' : 'offline';
  }
}
