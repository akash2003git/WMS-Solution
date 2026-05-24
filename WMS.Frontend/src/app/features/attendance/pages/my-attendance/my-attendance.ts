import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  BehaviorSubject,
  catchError,
  combineLatest,
  finalize,
  map,
  of,
  switchMap
} from 'rxjs';
import { PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';

import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { Pagination } from '../../../../shared/components/pagination/pagination';

import { AttendanceService } from '../../services/attendance';
import { WORK_MODES } from '../../constants/attendance.constants';

@Component({
  selector: 'app-my-attendance',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PageHeader,
    Pagination,
    ...MATERIAL_MODULES
  ],
  templateUrl: './my-attendance.html'
})
export class MyAttendance {
  private readonly attendanceService = inject(AttendanceService);
  private readonly fb = inject(FormBuilder);
  private readonly snackBar = inject(MatSnackBar);

  workModes = WORK_MODES;

  displayedColumns = [
    'date',
    'checkIn',
    'checkOut',
    'hours',
    'workMode'
  ];

  loading = false;
  actionLoading = false;
  errorMessage = '';

  pageNumber = 1;
  pageSize = 10;

  private readonly refresh$ =
    new BehaviorSubject<void>(undefined);

  workModeForm =
    this.fb.group({
      workMode: ['WFO', Validators.required]
    });

  dashboard$ =
    this.refresh$.pipe(
      switchMap(() => {
        queueMicrotask(() => {
          this.loading = true;
          this.errorMessage = '';
        });

        const currentDate = new Date();

        const fromDate =
          new Date(
            currentDate.getFullYear(),
            currentDate.getMonth(),
            1
          );

        const toDate =
          new Date(
            currentDate.getFullYear(),
            currentDate.getMonth() + 1,
            0
          );

        return combineLatest([
          this.attendanceService.getTodayAttendance(),
          this.attendanceService.getMyMonthlyReport(),
          this.attendanceService.getMyHistory({
            pageNumber: this.pageNumber,
            pageSize: this.pageSize,
            fromDate: this.formatDate(fromDate),
            toDate: this.formatDate(toDate)
          })
        ]).pipe(
          map(([today, report, history]) => ({
            today: today.data,
            report: report.data,
            history: history.data
          })),
          catchError(error => {
            console.error(error);

            queueMicrotask(() => {
              this.errorMessage =
                'Failed to load attendance';
            });

            return of(null);
          }),
          finalize(() => {
            queueMicrotask(() => {
              this.loading = false;
            });
          })
        );
      })
    );

  checkIn(): void {
    if (this.workModeForm.invalid) {
      return;
    }

    this.actionLoading = true;

    this.attendanceService
      .checkIn({
        workMode:
          this.workModeForm.value.workMode!
      })
      .pipe(
        finalize(() => {
          this.actionLoading = false;
        })
      )
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Checked in successfully',
            'Close',
            { duration: 3000 }
          );

          this.refresh$.next();
        },

        error: error => {
          console.error(error);

          this.snackBar.open(
            error?.error?.message
            ?? 'Failed to check in',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }

  checkOut(): void {
    this.actionLoading = true;

    this.attendanceService
      .checkOut()
      .pipe(
        finalize(() => {
          this.actionLoading = false;
        })
      )
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Checked out successfully',
            'Close',
            { duration: 3000 }
          );

          this.refresh$.next();
        },

        error: error => {
          console.error(error);

          this.snackBar.open(
            error?.error?.message
            ?? 'Failed to check out',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;

    this.refresh$.next();
  }

  private formatDate(date: Date): string {
    return date
      .toISOString()
      .split('T')[0];
  }
}
