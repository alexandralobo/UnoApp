import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { Guest } from '../_models/guest';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit { 
  gameLobbies: GameLobby[] = [];
  loading = false;
  guest: Guest;

  constructor(private http: HttpClient, private accountService: AccountService, private router: Router) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
   }

  ngOnInit(): void {
    this.getLobbies();
  }  

  getLobbies() {
    this.loading = true;

    this.http.get<GameLobby[]>('https://localhost:5001/api/gameLobby/').subscribe({ 
      next: response => {
        this.gameLobbies = response,
        this.loading = false
      },
      error: (e) => console.error(e)
    });
  }

  joinExistingGame(gameId) {   
    this.http.post('https://localhost:5001/api/gameLobby/joinExistingLobby', {username: this.guest.username, gamelobbyId: gameId}).subscribe({ 
      next: () => {
        this.loading = false
      },
      error: (e) => console.error(e)
    });
  }

  joinNewGame() {
    this.http.post('https://localhost:5001/api/gameLobby/joinNewLobby/' + this.guest.username, {}).subscribe({ 
      next: () => {
        this.loading = false
      },
      error: (e) => console.error(e)
    });
  }
}
