import { ChangeDetectorRef, ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { EmailAddress } from '../models/EmailAddress';
import { Next } from '../models/Next';
import { Input } from '@angular/core';
import { NgModule } from '@angular/core';


@Component({
  selector: 'app-input-form-list',
  templateUrl: './input-form-list.component.html',
  styleUrls: ['./input-form-list.component.css']
})
export class InputFormListComponent implements OnInit {

  constructor() { }

  selectedList : ContactList;
  selectedContact : Contact;
  isContactSelected : boolean = false;

  //public _reload = true;

  listOfContactLists : ContactList[];

  //create set of test contacts
  test1 : Contact = new Contact;
  test2 : Contact = new Contact;
  test3 : Contact = new Contact;
  test4 : Contact = new Contact;
  test5 : Contact = new Contact;

  list1 : ContactList = new ContactList;
  list2 : ContactList = new ContactList;
  list3 : ContactList = new ContactList;
  list4 : ContactList = new ContactList;
  list5 : ContactList = new ContactList;


  ngOnInit(): void {
    //create the hard coded values and dummy lists
    this.test1.first_name = "john";
    this.test1.email_address = new EmailAddress();
    this.test1.email_address.address = "john@gmail.com";

    this.test2.first_name = "lucas";
    this.test2.email_address = new EmailAddress();
    this.test2.email_address.address = "lucas@gmail.com";

    this.test3.first_name = "kurtis";
    this.test3.email_address = new EmailAddress();
    this.test3.email_address.address = "kurtis@gmail.com";

    this.test4.first_name = "burnadette";
    this.test4.email_address = new EmailAddress();
    this.test4.email_address.address = "burnadette@gmail.com";

    this.test5.first_name = "Dee";
    this.test5.email_address = new EmailAddress();
    this.test5.email_address.address = "Dee@gmail.com";

    //add them to the dummy lists
    this.list1.name = "testlist1";
    this.list1.contacts = [this.test1];
    this.list2.name = "testlist2";
    this.list2.contacts = [this.test2];
    this.list3.name = "testlist3";
    this.list3.contacts = [this.test3];
    this.list4.name = "testlist4";
    this.list4.contacts = [this.test4];
    this.list5.name = "testlist5";
    this.list5.contacts = [this.test5];


    //add the dummy lists to the dummy list of lists
    this.listOfContactLists = [this.list1, this.list2,this.list3,this.list4,this.list5];
    this.selectedList = this.list1;

  }

  onListSelect(index : number){
    this.selectedList = this.listOfContactLists[index];
    //document.getElementById()
    console.log('changed selected list');
    //this.cdr.detectChanges();
    //this.reload();
  }

  onContactSelect(index : number){
    this.selectedContact = this.selectedList.contacts[index];
    console.log('changed selected user');
    this.isContactSelected = true;
    //this.cdr.detectChanges();
    //this.reload();

  }
  onEditClick(){
    //open edit modal on SelectedList
  }

  onDeleteClick(){
    //open edit modal on SelectedList
  }
  /*
  private reload() {
    setTimeout(() => this._reload = false);
    setTimeout(() => this._reload = true);

  }
  */

}
