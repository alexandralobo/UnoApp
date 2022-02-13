import { Card } from "./card";

export interface GameLobby {
    gameLobbyId: number;
    drawableCards: Card[];    
    cardPot: Card[];
    lastCard: number;
    currentPlayer: string;
    gameStatus: string;
    numberOfElements: number;
}