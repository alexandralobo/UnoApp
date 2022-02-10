import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Player } from 'models/player';
import { Card } from 'models/card';
import { GameLobby } from 'models/game';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'The Uno Game';
  players: Player[] = [];
  cards : Card[] = [];
  currentCards: string[] = [];
  gameLobbyId = 2;
  gameLobby = {} as GameLobby;
  currentPlayer: string;
  currentCard: Card;

  constructor(private http: HttpClient) {} 

  ngOnInit() {
    this.getPlayers(this.gameLobbyId);
    this.getLobby(this.gameLobbyId);
    this.setCurrentPlayer();
  }

  getPlayers(gameLobbyId) {
    this.http.get<Player[]>('https://localhost:5001/api/gameLobby/members/'+ gameLobbyId).subscribe({
      next: response => this.players = response,
      error: (e) => console.error(e)
    })
  }

  getLobby(gameLobbyId) {
    this.http.get<GameLobby>('https://localhost:5001/api/gameLobby/' + gameLobbyId).subscribe({
      next: response => this.gameLobby = response,
      error: (e) => console.error(e)
    })
  }

  initialiseGame(gameLobbyId) {
    this.http.post<GameLobby>('https://localhost:5001/api/gameLobby/start', gameLobbyId).subscribe({
      next: response => this.gameLobby = response,
      error: (e) => console.error(e)
    })
  }

  setCurrentCard() {
    this.currentCard = this.gameLobby.cardPot.pop();
    this.gameLobby.cardPot.push(this.currentCard);
  }
  setCurrentPlayer() {
    this.currentPlayer = this.gameLobby.currentPlayer;
  }

  getCardsForPlayer(connectionId) {
    this.currentCards = [];
    var player = this.players.find(p => p.connectionId == connectionId);
    this.cards = player.cards; 

    for (let index = 0; index < this.cards.length; index++) {
      const card = this.cards[index];
      this.currentCards.push(`${index+1}) colour: ${card.colour}, type: ${card.type}, value: ${card.value}`);      
    }
  } 

  
}
