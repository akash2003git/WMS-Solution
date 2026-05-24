import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../material/material';

@Component({
  selector: 'app-confirm-dialog',
  imports: [
    ...MATERIAL_MODULES
  ],
  templateUrl: './confirm-dialog.html'
})
export class ConfirmDialog {
  private readonly dialogRef = inject(MatDialogRef<ConfirmDialog>);
  data = inject(MAT_DIALOG_DATA);

  confirm(): void {
    this.dialogRef.close(true);
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
