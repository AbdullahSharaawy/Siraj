import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../auth';
import { UserRole } from '../../../shared/utils/roles';

interface DonatedItem {
  id: string;
  name: string;
  status: 'pending' | 'confirmed' | 'received';
  donorName: string;
}

@Component({
  selector: 'app-donated-items',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './donated-items.html',
  styleUrl: './donated-items.css',
})
export class DonatedItems implements OnInit {
  isWorker = false;
  items: DonatedItem[] = [];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // "If worker" branch from the diagram — same route, same page,
    // extra controls rendered only for the worker role.
    this.isWorker = this.authService.getUserRole() === UserRole.Worker;
    // TODO: load real items from a data-access service
    this.items = [];
  }

  askForDonatedItems(): void {
    // worker-only: request items to go pick up / process
  }

  confirmDonatedItem(item: DonatedItem): void {
    // worker-only: confirm an item has been received
    item.status = 'confirmed';
  }

  markReceived(item: DonatedItem): void {
    // "press a button that says received" per the diagram
    item.status = 'received';
  }
}
