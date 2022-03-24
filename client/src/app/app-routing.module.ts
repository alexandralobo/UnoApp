import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutUsComponent } from './about-us/about-us.component';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { GameComponent } from './game/game.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { RulesComponent } from './rules/rules.component';
import { WelcomeScreenComponent } from './welcome-screen/welcome-screen.component';

const routes: Routes = [
  { path: '', data: { gameLobbyId: '', nav: false }, component: WelcomeScreenComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    //canActivate: [AuthGuard],
    children: [
      { path: 'home', component: HomeComponent, data: { nav: true } },
      { path: 'aboutUs', component: AboutUsComponent, data: { nav: true } },
      { path: 'dashboard', component: DashboardComponent, data: { nav: true } },
      {
        path: 'game', component: GameComponent, data: { nav: true },
        children:
          [{ path: 'rules', component: RulesComponent, data: { nav: true } },]
      },
      { path: 'login', component: RegisterComponent, data: { nav: true } }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
