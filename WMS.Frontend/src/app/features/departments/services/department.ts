import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { Department } from '../models/department.model';
import { CreateDepartmentRequest } from '../models/create-department.model';
import { UpdateDepartmentRequest } from '../models/update-department.model';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Departments`;

  getDepartments(): Observable<ApiResponse<Department[]>> {
    return this.http.get<ApiResponse<Department[]>>(this.apiUrl);
  }

  createDepartment(request: CreateDepartmentRequest) {
    return this.http.post<ApiResponse<string>>(this.apiUrl, request);
  }

  updateDepartment(id: number, request: UpdateDepartmentRequest) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${id}`, request);
  }

  deleteDepartment(id: number) {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${id}`);
  }
}
