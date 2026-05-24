import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { finalize } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { AnnouncementService } from '../../services/announcement';
import { Announcement } from '../../models/announcement.model';

@Component({
  selector: 'app-announcement-form-dialog',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './announcement-form-dialog.html'
})
export class AnnouncementFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly announcementService = inject(AnnouncementService);
  private readonly dialogRef = inject(MatDialogRef<AnnouncementFormDialog>);
  private readonly snackBar = inject(MatSnackBar);

  data = inject<Announcement | null>(MAT_DIALOG_DATA, { optional: true });
  isEditMode = !!this.data;
  loading = false;

  form = this.fb.group({
    title: [
      this.data?.title ?? '',
      Validators.required
    ],
    message: [
      this.data?.message ?? '',
      Validators.required
    ]
  });

  submit(): void {

    if (this.form.invalid) {

      this.form.markAllAsTouched();

      return;
    }

    this.loading = true;

    const request =
      this.form.getRawValue();

    if (this.isEditMode) {

      this.announcementService
        .updateAnnouncement(
          this.data!.announcementId,
          {
            title:
              request.title ?? '',

            message:
              request.message ?? ''
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
              {
                duration: 3000
              }
            );

            this.dialogRef.close(true);
          },

          error: (error: any) => {

            console.error(error);

            this.snackBar.open(
              error?.error?.message
              ?? 'Operation failed',
              'Close',
              {
                duration: 3000
              }
            );
          }
        });

      return;
    }

    this.announcementService
      .createAnnouncement({
        title:
          request.title ?? '',

        message:
          request.message ?? ''
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
            {
              duration: 3000
            }
          );

          this.dialogRef.close(true);
        },

        error: (error: any) => {

          console.error(error);

          this.snackBar.open(
            error?.error?.message
            ?? 'Operation failed',
            'Close',
            {
              duration: 3000
            }
          );
        }
      });
  }
}
