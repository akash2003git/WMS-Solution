import { Routes } from '@angular/router';
import { Login } from './features/auth/pages/login/login';
import { ResetPassword } from './features/auth/pages/reset-password/reset-password';
import { AdminDashboard } from './features/dashboard/pages/admin-dashboard/admin-dashboard';
import { ManagerDashboard } from './features/dashboard/pages/manager-dashboard/manager-dashboard';
import { EmployeeDashboard } from './features/dashboard/pages/employee-dashboard/employee-dashboard';
import { DepartmentList } from './features/departments/pages/department-list/department-list';
import { EmployeeList } from './features/employees/pages/employee-list/employee-list';
import { AttendanceList } from './features/attendance/pages/attendance-list/attendance-list';
import { LeaveList } from './features/leaves/pages/leave-list/leave-list';
import { ProjectList } from './features/projects/pages/project-list/project-list';
import { ClientList } from './features/clients/pages/client-list/client-list';
import { AnnouncementList } from './features/announcements/pages/announcement-list/announcement-list';
import { ReportDashboard } from './features/reports/pages/report-dashboard/report-dashboard';
import { MainLayout } from './core/layout/main-layout/main-layout';
import { Unauthorized } from './shared/components/unauthorized/unauthorized';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
import { loginRedirectGuard } from './core/guards/login-redirect-guard';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [loginRedirectGuard] },
  { path: 'reset-password', component: ResetPassword },
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
        data: { roles: ['Admin'] }
      },
      {
        path: 'manager/dashboard',
        component: ManagerDashboard,
        canActivate: [roleGuard],
        data: { roles: ['Manager'] }
      },
      {
        path: 'employee/dashboard',
        component: EmployeeDashboard,
        canActivate: [roleGuard],
        data: { roles: ['Employee'] }
      },
      {
        path: 'departments',
        component: DepartmentList,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: 'employees',
        component: EmployeeList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'attendance',
        component: AttendanceList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager', 'Employee']
        }
      },
      {
        path: 'leaves',
        component: LeaveList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager', 'Employee']
        }
      },
      {
        path: 'projects',
        component: ProjectList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'clients',
        component: ClientList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin']
        }
      },
      {
        path: 'announcements',
        component: AnnouncementList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager', 'Employee']
        }
      },
      {
        path: 'reports',
        component: ReportDashboard,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];
