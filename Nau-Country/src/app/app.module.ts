import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InputFormComponent } from './input-form/input-form.component';
import { CallbackComponent } from './callback/callback.component';
import { InputFormAddRemoveComponent } from './input-form-add-remove/input-form-add-remove.component';
import { InputFormAddRemoveListComponent } from './input-form-add-remove-list/input-form-add-remove-list.component';
import { InputFormOptInOutComponent } from './input-form-opt-in-out/input-form-opt-in-out.component';
import { InputFormImportFileComponent } from './input-form-import-file/input-form-import-file.component';


@NgModule({
  declarations: [
    AppComponent,
    InputFormComponent,
    CallbackComponent,
    InputFormAddRemoveComponent,
    InputFormAddRemoveListComponent,
    InputFormOptInOutComponent,
    InputFormImportFileComponent,
  ],
  imports: [
    NgbModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
