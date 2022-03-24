import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RulesComponent } from '../rules/rules.component';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {}
  inGame: boolean = false

  constructor(
    public accountService: AccountService,
    private router: Router,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {

  }

  openDialog() {
    const dialogRef = this.dialog.open(RulesComponent, {
      width: '850px',
      height: '925px'
    })
  }
  isInGame() {
    var url = this.router.url.split('?');
    return url[0].match("/game")
  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/dashboard');
    });
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/home');
  }

}
