import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { Leave } from '../models/leave.model';
import { ApplyLeaveRequest } from '../models/apply-leave-request.model';
import { LeaveFilter } from '../models/leave-filter.model';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Leaves`;

  getMyLeaves(): Observable<ApiResponse<Leave[]>> {
    return this.http.get<ApiResponse<Leave[]>>(`${this.apiUrl}/my-leaves`);
  }

  getLeaves(filter: LeaveFilter): Observable<ApiResponse<Leave[]>> {
    let params = new HttpParams();

    if (filter.employeeId) {
      params = params.set('employeeId', filter.employeeId);
    }

    if (filter.status) {
      params = params.set('status', filter.status);
    }

    if (filter.fromDate) {
      params = params.set('fromDate', filter.fromDate);
    }

    if (filter.toDate) {
      params = params.set('toDate', filter.toDate);
    }

    return this.http.get<ApiResponse<Leave[]>>(this.apiUrl, { params });
  }

  getPendingLeaves(): Observable<ApiResponse<Leave[]>> {
    return this.http.get<ApiResponse<Leave[]>>(`${this.apiUrl}/pending`);
  }

  applyLeave(request: ApplyLeaveRequest) {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/apply`, request);
  }

  cancelLeave(id: number) {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/${id}/cancel`, {});
  }

  approveLeave(id: number) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${id}/approve`, {});
  }

  rejectLeave(id: number) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${id}/reject`, {});
  }
}
