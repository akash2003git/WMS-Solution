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
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { LeaveService } from '../../services/leave';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { PageHeader } from '../../../../shared/components/page-header/page-header';

@Component({
  selector: 'app-manage-leaves',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    StatusChip,
    PageHeader,
    ...MATERIAL_MODULES
  ],
  templateUrl: './manage-leaves.html'
})
export class ManageLeaves {
  private readonly leaveService = inject(LeaveService);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);
  displayedColumns = [
    'employee',
    'leaveType',
    'fromDate',
    'toDate',
    'days',
    'status',
    'actions'
  ];

  loading = false;
  errorMessage = '';

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  filterForm =
    this.fb.group({
      status: ['Pending'],
      fromDate: [null],
      toDate: [null]
    });

  leaves$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });
      const filter = this.buildFilters();
      const request$ = filter.status === 'Pending'
        ? this.leaveService.getPendingLeaves()
        : this.leaveService.getLeaves(filter);
      return request$.pipe(
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

  applyFilters(): void {
    this.refresh$.next();
  }

  approveLeave(leaveId: number): void {
    this.leaveService
      .approveLeave(leaveId)
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Leave approved successfully',
            'Close',
            { duration: 3000 }
          );
          this.refresh$.next();
        },
        error: error => {
          console.error(error);
        }
      });
  }

  rejectLeave(leaveId: number): void {
    this.leaveService
      .rejectLeave(leaveId)
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Leave rejected successfully',
            'Close',
            { duration: 3000 }
          );
          this.refresh$.next();
        },

        error: error => {
          console.error(error);
        }
      });
  }

  private buildFilters() {
    const form = this.filterForm.getRawValue();
    return {
      status: form.status || undefined,
      fromDate: form.fromDate
        ? this.formatDate(form.fromDate)
        : undefined,
      toDate: form.toDate
        ? this.formatDate(form.toDate)
        : undefined
    };
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
