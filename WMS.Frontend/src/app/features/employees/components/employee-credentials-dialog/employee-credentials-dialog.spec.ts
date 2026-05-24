import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeCredentialsDialog } from './employee-credentials-dialog';

describe('EmployeeCredentialsDialog', () => {
  let component: EmployeeCredentialsDialog;
  let fixture: ComponentFixture<EmployeeCredentialsDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeCredentialsDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(EmployeeCredentialsDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
