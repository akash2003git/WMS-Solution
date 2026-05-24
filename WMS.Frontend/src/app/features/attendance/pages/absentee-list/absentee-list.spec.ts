import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbsenteeList } from './absentee-list';

describe('AbsenteeList', () => {
  let component: AbsenteeList;
  let fixture: ComponentFixture<AbsenteeList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AbsenteeList],
    }).compileComponents();

    fixture = TestBed.createComponent(AbsenteeList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
