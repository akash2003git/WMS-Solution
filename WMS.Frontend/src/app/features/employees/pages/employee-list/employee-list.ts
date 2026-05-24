import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import {
  BehaviorSubject,
  catchError,
  finalize,
  map,
  of,
  switchMap
} from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { Pagination } from '../../../../shared/components/pagination/pagination';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { DepartmentService } from '../../../departments/services/department';
import { EmployeeService } from '../../services/employee';
import { RoleService } from '../../services/role';
import { Employee } from '../../models/employee.model';
import { EmployeeFilter } from '../../models/employee-filter.model';
import { EmployeeFormDialog } from '../../components/employee-form-dialog/employee-form-dialog';
import { EmployeeCredentialsDialog } from '../../components/employee-credentials-dialog/employee-credentials-dialog';
import { EMPLOYEE_STATUS_OPTIONS } from '../../constants/employee.constants';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';

@Component({
  selector: 'app-employee-list',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PageHeader,
    Pagination,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './employee-list.html'
})
export class EmployeeList {
  private readonly employeeService = inject(EmployeeService);
  private readonly departmentService = inject(DepartmentService);
  private readonly roleService = inject(RoleService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly fb = inject(FormBuilder);

  readonly statusOptions = EMPLOYEE_STATUS_OPTIONS;

  displayedColumns = [
    'name',
    'email',
    'department',
    'role',
    'status',
    'actions'
  ];

  loading = false;

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  filterForm = this.fb.group({
    search: [''],
    departmentId: [null as number | null],
    roleId: [null as number | null],
    status: [1]
  });

  private filterState: EmployeeFilter = {
    pageNumber: 1,
    pageSize: 10,
    status: 1,
    sortBy: 'firstname',
    sortDirection: 'asc'
  };

  departments$ = this.departmentService
    .getDepartments()
    .pipe(
      map(response => response.data),
      catchError(() => of([]))
    );

  roles$ = this.roleService
    .getRoles()
    .pipe(
      map(response => response.data),
      catchError(() => of([]))
    );

  employees$ = this.refresh$
    .pipe(
      switchMap(() => {
        queueMicrotask(() => {
          this.loading = true;
        });
        return this.employeeService
          .getEmployees(this.filterState)
          .pipe(
            map(response => response.data),
            catchError((error) => {
              console.error(error);
              this.toastr.error('Failed to load employees');
              return of({
                items: [],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 0
              });
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
    const form = this.filterForm.getRawValue();
    this.filterState = {
      ...this.filterState,
      pageNumber: 1,
      search: form.search ?? undefined,
      departmentId: form.departmentId ?? undefined,
      roleId: form.roleId ?? undefined,
      status: form.status ?? undefined
    };
    this.refresh$.next();
  }

  onPageChange(event: PageEvent): void {
    this.filterState = {
      ...this.filterState,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize
    };
    this.refresh$.next();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(
      EmployeeFormDialog,
      { width: '700px' }
    );

    dialogRef
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }
        this.refresh$.next();
        this.dialog.open(EmployeeCredentialsDialog,
          {
            width: '500px',
            data: result
          }
        );
      });
  }

  openEditDialog(employee: Employee): void {
    const dialogRef =
      this.dialog.open(EmployeeFormDialog,
        {
          width: '700px',
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

  deleteEmployee(employee: Employee): void {
    const dialogRef = this.dialog.open(ConfirmDialog,
      {
        width: '400px',
        data: {
          title: 'Deactivate Employee',
          message: `Deactivate ${employee.fullName}?`
        }
      }
    );

    dialogRef.afterClosed().subscribe(confirmed => {
      if (!confirmed) {
        return;
      }

      this.employeeService.deleteEmployee(employee.employeeId)
        .subscribe({
          next: (response) => {
            this.toastr.success(response.message);
            this.refresh$.next();
          },
          error: () => {
            this.toastr.error('Operation failed');
          }
        });
    });
  }
}
