import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { GameComponent } from './game/game.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  {path: '', data: {gameLobbyId: ''}, component:HomeComponent},
  {
    path:'',
    runGuardsAndResolvers:'always',
    //canActivate: [AuthGuard],
    children: [
      {path: 'dashboard', component: DashboardComponent},
      {path: 'game', component: GameComponent}
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
