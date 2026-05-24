import { Routes } from '@angular/router';
import { Login } from './features/auth/pages/login/login';
import { ResetPassword } from './features/auth/pages/reset-password/reset-password';
import { AdminDashboard } from './features/dashboard/pages/admin-dashboard/admin-dashboard';
import { ManagerDashboard } from './features/dashboard/pages/manager-dashboard/manager-dashboard';
import { EmployeeDashboard } from './features/dashboard/pages/employee-dashboard/employee-dashboard';
import { DepartmentList } from './features/departments/pages/department-list/department-list';
import { EmployeeList } from './features/employees/pages/employee-list/employee-list';
import { AttendanceList } from './features/attendance/pages/attendance-list/attendance-list';
import { MyAttendance } from './features/attendance/pages/my-attendance/my-attendance';
import { AbsenteeList } from './features/attendance/pages/absentee-list/absentee-list';
import { ProjectList } from './features/projects/pages/project-list/project-list';
import { ClientList } from './features/clients/pages/client-list/client-list';
import { AnnouncementList } from './features/announcements/pages/announcement-list/announcement-list';
import { ReportDashboard } from './features/reports/pages/report-dashboard/report-dashboard';
import { MainLayout } from './core/layout/main-layout/main-layout';
import { Unauthorized } from './shared/components/unauthorized/unauthorized';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';
import { loginRedirectGuard } from './core/guards/login-redirect-guard';
import { MyLeaves } from './features/leaves/pages/my-leaves/my-leaves';
import { ManageLeaves } from './features/leaves/pages/manage-leaves/manage-leaves';
import { MyProjects } from './features/projects/pages/my-projects/my-projects';

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
        data: {
          roles: ['Admin', 'Manager']
        }
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
        path: 'my-attendance',
        component: MyAttendance,
        canActivate: [roleGuard],
        data: {
          roles: ['Employee']
        }
      },
      {
        path: 'manage-attendance',
        component: AttendanceList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'attendance/absentees',
        component: AbsenteeList,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'my-leaves',
        component: MyLeaves,
        canActivate: [roleGuard],
        data: {
          roles: ['Employee']
        }
      },
      {
        path: 'manage-leaves',
        component: ManageLeaves,
        canActivate: [roleGuard],
        data: {
          roles: ['Admin', 'Manager']
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
        path: 'my-projects',
        component: MyProjects,
        canActivate: [roleGuard],
        data: {
          roles: ['Employee']
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
