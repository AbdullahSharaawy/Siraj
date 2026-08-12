# Functional Requirements for The Charity System

## 1. Introduction

This document describes the functional requirements for The Charity system, a .NET 8-based charity platform API. The system supports user and organization management, campaign operations, donations, donated items, payment processing, and background job scheduling.

## 2. Scope

The system includes three main layers:
- `TheCharityPL`: ASP.NET Core Web API with controllers, authentication, Swagger, CORS, and middleware.
- `TheCharityBLL`: business logic layer with services, DTOs, mappers, and job scheduler integrations.
- `TheCharityDAL`: data access layer with Entity Framework Core context, entities, and repositories.

The API is intended to support a frontend application and external integrations via REST endpoints.

## 3. User Management Requirements

3.1 Register new users
- The system must allow user registration with email, username, password, full name, phone number, and address.
- The system must enforce password complexity rules: at least one digit, one uppercase letter, one lowercase letter, and a minimum length of 6.

3.2 Email confirmation
- The system must send an email confirmation link after registration.
- The system must allow users to confirm their email using a token.
- The system must allow resending the confirmation email if the user has not yet confirmed.

3.3 Authentication
- The system must support login using username or email and password.
- The system must issue JWT bearer tokens for authenticated sessions.
- The system must enforce that email confirmation is required before login.

3.4 Password reset
- The system must allow users to request a password reset via email.
- The system must allow users to reset their password using a secure token.
- The system must notify users when their password has been changed.

3.5 Profile management
- The system must allow retrieval of user details by user ID.
- The system must allow updates to user profile information.
- The system must allow users to change their password.

3.6 User lifecycle management
- The system must allow deleting users using soft delete.
- The system must allow restoring deleted users.
- The system must allow administrators to list all users, optionally including deleted users.

## 4. Authorization Requirements

4.1 Role-based authorization
- The system must support role-based access control including roles such as `SuperAdmin`, `OrganizationAdmin`, and `Admin`.
- The system must protect campaign management and administrative endpoints with appropriate roles.

4.2 External login
- The system must support external login cookie handling for OAuth handshake.
- Google and Facebook login support are scaffolded and may be enabled via configuration.

## 5. Campaign Management Requirements

5.1 Campaign CRUD operations
- The system must allow retrieval of all campaigns, optionally including deleted campaigns.
- The system must allow retrieval of campaign details by ID.
- The system must allow updating campaign records.
- The system must allow soft deletion and restoration of campaigns.

5.2 Campaign lifecycle and deadline management
- The system must identify and retrieve campaigns expiring soon.
- The system must identify expired campaigns.
- The system must allow extending campaign deadlines.
- The system must allow administrators to trigger auto-expiration of campaigns.

5.3 Solo and shared campaign support
- The system must support creating, reading, updating, and deleting solo campaigns.
- The system must support creating, reading, updating, and deleting shared campaigns.
- The system must allow retrieval of solo/shared campaigns by organization.
- The system must allow adding or removing organizations from a shared campaign.
- The system must allow retrieval of organization count for a shared campaign.

5.4 Progress and status tracking
- The system must allow updating campaign collected money totals.
- The system must allow incrementing campaign money when donations occur.
- The system must allow updating campaign status.
- The system must support campaign filtering by status, type, and active state.
- The system must support campaign search by title or description.
- The system must retrieve deleted campaigns for administrators.

5.5 Campaign analytics and statistics
- The system must provide counts for total, active, solo, and shared campaigns.
- The system must report total money raised and average achievement percentage.
- The system must return campaign count grouped by type and status.
- The system must provide statistical dashboards and trending campaigns.
- The system must provide top campaigns by achievement and donation amount.
- The system must provide recent and urgent campaign listings.

5.6 Bulk campaign operations
- The system must allow bulk update of campaign status.
- The system must allow bulk soft deletion of expired campaigns.

## 6. Donation Management Requirements

6.1 Donation CRUD operations
- The system must allow retrieval of all donations, optionally including deleted donations.
- The system must allow retrieval of donations by ID and with full details.
- The system must allow creation, update, soft deletion, and restoration of donations.

6.2 Donation filtering and search
- The system must allow retrieval of recent donations.
- The system must allow retrieval of donations by user ID and campaign ID.
- The system must allow filtering donations by amount range and date range.
- The system must allow searching donations by user and campaign.
- The system must allow retrieving donations for multiple users or campaigns.

6.3 Donation statistics and analytics
- The system must report total donation amount and count.
- The system must report totals by user and by campaign.
- The system must report average donation amounts overall, by user, and by campaign.
- The system must provide top donors and top campaigns.
- The system must provide donation trends and frequency by user.
- The system must provide dashboard summaries for latest, largest, daily, weekly, and monthly totals.

6.4 Donation reporting
- The system must generate monthly, quarterly, and yearly donation reports.
- The system must report donations by time of day and by day of week.
- The system must provide record count reports for specified date ranges.

6.5 Campaign-specific donation queries
- The system must report total raised amount for a campaign.
- The system must calculate campaign progress percentage.
- The system must list campaign donors and donation timeline.

6.6 User-specific donation queries
- The system must retrieve donation history for a user.
- The system must retrieve a user’s last donation date.
- The system must retrieve campaigns donated to by a user.

6.7 Donation bulk and maintenance operations
- The system must transfer donations from one campaign to another.
- The system must delete donations older than a specified threshold.

6.8 Validation and audit
- The system must verify donation existence.
- The system must verify whether a user has donated to a campaign.
- The system must identify suspicious donations above a threshold.

6.9 Donor engagement insights
- The system must identify recurring donors.
- The system must identify first-time donors within a date range.
- The system must calculate lifetime donor value.
- The system must identify loyal donors by donation amount and frequency.

## 7. Donated Item Management Requirements

7.1 Donated item lifecycle
- The system must allow retrieval of all donated items, optionally including deleted items.
- The system must allow creation, update, deletion, and restoration of donated items.
- The system must allow filtering donated items by organization, donor, category, availability, and deletion status.
- The system must support search of donated items.

7.2 Item media management
- The system must support uploading and deleting item images.
- The system must support uploading and deleting item attachments.
- The system must retrieve images and attachments for donated items.
- The system must identify primary item images.
- The system must report image counts per item.

7.3 Availability and categorization
- The system must allow updating item availability and category.
- The system must allow bulk category updates.
- The system must allow marking items unavailable in bulk.

7.4 Item analytics and inventory
- The system must report total item counts, available item counts, and organization-specific counts.
- The system must report item count by category and category distribution.
- The system must retrieve top donors and organizations by donations.
- The system must retrieve recent and trending donated items.
- The system must retrieve items without images, with attachments, and items by date range.

7.5 Item details and metadata
- The system must retrieve item details with donor and organization data.
- The system must retrieve total file size storage used per item and overall.
- The system must identify large attachments above a size threshold.

7.6 Item transfer and updates
- The system must allow transferring items between organizations.
- The system must allow updating the donor associated with an item.
- The system must retrieve recently updated items and donor activity metrics.

## 8. Organization Management Requirements

8.1 Organization lifecycle
- The system must allow retrieval of all organizations, optionally including deleted organizations.
- The system must allow retrieval of organization details.
- The system must allow creation, update, deletion, and restoration of organizations.
- The system must allow retrieval of deleted organizations.
- The system must allow retrieval of organization dropdown listings.

8.2 Organization search and filters
- The system must support organization search by keyword.
- The system must support filtering organizations by name and address.
- The system must retrieve recently registered organizations.
- The system must report total and active organization counts.

8.3 Contact methods
- The system must allow retrieval of organization contact methods.
- The system must allow creating, updating, deleting, and restoring contact methods.
- The system must filter contact methods by type and count contact methods by type.
- The system must retrieve organizations by contact method type.

8.4 Organization payment info
- The system must allow creation, update, deletion, and restoration of paymob api keys for each organization.
- The system must report last payment info updates.

8.5 Organization campaign metrics
- The system must identify organizations by campaign count.
- The system must identify organizations with active campaigns.
- The system must identify organizations with completed campaigns.
- The system must identify organizations without any campaigns.

## 9. Payment Processing Requirements

9.1 Paymob payment integration
- The system must create a Paymob payment request for campaign donations.
- The system must include user, campaign, and organization metadata in payment requests.
- The system must verify Paymob callbacks using HMAC signatures.
- The system must process successful payment callbacks and create donation records.
- The system must always return HTTP 200 to Paymob callbacks to avoid duplicate retries.

## 10. Background Jobs and Maintenance Requirements

10.1 Hangfire background processing
- The system must integrate Hangfire for background job scheduling.
- The system must register recurring jobs at startup.
- The system must expose a secured Hangfire dashboard at `/hangfire`.
- The system must support multiple job queues and worker threads.

## 11. Infrastructure and Operational Requirements

11.1 Persistence and data access
- The system must use SQL Server via Entity Framework Core.
- The system must use `TheCharityDbContext` for data access.
- The system must implement soft delete query filters for users, campaigns, and organizations.
- The system must use TPH inheritance for campaign entity types.

11.2 Health and documentation
- The system must expose a health check endpoint at `/health`.
- The system must expose Swagger/OpenAPI documentation in development mode.


## 12. Key Entities

- User
- Campaign
- SoloCampaign
- SharedCampaign
- Organization
- DonatedItem
- Donation
- Attachment
- ItemImage
- OrganizationContactMethod
- PaymentInfo
- ScheduledJob

## 13. Assumptions

- Soft delete is used rather than permanent delete for key entities.
- Campaigns can be either solo or shared, and shared campaigns can link to multiple organizations.
- Payment processing is handled by Paymob, with external callback verification.
- The frontend consumes the API and performs actions using the documented endpoints.
