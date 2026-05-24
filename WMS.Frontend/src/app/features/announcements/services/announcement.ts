import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { Announcement } from '../models/announcement.model';
import { CreateAnnouncementRequest } from '../models/create-announcement.model';
import { UpdateAnnouncementRequest } from '../models/update-announcement.model';

@Injectable({
  providedIn: 'root'
})
export class AnnouncementService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/Announcements`;

  getAnnouncements(): Observable<ApiResponse<Announcement[]>> {
    return this.http.get<ApiResponse<Announcement[]>>(this.apiUrl);
  }

  getActiveAnnouncements(): Observable<ApiResponse<Announcement[]>> {
    return this.http.get<ApiResponse<Announcement[]>>(`${this.apiUrl}/active`);
  }

  createAnnouncement(request: CreateAnnouncementRequest) {
    return this.http.post<ApiResponse<Announcement>>(this.apiUrl, request);
  }

  updateAnnouncement(announcementId: number, request: UpdateAnnouncementRequest) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${announcementId}`, request);
  }

  activateAnnouncement(announcementId: number) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${announcementId}/activate`, {});
  }

  deactivateAnnouncement(announcementId: number) {
    return this.http.put<ApiResponse<string>>(`${this.apiUrl}/${announcementId}/deactivate`, {});
  }
}
