import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavBar } from '../nav-bar/nav-bar';

@Component({
  selector: 'app-welcome',
  standalone: true,
  imports: [CommonModule, NavBar],
  templateUrl: './welcome.html',
  styleUrl: './welcome.css',
})
export class Welcome {}