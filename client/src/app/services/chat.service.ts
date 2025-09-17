import { inject, Injectable, signal } from '@angular/core';
import { User } from '../Models/user';
import { AuthService } from './auth.service';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private authService = inject(AuthService);
  private hubUrl = 'http://localhost:5000/hubs/chat';
  onlineUsers = signal<User[]>([]);

  currentOppendChat = signal<User | null>(null);

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
  }

  disconect() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((err) => console.log(err));
    }
  }
}
