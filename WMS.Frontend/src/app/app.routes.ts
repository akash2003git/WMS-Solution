import { Routes } from '@angular/router';
import { Login } from './features/auth/pages/login/login';
import { AdminDashboard } from './features/dashboard/pages/admin-dashboard/admin-dashboard';
import { ManagerDashboard } from './features/dashboard/pages/manager-dashboard/manager-dashboard';
import { EmployeeDashboard } from './features/dashboard/pages/employee-dashboard/employee-dashboard';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  {
    path: 'admin/dashboard',
    component: AdminDashboard,
    canActivate: [authGuard, roleGuard],
    data: {
      roles: ['Admin']
    }
  },
  {
    path: 'manager/dashboard',
    component: ManagerDashboard,
    canActivate: [authGuard, roleGuard],
    data: {
      roles: ['Manager']
    }
  },
  {
    path: 'employee/dashboard',
    component: EmployeeDashboard,
    canActivate: [authGuard, roleGuard],
    data: {
      roles: ['Employee']
    }
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
