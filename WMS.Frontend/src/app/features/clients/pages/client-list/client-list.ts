import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { BehaviorSubject, catchError, finalize, map, of, switchMap } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MATERIAL_MODULES } from '../../../../shared/material/material';
import { PageHeader } from '../../../../shared/components/page-header/page-header';
import { StatusChip } from '../../../../shared/components/status-chip/status-chip';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { ClientService } from '../../services/client';
import { Client } from '../../models/client.model';
import { ClientProject } from '../../models/client-project.model';
import { ClientFormDialog } from '../../components/client-form-dialog/client-form-dialog';

@Component({
  selector: 'app-client-list',
  imports: [
    CommonModule,
    PageHeader,
    StatusChip,
    ...MATERIAL_MODULES
  ],
  templateUrl: './client-list.html'
})
export class ClientList {
  private readonly clientService = inject(ClientService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  displayedColumns = [
    'clientName',
    'location',
    'phone',
    'projects',
    'status',
    'actions'
  ];

  loading = false;
  errorMessage = '';

  expandedClientId = signal<number | null>(null);
  clientProjects = signal<ClientProject[]>([]);
  projectsLoading = signal(false);

  private readonly refresh$ = new BehaviorSubject<void>(undefined);

  clients$ = this.refresh$.pipe(
    switchMap(() => {
      queueMicrotask(() => {
        this.loading = true;
        this.errorMessage = '';
      });

      return this.clientService.getClients().pipe(
        map(response => response.data),
        catchError(error => {
          console.error(error);

          queueMicrotask(() => {
            this.errorMessage = 'Failed to load clients';
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
    const dialogRef = this.dialog.open(ClientFormDialog, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.refresh$.next();
      }
    });
  }

  openEditDialog(client: Client): void {
    const dialogRef = this.dialog.open(ClientFormDialog, {
      width: '600px',
      data: client
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.refresh$.next();
      }
    });
  }

  deleteClient(client: Client): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: {
        title: 'Delete Client',
        message: 'Are you sure you want to delete this client?'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) {
        return;
      }

      this.clientService.deleteClient(client.clientId).subscribe({
        next: response => {
          this.snackBar.open(
            response.message,
            'Close',
            { duration: 3000 }
          );
          this.refresh$.next();
        },
        error: error => {
          console.error(error);
          this.snackBar.open(
            error?.error?.message ?? 'Failed to delete client',
            'Close',
            { duration: 3000 }
          );
        }
      });
    });
  }

  toggleExpansion(client: Client): void {
    if (this.expandedClientId() === client.clientId) {
      this.expandedClientId.set(null);
      return;
    }

    this.expandedClientId.set(client.clientId);
    this.loadProjects(client.clientId);
  }

  private loadProjects(clientId: number): void {
    this.projectsLoading.set(true);

    this.clientService
      .getClientProjects(clientId)
      .pipe(
        finalize(() => {
          this.projectsLoading.set(false);
        })
      )
      .subscribe({
        next: response => {
          this.clientProjects.set(response.data);
        },
        error: error => {
          console.error(error);
          this.snackBar.open(
            'Failed to load projects',
            'Close',
            { duration: 3000 }
          );
        }
      });
  }
}
