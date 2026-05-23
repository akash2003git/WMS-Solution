import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth';

export const loginRedirectGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const user = authService.getCurrentUser();

  if (!user) {
    return true;
  }

  switch (user.role) {
    case 'Admin':
      router.navigate(['/admin/dashboard']);
      break;

    case 'Manager':
      router.navigate(['/manager/dashboard']);
      break;

    case 'Employee':
      router.navigate(['/employee/dashboard']);
      break;
  }

  return false;
};
