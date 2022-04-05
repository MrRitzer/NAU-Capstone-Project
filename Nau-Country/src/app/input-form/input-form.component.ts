import { Component, OnInit } from '@angular/core';

//import the children to inherit
import { InputFormContactComponent } from '../input-form-contact/input-form-contact.component';
import { InputFormListComponent } from '../input-form-list/input-form-list.component';
import { Contact } from '../models/Contact';
import { CCService } from '../cc.service';
import { GetManyResponse } from '../models/GetManyResponse';
import { EmailAddress } from '../models/EmailAddress';
import { GetListsResponse } from '../models/GetListsResponse';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-input-form',
  templateUrl: './input-form.component.html',
  styleUrls: ['./input-form.component.css']
})
export class InputFormComponent implements OnInit {
  temp : GetManyResponse;
  templists : GetListsResponse;
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
        console.log(this.temp.contacts_count);
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getManyContacts(lists, 20)
      .subscribe(observer);
  }
  GetListsTest() : void {
    let lists = new Array<string>();
    lists.push("General Interest");
    lists.push("Testing");

    this.templists = new GetListsResponse();

    const observer = {
      next: (response : GetListsResponse) => {
        this.templists = response;
        console.log(this.templists.lists_count);
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getContactLists()
      .subscribe(observer);
  }

  CreateContactTest() : void {
    //create a fake contact
    let contact : Contact = new Contact();
    let email : EmailAddress = new EmailAddress();

  }

  DeleteContactTest(c : Contact) : void {
    let id : string = c.contact_id;
    let deleteStatus : string = "";
    this.ccService.deleteContact(id).subscribe(() => deleteStatus = 'Delete Successful');
  }
}
