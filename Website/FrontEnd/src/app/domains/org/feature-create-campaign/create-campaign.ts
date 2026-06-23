import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

type CampaignCategory = 'money' | 'item';

@Component({
  selector: 'app-create-campaign',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-campaign.html',
  styleUrl: './create-campaign.css',
})
export class CreateCampaign {
  title = '';
  description = '';
  category: CampaignCategory = 'money';
  goalAmount: number | null = null;
  isLoading = false;
  apiError = '';

  constructor(private router: Router) {}

  async onSubmit(): Promise<void> {
    this.apiError = '';
    if (!this.title) {
      this.apiError = 'Campaign title is required.';
      return;
    }

    this.isLoading = true;
    try {
      // TODO: wire to org data-access service. Category drives the
      // donor-facing filter on the user side (campaign details page
      // branches "If item or money" based on this same value).
      this.router.navigate(['/org-dashboard/campaigns']);
    } catch (err: any) {
      this.apiError = err?.message || 'Could not create campaign. Please try again.';
    } finally {
      this.isLoading = false;
    }
  }
}
