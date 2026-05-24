import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { Project } from '../models/project.model';
import { CreateProjectRequest } from '../models/create-project.model';
import { UpdateProjectRequest } from '../models/update-project.model';
import { AssignEmployeeRequest } from '../models/assign-employee.model';
import { ProjectAllocation } from '../models/project-allocation.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Projects`;

  getProjects(): Observable<ApiResponse<Project[]>> {
    return this.http.get<ApiResponse<Project[]>>(this.apiUrl);
  }

  getMyProjects(): Observable<ApiResponse<Project[]>> {
    return this.http.get<ApiResponse<Project[]>>(`${this.apiUrl}/my-projects`);
  }

  createProject(request: CreateProjectRequest) {
    return this.http.post<ApiResponse<Project>>(this.apiUrl, request);
  }

  updateProject(projectId: number, request: UpdateProjectRequest) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${projectId}`, request);
  }

  getProjectAllocations(projectId: number) {
    return this.http.get<ApiResponse<ProjectAllocation[]>>(`${this.apiUrl}/${projectId}/allocations`);
  }

  assignEmployee(projectId: number, request: AssignEmployeeRequest) {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/${projectId}/assign-employee`, request);
  }

  removeEmployee(projectId: number, employeeId: number) {
    return this.http.delete<ApiResponse<string>>(`${this.apiUrl}/${projectId}/remove-employee/${employeeId}`);
  }
}
