import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, ArcElement, DoughnutController, Tooltip, Legend, ChartData, ChartOptions } from 'chart.js';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { DashboardService } from '../../services/dashboard';
import { IEmployeeDashboard } from '../../models/employee-dashboard.model';

Chart.register(
  ArcElement,
  DoughnutController,
  Tooltip,
  Legend
);

@Component({
  selector: 'app-employee-dashboard',
  imports: [
    CommonModule,
    BaseChartDirective,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './employee-dashboard.html'
})
export class EmployeeDashboard {
  private readonly dashboardService = inject(DashboardService);

  errorMessage = '';

  doughnutData: ChartData<'doughnut'> = {
    labels: [
      'Present Days',
      'Leaves'
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
          label: (ctx) => ` ${ctx.label}: ${ctx.parsed}`
        }
      }
    }
  };

  dashboard$: Observable<IEmployeeDashboard | null> = this.dashboardService
    .getEmployeeDashboard()
    .pipe(
      map(response => response.data),
      tap(data => {
        if (!data) {
          return;
        }

        this.doughnutData = {
          ...this.doughnutData,
          datasets: [
            {
              ...this.doughnutData.datasets[0],
              data: [
                data.presentDaysThisMonth,
                data.leaveCountThisMonth
              ]
            }
          ]
        };
      }),
      catchError(error => {
        console.error(error);
        this.errorMessage = 'Failed to load dashboard';
        return of(null);
      })
    );
}
