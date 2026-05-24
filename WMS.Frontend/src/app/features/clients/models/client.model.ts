export interface Client {
  clientId: number;
  clientName: string;
  clientAddress?: string;
  clientPhoneNumber?: string;
  clientLocation?: string;
  status: boolean;
  totalProjects: number;
}
