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

import { IAdminDashboard } from '../../models/admin-dashboard.model';

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
  selector: 'app-admin-dashboard',
  imports: [
    CommonModule,
    BaseChartDirective,
    PageHeader,
    ...MATERIAL_MODULES
  ],
  templateUrl: './admin-dashboard.html'
})
export class AdminDashboard {

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
          '#f59e0b'
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
      'Employees',
      'Departments',
      'Projects',
      'Inactive'
    ],

    datasets: [
      {
        label: 'Organization Overview',

        data: [],

        backgroundColor: [
          '#3b82f6',
          '#8b5cf6',
          '#10b981',
          '#ef4444'
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

  dashboard$: Observable<IAdminDashboard | null> =
    this.dashboardService
      .getAdminDashboard()
      .pipe(

        map(response => response.data),

        tap(data => {

          if (!data) {
            return;
          }

          const activeEmployees =
            data.totalEmployees
            -
            data.inactiveEmployees;

          const absentToday =
            activeEmployees
            -
            data.attendanceToday;

          this.doughnutData = {
            ...this.doughnutData,

            datasets: [
              {
                ...this.doughnutData.datasets[0],

                data: [
                  data.attendanceToday,
                  absentToday
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
                  data.totalEmployees,
                  data.totalDepartments,
                  data.activeProjects,
                  data.inactiveEmployees
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
