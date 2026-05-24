import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  finalize,
  map,
  of,
  switchMap
} from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { LeaveService } from '../../services/leave';
import { Leave } from '../../models/leave.model';
import { LeaveFormDialog } from '../../components/leave-form-dialog/leave-form-dialog';

@Component({
  selector: 'app-my-leaves',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './my-leaves.html'
})
export class MyLeaves {
  private readonly leaveService = inject(LeaveService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns = [
    'leaveType',
    'fromDate',
    'toDate',
    'totalDays',
    'status',
    'appliedOn',
    'actions'
  ];

  loading = false;
  errorMessage = '';

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  leaves$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      return this.leaveService
        .getMyLeaves()
        .pipe(
          map(response => response.data),
          catchError(error => {
            console.error(error);
            queueMicrotask(() => {
              this.errorMessage = 'Failed to load leaves';
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

  openApplyDialog(): void {
    const dialogRef = this.dialog.open(
      LeaveFormDialog,
      {
        width: '500px'
      }
    );

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open(
          'Leave applied successfully',
          'Close',
          { duration: 3000 }
        );
        this.refresh$.next();
      }
    });
  }

  cancelLeave(leave: Leave): void {
    const dialogRef =
      this.dialog.open(
        ConfirmDialog,
        {
          width: '400px',
          data: {
            title: 'Cancel Leave',
            message: 'Are you sure you want to cancel this leave request?'
          }
        }
      );

    dialogRef.afterClosed().subscribe(result => {
      if (!result) { return; }
      this.leaveService
        .cancelLeave(leave.leaveId)
        .subscribe({
          next: () => {
            this.snackBar.open(
              'Leave cancelled successfully',
              'Close',
              { duration: 3000 }
            );
            this.refresh$.next();
          },
          error: error => {
            console.error(error);
            this.snackBar.open(
              error?.error?.message
              ?? 'Failed to cancel leave',
              'Close',
              { duration: 3000 }
            );
          }
        });
    });
  }

  canCancel(leave: Leave): boolean {
    return leave.status === 'Pending';
  }
}
