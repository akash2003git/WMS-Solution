export interface Project {
  projectId: number;
  projectName: string;
  clientId?: number;
  clientName?: string;
  startDate?: string;
  endDate?: string;
  status: string;
  totalEmployees: number;
}
