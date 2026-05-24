import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentFormDialog } from './department-form-dialog';

describe('DepartmentFormDialog', () => {
  let component: DepartmentFormDialog;
  let fixture: ComponentFixture<DepartmentFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepartmentFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(DepartmentFormDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
