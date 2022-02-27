import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { take } from 'rxjs';
import { Group } from '../_models/group';
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
  players;
  images : any[] = [];
  imagesSelected : any[] = [];

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    public gameService:  GameService,
    private router: Router,
    private route: ActivatedRoute) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;    
  }

  ngOnInit(): void { 
    this.gameLobbyId = +this.route.snapshot.queryParamMap.get("gameLobbyId");
    this.gameService.createHubConnection(this.guest, this.gameLobbyId);
    this.loadPlayers();
    // this.gameService.players$.forEach(player => {
    //   this.imagesSelected.push(this.getRandomImage());      
    // });
  }

  loadPlayers() {
    this.gameService.players$.subscribe({
      next: players => this.players = players.filter(p => p.username !== this.guest.username)
    });
  }
  getRandomImage(position) {
    this.images = ["f1.jpg", "f2.jpg", "f3.jpg", "f4.jpg", "m1.jpg", "m2.jpg","m3.jpg", "m4.jpg"]        

    // return "/assets/images/" + this.images[Math.floor(Math.random() * this.images.length)];

    return "/assets/images/" + this.images[position];
  }



}
