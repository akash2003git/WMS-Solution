import { BreakpointObserver } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { Component, ViewChild, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';
import { MatSidenav } from '@angular/material/sidenav';
import { MATERIAL_MODULES } from '../../../shared/material/material';
import { NAVIGATION_ITEMS } from '../../constants/navigation';
import { AuthService } from '../../../features/auth/services/auth';

@Component({
  selector: 'app-main-layout',
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    ...MATERIAL_MODULES
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css'
})
export class MainLayout {
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;

  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly breakpointObserver = inject(BreakpointObserver);

  currentUser = this.authService.getCurrentUser();

  navigationItems = NAVIGATION_ITEMS.filter(item =>
    item.roles.includes(
      this.currentUser?.role ?? ''
    )
  );

  isMobile$ = this.breakpointObserver
    .observe('(max-width: 768px)')
    .pipe(
      map(result => result.matches)
    );

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  closeSidenavOnMobile(): void {
    this.isMobile$.subscribe(isMobile => {
      if (
        isMobile &&
        this.sidenav
      ) {
        this.sidenav.close();
      }
    });
  }
}
