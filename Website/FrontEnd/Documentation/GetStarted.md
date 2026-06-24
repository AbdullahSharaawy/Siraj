# Getting Started for the frontend

---

## Prerequisites

Make sure you have the following installed before you begin:

- [Node.js](https://nodejs.org/) (v18 or higher)
- [Git](https://git-scm.com/)
- Angular CLI install it globally with:

```bash
npm install -g @angular/cli
```

---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/CSGoat0/TheCharity.git
cd TheCharity
```

### 2. Navigate to the frontend folder

```bash
cd Website/FrontEnd
```

### 3. Install dependencies

```bash
npm install
```

### 4. Start the dev server

```bash
ng serve
```

Then open your browser and go to `http://localhost:4200`.

---

## How the project is organized

Read `file-structure.md` first if you haven't — this project is split
into **domains** (`public`, `auth`, `user`, `org`, `admin`) instead of
one flat `Components`/`Pages` folder. Everything below assumes you know
which domain you're working in.

Quick reference for which domain a new screen belongs in:

| Who is it for | Domain |
|---|---|
| Logged-out visitor, marketing | `public` |
| Sign in / sign up / session | `auth` |
| A donor (including a donor who is also a Worker) | `user` |
| An org managing its own campaigns/profile | `org` |
| A platform admin managing orgs from outside | `admin` |

If you're not sure, see "Roles vs domains" in `file-structure.md` before
creating anything — it's much cheaper to pick the right domain up front
than to move a feature later.

---

## Adding a new feature (page) inside an existing domain

### 1. Generate the component inside that domain's folder

```bash
ng generate component domains/<domain>/feature-<your-feature-name>
```

For example, adding a donor-facing "campaign details" page:

```bash
ng generate component domains/user/feature-campaign-details
```

### 2. Export it from that domain's `index.ts`

```typescript
// domains/user/index.ts
export { CampaignDetails } from './feature-campaign-details/campaign-details';
```

This step is not optional — `app.routes.ts` and any other domain are
only allowed to import from `index.ts`, never reach into
`feature-campaign-details/` directly. If you skip this, nothing outside
the domain can use the component.

### 3. Add the route in `app.routes.ts`, importing from the barrel

```typescript
{
  path: 'campaigns/:campaignId',
  loadComponent: () =>
    import('./domains/user').then((m) => m.CampaignDetails),
},
```

Always `loadComponent`/lazy-load rather than eagerly importing — this
keeps each domain code-split so loading `/sign-in` doesn't pull in the
admin dashboard's JS.

### 4. Link to it from anywhere using `routerLink`

```html
<a [routerLink]="['/campaigns', campaignId]">View campaign</a>
```

---

## Adding a brand new domain

You shouldn't need to do this often — `public`/`auth`/`user`/`org`/`admin`
cover the diagrammed product. If a genuinely new business area shows up:

```bash
mkdir -p src/app/domains/<new-domain>/feature-<first-screen>
mkdir -p src/app/domains/<new-domain>/data-access
touch src/app/domains/<new-domain>/index.ts
```

Then export from `index.ts` and route to it the same way as above.

---

## Adding a reusable (non-domain) component

If a component has no business logic and could be reused by *any*
domain (a button variant, a generic card, a loading spinner), it
doesn't belong in a domain — put it in `shared/ui/`:

```bash
ng generate component shared/ui/<component-name>
```

If a component is specific to one domain but reused across that
domain's own features (like `ui-user-navbar` inside `user`), it goes in
that domain as `ui-<component-name>`, not in `shared/`.

---

## Working with roles (Worker, Admin tiers)

Role checks always read from `shared/utils/roles.ts`'s `UserRole` /
`AdminRole` enums — never hardcode a role string like `'worker'` in a
component. Read the current user's role via `AuthService` (exported
from `domains/auth`):

```typescript
import { AuthService } from '../../auth';
import { UserRole } from '../../../shared/utils/roles';

constructor(private authService: AuthService) {}

ngOnInit() {
  this.isWorker = this.authService.getUserRole() === UserRole.Worker;
}
```

For a route that should be entirely blocked for non-admins (rather than
just hiding a section of UI), use `adminGuard` from
`core/guards/role-guards.ts`:

```typescript
{
  path: 'admin',
  canActivate: [adminGuard],
  loadComponent: () => import('./domains/admin').then((m) => m.AdminDashboard),
},
```

---

## Notes

- Global CSS variables (colors, fonts, spacing) are defined in
  `app.css` — see `file-structure.md`'s "Design tokens" section for the
  real values and a gotcha about `--background-color`. Buttons like
  CTA's are defined in `styles.css` at the root, along with the navbar
  shell shared by both navbars. Use variables via `var(--variable-name)`
  — do not hardcode colors.
- The design uses two fonts: **Playfair Display** for headings and
  **Lato** for body text. Keep this consistent.
- This project uses **Angular SSR**. Never use `document` or `window`
  directly — wrap them in a platform check or you will get a
  `document is not defined` warning, or a crash if things get bad:

```typescript
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

if (isPlatformBrowser(this.platformId)) {
  // safe to use document / window here
}
```

---

## Checking what's actually built vs. just planned

Not every screen in the product diagram has a component yet. Each
domain's `index.ts` has the real exports at the top and a commented-out
list of planned-but-not-built components at the bottom — check there
before assuming something exists. `app.routes.ts` only routes to what's
real; planned routes are commented out alongside it.

---

## Enforcing the domain boundary (recommended, not yet set up)

Right now, "only import through `index.ts`" is a convention a reviewer
has to catch by eye. To make it a lint error instead:

```bash
npm install --save-dev eslint-plugin-boundaries
```

or, if you want the full task-graph/caching benefits too and are
willing to take on the tooling:

```bash
npx nx init
```

Either can be layered onto this exact folder structure without moving
any files.

---

## Need Help?

Reach out to the frontend lead or open an issue in the repo.
