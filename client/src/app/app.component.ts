import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Player } from 'src/app/_models/player';
import { Card } from 'src/app/_models/card';
import { GameLobby } from 'src/app/_models/game';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{  
  currentId : number;

  constructor(private accountService: AccountService, private presence: PresenceService) {} 

  ngOnInit() {   
      this.setCurrentUser();
  }

  setCurrentUser() {
    const guest: User = JSON.parse(localStorage.getItem('user'));

    if (guest) {
      this.accountService.setCurrentUser(guest);
      this.presence.createHubConnection(guest);
    }
  }

   

  
}
