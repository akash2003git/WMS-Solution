export interface MonthlyAttendanceReport {
  employeeId: number;
  employeeName: string;
  year: number;
  month: number;
  totalPresentDays: number;
  totalAbsentDays: number;
  totalHoursWorked: number;
  averageHoursPerDay: number;
  attendancePercentage: number;
}
