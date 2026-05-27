export interface JwtPayload {
  exp: number;
  role?: string;
  employeeId?: string;
  departmentId?: string;
}
