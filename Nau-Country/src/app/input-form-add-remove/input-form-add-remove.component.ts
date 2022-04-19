import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { EmailAddress } from '../models/EmailAddress';
import { CustomField } from '../models/CustomField';
import { PhoneNumber } from '../models/PhoneNumber';
import { StreetAddress } from '../models/StreetAddress';
import { CCService } from '../cc.service'
import { GetManyResponse } from '../models/GetManyResponse';
import { GetListsResponse } from '../models/GetListsResponse';
import { Observable } from 'rxjs';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-input-form-add-remove',
  templateUrl: './input-form-add-remove.component.html',
  styleUrls: ['./input-form-add-remove.component.css']
})
export class InputFormAddRemoveComponent implements OnInit {

  closeResult: string = '';
  constructor(private ccService: CCService) {}
  
  selectedList : Array<Contact>;
  selectedListName : string;
  selectedContact : Contact;
  isContactSelected : boolean = false;

  //public _reload = true;

  dropdownSettings:IDropdownSettings = {};

  selectedItemsAdd: Array<string>;
  selectedItemsRemove: Array<string>;
  listOfContactLists : ContactList[];
  listCall : string[];

  // Create contacts for component
  newContact: Contact = new Contact;
  newEmail: EmailAddress = new EmailAddress;
  oldContact: EmailAddress = new EmailAddress;


  // Create test contact lists
  list1 : ContactList = new ContactList;
  list2 : ContactList = new ContactList;
  list3 : ContactList = new ContactList;
  list4 : ContactList = new ContactList;
  list5 : ContactList = new ContactList;
  response : Observable<GetListsResponse>;

  ngOnInit(): void {
    //Declare initial values
    this.oldContact.address = '';
    this.newContact.first_name = '';
    this.newContact.last_name = '';

    //API CALL TO GRAB LIST OF CONTACT LISTS
    this.response = this.ccService.getContactLists();

    //add them to the dummy lists
    this.list1.name = "testlist1";
    this.list2.name = "testlist2";
    this.list3.name = "testlist3";
    this.list4.name = "testlist4";
    this.list5.name = "testlist5";
    this.list1.list_id = "testlist1";
    this.list2.list_id = "testlist2";
    this.list3.list_id = "testlist3";
    this.list4.list_id = "testlist4";
    this.list5.list_id = "testlist5";

    //add the dummy lists to the dummy list of lists
    this.listOfContactLists = [this.list1, this.list2,this.list3,this.list4,this.list5];

    this.selectedItemsAdd = [];
    this.selectedItemsRemove = [];
    
    this.dropdownSettings = {
      singleSelection: false,
      textField: 'name',
      idField: 'list_id',
      enableCheckAll: false,
      itemsShowLimit: 3,
      allowSearchFilter: false,
    };
  }

  onListSelect(index : number){
    //change the name of the selected list
    this.selectedListName = this.listOfContactLists[index].name

    //API CALL TO GET LIST OF CONTACTS
    //this.listCall = [this.listOfContactLists[index].list_id]
    this.selectedList = this.listOfContactLists[index].contacts
    //this.ccService.getManyContacts(this.listCall, 100).subscribe((response : GetManyResponse) => {this.selectedList = response.contacts;});

    console.log(this.selectedListName);
  }

  onAddClick() {
    this.newEmail.permission_to_send = "implicit";
    this.newEmail.created_at = new Date("2000-01-01T00:00:00.000+00:00");
    this.newEmail.updated_at = new Date('"2000-01-01T00:00:00.000+00:00"');
    this.newEmail.opt_in_source = "Contact";
    this.newEmail.opt_in_date = new Date("2000-01-01T00:00:00.000+00:00");
    this.newEmail.opt_out_reason = "This is a test";
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
    this.newContact.list_memberships.push("2e40d64e-9435-11ec-b993-fa163ee7c533")

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
  }

  onRemoveClick(){

  }

  isDisabledAdd(): boolean {
    try {
      console.log(this.selectedItemsAdd);
      return !(this.newEmail.address != '' && this.newContact.first_name != '' && this.newContact.last_name != '' && this.selectedItemsAdd.length != 0);
    }
    catch {
      return true;
    }
  }

  isDisabledRemove(): boolean {
    try {
      return !(this.oldContact.address != '' && this.selectedItemsRemove.length != 0);
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
