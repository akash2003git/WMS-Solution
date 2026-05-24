import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable, finalize } from 'rxjs';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { ProjectService } from '../../services/project';
import { ClientService } from '../../../clients/services/client';
import { PROJECT_STATUSES } from '../../constants/project.constants';

@Component({
  selector: 'app-project-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './project-form-dialog.html'
})
export class ProjectFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly projectService = inject(ProjectService);
  private readonly clientService = inject(ClientService);
  private readonly dialogRef = inject(MatDialogRef<ProjectFormDialog, boolean>);

  data = inject(MAT_DIALOG_DATA);
  statuses = PROJECT_STATUSES;
  clients$ = this.clientService.getClients();
  submitting = false;
  errorMessage = '';

  form = this.fb.group({
    projectName: [
      this.data?.project?.projectName ?? '',
      Validators.required
    ],
    clientId: [
      this.data?.project?.clientId ?? null
    ],
    startDate: [
      this.data?.project?.startDate ?? null
    ],
    endDate: [
      this.data?.project?.endDate ?? null
    ],
    status: [
      this.data?.project?.status ?? 'Active',
      Validators.required
    ]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const form = this.form.getRawValue();

    const payload = {
      ...form,
      startDate: form.startDate ? this.formatDate(form.startDate) : null,
      endDate: form.endDate ? this.formatDate(form.endDate) : null
    };

    const request$: Observable<any> = this.data?.project
      ? this.projectService.updateProject(this.data.project.projectId, payload)
      : this.projectService.createProject(payload);

    request$
      .pipe(
        finalize(() => {
          this.submitting = false;
        })
      )
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (error: any) => {
          console.error(error);
          this.errorMessage = error?.error?.message ?? 'Failed to save project';
        }
      });
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }
}
