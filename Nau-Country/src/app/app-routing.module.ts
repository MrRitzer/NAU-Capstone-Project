import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CallbackComponent } from './callback/callback.component';
import { InputFormAddRemoveComponent } from './input-form-add-remove/input-form-add-remove.component';
import { InputFormAddRemoveListComponent } from './input-form-add-remove-list/input-form-add-remove-list.component';
import { InputFormOptInOutComponent } from './input-form-opt-in-out/input-form-opt-in-out.component';
import { InputFormImportFileComponent } from './input-form-import-file/input-form-import-file.component';

const routes: Routes = [
  { path: "contacts", component: InputFormAddRemoveComponent },
  { path: "lists", component: InputFormAddRemoveListComponent },
  { path: "opt", component: InputFormOptInOutComponent },
  { path: "importfile", component: InputFormImportFileComponent },
  { path: '', redirectTo: '/contacts', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
