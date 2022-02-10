import { Card } from "./card";

export interface Player {
    connectionId: number;
    username: string;    
    cards: Card[];
}