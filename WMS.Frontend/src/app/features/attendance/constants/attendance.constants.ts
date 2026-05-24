export interface Absentee {
  employeeId: number;
  employeeName: string;
  department: string;
  role: string;
}

export const WORK_MODES = [
  'WFO',
  'WFH',
  'Hybrid'
];
