import { Component } from '@angular/core';
import { HeroSlider } from '../../Components/hero-slider/hero-slider';
import { Welcome } from '../../Components/welcome/welcome';
import { NavBar } from '../../Components/nav-bar/nav-bar';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [NavBar, Welcome, HeroSlider],
  templateUrl: './main-page.html',
  styleUrl: './main-page.css',
})
export class MainPage {}