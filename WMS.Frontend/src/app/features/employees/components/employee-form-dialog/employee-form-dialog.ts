import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize, map } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { DepartmentService } from '../../../departments/services/department';
import { RoleService } from '../../services/role';
import { EmployeeService } from '../../services/employee';
import { Employee } from '../../models/employee.model';
import { GENDER_OPTIONS } from '../../constants/employee.constants';

@Component({
  selector: 'app-employee-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './employee-form-dialog.html'
})
export class EmployeeFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly employeeService = inject(EmployeeService);
  private readonly departmentService = inject(DepartmentService);
  private readonly roleService = inject(RoleService);
  private readonly toastr = inject(ToastrService);
  private readonly dialogRef = inject(MatDialogRef<EmployeeFormDialog>);

  readonly data = inject<Employee | null>(MAT_DIALOG_DATA);
  readonly genders = GENDER_OPTIONS;

  isLoading = false;
  isEditMode = !!this.data;

  departments$ = this.departmentService
    .getDepartments()
    .pipe(map(response => response.data));

  roles$ = this.roleService
    .getRoles()
    .pipe(map(response => response.data));

  form = this.fb.group({
    firstName: [this.data?.firstName ?? '', Validators.required],
    lastName: [this.data?.lastName ?? '', Validators.required],
    email: [this.data?.email ?? '', [Validators.required, Validators.email]],
    phoneNumber: [this.data?.phoneNumber ?? '', Validators.required],
    gender: [this.data?.gender ?? '', Validators.required],
    dob: [this.data?.dob ?? '', Validators.required],
    doj: [this.data?.doj ?? '', Validators.required],
    departmentId: [this.data?.departmentId ?? null, Validators.required],
    roleId: [this.data?.roleId ?? null, Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const request = this.form.getRawValue();

    if (this.isEditMode) {
      this.employeeService
        .updateEmployee(this.data!.employeeId, request as never)
        .pipe(finalize(() => { this.isLoading = false; }))
        .subscribe({
          next: (response) => {
            this.toastr.success(response.message);
            this.dialogRef.close(true);
          },
          error: (error: any) => {
            const message = error?.error?.Message
              || 'Operation failed';
            this.toastr.error(message);
          }
        });
      return;
    }

    this.employeeService
      .createEmployee(request as never)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: (response) => {
          this.toastr.success(response.message);
          this.dialogRef.close(response.data);
        },

        error: (error: any) => {
          const message = error?.error?.Message
            || 'Operation failed';
          this.toastr.error(message);
        }
      });
  }
}
