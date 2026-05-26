import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceReportDialog } from './attendance-report-dialog';

describe('AttendanceReportDialog', () => {
  let component: AttendanceReportDialog;
  let fixture: ComponentFixture<AttendanceReportDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AttendanceReportDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(AttendanceReportDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
