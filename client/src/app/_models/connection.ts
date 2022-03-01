import { Card } from "./card";

export interface Connection {
    connectionId: string;
    username: string;
    cards: Card[];
}