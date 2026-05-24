export interface Leave {
  leaveId: number;
  employeeId: number;
  employeeName: string;
  leaveType: string;
  reason?: string;
  fromDate: string;
  toDate: string;
  totalDays: number;
  status: string;
  appliedOn: string;
  approvedBy?: number;
  approvedOn?: string;
}
