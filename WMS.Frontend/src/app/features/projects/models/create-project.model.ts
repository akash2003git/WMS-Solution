export interface CreateProjectRequest {
  projectName: string;
  clientId?: number | null;
  startDate?: string | null;
  endDate?: string | null;
  status: string;
}
