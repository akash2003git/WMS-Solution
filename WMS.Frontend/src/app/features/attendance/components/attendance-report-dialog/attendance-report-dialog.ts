import { CommonModule } from '@angular/common';

import {
  Component,
  Inject,
  inject
} from '@angular/core';

import {
  MAT_DIALOG_DATA,
  MatDialogModule
} from '@angular/material/dialog';

import {
  catchError,
  map,
  Observable,
  of,
  shareReplay
} from 'rxjs';

import { MATERIAL_MODULES }
  from '../../../../shared/material/material';

import { AttendanceService }
  from '../../services/attendance';

import { MonthlyAttendanceReport }
  from '../../models/monthly-attendance-report.model';

@Component({
  selector: 'app-attendance-report-dialog',

  imports: [
    CommonModule,
    MatDialogModule,
    ...MATERIAL_MODULES
  ],

  templateUrl:
    './attendance-report-dialog.html'
})
export class AttendanceReportDialog {

  private readonly attendanceService =
    inject(AttendanceService);

  errorMessage = '';

  report$:
    Observable<
      MonthlyAttendanceReport | null
    >;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: {
      employeeId: number;
      employeeName: string;
    }
  ) {

    this.report$ =
      this.attendanceService
        .getEmployeeMonthlyReport(
          this.data.employeeId
        )
        .pipe(

          map(response =>
            response.data
          ),

          catchError(error => {

            console.error(error);

            this.errorMessage =
              'Failed to load report';

            return of(null);
          }),

          shareReplay(1)
        );
  }
}
