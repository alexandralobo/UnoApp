import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Card } from '../_models/card';
import { Connection } from '../_models/connection';
import { GameLobby } from '../_models/game';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { CardService } from '../_services/card.service';
import { GameService } from '../_services/game.service';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  guest: User;
  
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

  error: string = "";
  message: string = "";
  pickedColour: string = "none";

  uno: boolean = false;

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
    this.gameService.gameLobby$.subscribe(game => {this.gameLobby = game, this.pickedColour = game[0]?.pickedColour});

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

  setCurrentPlayer(username, id) {
    if (username === this.gameLobby[0].currentPlayer) {
      //console.log("HERE " + id)
      document.getElementById(id).style.border = "10px solid #FF0000";
      //document.getElementById(id).style.border
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
    var myCard = document.getElementById(id);   

    if (this.cards.includes(card)) {
      this.cards = this.cards.filter(c => c.cardId !=card.cardId);       
      myCard.classList.remove('card-selected');

    } else {
      this.cards.push(card);
      myCard.classList.add('card-selected');
    } 

    if (this.cards.length === 0) {
      document.getElementById("play").style.display = "none";
    } else {
      document.getElementById("play").style.display = "block";
    }
  }

  async submitPlay() {
    this.error = "";
    //console.error("Calling submitPlay");
    if (this.cards === []) {
      this.submitted = false;
    } else {
      
      if (this.gameLobby[0].pickedColour === "none") {

        try {
          await this.gameService.play(this.cards)
            .then(msg => this.message = msg);

        } catch (e) {
          console.error(e);
          var error: string = e.toString();
          this.error = error.split(':', 3)[2];
        }
        

      } else {
        try {
          await this.gameService.playByColour(this.cards, this.gameLobby[0].pickedColour)          
            .then(msg => this.message = msg);
            
        } catch (e) {
          console.error(e);
          var error: string = e.toString();
          this.error = error.split(':', 3)[2];
        }          
      }
      //console.log(this.message);       
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
    this.error = "";
    if (this.gameLobby[0].pickedColour === "none") {
      this.gameService.getCard()
      .catch(error => this.error = error)
    } else {
      try {
        this.gameService.getCardByColour(this.pickedColour);
      } catch (e) { 
        console.error(e);
        var error: string = e.toString();
        this.error = error.split(':', 3)[2];
      }
    }
    
  }

  pickColour(colour) { 
    this.error = "";
    try {
      this.gameService.pickColour(colour)
        .then(msg => this.message = msg);
    } catch (e) { 
      console.error(e);
      var error: string = e.toString();
      this.error = error.split(':', 3)[2];
    }
     
    if (this.message === "Next") {
      this.pickedColour === colour;
      document.getElementById("pick-colour").style.display = "none";      
    }
  }

  UNO() {
    this.error = "";
    try {
      this.gameService.UNO()
        .then(msg => this.message = msg); 
    } catch (e) {
      console.error(e);
      var error: string = e.toString();
      this.error = error.split(':', 3)[2];
    }

      if (this.message === "Uno!") {
        this.uno = true;
        document.getElementById("uno").style.display = "none";
      }
  }
  
  catchUno(username) {
    this.error = "";
    try {
      this.gameService.catchUno(username)
      .then(msg => this.message = msg);  
    } catch (e) {
      console.error(e);
      var error: string = e.toString();
      this.error = error.split(':', 3)[2];
    }
    
  }

  

}
