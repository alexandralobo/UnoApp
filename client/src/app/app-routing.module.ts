import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HomeComponent } from './home/home.component';

const routes: Routes = [
  {path: '', component:HomeComponent},
  {
    path:'',
    runGuardsAndResolvers:'always',
    //canActivate: [AuthGuard],
    children: [
      {path: 'dashboard', component: DashboardComponent}
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
