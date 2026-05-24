import { CommonModule } from '@angular/common';
import { Component, computed, inject, input, output, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { map, startWith } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { ProjectService } from '../../services/project';
import { EmployeeService } from '../../../employees/services/employee';
import { ProjectAllocation } from '../../models/project-allocation.model';

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
  private readonly projectService = inject(ProjectService);
  private readonly employeeService = inject(EmployeeService);
  private readonly snackBar = inject(MatSnackBar);

  projectId = input.required<number>();
  allocations = input.required<ProjectAllocation[]>();
  allocationChanged = output<void>();

  private readonly employees = signal<any[]>([]);
  employeeControl = new FormControl('');

  filteredEmployees$ = this.employeeControl.valueChanges.pipe(
    startWith(''),
    map(value => {
      const query = (value ?? '').toLowerCase();
      return this.availableEmployees().filter(
        employee =>
          employee.fullName.toLowerCase().includes(query) ||
          employee.email.toLowerCase().includes(query)
      );
    })
  );

  availableEmployees = computed(() => {
    const allocatedIds = this.allocations().map(
      allocation => allocation.employeeId
    );

    return this.employees().filter(
      employee => !allocatedIds.includes(employee.employeeId)
    );
  });

  constructor() {
    this.loadEmployees();
  }

  assignEmployee(employeeId: number): void {
    this.projectService
      .assignEmployee(this.projectId(), { employeeId })
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Employee assigned successfully',
            'Close',
            { duration: 3000 }
          );
          this.employeeControl.setValue('');
          this.allocationChanged.emit();
        },
        error: error => {
          console.error(error);
          this.snackBar.open(
            error?.error?.message ?? 'Failed to assign employee',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }

  removeEmployee(employeeId: number): void {
    this.projectService
      .removeEmployee(this.projectId(), employeeId)
      .subscribe({
        next: () => {
          this.snackBar.open(
            'Employee removed successfully',
            'Close',
            { duration: 3000 }
          );
          this.allocationChanged.emit();
        },
        error: error => {
          console.error(error);
          this.snackBar.open(
            error?.error?.message ?? 'Failed to remove employee',
            'Close',
            { duration: 3000 }
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
          this.employees.set(response.data.items);
        }
      });
  }
}
