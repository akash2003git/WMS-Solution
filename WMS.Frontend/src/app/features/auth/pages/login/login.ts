import { CommonModule } from '@angular/common';
import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { AuthService } from '../../services/auth';
@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ...MATERIAL_MODULES
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastr = inject(ToastrService);
  private readonly cdr = inject(ChangeDetectorRef);

  isLoading = false;
  hidePassword = true;
  loginForm = this.fb.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;

    this.authService
      .login({
        username: this.loginForm.controls.username.value ?? '',
        password: this.loginForm.controls.password.value ?? ''
      })
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (response) => {
          const data = response.data;
          this.toastr.success(response.message);

          if (data.requiresPasswordChange) {
            sessionStorage.setItem('reset_username', data.username);
            this.router.navigate(['/reset-password']);
            return;
          }

          this.authService.setSession(data);

          switch (data.role) {
            case 'Admin':
              this.router.navigate(['/admin/dashboard']);
              break;

            case 'Manager':
              this.router.navigate(['/manager/dashboard']);
              break;

            case 'Employee':
              this.router.navigate(['/employee/dashboard']);
              break;

            default:
              this.router.navigate(['/login']);
          }
        },

        error: (error) => {
          const message = error?.error?.Message
            || error?.error?.Errors?.[0]
            || 'Login failed';
          this.toastr.error(message);
        }
      });
  }
}
