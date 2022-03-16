import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { GuestRegisterComponent } from './guest-register/guest-register.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HomeComponent } from './home/home.component';
import { SharedModule } from './_modules/shared.module';
import { GameComponent } from './game/game.component';
import { RegisterComponent } from './register/register.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';

@NgModule({
  declarations: [    
    AppComponent,
    NavComponent,
    GuestRegisterComponent,
    TextInputComponent,
    DashboardComponent,
    HomeComponent,
    GameComponent,
    RegisterComponent,
    DateInputComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
