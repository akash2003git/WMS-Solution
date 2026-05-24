import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { BaseChartDirective } from 'ng2-charts';

import {
  Chart,
  ArcElement,
  DoughnutController,
  BarElement,
  BarController,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend,
  ChartData,
  ChartOptions
} from 'chart.js';

import { MATERIAL_MODULES } from '../../../../shared/material/material';

import { PageHeader } from '../../../../shared/components/page-header/page-header';

import { DashboardService } from '../../services/dashboard';

import { IManagerDashboard } from '../../models/manager-dashboard.model';

Chart.register(
  ArcElement,
  DoughnutController,
  BarElement,
  BarController,
  CategoryScale,
  LinearScale,
  Tooltip,
  Legend
);

@Component({
  selector: 'app-manager-dashboard',
  imports: [
    CommonModule,
    BaseChartDirective,
    PageHeader,
    ...MATERIAL_MODULES
  ],
  templateUrl: './manager-dashboard.html'
})
export class ManagerDashboard {

  private readonly dashboardService =
    inject(DashboardService);

  errorMessage = '';

  doughnutData: ChartData<'doughnut'> = {
    labels: [
      'Present Today',
      'Absent Today'
    ],

    datasets: [
      {
        data: [],

        backgroundColor: [
          '#4ade80',
          '#ef4444'
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

  barData: ChartData<'bar'> = {
    labels: [
      'Attendance',
      'Pending Leaves',
      'Project Allocations',
      'Projects'
    ],

    datasets: [
      {
        label: 'Team Metrics',

        data: [],

        backgroundColor: [
          '#2563eb',
          '#f59e0b',
          '#10b981',
          '#8b5cf6'
        ],

        borderRadius: 8,

        barPercentage: 0.6
      }
    ]
  };

  barOptions: ChartOptions<'bar'> = {
    responsive: true,

    maintainAspectRatio: false,

    scales: {
      x: {
        grid: {
          display: false
        },

        border: {
          display: false
        },

        ticks: {
          color: '#6b7280'
        }
      },

      y: {
        beginAtZero: true,

        grid: {
          color: '#f3f4f6'
        },

        border: {
          display: false
        },

        ticks: {
          stepSize: 1,
          color: '#6b7280'
        }
      }
    },

    plugins: {
      legend: {
        display: false
      }
    }
  };

  dashboard$: Observable<IManagerDashboard | null> =
    this.dashboardService
      .getManagerDashboard()
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
                  data.teamAttendanceToday,
                  data.teamAbsentToday
                ]
              }
            ]
          };

          this.barData = {
            ...this.barData,

            datasets: [
              {
                ...this.barData.datasets[0],

                data: [
                  data.teamAttendanceToday,
                  data.pendingLeaveRequests,
                  data.activeProjectAllocations,
                  data.activeProjects
                ]
              }
            ]
          };
        }),

        catchError(error => {

          console.error(error);

          this.errorMessage =
            'Failed to load dashboard';

          return of(null);
        })
      );
}
