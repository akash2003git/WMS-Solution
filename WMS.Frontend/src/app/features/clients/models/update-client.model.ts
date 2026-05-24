export interface UpdateClientRequest {
  clientName: string;
  clientAddress?: string;
  clientPhoneNumber?: string;
  clientLocation?: string;
  status: boolean;
}
