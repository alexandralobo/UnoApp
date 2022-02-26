import { Injectable, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { Guest } from '../_models/guest';
import { AccountService } from './account.service';
import { Router } from '@angular/router';
import { Group } from '../_models/Group';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  gameLobby: GameLobby;  
  gameLobbies: GameLobby[] = []; 
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private gameLobbySource = new BehaviorSubject<GameLobby>({
    gameLobbyId: 0,
    gameLobbyName: null,
    drawableCards: [],    
    cardPot: [],
    lastCard: null,
    currentPlayer: null,
    gameStatus: null,
    numberOfElements: 0,
  });
  gameLobby$ = this.gameLobbySource.asObservable();
  guest: Guest;
  gameLobbyId: number;  
  players?: Group;

  constructor(private http: HttpClient, private accountService: AccountService, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;    
  }

  createHubConnection(guest: Guest, gameLobbyId: number) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'gamelobby?lobbyid=' + gameLobbyId, {
        accessTokenFactory: () => guest.token
      })
      .withAutomaticReconnect()
      .build()
    
    this.hubConnection.start().catch(error => console.log(error));
      
    this.hubConnection.on('GetGameLobby', gameLobby => {
      this.gameLobbySource.next(gameLobby);
    })

    this.hubConnection.on('UpdatedGroup', (group: Group) => {
      if (group.connections.some(x => x.username === guest.username)) {
        this.players = group;
      }
    })


    // this.hubConnection.on('GetGameLobbies', (gameLobbies : GameLobby[]) => {
    //   this.gameLobbies = gameLobbies;
    // })

    if (this.players?.connections.length == 4) {
      this.startGame(this.gameLobby);
    }
    
  }

  joinNewGame(model: any) {
    return this.http.post<number>('https://localhost:5001/api/gameLobby/joinNewLobby/' + this.guest.username, model);
  }

  getLobby(lobbyId) {
    
  } 

  async startGame(lobby) {
    return this.hubConnection.invoke('StartGame', {gameLobby: lobby})
      .catch(error => console.log(error));
  }


  

}
