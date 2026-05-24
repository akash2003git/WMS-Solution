import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResponse } from '../../employees/models/paged-response.model';
import { Attendance } from '../models/attendance.model';
import { AttendanceFilter } from '../models/attendance-filter.model';
import { AttendanceRequest } from '../models/attendance-request.model';
import { MonthlyAttendanceReport } from '../models/monthly-attendance-report.model';
import { TodayAttendance } from '../models/today-attendance.model';
import { Absentee } from '../models/absentee.model';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Attendance`;

  getTodayAttendance(): Observable<ApiResponse<TodayAttendance>> {
    return this.http.get<ApiResponse<TodayAttendance>>(`${this.apiUrl}/today`);
  }

  checkIn(request: AttendanceRequest) {
    return this.http.post<ApiResponse<Attendance>>(`${this.apiUrl}/check-in`, request);
  }

  checkOut() {
    return this.http.post<ApiResponse<Attendance>>(`${this.apiUrl}/check-out`, {});
  }

  getMyHistory(filter: AttendanceFilter) {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber)
      .set('pageSize', filter.pageSize);

    if (filter.fromDate) {
      params = params.set('fromDate', filter.fromDate);
    }

    if (filter.toDate) {
      params = params.set('toDate', filter.toDate);
    }

    return this.http.get<ApiResponse<PagedResponse<Attendance>>>
      (`${this.apiUrl}/my-history`, { params });
  }

  getMyMonthlyReport() {
    return this.http.get<ApiResponse<MonthlyAttendanceReport>>(`${this.apiUrl}/my-monthly-report`);
  }

  getAttendanceHistory(filter: AttendanceFilter) {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber)
      .set('pageSize', filter.pageSize);

    if (filter.employeeId) {
      params = params.set('employeeId', filter.employeeId);
    }

    if (filter.fromDate) {
      params = params.set('fromDate', filter.fromDate);
    }

    if (filter.toDate) {
      params = params.set('toDate', filter.toDate);
    }

    return this.http.get<ApiResponse<PagedResponse<Attendance>>>(this.apiUrl, { params });
  }

  getEmployeeMonthlyReport(employeeId: number) {
    return this.http.get<ApiResponse<MonthlyAttendanceReport>>(
      `${this.apiUrl}/employee/${employeeId}/monthly-report`
    );
  }

  getAbsentees() {
    return this.http.get<ApiResponse<Absentee[]>>(`${this.apiUrl}/absentees`);
  }
}
