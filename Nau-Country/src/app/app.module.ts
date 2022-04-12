import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { InputFormComponent } from './input-form/input-form.component';
import { CallbackComponent } from './callback/callback.component';
import { InputFormAddRemoveComponent } from './input-form-add-remove/input-form-add-remove.component';


@NgModule({
  declarations: [
    AppComponent,
    InputFormComponent,
    CallbackComponent,
    InputFormAddRemoveComponent,
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
