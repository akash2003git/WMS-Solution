import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { ProjectService } from '../../services/project';

@Component({
  selector: 'app-my-projects',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './my-projects.html'
})
export class MyProjects {
  private readonly projectService = inject(ProjectService);

  loading = false;
  errorMessage = '';

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  projects$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      return this.projectService.getMyProjects().pipe(
        map(response => response.data),
        catchError(error => {
          console.error(error);

          queueMicrotask(() => {
            this.errorMessage = 'Failed to load projects';
          });

          return of([]);
        }),
        finalize(() => {
          queueMicrotask(() => {
            this.loading = false;
          });
        })
      );
    })
  );
}
