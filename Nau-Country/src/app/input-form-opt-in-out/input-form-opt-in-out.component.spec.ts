import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormOptInOutComponent } from './input-form-opt-in-out.component';

describe('InputFormOptInOutComponent', () => {
  let component: InputFormOptInOutComponent;
  let fixture: ComponentFixture<InputFormOptInOutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormOptInOutComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormOptInOutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
