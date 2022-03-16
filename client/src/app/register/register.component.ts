import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService, 
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit(): void {
    this.initialiseForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initialiseForm() {
    this.registerForm = this.fb.group({
      gender:['male'],
      username:['', Validators.required],
      dateOfBirth:['', Validators.required],
      password: ['', [Validators.required,Validators.minLength(8)]],
      confirmPassword:['', [Validators.required, this.matchValues('password')]]
    })

    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })

    }
    
    matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
        return control?.value === control?.parent?.controls[matchTo].value ? null : {isMatching: true}
      }
    }
  
    register() {
      this.accountService.register(this.registerForm.value).subscribe({
        next: () => {this.router.navigateByUrl('/dashboard')},
        error: e => {this.validationErrors = e}
      })
    }
  
    cancel() {
      this.cancelRegister.emit(false);
    }
  }