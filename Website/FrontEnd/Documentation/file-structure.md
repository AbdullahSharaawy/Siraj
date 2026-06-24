# File structure

This project is organized by **domain**, not by file type. The old
layout had one flat `Components/` and `Pages/` folder; this one groups
everything related to a business area (auth, the donor experience, an
org managing itself, platform admins) into its own folder under
`domains/`.

```bash
src/
├── app
│   ├── app.config.server.ts
│   ├── app.config.ts
│   ├── app.css              <-- Global colors/variables
│   ├── app.html
│   ├── app.routes.server.ts
│   ├── app.routes.ts        <-- Top-level routes. ONLY imports from a
│   │                            domain's index.ts, never its internals.
│   ├── app.spec.ts
│   ├── app.ts
│   │
│   ├── core/                <-- App-shell concerns, not business logic
│   │   ├── guards/
│   │   │   └── role-guards.ts   (adminGuard, workerGuard)
│   │   └── services/         (singletons used app-wide, not specific
│   │                          to one domain)
│   │
│   ├── shared/               <-- Reusable
│   │   ├── ui/               (buttons, inputs, spinner — pure
│   │   │                      presentation, no business logic)
│   │   └── utils/
│   │       └── roles.ts      (UserRole, AdminRole the role model
│   │                          every domain reads from instead of
│   │                          inventing its own permission strings
│   │
│   └── domains/
│       ├── public/           <-- Marketing site, logged-out visitors
│       │   ├── feature-welcome/
│       │   ├── ui-nav-bar/   (the LOGGED-OUT navbar)
│       │   └── index.ts
│       │
│       ├── auth/             <-- Sign in / sign up / session
│       │   ├── feature-sign-in/
│       │   ├── feature-sign-up/
│       │   ├── data-access/
│       │   │   └── authentication.ts   (AuthService — the ONLY place
│       │   │                            that calls the auth API)
│       │   └── index.ts
│       │
│       ├── user/             <-- Donor-facing. "Worker" is a ROLE
│       │   │                     inside this domain, not its own
│       │   │                     domain, see "Roles vs domains" below.
│       │   ├── feature-user-page/      (shell: navbar + router-outlet)
│       │   ├── feature-donated-items/  (worker-gated UI lives here)
│       │   ├── feature-donation-history/   (planned)
│       │   ├── feature-org-browse/         (planned)
│       │   ├── feature-campaign-details/   (planned)
│       │   ├── ui-user-navbar/         (the LOGGED-IN navbar)
│       │   ├── data-access/
│       │   └── index.ts
│       │
│       ├── org/              <-- An ORG managing ITSELF: its own
│       │   │                     campaigns, profile, analytics.
│       │   │                     Not the same as admin.
│       │   ├── feature-org-dashboard/
│       │   ├── feature-create-campaign/
│       │   ├── feature-view-campaigns/     (planned)
│       │   ├── data-access/
│       │   └── index.ts
│       │
│       └── admin/            <-- Platform operators managing ORGS
│           │                     from outside: create/edit orgs,
│           │                     payment keys, promote users to
│           │                     admin. Separate people, separate
│           │                     permission level from org. Nothing
│           │                     built here yet, see index.ts.
│           ├── feature-admin-dashboard/    (planned)
│           ├── feature-org-management/     (planned)
│           ├── data-access/
│           └── index.ts
│
├── file-structure.md  <-- you are here
├── index.html
├── main.server.ts
├── main.ts
├── server.ts
└── styles.css         <-- Global styles + the navbar shell shared by
                            ui-nav-bar and ui-user-navbar
```

## The one rule that matters: import through `index.ts`

Every domain has an `index.ts` (its "public API" / barrel). **Code
outside a domain must only import from that domain's `index.ts`, never
from a file inside it.**

```ts
// correct: goes through the barrel
import { SignIn, AuthService } from '../../domains/auth';

// wrong: reaches into auth's internals from outside the domain
import { AuthService } from '../../domains/auth/data-access/authentication';
```

Inside a single domain, importing between its own sibling folders is
fine and doesn't need the barrel (e.g. `feature-user-page` importing
`UserNavbar` directly from `../ui-user-navbar/user-navbar`).

This is currently a **convention**, not yet enforced by tooling. See
"Enforcing the boundary" at the end of `GetStarted.md` for how to make
it a lint error instead of something people can accidentally break.

## Naming convention inside a domain

- `feature-*`: a routable page/screen with business logic specific to
  that domain (e.g. `feature-sign-in`, `feature-create-campaign`)
- `ui-*`: a presentational component reused across that domain's
  features but not meant to be used by other domains (e.g.
  `ui-user-navbar`)
- `data-access/`: services that talk to the backend for this domain.
  This is the only place HTTP calls for that domain's data should
  happen.

## Roles vs domains please don't confuse the two

The diagram this structure is based on has two separate hierarchies.
Mixing them up is the easiest way to end up with the wrong folder:

| | Lives in | Why |
|---|---|---|
| Donor -> **Worker** | `domains/user` (it's a role, not a domain) | A worker is a donor account with an extra permission flag. Same routes `feature-donated-items` branches its template on `AuthService.getUserRole()` rather than routing somewhere different. |
| Ultra Super Admin -> Super Admin of Org -> Sub Admin | `domains/admin` (its own domain) | These manage orgs **from outside** creating orgs, editing them, payment keys, promoting users to admin. They have different permission level than an org managing its own campaigns. |
| An org managing its own campaigns | `domains/org` | Separate from `admin` even though both touch "organizations" — org only ever acts on itself. |

If you're adding a new screen and you're not sure whether it's a new
role inside an existing domain or a genuinely new domain, ask: *does
this person manage other people/orgs from outside, or just operate
within their own account?* Outside -> probably a new or existing admin
concern. Within their own account -> existing domain, maybe a new role
flag.

## Design tokens

Real values, defined in `app.css`:

```css
--pr-color:       #FFD400;  /* primary — yellow */
--pr-light-color: #FFC300;
--sc-color:       #FF8C00;  /* secondary — orange */
--sc-light-color: #FF5F00;

--charcoal:         #1e1e1e;
--warm-white:       #fdf4e8;
--off-white:        #fdf4e8;   /* same as warm-white, used for surfaces */
--warm-gray:        #7a6a5a;
--background-color: #fdf4e8;   /* same as off-white — page background */
--border-color:     #e8ddd0;

--font-display: 'Playfair Display', Georgia, serif;  /* headings */
--font-body:    'Lato', 'Trebuchet MS', sans-serif;    /* everything else */

--nb-height: 72px;
--nb-h-sm:   60px;
```

**Watch out:** `--background-color`, `--off-white`, and `--warm-white`
are all literally the same hex value (`#fdf4e8`). If you're styling
something that needs to be visible *against* the page background (a
progress track, a divider), reach for `--border-color` instead — using
`--background-color` there will make it invisible. This bit us once
already in `welcome.css`'s `.camp-progress`.
