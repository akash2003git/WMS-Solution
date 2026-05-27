import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import {
  finalize,
  map,
  startWith
} from 'rxjs';

import {
  MAT_DIALOG_DATA,
  MatDialogRef
} from '@angular/material/dialog';

import { MatSnackBar } from '@angular/material/snack-bar';

import { MATERIAL_MODULES }
  from '../../../../shared/material/material';

import { ProjectService }
  from '../../services/project';

import { EmployeeService }
  from '../../../employees/services/employee';

import { ProjectAllocation }
  from '../../models/project-allocation.model';

import { Project }
  from '../../models/project.model';

@Component({
  selector: 'app-project-allocation-panel',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './project-allocation-panel.html'
})
export class ProjectAllocationPanel {

  private readonly projectService =
    inject(ProjectService);

  private readonly employeeService =
    inject(EmployeeService);

  private readonly snackBar =
    inject(MatSnackBar);

  private readonly dialogRef =
    inject(MatDialogRef<ProjectAllocationPanel>);

  readonly data = inject<{
    project: Project;
  }>(MAT_DIALOG_DATA);

  project = this.data.project;

  allocations =
    signal<ProjectAllocation[]>([]);

  loading =
    signal(false);

  employees =
    signal<any[]>([]);

  employeeControl =
    new FormControl<string>(
      '',
      {
        nonNullable: true
      }
    );

  filteredEmployees$ =
    this.employeeControl.valueChanges.pipe(

      startWith(''),

      map(value => {

        const query =
          value.toLowerCase();

        return this
          .availableEmployees()
          .filter((employee: any) =>

            employee.fullName
              .toLowerCase()
              .includes(query)

            ||

            employee.email
              .toLowerCase()
              .includes(query)

          );

      })

    );

  availableEmployees = computed(() => {

    const allocatedIds =
      this.allocations().map(
        allocation => allocation.employeeId
      );

    return this.employees().filter(
      (employee: any) =>
        !allocatedIds.includes(
          employee.employeeId
        )
    );

  });

  constructor() {

    this.loadEmployees();

    this.loadAllocations();

  }

  assignEmployee(
    employeeId: number
  ): void {

    this.projectService
      .assignEmployee(
        this.project.projectId,
        { employeeId }
      )
      .subscribe({

        next: () => {

          this.snackBar.open(
            'Employee assigned successfully',
            'Close',
            {
              duration: 3000
            }
          );

          this.employeeControl.setValue('');

          this.loadAllocations();

        },

        error: error => {

          console.error(error);

          this.snackBar.open(
            error?.error?.message
            ??
            'Failed to assign employee',
            'Close',
            {
              duration: 3000
            }
          );

        }

      });

  }

  removeEmployee(
    employeeId: number
  ): void {

    this.projectService
      .removeEmployee(
        this.project.projectId,
        employeeId
      )
      .subscribe({

        next: () => {

          this.snackBar.open(
            'Employee removed successfully',
            'Close',
            {
              duration: 3000
            }
          );

          this.loadAllocations();

        },

        error: error => {

          console.error(error);

          this.snackBar.open(
            error?.error?.message
            ??
            'Failed to remove employee',
            'Close',
            {
              duration: 3000
            }
          );

        }

      });

  }

  private loadAllocations(): void {

    this.loading.set(true);

    this.projectService
      .getProjectAllocations(
        this.project.projectId
      )
      .pipe(

        finalize(() => {

          this.loading.set(false);

        })

      )
      .subscribe({

        next: response => {

          this.allocations.set(
            response.data
          );

        },

        error: error => {

          console.error(error);

          this.snackBar.open(
            'Failed to load allocations',
            'Close',
            {
              duration: 3000
            }
          );

        }

      });

  }

  private loadEmployees(): void {

    this.employeeService
      .getEmployees({
        pageNumber: 1,
        pageSize: 100,
        status: 1
      })
      .subscribe({

        next: response => {

          this.employees.set(
            response.data.items
          );

        }

      });

  }

  close(): void {

    this.dialogRef.close(true);

  }

}
