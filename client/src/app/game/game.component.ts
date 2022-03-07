import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { take } from 'rxjs';
import { GameLobby } from '../_models/game';
import { Group } from '../_models/group';
import { Guest } from '../_models/guest';
import { Connection } from '../_models/connection';
import { AccountService } from '../_services/account.service';
import { GameService } from '../_services/game.service';
import { CardService } from '../_services/card.service';
import { Card } from '../_models/card';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  guest: Guest;
  
  gameLobbyId: number;   
  gameLobby: GameLobby[];
  prevPlayer : string;

  players: Connection[];
  otherPlayers: Connection[];

  images: any[] = [];
  profilePics: string[] = []

  cardsPosition: string[];
  
  card: Card;
  cards: Card[] = [];
  submitted : boolean = false;

  message: string;
  pickedColour: string = "none";

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    public gameService:  GameService,
    private cardService: CardService,
    private router: Router,
    private route: ActivatedRoute, 
    private toastr: ToastrService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;    
  }

  ngOnInit(): void { 
    this.gameLobbyId = +this.route.snapshot.queryParamMap.get("gameLobbyId");
    this.gameService.createHubConnection(this.guest, this.gameLobbyId);
    //this.loadPlayers();
    //this.loadGameLobby();
    this.gameService.players$.subscribe(players => this.players = players);
    this.gameService.players$.subscribe(players => this.otherPlayers = players.filter(p => p.username !== this.guest.username));
    this.gameService.gameLobby$.subscribe(game => {this.gameLobby = game, this.pickedColour = game[0].pickedColour});

    // this.gameService.players$.forEach(player => {
    //   this.imagesSelected.push(this.getRandomImage());      
    // });
    for (let index = 0; index < 4; index++) {
      this.profilePics.push(this.getRandomImage(index));
    }

    for (let i = 0; i < this.gameLobby[0]?.numberOfElements; i++) {
      var player = this.players[i];
      for (let j = 0; j < 10; j++) {
        this.cardsPosition.push('calc(300px + '+ j +' * 50px)')
      }      
    }
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
  }

  // Cards methods

  // working fine
  setCardPosition(index, element, position) {
    //console.error("Calling setCardPosition");
    var myDiv = document.getElementById(element);


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
    //console.error("Calling getCardSource");
    var cardName = this.cardService.setCardName(card);
    return "/assets/images/Cards/" + cardName;
  }

  // working
  getCardSourceById(id) {
    //console.error("Calling getCardSourceById");
    this.cardService.getCardById(id).subscribe(c => { this.card = c });
    // var cardName = this.cardService.setCardName(this.card);
    return "/assets/images/Cards/" + this.card.fileName;
  }

  cardsToPlay(card, id) {
    //console.error("Calling cardsToPlay");
    //console.error("id: " + id)
    var myCard = document.getElementById(id);   

    if (this.cards.includes(card)) {
      
      this.cards = this.cards.filter(c => c.cardId !=card.cardId);       
      myCard.style.borderColor = 'none';

    } else {
      this.cards.push(card);
      myCard.style.borderColor = 'red';
    } 
  }

  async submitPlay() {
    //console.error("Calling submitPlay");
    if (this.cards === []) {
      this.submitted = false;
    } else {
      
      if (this.pickedColour === "none") {
        await this.gameService.play(this.cards)
        .then(msg => this.message = msg);

      } else {
        await this.gameService.playByColour(this.cards, this.pickedColour)
          .then(msg => {
            this.message = msg,
            this.pickedColour = "none"
          });
          
      }

      console.error(this.message); 
      
      if (this.message === "Pick a colour") {
        document.getElementById("pick-colour").style.display = "block";
      }
      this.cards = [];
      this.submitted = true;

      this.gameLobby.forEach(_gameLobby => {
        this.prevPlayer = _gameLobby.currentPlayer;
      });
      
    }    
  }

  getCard() {
    this.gameService.getCard();
  }

  pickColour(colour) { 
     this.gameService.pickColour(colour)
      .then(msg => {
        this.message = msg,
        this.pickedColour = colour,
        document.getElementById("pick-colour").style.display = "none"
      } 
    )
    //console.error(this.pickedColour);
    // var myDiv = document.getElementById("pick-colour");
    // myDiv.style.display = "none";
  }
  

  

}
