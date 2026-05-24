import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { IEmployeeDashboard } from '../models/employee-dashboard.model';
import { IAdminDashboard } from '../models/admin-dashboard.model';
import { IManagerDashboard } from '../models/manager-dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Dashboard`;

  getEmployeeDashboard(): Observable<ApiResponse<IEmployeeDashboard>> {
    return this.http.get<ApiResponse<IEmployeeDashboard>>(
      `${this.apiUrl}/employee`
    );
  }

  getAdminDashboard(): Observable<ApiResponse<IAdminDashboard>> {
    return this.http.get<ApiResponse<IAdminDashboard>>(
      `${this.apiUrl}/admin`
    );
  }

  getManagerDashboard(): Observable<ApiResponse<IManagerDashboard>> {
    return this.http.get<ApiResponse<IManagerDashboard>>(
      `${this.apiUrl}/manager`
    );
  }
}
