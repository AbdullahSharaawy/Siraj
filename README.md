### What this project is

`TheCharity` is a .NET 8 charity platform backend with a REST API, database persistence, authentication, payment integration, email configuration, and background job scheduling.

### Architecture

- TheCharityPL  
  - ASP.NET Core Web API project
  - Hosts controllers, middleware, Swagger, CORS, authentication, and Hangfire dashboard
- TheCharityBLL  
  - Business logic layer
  - Service registration, service implementations, DTOs, mappers, and job scheduling
- TheCharityDAL  
  - Data access layer
  - Entity Framework Core `TheCharityDbContext`
  - Entities and repository implementations

### Key features

- API endpoints for:
  - campaigns
  - donated items
  - donations
  - organizations
  - users
  - payment and payment callbacks
  - external login
- Authentication and authorization:
  - JWT bearer tokens
  - ASP.NET Identity with `User` entity
  - Role-based authorization (`SuperAdmin`, `OrganizationAdmin`, etc.)
- Database:
  - SQL Server via Entity Framework Core
  - Identity user store
  - Soft delete filters for `User`, `Campaign`, and `Organization`
  - Table-per-hierarchy campaign inheritance for `SharedCampaign` and `SoloCampaign`
- Payments:
  - Paymob integration with payment creation and callback handling
- Email:
  - Configured via `EmailSettings`
- Background jobs:
  - Hangfire integration
  - Recurring jobs via `IJobRegistry`
- Health checks:
  - Database health check exposed at `/health`

### Important implementation details

- Program.cs configures:
  - service registration from `TheCharityBLL.Helpers.ServiceExtensions`
  - Swagger/OpenAPI
  - CORS policy for Angular development
  - Hangfire server and dashboard
  - global exception handling middleware
- ServiceExtensions.cs configures:
  - Identity and password rules
  - EF Core DB context and health checks
  - dependency injection for repositories and services
  - JWT authentication and optional external login cookies
  - Hangfire job services
- `TheCharityDbContext` includes:
  - `DbSet<Attachment>`, `Campaign`, `DonatedItem`, `Donation`, `ItemImage`, `Organization`, `PaymentInfo`, `SharedCampaign`, `SoloCampaign`, `ScheduledJob`
  - relationship mapping and query filters

