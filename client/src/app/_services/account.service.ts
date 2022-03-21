import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  typ = "";
  constructor(private http: HttpClient, private presence: PresenceService) { }

  createGuest(model: any) {
    return this.http.post(this.baseUrl + 'member/join', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + "member/signup", model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'member/login', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          console.log(JSON.stringify(user));
          this.presence.createHubConnection(user);
        }
      })
    )
  }

  // later take care of the roles
  setCurrentUser(user: User) {
    this.typ = this.getDecodedToken(user.token).typ;
    //const roles = this.getDecodedToken(user.token).role;
    //Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    //console.log(JSON.stringify(user))
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  // Type
  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
