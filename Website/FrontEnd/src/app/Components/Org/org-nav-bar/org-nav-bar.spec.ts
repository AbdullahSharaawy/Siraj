import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OrgNavBar } from './org-nav-bar';

describe('OrgNavBar', () => {
  let component: OrgNavBar;
  let fixture: ComponentFixture<OrgNavBar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrgNavBar],
    }).compileComponents();

    fixture = TestBed.createComponent(OrgNavBar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
