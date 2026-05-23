import { ActivatedRouteSnapshot, CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as string[];
  const user = authService.getCurrentUser();

  if (user && expectedRoles.includes(user.role)) {
    return true;
  }

  router.navigate(['/unauthorized']);
  return false;
};
