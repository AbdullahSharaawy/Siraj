import { Routes } from '@angular/router';
import { MainPage } from './Pages/main-page/main-page';
import { SignIn } from './Pages/sign-in/sign-in';
import { SignUp } from './Pages/sign-up/sign-up';
import { UserPage } from './Pages/User/user-page/user-page';
import { UserDashboard } from './Components/User/user-dashboard/user-dashboard';

export const routes: Routes = [
  // Change this line! It was { path: '', component: MainPage }
  { path: '', redirectTo: 'user', pathMatch: 'full' }, 

  { path: 'main', component: MainPage },
  { path: 'sign-in', component: SignIn },
  { path: 'sign-up', component: SignUp },

  {
    path: 'user',
    component: UserPage, // This is your "Shell"
    children: [
      { path: 'dashboard', component: UserDashboard }, //
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  }
];