import { Routes } from '@angular/router';
import { adminGuard } from './core/guards/role-guards';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./domains/public').then((m) => m.Welcome),
  },
  {
    path: 'sign-in',
    loadComponent: () =>
      import('./domains/auth').then((m) => m.SignIn),
  },
  {
    path: 'sign-up',
    loadComponent: () =>
      import('./domains/auth').then((m) => m.SignUp),
  },

  {
    path: '',
    loadComponent: () =>
      import('./domains/user').then((m) => m.UserPage),
    children: [
      { path: 'dashboard', loadComponent: () => import('./domains/user').then(m => m.UserDashboard) },
      { path: 'donated-items', loadComponent: () => import('./domains/user').then(m => m.DonatedItems) },
    ],
  },

  // Org domain: an org managing ITSELF. Distinct from admin below,
  // which manages orgs from outside.
  {
    path: 'org-dashboard',
    loadComponent: () =>
      import('./domains/org').then((m) => m.OrgDashboard),
    children: [
      { path: 'campaigns/create', loadComponent: () => import('./domains/org').then(m => m.CreateCampaign) },
      // TODO as components are built (see domains/org/index.ts):
      // profile, campaigns, campaigns/:campaignId/edit,
      // campaigns/:campaignId/donations, analytics
    ],
  },

  // Admin domain: separate people/permissions from org — see
  // shared/utils/roles.ts for why this is its own domain, not nested
  // under org. adminGuard is wired and ready; nothing to guard yet
  // (domains/admin/index.ts is all commented-out exports for now).
  // Uncomment once at least AdminDashboard exists:
  //
  // {
  //   path: 'admin',
  //   canActivate: [adminGuard],
  //   loadComponent: () => import('./domains/admin').then((m) => m.AdminDashboard),
  //   children: [ ... ],
  // },
];
