import { Injectable, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http'
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { Guest } from '../_models/guest';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  gameLobby: GameLobby[] = [];  
  //baseUrl = environment.apiUrl;
  //hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  

  constructor(private http: HttpClient) {}

  createHubConnection(guest: Guest) 
  {

  }
  

 
}
