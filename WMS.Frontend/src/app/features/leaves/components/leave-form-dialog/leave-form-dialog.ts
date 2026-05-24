import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize } from 'rxjs';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { LeaveService } from '../../services/leave';
import { LEAVE_TYPES } from '../../constants/leave.constants';

@Component({
  selector: 'app-leave-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './leave-form-dialog.html'
})
export class LeaveFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly leaveService = inject(LeaveService);
  private readonly dialogRef = inject(MatDialogRef<LeaveFormDialog>);

  data = inject(MAT_DIALOG_DATA);
  leaveTypes = LEAVE_TYPES;
  submitting = false;
  errorMessage = '';

  form = this.fb.group({
    leaveType: ['', Validators.required],
    reason: [''],
    fromDate: [null as Date | null, Validators.required],
    toDate: [null as Date | null, Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.getRawValue();

    const request = {
      leaveType: formValue.leaveType ?? '',
      reason: formValue.reason ?? '',
      fromDate: this.formatDate(formValue.fromDate as Date),
      toDate: this.formatDate(formValue.toDate as Date)
    };

    this.submitting = true;
    this.errorMessage = '';

    this.leaveService
      .applyLeave(request)
      .pipe(finalize(() => { this.submitting = false; }))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: error => {
          console.error(error);
          this.errorMessage = error?.error?.Message
            ?? error?.error?.Errors?.[0]
            ?? 'Failed to apply leave';
        }
      });
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
