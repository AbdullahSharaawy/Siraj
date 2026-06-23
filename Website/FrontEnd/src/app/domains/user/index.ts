export { UserPage } from './feature-user-page/user-page';
export { UserNavbar } from './ui-user-navbar/user-navbar';
export type { NavLink } from './ui-user-navbar/user-navbar';
export { DonatedItems } from './feature-donated-items/donated-items';

export { FeatureDashboard as UserDashboard } from './feature-dashboard/feature-dashboard';

// NOT YET BUILT app.routes.ts references these but the files don't
// exist yet. Build them in feature-* folders alongside donated-items/
// and uncomment here, or routing will throw a module-not-found error.
// export { UserProfile } from './feature-profile/user-profile';
// export { DonationHistory } from './feature-donation-history/donation-history';
// export { OrganizationList } from './feature-org-browse/organization-list';
// export { OrganizationCampaigns } from './feature-org-browse/organization-campaigns';
// export { CampaignDetails } from './feature-campaign-details/campaign-details';
// export { Donate } from './feature-campaign-details/donate';
// export { DonationApplication } from './feature-campaign-details/donation-application';
