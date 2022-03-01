import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { take } from 'rxjs';
import { GameLobby } from '../_models/game';
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
  gameLobby: GameLobby[];

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
    this.loadGameLobby();
    // this.gameService.players$.forEach(player => {
    //   this.imagesSelected.push(this.getRandomImage());      
    // });
  }

  loadGameLobby() {
    this.gameService.gameLobby$.subscribe({
      next: game => this.gameLobby = game
    })
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

  async startGame() {
    await this.gameService.startGame(this.gameLobbyId)
      .catch(Error);

    this.gameService.getLobby().subscribe({
      next: game => this.gameLobby = [game]
    });
    // //this.toggleDiv();
    // this.loadGameLobby();
  }

  toggleDiv() {
    var myDiv = document.getElementById('start-prompt');
    var displaySetting = myDiv.style.display;

    if (displaySetting != 'block') {
      myDiv.style.display = 'none';
    } else {
      myDiv.style.display = 'block';
    }
  }

  setCardPosition(index, element, position) {
    console.log(element);
    var myDiv = document.getElementById(element);
    var displaySetting = myDiv.style.left;
    //console.error(1 + ") " + myDiv.style.left);
    if (position === 'bottom') { 
      myDiv.style.left = 'calc(300px + '+ index +' * 50px)';
    } else if (position === 'right') {
      
    } else if (position === 'top') { // working fine
      myDiv.style.left = 'calc(150px + '+ "(" + index +' * 20px))';
    } else if (position ==='left') {

    }
    
    //console.error(2 + ") " + myDiv.style.left);
  }

}
