import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameLobby } from '../_models/game';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { GameService } from '../_services/game.service';
import { PresenceService } from '../_services/presence.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit { 
  createForm: FormGroup;
  joinExistingForm: FormGroup;
  gameLobbies: GameLobby[] = [];
  loading = false;
  guest: User;
  create = false;
  private routeData;

  constructor(
    private http: HttpClient,
    private accountService: AccountService,
    private gameService: GameService,
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private toastr: ToastrService,
    public presence: PresenceService) 
    {
    this.accountService.currentUser$.pipe(take(1)).subscribe(guest => this.guest = guest);
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
   }

  ngOnInit(): void {
    this.getLobbies();
    this.initialiseForm();
  } 
  
  initialiseForm() {
    this.createForm = this.fb.group({
      lobbyName:['', Validators.required]
    })
  }

  getLobbies() {
    this.loading = true;
    
    this.http.get<GameLobby[]>('https://localhost:5001/api/gameLobby/').subscribe({ 
      next: response => {
        response.forEach(lobby => {    
          if (lobby.gameStatus === "waiting") {
            this.gameLobbies = [];
            this.gameLobbies.push(lobby);
          }
        });
        //this.gameLobbies = response,
        this.loading = false
      },
      error: (e) => console.error(e)
    });
  }

  joinExistingGame(gameId) {
    this.loading = true;   
    
    this.joinExistingForm = this.fb.group({
      gameLobbyId: [gameId]
    })
    this.http.post('https://localhost:5001/api/gameLobby/joinExistingLobby/' + this.guest.username, this.joinExistingForm.value).subscribe({ 
      next: () => {
        this.loading = false
        this.router.navigate(["/game"], {queryParams: {gameLobbyId: gameId}});
      },
      error: (e) => console.error(e)
    });
  }

  joinNewGame() {

    this.loading = true; 
    var gameId;
    this.gameService.joinNewGame(this.createForm.value).subscribe(
      { next: response => {
        this.loading = false
        gameId = response;
        this.router.navigate(["/game"], {queryParams: {gameLobbyId: gameId}});
      } 
      });  
  }    

  startCreatingNewGame() {
    this.create = true;
  }
}
