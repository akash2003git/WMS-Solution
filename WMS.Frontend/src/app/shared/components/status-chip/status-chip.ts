import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-status-chip',
  imports: [],
  templateUrl: './status-chip.html'
})
export class StatusChip {
  status = input.required<string>();
  classes = computed(() => {
    const value = this.status().toLowerCase();

    switch (value) {
      case 'active':
      case 'approved':
      case 'completed':
        return `
            bg-green-100
            text-green-700
            border-green-200
          `;

      case 'inactive':
      case 'rejected':
      case 'cancelled':
        return `
            bg-red-100
            text-red-700
            border-red-200
          `;

      case 'pending':
      case 'in progress':
        return `
            bg-yellow-100
            text-yellow-700
            border-yellow-200
          `;

      default:
        return `
            bg-gray-100
            text-gray-700
            border-gray-200
          `;
    }
  });
}
