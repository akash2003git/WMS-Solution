import { Routes } from '@angular/router';
import { Login } from './features/auth/pages/login/login';
import { AdminDashboard } from './features/dashboard/pages/admin-dashboard/admin-dashboard';
import { ManagerDashboard } from './features/dashboard/pages/manager-dashboard/manager-dashboard';
import { EmployeeDashboard } from './features/dashboard/pages/employee-dashboard/employee-dashboard';
import { MainLayout } from './core/layout/main-layout/main-layout';
import { Unauthorized } from './shared/components/unauthorized/unauthorized';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
import { loginRedirectGuard } from './core/guards/login-redirect-guard';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [loginRedirectGuard] },
  { path: 'unauthorized', component: Unauthorized },
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'admin/dashboard',
        component: AdminDashboard,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin']
        }
      },
      {
        path: 'manager/dashboard',
        component: ManagerDashboard,
        canActivate: [roleGuard],
        data: {
          roles: ['Manager']
        }
      },
      {
        path: 'employee/dashboard',
        component: EmployeeDashboard,
        canActivate: [roleGuard],
        data: {
          roles: ['Employee']
        }
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
