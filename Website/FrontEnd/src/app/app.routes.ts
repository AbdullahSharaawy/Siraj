import { Routes } from '@angular/router';
import { MainPage } from './Pages/main-page/main-page';
import { SignIn } from './Pages/sign-in/sign-in';
import { SignUp } from './Pages/sign-up/sign-up';

export const routes: Routes = [
  { path: '', component: MainPage },
  { path: 'sign-in', component: SignIn },
  { path: 'sign-up', component: SignUp },
  // add routes here
];