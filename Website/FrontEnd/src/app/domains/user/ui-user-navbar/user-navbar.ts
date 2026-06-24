import { Component, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

export interface NavLink {
  label: string;
  href: string;
  children?: NavLink[];
}

@Component({
  selector: 'app-user-navbar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './user-navbar.html',
  styleUrl: './user-navbar.css',
})
export class UserNavbar {
  activeDropdown = signal<string | null>(null);

  links: NavLink[] = [
    { label: 'Dashboard', href: '/dashboard' },
    { label: 'Organizations', href: '/organizations' },
    { label: 'Campaigns', href: '/campaigns' },
    { label: 'Donations', href: '/donations' },
  ];

  openDropdown(label: string) {
    this.activeDropdown.set(label);
  }

  closeDropdown() {
    this.activeDropdown.set(null);
  }
}
