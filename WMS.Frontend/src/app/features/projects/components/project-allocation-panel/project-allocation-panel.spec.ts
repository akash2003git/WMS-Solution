import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectAllocationPanel } from './project-allocation-panel';

describe('ProjectAllocationPanel', () => {
  let component: ProjectAllocationPanel;
  let fixture: ComponentFixture<ProjectAllocationPanel>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectAllocationPanel],
    }).compileComponents();

    fixture = TestBed.createComponent(ProjectAllocationPanel);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
