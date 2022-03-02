import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { Card } from '../_models/card';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  cards = new Map<number,string>();
  index: number;  
  cardName: string;
  
  constructor(private http: HttpClient) {     
  }

  getDeck() {
    this.http.get<Card[]>('https://localhost:5001/api/card').subscribe({
      next: cards => this.setCards(cards),
      error: e => console.error(e)
    })
  }

  setCards(cards: Card[]) {
      for (let i = 0; i < cards.length; i++) {
        this.setCardName(cards[i]);      
        this.cards.set(cards[i].cardId, this.cardName);
      }

    this.cards.forEach(card => {
      console.log(card)
    });
  }

  getCards(cards: Card[]) {
    var cardsNames: string[] = [];
    for (let i = 0; i < cards.length; i++) {      
      cardsNames.push(this.cards.get(cards[i].cardId));      
    }  
    return cardsNames;  
  }

  // working
  getCardById(id) {
    return this.http.get<Card>('https://localhost:5001/api/card/'+ id);
  }
  
  // Working
  setCardName(card: Card) {
    if (card.value === -1) {
      return card.colour.charAt(0) + "" +card.type + ".jpg";
    } else {
      return card.colour.charAt(0) + "" + card.value + ".jpg";
    }
  }


}
