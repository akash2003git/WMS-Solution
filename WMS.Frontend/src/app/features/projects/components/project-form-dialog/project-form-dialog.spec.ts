import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectFormDialog } from './project-form-dialog';

describe('ProjectFormDialog', () => {
  let component: ProjectFormDialog;
  let fixture: ComponentFixture<ProjectFormDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectFormDialog],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectFormDialog);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
