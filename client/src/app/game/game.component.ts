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
  user: User;

  gameLobbyId: number;
  gameLobby: GameLobby[];
  prevPlayer: string;

  players: Connection[];
  otherPlayers: Connection[];

  images: any[] = [];
  profilePics: string[] = []

  cardsPosition: string[];

  card: Card;
  cards: Card[] = [];
  submitted: boolean = false;

  error: string = "";
  message: string = "";
  pickedColour: string = "none";

  uno: boolean = false;
  private: boolean = false;

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    public gameService: GameService,
    private cardService: CardService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.gameLobbyId = +this.route.snapshot.queryParamMap.get("gameLobbyId");
    this.gameService.createHubConnection(this.user, this.gameLobbyId);
    //this.loadPlayers();
    //this.loadGameLobby();
    this.gameService.players$.subscribe(players => this.players = players);
    this.gameService.players$.subscribe(players => this.otherPlayers = this.orderOfPlayers(players));
    //this.orderOfPlayers(this.players);
    this.gameService.gameLobby$.subscribe(game => {
      this.gameLobby = game,
        this.pickedColour = game[0]?.pickedColour,
        this.private = game[0]?.password != ""
    });

    //this.gameService.gameLobby$.subscribe(game => console.log(game[0]?.password != ''));

    // this.gameService.players$.forEach(player => {
    //   this.imagesSelected.push(this.getRandomImage());      
    // });
    for (let index = 0; index < 4; index++) {
      this.profilePics.push(this.getRandomImage(index));
    }

    for (let i = 0; i < this.gameLobby[0]?.numberOfElements; i++) {
      var player = this.players[i];
      for (let j = 0; j < 10; j++) {
        this.cardsPosition?.push('calc(300px + ' + j + ' * 50px)')
      }
    }
    console.log(this.error)
  }

  orderOfPlayers(players: Connection[]): Connection[] {

    let tmpPlayers = [];

    let currentPlayerIndex = players.findIndex(player => player.username === this.user.username);

    for (let index = currentPlayerIndex + 1; index < players.length; index++) {
      tmpPlayers.push(players[index])
    }

    for (let index = 0; index < currentPlayerIndex; index++) {
      tmpPlayers.push(players[index])
    }

    return tmpPlayers;
  }

  getRandomImage(position) {
    this.images = ["f1.jpg", "f2.jpg", "f3.jpg", "f4.jpg", "m1.jpg", "m2.jpg", "m3.jpg", "m4.jpg"]
    // return "/assets/images/" + this.images[Math.floor(Math.random() * this.images.length)];
    return "/assets/images/" + this.images[position];
  }

  // Game methods
  async startGame() {
    await this.gameService.startGame(this.gameLobbyId)
      .catch(e => console.log(e));
  }

  // Cards methods

  // working fine
  setCardPosition(index, element, position) {
    var myDiv = document.getElementById(element);


    if (position === 'bottom') {
      myDiv.style.left = 'calc(300px + ' + index + ' * 50px)';
    } else if (position === 'right') {
      myDiv.style.right = 'calc(-50px + ' + index + ' * 20px)';
    } else if (position === 'top') {
      myDiv.style.left = 'calc(150px + ' + "(" + index + ' * 20px))';
    } else if (position === 'left') {
      myDiv.style.left = 'calc(-50px + ' + index + ' * 20px)';;
    }
  }

  setCurrentPlayer(username, id) {
    if (username === this.gameLobby[0].currentPlayer) {
      document.getElementById(id).style.border = "10px solid #FF0000";
    }
  }

  // working
  getCardSource(card: Card) {
    var cardName = this.cardService.setCardName(card);
    return "/assets/images/Cards/" + cardName;
  }

  // working
  getCardSourceById(id) {
    this.cardService.getCardById(id).subscribe(c => { this.card = c });
    return "/assets/images/Cards/" + this.card.fileName;
  }

  cardsToPlay(card, id) {
    //console.error("Calling cardsToPlay");
    var myCard = document.getElementById(id);

    if (this.cards.includes(card)) {
      this.cards = this.cards.filter(c => c.cardId != card.cardId);
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

  togglePrivacy() {
    this.private = !this.private
    try {
      this.gameService.isPrivate(this.private);
    } catch (e) {
      console.error(e);
      var error: string = e.toString();
      this.error = error.split(':', 3)[2];
    }
  }

  ngOnDestroy(): void {
    this.gameService.stopHubConnection();

    if (this.gameLobby[0].gameStatus === 'finished') {
      try {
        this.gameService.finishedGame();
      } catch (e) {
        console.error(e);
        var error: string = e.toString();
        this.error = error.split(':', 3)[2];
      }

    }
  }

}
