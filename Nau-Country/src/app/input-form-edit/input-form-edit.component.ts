import { Component, OnInit } from '@angular/core';
import { DropdownContact } from '../models/DropDownContact';
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
  dropdownContacts: Array<DropdownContact>;

  ngOnInit(): void {
    this.GetLists();
    this.contact.email_address = new EmailAddress;
    this.contacts = new GetManyResponse();
    this.dropdownContacts = [];
    this.selectedList = '';
    this.selectedContact = '';
    
    this.dropdownSettingsList = {
      singleSelection: true,
      textField: 'name',
      idField: 'list_id',
      enableCheckAll: false,
      allowSearchFilter: false,
    };
    this.dropdownSettingsContact = {
      singleSelection: true,
      textField: 'name',
      idField: 'email_address',
      enableCheckAll: false,
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
    let tempContact: DropdownContact = {name:'',email_address:''};
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
        this.dropdownContacts = [];
        this.contacts.contacts.forEach(c => {
          tempContact.name = c.first_name + " " + c.last_name;
          tempContact.email_address = c.email_address.address;
          this.dropdownContacts.push(tempContact);
        });
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
    this.dropdownContacts = [];
  }

  onItemSelectContact(item: any) {
    console.log(item);
    this.selectedContact = item.email_address;
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
