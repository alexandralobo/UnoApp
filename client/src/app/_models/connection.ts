import { Card } from "./card";

export interface Connection {
    connectionId: string;
    username: string;
    uno: boolean;
    cards: Card[];
}