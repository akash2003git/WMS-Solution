import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';

import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { AttendanceService } from '../../services/attendance';

@Component({
  selector: 'app-absentee-list',
  imports: [
    CommonModule,
    PageHeader,
    ...MATERIAL_MODULES
  ],
  templateUrl: './absentee-list.html'
})
export class AbsenteeList {
  private readonly attendanceService =
    inject(AttendanceService);

  displayedColumns = [
    'employeeName',
    'department',
    'role'
  ];

  absentees$ =
    this.attendanceService
      .getAbsentees()
      .pipe(
        map(response => response.data),

        catchError(error => {
          console.error(error);
          return of([]);
        })
      );
}
