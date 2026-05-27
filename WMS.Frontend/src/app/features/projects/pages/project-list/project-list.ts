import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { ProjectService } from '../../services/project';
import { Project } from '../../models/project.model';
import { ProjectFormDialog } from '../../components/project-form-dialog/project-form-dialog';
import { ProjectAllocationPanel } from '../../components/project-allocation-panel/project-allocation-panel';

@Component({
  selector: 'app-project-list',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './project-list.html'
})
export class ProjectList {
  private readonly projectService = inject(ProjectService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns = [
    'projectName',
    'client',
    'status',
    'employees',
    'startDate',
    'endDate',
    'actions'
  ];

  loading = false;
  errorMessage = '';

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  projects$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      return this.projectService.getProjects().pipe(
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

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(ProjectFormDialog, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open(
          'Project created successfully',
          'Close',
          { duration: 3000 }
        );
        this.refresh$.next();
      }
    });
  }

  openEditDialog(project: Project): void {
    const dialogRef = this.dialog.open(ProjectFormDialog, {
      width: '600px',
      data: { project }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open(
          'Project updated successfully',
          'Close',
          { duration: 3000 }
        );
        this.refresh$.next();
      }
    });
  }

  openAllocationDialog(project: Project): void {

    const dialogRef = this.dialog.open(
      ProjectAllocationPanel,
      {
        width: '1100px',
        maxWidth: '95vw',
        data: {
          project
        }
      }
    );

    dialogRef.afterClosed().subscribe(updated => {

      if (updated) {

        this.refresh$.next();

      }

    });

  }
}
