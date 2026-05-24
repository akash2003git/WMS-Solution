export interface EmployeeFilter {
  search?: string;
  departmentId?: number;
  roleId?: number;
  status?: number;
  pageNumber: number;
  pageSize: number;
  sortBy?: string;
  sortDirection?: string;
}
