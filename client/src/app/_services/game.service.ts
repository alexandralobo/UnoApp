import { Injectable, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { User } from '../_models/user';
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

  private gameLobbySource = new BehaviorSubject<GameLobby[]>([]);
  gameLobby$ = this.gameLobbySource.asObservable();

  private playersSource = new BehaviorSubject<Connection[]>([]);
  players$ = this.playersSource.asObservable();;

  user: User;
  gameLobbyId: number;
  nrOfElements: number;

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private cardService: CardService,
    private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  createHubConnection(user: User, gameLobbyId: number) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'gamelobby?lobbyid=' + gameLobbyId, {
        accessTokenFactory: () => user.token
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

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
      return this.hubConnection.invoke('RemoveFromLobby');
    }
    // here remove connection of the games
  }

  joinNewGame(model: any) {
    return this.http.post<number>('https://localhost:5001/api/gameLobby/joinNewLobby/' + this.user.username, model);
  }

  joinExistingGame(model: any) {

  }

  joinPrivateGame(model: any) {
    return this.http.post<number>('https://localhost:5001/api/gameLobby/joinPrivateRoom/' + this.user.username, model);
  }

  async startGame(gameLobbyId) {
    return this.hubConnection.invoke('StartGame');
  }

  async play(cards) {
    return this.hubConnection.invoke('Play', cards);
  }

  async playByColour(cards, colour) {
    return this.hubConnection.invoke('PlayWithChosenColour', cards, colour);
  }

  async getCard() {
    return this.hubConnection.invoke('GetCard');
  }

  async getCardByColour(colour) {
    return this.hubConnection.invoke('GetCardWithChosenColour', colour);
  }

  async pickColour(colour) {
    return this.hubConnection.invoke("PickColour", colour);
  }

  async UNO() {
    return this.hubConnection.invoke("ChangeUnoStatus");
  }

  async catchUno(username) {
    return this.hubConnection.invoke("CatchUno", username);
  }
  async isPrivate(priv) {
    return this.hubConnection.invoke("IsPrivate", priv);
  }

  finishedGame() {
    return this.hubConnection.invoke('FinishedGame');
  }
  // not sure if I need this method
  //   getLobby() {
  //     return this.http.get<GameLobby>('https://localhost:5001/api/gameLobby/' + this.gameLobbyId); 
  //   }
}
