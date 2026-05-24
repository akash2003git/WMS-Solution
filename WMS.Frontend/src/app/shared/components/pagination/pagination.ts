import { Component, input, output } from '@angular/core';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-pagination',
  imports: [
    MatPaginatorModule
  ],
  templateUrl: './pagination.html'
})
export class Pagination {
  pageIndex = input.required<number>();
  pageSize = input.required<number>();
  totalCount = input.required<number>();
  pageChange = output<PageEvent>();

  onPageChange(event: PageEvent): void {
    this.pageChange.emit(event);
  }
}
