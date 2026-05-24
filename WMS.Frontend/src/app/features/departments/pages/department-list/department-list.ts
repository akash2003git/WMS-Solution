import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, catchError, finalize, map, Observable, of, switchMap } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { DepartmentService } from '../../services/department';
import { Department } from '../../models/department.model';
import { DepartmentFormDialog } from '../../components/department-form-dialog/department-form-dialog';
import { AuthService } from '../../../auth/services/auth';

@Component({
  selector: 'app-department-list',
  imports: [
    CommonModule,
    PageHeader,
    ...MATERIAL_MODULES
  ],
  templateUrl: './department-list.html'
})
export class DepartmentList {
  private readonly departmentService = inject(DepartmentService);
  private readonly dialog = inject(MatDialog);
  private readonly toastr = inject(ToastrService);
  private readonly authService = inject(AuthService);

  displayedColumns = [
    'departmentName',
    'description',
    'employeeCount',
    'actions'
  ];
  isLoading = false;

  currentUser = this.authService.getCurrentUser();

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  departments$: Observable<Department[]> = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.isLoading = true;
      });
      return this.departmentService
        .getDepartments()
        .pipe(
          map(response => response.data ?? []),
          catchError(error => {
            const message = error?.error?.Message || 'Failed to load departments';
            this.toastr.error(message);
            return of([]);
          }),
          finalize(() => {
            queueMicrotask(() => {
              this.isLoading = false;
            });
          })
        );
    })
  );

  refreshDepartments(): void {
    this.refresh$.next();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(
      DepartmentFormDialog,
      { width: '500px' }
    );
    dialogRef
      .afterClosed()
      .subscribe(result => {
        if (result) {
          this.refreshDepartments();
        }
      });
  }

  openEditDialog(department: Department): void {
    const dialogRef = this.dialog.open(
      DepartmentFormDialog,
      {
        width: '500px',
        data: department
      }
    );

    dialogRef
      .afterClosed()
      .subscribe(result => {
        if (result) {
          this.refreshDepartments();
        }
      });
  }

  deleteDepartment(department: Department): void {
    const dialogRef = this.dialog.open(
      ConfirmDialog,
      {
        width: '400px',
        data: {
          title: 'Delete Department',
          message: `Delete "${department.departmentName}" department?`
        }
      }
    );

    dialogRef
      .afterClosed()
      .subscribe(result => {
        if (!result) {
          return;
        }

        this.departmentService.deleteDepartment(department.departmentId)
          .subscribe({
            next: (response) => {
              this.toastr.success(response.message);
              this.refreshDepartments();
            },

            error: (error) => {
              const message = error?.error?.Message
                || 'Delete failed';
              this.toastr.error(message);
            }
          });
      });
  }
}
