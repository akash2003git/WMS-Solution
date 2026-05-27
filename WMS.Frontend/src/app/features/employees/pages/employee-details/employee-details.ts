import { CommonModule, Location } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import {
  BehaviorSubject,
  catchError,
  finalize,
  forkJoin,
  map,
  of,
  switchMap,
  tap
} from 'rxjs';

import {
  Chart,
  ArcElement,
  DoughnutController,
  Tooltip,
  Legend,
  ChartData,
  ChartOptions
} from 'chart.js';

import { BaseChartDirective } from 'ng2-charts';

import { MatDialog } from '@angular/material/dialog';

import { MATERIAL_MODULES }
  from '../../../../shared/material/material';

import { PageHeader }
  from '../../../../shared/components/page-header/page-header';

import { StatusChip }
  from '../../../../shared/components/status-chip/status-chip';

import { ConfirmDialog }
  from '../../../../shared/components/confirm-dialog/confirm-dialog';

import { EmployeeService }
  from '../../services/employee';

import { AttendanceService }
  from '../../../attendance/services/attendance';

import { ProjectService }
  from '../../../projects/services/project';

import { EmployeeFormDialog }
  from '../../components/employee-form-dialog/employee-form-dialog';

import { ToastrService }
  from 'ngx-toastr';

Chart.register(
  ArcElement,
  DoughnutController,
  Tooltip,
  Legend
);

@Component({
  selector: 'app-employee-details',
  imports: [
    CommonModule,
    BaseChartDirective,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './employee-details.html'
})
export class EmployeeDetails {

  private readonly employeeService =
    inject(EmployeeService);

  private readonly attendanceService =
    inject(AttendanceService);

  private readonly projectService =
    inject(ProjectService);

  private readonly route =
    inject(ActivatedRoute);

  private readonly dialog =
    inject(MatDialog);

  private readonly toastr =
    inject(ToastrService);

  private readonly router =
    inject(Router);

  private readonly location =
    inject(Location);

  loading = false;

  errorMessage = '';

  private readonly refresh$ =
    new BehaviorSubject<void>(undefined);

  doughnutData: ChartData<'doughnut'> = {
    labels: [
      'Present Days',
      'Absent Days'
    ],

    datasets: [
      {
        data: [],

        backgroundColor: [
          '#4ade80',
          '#f87171'
        ],

        borderWidth: 0,

        hoverOffset: 4
      }
    ]
  };

  doughnutOptions: ChartOptions<'doughnut'> = {
    responsive: true,

    maintainAspectRatio: false,

    cutout: '72%',

    plugins: {
      legend: {
        display: false
      },

      tooltip: {
        callbacks: {
          label: (ctx) =>
            ` ${ctx.label}: ${ctx.parsed}`
        }
      }
    }
  };

  vm$ =
    this.refresh$.pipe(

      switchMap(() => {

        const employeeId =
          Number(
            this.route.snapshot.paramMap.get('id')
          );

        queueMicrotask(() => {
          this.loading = true;
          this.errorMessage = '';
        });

        return forkJoin({

          employee:
            this.employeeService
              .getEmployeeById(employeeId)
              .pipe(map(response => response.data)),

          attendance:
            this.attendanceService
              .getEmployeeMonthlyReport(employeeId)
              .pipe(map(response => response.data)),

          projects:
            this.projectService
              .getEmployeeProjects(employeeId)
              .pipe(map(response => response.data))

        }).pipe(

          tap(data => {

            this.doughnutData = {
              ...this.doughnutData,

              datasets: [
                {
                  ...this.doughnutData.datasets[0],

                  data: [
                    data.attendance.totalPresentDays,
                    data.attendance.totalAbsentDays
                  ]
                }
              ]
            };

          }),

          catchError(error => {

            console.error(error);

            queueMicrotask(() => {
              this.errorMessage =
                'Failed to load employee details';
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

  goBack(): void {
    this.location.back();
  }

  openEditDialog(employee: any): void {

    const dialogRef =
      this.dialog.open(
        EmployeeFormDialog,
        {
          width: '800px',
          maxWidth: '95vw',
          data: employee
        }
      );

    dialogRef
      .afterClosed()
      .subscribe(updated => {

        if (updated) {
          this.refresh$.next();
        }

      });

  }

  deleteEmployee(
    employeeId: number
  ): void {

    const dialogRef =
      this.dialog.open(
        ConfirmDialog,
        {
          width: '400px',

          data: {
            title: 'Delete Employee',
            message:
              'Are you sure you want to delete this employee?'
          }
        }
      );

    dialogRef
      .afterClosed()
      .subscribe(confirmed => {

        if (!confirmed) {
          return;
        }

        this.employeeService
          .deleteEmployee(employeeId)
          .subscribe({

            next: (response) => {

              this.toastr.success(
                response.message
              );

              this.router.navigate([
                '/employees'
              ]);

            },

            error: (error) => {

              const message =
                error?.error?.Message
                ||
                'Failed to delete employee';

              this.toastr.error(message);

            }

          });

      });

  }

}
