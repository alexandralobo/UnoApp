import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
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
import { WelcomeScreenComponent } from './welcome-screen/welcome-screen.component';
import { JwtInterceptor } from './_interceptor/jwt.interceptor';
import { AboutUsComponent } from './about-us/about-us.component';
import { RulesComponent } from './rules/rules.component';

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
    DateInputComponent,
    WelcomeScreenComponent,
    AboutUsComponent,
    RulesComponent
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
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
