import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormListComponent } from './input-form-list.component';

describe('InputFormListComponent', () => {
  let component: InputFormListComponent;
  let fixture: ComponentFixture<InputFormListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
