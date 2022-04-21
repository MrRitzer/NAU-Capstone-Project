import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { EmailAddress } from '../models/EmailAddress';
import { CustomField } from '../models/CustomField';
import { PhoneNumber } from '../models/PhoneNumber';
import { StreetAddress } from '../models/StreetAddress';
import { CCService } from '../cc.service'
import { GetManyResponse } from '../models/GetManyResponse';
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-input-form-add-remove',
  templateUrl: './input-form-add-remove.component.html',
  styleUrls: ['./input-form-add-remove.component.css']
})
export class InputFormAddRemoveComponent implements OnInit {

  constructor(private ccService: CCService) {}

  dropdownSettings:IDropdownSettings = {};
  selectedItemsAddIds: Array<string>;

  // Create contacts for component
  newContact: Contact = new Contact;
  newEmail: EmailAddress = new EmailAddress;
  oldContact: EmailAddress = new EmailAddress;

  // Get Lists
  contactLists : GetListsResponse;

  ngOnInit(): void {
    //Declare initial values
    this.oldContact.address = '';
    this.newContact.first_name = '';
    this.newContact.last_name = '';

    //API CALL TO GRAB LIST OF CONTACT LISTS
    this.GetLists();

    this.selectedItemsAddIds = [];
    
    this.dropdownSettings = {
      singleSelection: false,
      textField: 'name',
      idField: 'list_id',
      enableCheckAll: false,
      itemsShowLimit: 3,
      allowSearchFilter: false,
    };
  }

  GetLists() : void {
    this.contactLists = new GetListsResponse();

    const observer = {
      next: (response : GetListsResponse) => {
        this.contactLists = response;
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getContactLists()
      .subscribe(observer);
  }

  onAddClick() {
    this.newEmail.permission_to_send = "implicit";
    this.newEmail.created_at = new Date();
    this.newEmail.updated_at = new Date();
    this.newEmail.opt_in_source = "Contact";
    this.newEmail.opt_in_date = new Date();
    this.newEmail.opt_out_reason = "New contact";
    this.newEmail.confirm_status = "confirmed";

    this.newContact.email_address = this.newEmail;
    this.newContact.job_title = "";
    this.newContact.company_name = "";
    this.newContact.birthday_day = 1;
    this.newContact.birthday_month = 1;
    this.newContact.anniversary = "2000-01-01";
    this.newContact.update_source = "Account";
    this.newContact.create_source = "Account";
    this.newContact.custom_fields = new Array<CustomField>();
    this.newContact.phone_numbers = new Array<PhoneNumber>();
    this.newContact.street_addresses = new Array<StreetAddress>();
    this.newContact.list_memberships = new Array<string>();
    this.selectedItemsAddIds.forEach(id => {
      this.newContact.list_memberships.push(id);
    });

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
    this.ccService.createContact(this.newContact).subscribe(observer);
    this.newEmail.address = '';
    this.newContact.first_name = '';
    this.newContact.last_name = '';
  }

  onRemoveClick(){
    let id: string = '';
    let deleteStatus : string = "";
    let temp: Observable<Contact> = this.ccService.getContact(this.oldContact.address);
    temp.subscribe(data => {
      this.ccService.deleteContact(data.contact_id).subscribe(() => deleteStatus = 'Delete Successful');
      this.oldContact.address = '';
    })
  }

  onItemSelectAdd(item: any) {
    this.selectedItemsAddIds.push(item.list_id)
  }

  onItemDeSelectAdd(item: any) {
    this.selectedItemsAddIds.forEach((value,index)=>{
      if(value==item.list_id) this.selectedItemsAddIds.splice(index,1);
    });
  }

  isDisabledAdd(): boolean {
    try {
      return !(this.newEmail.address != '' && this.newContact.first_name != '' && this.newContact.last_name != '' && this.selectedItemsAddIds.length != 0);
    }
    catch {
      return true;
    }
  }

  isDisabledRemove(): boolean {
    try {
      return !(this.oldContact.address != '');
    }
    catch {
      return true;
    }
  }
  /*
  private reload() {
    setTimeout(() => this._reload = false);
    setTimeout(() => this._reload = true);

  }
  */

}
