import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { take } from 'rxjs';
import { GameLobby } from '../_models/game';
import { Group } from '../_models/group';
import { Guest } from '../_models/guest';
import { AccountService } from '../_services/account.service';
import { GameService } from '../_services/game.service';
import { CardService } from '../_services/card.service';
import { Card } from '../_models/card';

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
  card: Card;

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    public gameService:  GameService,
    private cardService: CardService,
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

  // Game methods
  async startGame() {
    await this.gameService.startGame(this.gameLobbyId)
      .catch(Error);

    this.gameService.getLobby().subscribe({
      next: game => this.gameLobby = [game]
    });
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

  // Cards methods

  // working fine
  setCardPosition(index, element, position) {
    var myDiv = document.getElementById(element);
    var displaySetting = myDiv.style.left;

    if (position === 'bottom') { 
      myDiv.style.left = 'calc(300px + '+ index +' * 50px)';
    } else if (position === 'right') {
      myDiv.style.right = 'calc(-50px + '+ index +' * 20px)';
    } else if (position === 'top') { 
      myDiv.style.left = 'calc(150px + '+ "(" + index +' * 20px))';
    } else if (position ==='left') {
      myDiv.style.left = 'calc(-50px + '+ index +' * 20px)';;
    }
  }

  // working
  getCardSource(card : Card) {
    var cardName = this.cardService.setCardName(card);
    return "/assets/images/Cards/" + cardName;
  }

  // working
  getCardSourceById(id) {
    this.cardService.getCardById(id).subscribe(c => { this.card = c });
    var cardName = this.cardService.setCardName(this.card);
    return "/assets/images/Cards/" + cardName;
  }

}
