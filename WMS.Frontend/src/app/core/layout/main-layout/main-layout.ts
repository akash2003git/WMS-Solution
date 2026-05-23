import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MATERIAL_MODULES } from '../../../shared/material/material';
import { AuthService } from '../../../features/auth/services/auth';

@Component({
  selector: 'app-main-layout',
  imports: [
    CommonModule,
    RouterOutlet,
    ...MATERIAL_MODULES
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  currentUser = this.authService.getCurrentUser();
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
