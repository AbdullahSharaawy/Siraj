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

## Adding a New Page

### 1. Generate the component

```bash
ng generate component Pages/your-page-name
```

### 2. Register the route in `app.routes.ts`

```typescript
import { YourPageName } from './Pages/your-page-name/your-page-name';

export const routes: Routes = [
  { path: '', component: MainPage },
  { path: 'your-page-name', component: YourPageName },
];
```

### 3. Link to it from anywhere using `routerLink`

```html
<a [routerLink]="'/your-page-name'">Go to page</a>
```

---

## Adding a New Components

### 1. Generate the component

```bash
ng generate component Components/your-component-name
```

### 2. Either add it to your page or use it to help make other components

---

## Notes

- Global CSS variables (colors, fonts, spacing) are defined in `app.css` and buttons like CTA's are defined in `styles.css` at the root. Use them via `var(--variable-name)` do not as much as possiable to not hardcode colors.
- The design uses two fonts: **Playfair Display** for headings and **Lato** for body text. Keep this consistent.

This project uses **Angular SSR**. Never use `document` or `window` directly, wrap them in a platform check or you will get a `document is not defined` warning or crash if things get bad:
```typescript
import { Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

if (isPlatformBrowser(this.platformId)) {
  // safe to use document / window here
}
```
---

## Need Help?

Reach out to the frontend lead or open an issue in the repo.