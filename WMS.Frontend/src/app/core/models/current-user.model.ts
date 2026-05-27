export interface CurrentUser {
  userId: number;
  username: string;
  role: string;
  token: string;
  employeeId?: number;
  departmentId?: number;
}
