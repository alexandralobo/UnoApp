import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Guest } from '../_models/guest';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<Guest>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) { }

  register(model: any) {
    return this.http.post(this.baseUrl + 'guests/creation', model).pipe(
      map((guest: Guest) => {
        if (guest) {
          this.setCurrentUser(guest);
          this.presence.createHubConnection(guest);
        }
      })
    )
  }

  // later take care of the roles
  setCurrentUser(guest: Guest) {
    //guest.roles = [];
    //const roles = this.getDecodedToken(user.token).role;
    //Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('guest', JSON.stringify(guest));
    this.currentUserSource.next(guest);
  }

  logout() {
    localStorage.removeItem('guest');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }
  
  /* getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }*/
}
