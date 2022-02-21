import { Injectable, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { Guest } from '../_models/guest';
import { AccountService } from './account.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  gameLobby: GameLobby[] = [];  
  //baseUrl = environment.apiUrl;
  //hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  guest: Guest;
  

  constructor(private http: HttpClient, private accountService: AccountService, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  createHubConnection(guest: Guest) 
  {

  }

  joinNewGame(model: any) {
    return this.http.post('https://localhost:5001/api/gameLobby/joinNewLobby/' + this.guest.username, model).pipe();
  }
  

 
}
