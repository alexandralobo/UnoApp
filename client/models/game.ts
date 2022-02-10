import { Card } from "./card";

export interface GameLobby {
    gameLobbyId: number;
    drawableCards: Card[];    
    cardPot: Card[];
    currentPlayer: string;
    gameStatus: string;
    numberOfElements: number;
}