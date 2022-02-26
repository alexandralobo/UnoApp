import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { take } from 'rxjs';
import { Group } from '../_models/Group';
import { Guest } from '../_models/guest';
import { AccountService } from '../_services/account.service';
import { GameService } from '../_services/game.service';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  guest: Guest;
  gameLobbyId: number; 
  players : Group;

  constructor(private http: HttpClient, private accountService: AccountService, public gameService:  GameService, private router: Router, private route: ActivatedRoute) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;    
  }

  ngOnInit(): void { 
    this.gameLobbyId = +this.route.snapshot.queryParamMap.get("gameLobbyId");
    this.gameService.createHubConnection(this.guest, this.gameLobbyId);
    this.players = this.gameService.players;
  }

  loadPlayers(gameLobbyId) {
    this.players = this.gameService.players;
  }



}
