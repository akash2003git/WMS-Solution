export interface Attendance {
  attendanceId: number;
  employeeId: number;
  employeeName: string;
  attendanceDate: string;
  checkIn: string;
  checkOut?: string;
  totalHours?: number;
  workMode?: string;
}
