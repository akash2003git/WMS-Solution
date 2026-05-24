export interface TodayAttendance {
  hasCheckedIn: boolean;
  hasCheckedOut: boolean;
  checkIn?: string;
  checkOut?: string;
  totalHours?: number;
  workMode?: string;
}
