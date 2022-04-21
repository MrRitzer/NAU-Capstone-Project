import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormEditComponent } from './input-form-edit.component';

describe('InputFormEditComponent', () => {
  let component: InputFormEditComponent;
  let fixture: ComponentFixture<InputFormEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
