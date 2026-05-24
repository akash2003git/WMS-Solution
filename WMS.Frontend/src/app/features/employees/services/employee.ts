import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { CreateEmployeeRequest } from '../models/create-employee.model';
import { CreateEmployeeResponse } from '../models/create-employee-response.model';
import { EmployeeFilter } from '../models/employee-filter.model';
import { Employee } from '../models/employee.model';
import { PagedResponse } from '../models/paged-response.model';
import { UpdateEmployeeRequest } from '../models/update-employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Employees`;

  getEmployees(filter: EmployeeFilter): Observable<ApiResponse<PagedResponse<Employee>>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber)
      .set('pageSize', filter.pageSize);

    if (filter.search) {
      params = params.set('search', filter.search);
    }

    if (filter.departmentId) {
      params = params.set('departmentId', filter.departmentId);
    }

    if (filter.roleId) {
      params = params.set('roleId', filter.roleId);
    }

    if (filter.status) {
      params = params.set('status', filter.status);
    }

    if (filter.sortBy) {
      params = params.set('sortBy', filter.sortBy);
    }

    if (filter.sortDirection) {
      params = params.set('sortDirection', filter.sortDirection);
    }

    return this.http.get<ApiResponse<PagedResponse<Employee>>>(this.apiUrl, { params }
    );
  }

  createEmployee(request: CreateEmployeeRequest) {
    return this.http.post<ApiResponse<CreateEmployeeResponse>>(this.apiUrl, request);
  }

  updateEmployee(id: number, request: UpdateEmployeeRequest) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${id}`, request);
  }

  deleteEmployee(id: number) {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
