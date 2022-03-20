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
  currentId: number;  

  constructor(private accountService: AccountService, private presence: PresenceService) {} 

  ngOnInit() {   
    
  }

  // setCurrentUser() {
  //   const user: User = JSON.parse(localStorage.getItem('user'));

  //   if (user) {
  //     this.accountService.setCurrentUser(user);
  //     this.presence.createHubConnection(user);
  //   }
  // } 

  
}
