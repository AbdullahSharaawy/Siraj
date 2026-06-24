import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../domains/auth';
import { UserRole } from '../../shared/utils/roles';

export const adminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.getAdminRole()) {
    return true;
  }
  router.navigate(['/sign-in']);
  return false;
};

export const workerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.getUserRole() === UserRole.Worker) {
    return true;
  }
  router.navigate(['/dashboard']);
  return false;
};
