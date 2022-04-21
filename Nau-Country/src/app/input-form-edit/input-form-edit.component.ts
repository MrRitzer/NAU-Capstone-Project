import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { EmailAddress } from '../models/EmailAddress';
import { CCService } from '../cc.service'
import { GetManyResponse } from '../models/GetManyResponse';
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

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

  // Get Lists
  contactLists : GetListsResponse;
  contacts: GetManyResponse;

  ngOnInit(): void {
    this.GetLists();
    this.contact.email_address = new EmailAddress;
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
      textField: 'first_name',
      idField: 'email_address',
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
    const observer = {
      next: (response : GetManyResponse) => {
        this.contacts = response;
      },
      error: (e: string) => {
        console.error("Request failed with error: " + e);
      }
    }
    this.ccService.getManyContacts(lists, 20)
      .subscribe(observer => {
        this.contacts = observer;
      });
  }

  onUpdateClick() {
    this.ccService.updateContact(this.contact).subscribe(data => {
      this.selectedContact = '';
      this.contact = new Contact;
      this.contact.email_address = new EmailAddress;
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
    this.contact.email_address = new EmailAddress;
  }

  isDisabled(): boolean {
    try {
      return !(this.selectedList != '' && this.selectedContact != '');
    }
    catch {
      return true;
    }
  }
}
