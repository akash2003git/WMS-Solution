import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MATERIAL_MODULES } from '../../../../shared/material/material';

@Component({
  selector: 'app-employee-credentials-dialog',
  imports: [
    ...MATERIAL_MODULES
  ],
  templateUrl: './employee-credentials-dialog.html'
})
export class EmployeeCredentialsDialog {
  private readonly dialogRef = inject(MatDialogRef<EmployeeCredentialsDialog>);
  data = inject(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }

  copyCredentials(): void {
    const text =
      `Username: ${this.data.username}
Temporary Password: ${this.data.temporaryPassword}`;
    navigator.clipboard.writeText(text);
  }
}
