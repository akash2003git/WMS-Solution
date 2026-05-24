import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { DepartmentService } from '../../services/department';
import { Department } from '../../models/department.model';

@Component({
  selector: 'app-department-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './department-form-dialog.html'
})
export class DepartmentFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly toastr = inject(ToastrService);
  private readonly departmentService = inject(DepartmentService);
  private readonly dialogRef = inject(MatDialogRef<DepartmentFormDialog>);

  data = inject<Department | null>(MAT_DIALOG_DATA, { optional: true });
  isEditMode = !!this.data;
  isLoading = false;
  form = this.fb.group({
    departmentName: [this.data?.departmentName ?? '', [Validators.required]],
    description: [this.data?.description ?? '']
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    const request = this.form.getRawValue();

    const operation = this.isEditMode
      ? this.departmentService.updateDepartment(
        this.data!.departmentId,
        {
          departmentName: request.departmentName ?? '',
          description: request.description ?? ''
        }
      )
      : this.departmentService.createDepartment({
        departmentName: request.departmentName ?? '',
        description: request.description ?? ''
      });

    operation
      .pipe(finalize(() => {
        this.isLoading = false;
      })
      )
      .subscribe({
        next: (response) => {
          this.toastr.success(
            response.message
          );
          this.dialogRef.close(true);
        },

        error: (error) => {
          const message =
            error?.error?.Message
            || 'Operation failed';
          this.toastr.error(message);
        }
      });
  }
}
