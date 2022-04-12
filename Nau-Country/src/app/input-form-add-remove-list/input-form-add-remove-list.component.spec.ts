import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormAddRemoveListComponent } from './input-form-add-remove-list.component';

describe('InputFormAddRemoveListComponent', () => {
  let component: InputFormAddRemoveListComponent;
  let fixture: ComponentFixture<InputFormAddRemoveListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormAddRemoveListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormAddRemoveListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
