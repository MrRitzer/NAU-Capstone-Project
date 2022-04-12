import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputFormImportFileComponent } from './input-form-import-file.component';

describe('InputFormImportFileComponent', () => {
  let component: InputFormImportFileComponent;
  let fixture: ComponentFixture<InputFormImportFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InputFormImportFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InputFormImportFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
