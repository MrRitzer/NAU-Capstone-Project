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
  selector: 'app-input-form-edit',
  templateUrl: './input-form-edit.component.html',
  styleUrls: ['./input-form-edit.component.css']
})
export class InputFormEditComponent implements OnInit {

  constructor(private ccService: CCService) {}

  dropdownSettingsList:IDropdownSettings = {};
  dropdownSettingsContact:IDropdownSettings = {};
  selectedList: string;
  selectedContact: string;

  // Create contacts for component
  contact: Contact = new Contact;
  email: EmailAddress = new EmailAddress;

  // Get Lists
  contactLists : GetListsResponse;
  contacts: GetManyResponse;

  ngOnInit(): void {
    this.GetLists();
    this.contacts = new GetManyResponse();
    this.selectedList = '';
    this.selectedContact = '';
    
    this.dropdownSettingsList = {
      singleSelection: true,
      textField: 'name',
      idField: 'list_id',
      enableCheckAll: false,
      itemsShowLimit: 3,
      allowSearchFilter: false,
    };
    this.dropdownSettingsContact = {
      singleSelection: true,
      textField: 'name',
      idField: 'email_address.address',
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

  GetContacts() : void {
    let lists = new Array<string>();
    lists.push(this.selectedList);
    this.contacts = new GetManyResponse();

    const observer = {
      next: (response : GetManyResponse) => {
        this.contacts = response;
        console.log(this.contacts.contacts_count);
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }

    this.ccService.getManyContacts(lists, 20)
      .subscribe(observer);
  }

  onUpdateClick() {
    this.ccService.updateContact(this.contact).subscribe(data => {
      this.selectedContact = '';
      this.contact = new Contact;
    });
  }

  onItemSelectList(item: any) {
    this.selectedList = item.list_id;
    this.GetContacts();
  }

  onItemDeSelectList() {
    this.selectedList = '';
    this.contacts = new GetManyResponse();
  }

  onItemSelectContact(item: any) {
    this.selectedContact = item.email_address.address;
    this.ccService.getContact(this.selectedContact).subscribe(data => {
      this.contact = data;
    });
  }

  onItemDeSelectContact() {
    this.selectedContact = '';
    this.contact = new Contact;
  }

  isDisabled(): boolean {
    try {
      return !(this.contact.email_address.address != '' && this.contact.first_name != '' && this.contact.last_name != '' && this.selectedList != '');
    }
    catch {
      return true;
    }
  }
}
