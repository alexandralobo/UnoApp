import { Card } from "./card";

export interface GameLobby {
    gameLobbyId: number;
    gameLobbyName: string;
    drawableCards: Card[];    
    cardPot: Card[];
    lastCard: number;
    currentPlayer: string;
    gameStatus: string;
    numberOfElements: number;
}