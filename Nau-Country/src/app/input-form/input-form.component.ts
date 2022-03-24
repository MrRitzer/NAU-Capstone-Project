import { Component, OnInit } from '@angular/core';

//import the children to inherit
import { InputFormContactComponent } from '../input-form-contact/input-form-contact.component';
import { InputFormListComponent } from '../input-form-list/input-form-list.component';
import { Contact } from '../models/Contact';
import { CCService } from '../cc.service';
import { GetManyResponse } from '../models/GetManyResponse';

@Component({
  selector: 'app-input-form',
  templateUrl: './input-form.component.html',
  styleUrls: ['./input-form.component.css']
})
export class InputFormComponent implements OnInit {
  temp : GetManyResponse;
  constructor(private ccService : CCService) { }

  ngOnInit(): void {
  }

  GetContactsTest() : void {
    let lists = new Array<string>();
    lists.push("General Interest");
    lists.push("Testing");

    this.temp = new GetManyResponse();
    
    const observer = {
      next: (response : GetManyResponse) => {
        this.temp = response;
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getManyContacts(lists, 20)
      .subscribe(observer);
  }
}
