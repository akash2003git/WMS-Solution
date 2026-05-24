import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { ClientService } from '../../services/client';
import { Client } from '../../models/client.model';

@Component({
  selector: 'app-client-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './client-form-dialog.html'
})
export class ClientFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly clientService = inject(ClientService);
  private readonly dialogRef = inject(MatDialogRef<ClientFormDialog>);
  private readonly snackBar = inject(MatSnackBar);

  data = inject<Client | null>(MAT_DIALOG_DATA, { optional: true });
  isEditMode = !!this.data;
  loading = false;

  form = this.fb.group({
    clientName: [
      this.data?.clientName ?? '',
      Validators.required
    ],
    clientAddress: [
      this.data?.clientAddress ?? ''
    ],
    clientPhoneNumber: [
      this.data?.clientPhoneNumber ?? ''
    ],
    clientLocation: [
      this.data?.clientLocation ?? ''
    ],
    status: [
      this.data?.status ?? true
    ]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const request = this.form.getRawValue();

    if (this.isEditMode) {
      this.clientService
        .updateClient(
          this.data!.clientId,
          {
            clientName: request.clientName ?? '',
            clientAddress: request.clientAddress ?? '',
            clientPhoneNumber: request.clientPhoneNumber ?? '',
            clientLocation: request.clientLocation ?? '',
            status: request.status ?? true
          }
        )
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe({
          next: response => {
            this.snackBar.open(
              response.message,
              'Close',
              { duration: 3000 }
            );
            this.dialogRef.close(true);
          },
          error: (error: any) => {
            console.error(error);
            this.snackBar.open(
              error?.error?.message ?? 'Operation failed',
              'Close',
              { duration: 3000 }
            );
          }
        });

      return;
    }

    this.clientService
      .createClient({
        clientName: request.clientName ?? '',
        clientAddress: request.clientAddress ?? '',
        clientPhoneNumber: request.clientPhoneNumber ?? '',
        clientLocation: request.clientLocation ?? ''
      })
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: response => {
          this.snackBar.open(
            response.message,
            'Close',
            { duration: 3000 }
          );
          this.dialogRef.close(true);
        },
        error: (error: any) => {
          console.error(error);
          this.snackBar.open(
            error?.error?.message ?? 'Operation failed',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }
}
