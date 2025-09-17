import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiResponse } from '../Models/api-response';
import { User } from '../Models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = "http://localhost:5000/api/account";
  private token = "token"
  
  httpClient = inject(HttpClient);

  register(data: FormData) : Observable<ApiResponse<string>>
  {
    return this.httpClient.post<ApiResponse<string>>(`${this.baseUrl}/register`, data)
    .pipe(
      tap(response => 
        localStorage.setItem(this.token, response.data)
      )
    );
  } 

  login(email: string, password: string) : Observable<ApiResponse<string>>
  {
    return this.httpClient.post<ApiResponse<string>>(`${this.baseUrl}/login`, {email, password})
    .pipe(
      tap(response => {
        if(response.isSuccess)
          localStorage.setItem(this.token, response.data)

        return response;
      }
      )
    );
  }

  user(): Observable<ApiResponse<User>> {
    return this.httpClient.get<ApiResponse<User>>(`${this.baseUrl}/user`, {
      headers:{
        Authorization: `Bearer ${this.getAccessToken()}`
      }
    })
    .pipe(tap(res => {
      if(res.isSuccess)
        localStorage.setItem("user", JSON.stringify(res.data));
      
      return res;
    }));
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.token);
  }

  isLoggedIn(): boolean {
    return this.getAccessToken() !== null;
  }

  logout(){
    localStorage.removeItem(this.token);
    localStorage.removeItem("user");
  }

  get currentLoggedUser(): User | null {
    const user = JSON.parse(localStorage.getItem("user") || 'null');
    return user;
  }
}
