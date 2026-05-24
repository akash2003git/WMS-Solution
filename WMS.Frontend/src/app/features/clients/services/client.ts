import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { Client } from '../models/client.model';
import { CreateClientRequest } from '../models/create-client.model';
import { UpdateClientRequest } from '../models/update-client.model';
import { ClientProject } from '../models/client-project.model';

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Clients`;

  getClients(): Observable<ApiResponse<Client[]>> {
    return this.http.get<ApiResponse<Client[]>>(this.apiUrl);
  }

  createClient(request: CreateClientRequest) {
    return this.http.post<ApiResponse<Client>>(this.apiUrl, request);
  }

  updateClient(clientId: number, request: UpdateClientRequest) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${clientId}`, request);
  }

  deleteClient(clientId: number) {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${clientId}`);
  }

  getClientProjects(clientId: number) {
    return this.http.get<ApiResponse<ClientProject[]>>(`${this.apiUrl}/${clientId}/projects`);
  }
}
