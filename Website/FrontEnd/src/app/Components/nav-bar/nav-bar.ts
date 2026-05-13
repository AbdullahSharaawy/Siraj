import {
  Component,
  HostListener,
  signal,
  computed,
  OnInit,
  OnDestroy,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface NavLink {
  label: string;
  href: string;
  children?: NavLink[];
}

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './nav-bar.html',
  styleUrls: ['./nav-bar.css'],
})
export class NavBar implements OnInit, OnDestroy {

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  links: NavLink[] = [
    { label: 'Journal', href: '/journal' },
    { label: 'About', href: '/about' },
  ];

  activeDropdown = signal<string | null>(null);

  ngOnInit(): void {}

  ngOnDestroy(): void {
    if (isPlatformBrowser(this.platformId)) {
      document.body.style.overflow = '';
    }
  }

  openDropdown(label: string): void { this.activeDropdown.set(label); }
  closeDropdown(): void             { this.activeDropdown.set(null); }
}