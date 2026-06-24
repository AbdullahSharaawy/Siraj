import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserNavbar } from '../ui-user-navbar/user-navbar';

@Component({
  selector: 'app-user-page',
  standalone: true,
  imports: [RouterOutlet, UserNavbar],
  templateUrl: './user-page.html',
  styleUrl: './user-page.css',
})
export class UserPage {
  // We can add logic here to handle user-specific global states, 
  // like checking if the user session is active.
}
