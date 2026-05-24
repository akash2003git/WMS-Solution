import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, ReactiveFormsModule, ...MATERIAL_MODULES],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css'
})
export class ResetPassword implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly toastr = inject(ToastrService);
  private readonly router = inject(Router);

  isLoading = false;
  hideCurrentPassword = true;
  hideNewPassword = true;
  hideConfirmPassword = true;

  resetPasswordForm = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  });

  ngOnInit(): void {
    if (!sessionStorage.getItem('reset_username')) {
      this.router.navigate(['/login']);
    }
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    const form = this.resetPasswordForm.getRawValue();

    if (form.newPassword !== form.confirmPassword) {
      this.toastr.error('Passwords do not match');
      return;
    }

    const username = sessionStorage.getItem('reset_username');
    if (!username) {
      this.router.navigate(['/login']);
      return;
    }

    this.isLoading = true;

    this.authService
      .resetPassword({
        username,
        currentPassword: form.currentPassword ?? '',
        newPassword: form.newPassword ?? ''
      })
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: (response) => {
          this.toastr.success(response.message);
          sessionStorage.removeItem('reset_username');
          this.authService.logout();
          this.router.navigate(['/login']);
        },
        error: (error) => {
          const message = error?.error?.Message
            || error?.error?.Errors?.[0]
            || 'Password reset failed';
          this.toastr.error(message);
        }
      });
  }
}
