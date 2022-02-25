import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormContactComponent } from './input-form-contact.component';

describe('InputFormContactComponent', () => {
  let component: InputFormContactComponent;
  let fixture: ComponentFixture<InputFormContactComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormContactComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormContactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
