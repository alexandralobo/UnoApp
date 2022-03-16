import { Card } from "./card";

export interface GameLobby {
    gameLobbyId: number;
    gameLobbyName: string;
    drawableCards: Card[];    
    cardPot: Card[];
    lastCard: number;
    pickedColour: string;
    currentPlayer: string;
    gameStatus: string;
    numberOfElements: number;
    password: string;
}