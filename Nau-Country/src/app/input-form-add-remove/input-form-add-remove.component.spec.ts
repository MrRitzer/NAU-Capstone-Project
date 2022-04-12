import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormAddRemoveComponent } from './input-form-add-remove.component';

describe('InputFormListComponent', () => {
  let component: InputFormAddRemoveComponent;
  let fixture: ComponentFixture<InputFormAddRemoveComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormAddRemoveComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormAddRemoveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
