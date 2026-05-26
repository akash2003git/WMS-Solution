import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { PageEvent } from '@angular/material/paginator';
import {
  BehaviorSubject,
  catchError,
  finalize,
  map,
  of,
  switchMap
} from 'rxjs';

import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { AttendanceService } from '../../services/attendance';
import { EmployeeService } from '../../../employees/services/employee';
import { RouterLink } from '@angular/router';
import { AttendanceReportDialog }
  from '../../components/attendance-report-dialog/attendance-report-dialog';
import { MatDialog } from '@angular/material/dialog';


@Component({
  selector: 'app-attendance-list',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PageHeader,
    Pagination,
    RouterLink,
    ...MATERIAL_MODULES
  ],
  templateUrl: './attendance-list.html'
})
export class AttendanceList {
  private readonly attendanceService =
    inject(AttendanceService);

  private readonly employeeService =
    inject(EmployeeService);

  private readonly fb =
    inject(FormBuilder);

  private readonly dialog =
    inject(MatDialog);

  displayedColumns = [
    'employeeName',
    'attendanceDate',
    'checkIn',
    'checkOut',
    'totalHours',
    'workMode'
  ];

  loading = false;
  errorMessage = '';


  filterForm = this.fb.group({
    employeeId: [null],
    fromDate: [this.startOfToday()],
    toDate: [this.startOfToday()]
  });

  private readonly refresh$ =
    new BehaviorSubject<void>(undefined);

  pagination = {
    pageNumber: 1,
    pageSize: 10
  };

  employees$ =
    this.employeeService
      .getEmployees({
        pageNumber: 1,
        pageSize: 50,
        status: 1
      })
      .pipe(
        map(response => response.data.items),
        catchError(error => {
          console.error(error);
          return of([]);
        })
      );

  attendanceResponse$ =
    this.refresh$.pipe(
      switchMap(() => {

        queueMicrotask(() => {
          this.loading = true;
          this.errorMessage = '';
        });

        return this.attendanceService
          .getAttendanceHistory(
            this.buildFilters()
          )
          .pipe(
            map(response => response.data),

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

  applyFilters(): void {
    this.pagination.pageNumber = 1;
    this.refresh$.next();
  }

  onPageChange(event: PageEvent): void {
    this.pagination.pageNumber =
      event.pageIndex + 1;

    this.pagination.pageSize =
      event.pageSize;

    this.refresh$.next();
  }

  openEmployeeReport(
    employeeId: number,
    employeeName: string
  ): void {

    this.dialog.open(
      AttendanceReportDialog,
      {
        width: '900px',
        maxWidth: '95vw',

        data: {
          employeeId,
          employeeName
        }
      }
    );
  }

  private buildFilters() {
    const form =
      this.filterForm.getRawValue();

    return {
      employeeId:
        form.employeeId || undefined,

      fromDate:
        form.fromDate
          ? this.formatDate(form.fromDate)
          : undefined,

      toDate:
        form.toDate
          ? this.formatDate(form.toDate)
          : undefined,

      pageNumber:
        this.pagination.pageNumber,

      pageSize:
        this.pagination.pageSize
    };
  }

  private formatDate(date: Date): string {
    return date
      .toISOString()
      .split('T')[0];
  }

  private startOfToday(): Date {
    const today = new Date();

    today.setHours(0, 0, 0, 0);

    return today;
  }
}
