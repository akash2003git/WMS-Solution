import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { ProjectService } from '../../services/project';
import { Project } from '../../models/project.model';
import { ProjectAllocation } from '../../models/project-allocation.model';
import { ProjectFormDialog } from '../../components/project-form-dialog/project-form-dialog';
import { ProjectAllocationPanel } from '../../components/project-allocation-panel/project-allocation-panel';

@Component({
  selector: 'app-project-list',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ProjectAllocationPanel,
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

  expandedProjectId = signal<number | null>(null);
  allocations = signal<ProjectAllocation[]>([]);
  allocationsLoading = signal(false);
  projects = signal<Project[]>([]);

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  projects$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      return this.projectService.getProjects().pipe(
        map(response => {
          this.projects.set(response.data);
          return response.data;
        }),
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

  toggleExpansion(project: Project): void {
    if (this.expandedProjectId() === project.projectId) {
      this.expandedProjectId.set(null);
      return;
    }

    this.expandedProjectId.set(project.projectId);
    this.loadAllocations(project.projectId);
  }

  reloadExpandedProject(): void {
    const projectId = this.expandedProjectId();
    if (!projectId) {
      return;
    }

    this.loadAllocations(projectId);
  }

  private loadAllocations(projectId: number): void {
    this.allocationsLoading.set(true);

    this.projectService
      .getProjectAllocations(projectId)
      .pipe(
        finalize(() => {
          this.allocationsLoading.set(false);
        })
      )
      .subscribe({
        next: response => {
          this.allocations.set(response.data);

          this.projects.update(projects =>
            projects.map(project => {
              if (project.projectId !== projectId) {
                return project;
              }

              return {
                ...project,
                totalEmployees: response.data.length
              };
            })
          );
        },
        error: error => {
          console.error(error);
          this.snackBar.open(
            'Failed to load allocations',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }
}
