import { Injectable, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { Guest } from '../_models/guest';
import { AccountService } from './account.service';
import { Router } from '@angular/router';
import { Group } from '../_models/group';
import { Connection } from '../_models/connection';
import { CardService } from './card.service';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  gameLobby: GameLobby;
  gameLobbies: GameLobby[] = [];
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  started: boolean = false;

  // private gameLobbySource = new BehaviorSubject<GameLobby>({
  //   gameLobbyId: 0,
  //   gameLobbyName: null,
  //   drawableCards: [],
  //   cardPot: [],
  //   lastCard: null,
  //   currentPlayer: null,
  //   gameStatus: null,
  //   numberOfElements: 0,
  // });
  private gameLobbySource = new BehaviorSubject<GameLobby[]>([]);
  gameLobby$ = this.gameLobbySource.asObservable();

  private playersSource = new BehaviorSubject<Connection[]>([]);
  players$ = this.playersSource.asObservable();;

  guest: Guest;
  gameLobbyId: number;
  nrOfElements: number;

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private cardService: CardService,
    private router: Router) {
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
    this.hubConnection.serverTimeoutInMilliseconds = 100000; // 100 second

    this.hubConnection.on('GetGameLobby', (gameLobby: GameLobby) => {  
      this.gameLobbySource.next([gameLobby]);
      this.nrOfElements = gameLobby.numberOfElements;
      this.gameLobbyId = gameLobby.gameLobbyId;
    })   
    
    this.hubConnection.on('UpdatedGroup', (group: Group) => {
        this.playersSource.next(group.connections);
    })
  }

  joinNewGame(model: any) {
    return this.http.post<number>('https://localhost:5001/api/gameLobby/joinNewLobby/' + this.guest.username, model);
  }

  async startGame(gameLobbyId) {
    return this.hubConnection.invoke('StartGame', gameLobbyId)
      .catch(error => console.log(error));
  }

  async play(cards) {
    return this.hubConnection.invoke('Play', cards)
      .catch(error => console.log(error));
  }  

  async playByColour(cards, colour) {
    return this.hubConnection.invoke('PlayWithChosenColour', cards, colour)
      .catch(error => console.log(error));
  }

  async getCard() {
    return this.hubConnection.invoke('GetCard')
      .catch(error => console.log(error));
  }

  async getCardByColour(colour) {
    return this.hubConnection.invoke('GetCardWithChosenColour', colour)
      .catch(error => console.log(error));
  }

  async pickColour(colour) {
    return this.hubConnection.invoke("PickColour", colour)
      .catch(error => console.log(error));
  }

  async UNO() {
    return this.hubConnection.invoke("ChangeUnoStatus")
      .catch(error => console.log(error));
  }

  async catchUno(username) {
    return this.hubConnection.invoke("CatchUno", username)
      .catch(error => console.log(error));
  }

  // not sure if I need this method
  getLobby() {
    return this.http.get<GameLobby>('https://localhost:5001/api/gameLobby/' + this.gameLobbyId); 
  }

  
}
