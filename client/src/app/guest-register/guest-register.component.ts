import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
//import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-guest-register',
  templateUrl: './guest-register.component.html',
  styleUrls: ['./guest-register.component.css']
})
export class GuestRegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;

  constructor(private accountService: AccountService, private fb: FormBuilder, private router: Router, private http: HttpClient) { }

  ngOnInit(): void {
    this.initialiseForm();
  }

  initialiseForm() {
    this.registerForm = this.fb.group({
      username:['', Validators.required]
    })
  }

  register() {
    this.accountService.createGuest(this.registerForm.value).subscribe({
      next: response => {
        this.router.navigateByUrl('/dashboard')
        //console.log("Registered!")
      },
      error: e => {}
    })
  }
  
  cancel() {
    this.cancelRegister.emit(false);
  }
}
