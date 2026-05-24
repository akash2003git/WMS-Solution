import { NavigationItem } from '../models/navigation-item.model';

export const NAVIGATION_ITEMS: NavigationItem[] = [
  {
    label: 'Dashboard',
    icon: 'dashboard',
    route: '/admin/dashboard',
    roles: ['Admin']
  },
  {
    label: 'Dashboard',
    icon: 'dashboard',
    route: '/manager/dashboard',
    roles: ['Manager']
  },
  {
    label: 'Dashboard',
    icon: 'dashboard',
    route: '/employee/dashboard',
    roles: ['Employee']
  },
  {
    label: 'Departments',
    icon: 'apartment',
    route: '/departments',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'Employees',
    icon: 'groups',
    route: '/employees',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'My Attendance',
    icon: 'schedule',
    route: '/my-attendance',
    roles: ['Employee']
  },
  {
    label: 'Manage Attendance',
    icon: 'calendar_month',
    route: '/manage-attendance',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'Absentees',
    icon: 'person_off',
    route: '/attendance/absentees',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'My Leaves',
    icon: 'event_note',
    route: '/my-leaves',
    roles: ['Employee']
  },
  {
    label: 'Manage Leaves',
    icon: 'event_busy',
    route: '/manage-leaves',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'Projects',
    icon: 'assignment',
    route: '/projects',
    roles: ['Admin', 'Manager']
  },
  {
    label: 'My Projects',
    icon: 'assignment',
    route: '/my-projects',
    roles: ['Employee']
  },
  {
    label: 'Clients',
    icon: 'business',
    route: '/clients',
    roles: ['Admin']
  },
  {
    label: 'Announcements',
    icon: 'campaign',
    route: '/announcements',
    roles: ['Admin', 'Manager', 'Employee']
  },
  {
    label: 'Reports',
    icon: 'analytics',
    route: '/reports',
    roles: ['Admin']
  }
];
