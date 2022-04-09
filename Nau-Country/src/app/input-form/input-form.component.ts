import { Component, OnInit } from '@angular/core';

//import the children to inherit
import { InputFormContactComponent } from '../input-form-contact/input-form-contact.component';
import { InputFormListComponent } from '../input-form-list/input-form-list.component';
import { Contact } from '../models/Contact';
import { CCService } from '../cc.service';
import { GetManyResponse } from '../models/GetManyResponse';
import { EmailAddress } from '../models/EmailAddress';
import { CustomField } from '../models/CustomField';
import { PhoneNumber } from '../models/PhoneNumber';
import { StreetAddress } from '../models/StreetAddress';

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
        console.log(this.temp.contacts_count);
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getManyContacts(lists, 20)
      .subscribe(observer);
  }

  CreateContactTest() : void {
    //create a fake contact
    let contact : Contact = new Contact();
    let email : EmailAddress = new EmailAddress();

    email.address = "test5@gmail.com";
    email.permission_to_send = "implicit";
    email.created_at = new Date("2016-03-03T15:53:04.000+00:00");
    email.updated_at = new Date("2016-03-03T15:56:29.000+00:00");
    email.opt_in_source = "Contact";
    email.opt_in_date = new Date("2016-01-23T13:48:44.108Z");
    email.opt_out_reason = "This is a test";
    email.confirm_status = "confirmed";

    contact.email_address = email;
    contact.first_name = "Jane";
    contact.last_name = "Doe";
    contact.job_title = "Musician";
    contact.company_name = "";
    contact.birthday_day = 11;
    contact.birthday_month = 3;
    contact.anniversary = "2006-11-15";
    contact.update_source = "Account";
    contact.create_source = "Account";
    contact.custom_fields = new Array<CustomField>();
    contact.phone_numbers = new Array<PhoneNumber>();
    contact.street_addresses = new Array<StreetAddress>();
    contact.list_memberships = new Array<string>();
    contact.list_memberships.push("2e40d64e-9435-11ec-b993-fa163ee7c533")


    let temp : Contact = new Contact();
    const observer = {
      next: (response : Contact) => {
        temp = response;
        console.log(temp.first_name + " " + temp.last_name);
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.createContact(contact).subscribe(observer);
  }
}
