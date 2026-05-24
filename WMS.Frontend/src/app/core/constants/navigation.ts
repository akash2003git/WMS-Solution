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
    label: 'Attendance',
    icon: 'calendar_month',
    route: '/attendance',
    roles: ['Admin', 'Manager', 'Employee']
  },
  {
    label: 'Leaves',
    icon: 'event_busy',
    route: '/leaves',
    roles: ['Admin', 'Manager', 'Employee']
  },
  {
    label: 'Projects',
    icon: 'assignment',
    route: '/projects',
    roles: ['Admin', 'Manager']
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
