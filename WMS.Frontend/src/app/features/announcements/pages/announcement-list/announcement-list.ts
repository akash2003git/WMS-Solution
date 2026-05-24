import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { AnnouncementService } from '../../services/announcement';
import { Announcement } from '../../models/announcement.model';
import { AnnouncementFormDialog } from '../../components/announcement-form-dialog/announcement-form-dialog';
import { AuthService } from '../../../auth/services/auth';

@Component({
  selector: 'app-announcement-list',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './announcement-list.html'
})
export class AnnouncementList {
  private readonly announcementService = inject(AnnouncementService);
  private readonly authService = inject(AuthService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  currentUser = this.authService.getCurrentUser();

  isAdminView = this.currentUser?.role === 'Admin' || this.currentUser?.role === 'Manager';

  displayedColumns = [
    'title',
    'createdOn',
    'status',
    'actions'
  ];

  loading = false;
  errorMessage = '';

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  announcements$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      const request$ = this.isAdminView
        ? this.announcementService.getAnnouncements()
        : this.announcementService.getActiveAnnouncements();

      return request$.pipe(
        map(response => response.data ?? []),
        map(announcements =>
          [...announcements].sort((a, b) =>
            new Date(b.createdOn).getTime() - new Date(a.createdOn).getTime()
          )
        ),
        catchError(error => {
          console.error(error);

          queueMicrotask(() => {
            this.errorMessage = 'Failed to load announcements';
          });

          return of([]);
        }),
        finalize(() => {
          queueMicrotask(() => {
            this.loading = false;
          });
        })
      );
    })
  );

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(
      AnnouncementFormDialog,
      {
        width: '650px'
      }
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.refresh$.next();
      }
    });
  }

  openEditDialog(announcement: Announcement): void {
    const dialogRef = this.dialog.open(
      AnnouncementFormDialog,
      {
        width: '650px',
        data: announcement
      }
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.refresh$.next();
      }
    });
  }

  toggleAnnouncementStatus(announcement: Announcement): void {
    const operation = announcement.isActive
      ? this.announcementService.deactivateAnnouncement(announcement.announcementId)
      : this.announcementService.activateAnnouncement(announcement.announcementId);

    operation.subscribe({
      next: response => {
        this.snackBar.open(
          response.message,
          'Close',
          {
            duration: 3000
          }
        );

        this.refresh$.next();
      },
      error: (error: any) => {
        console.error(error);

        this.snackBar.open(
          error?.error?.message ?? 'Operation failed',
          'Close',
          {
            duration: 3000
          }
        );
      }
    });
  }
}
