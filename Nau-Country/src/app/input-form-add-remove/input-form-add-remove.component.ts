import { ChangeDetectorRef, ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import {NgbModal, ModalDismissReasons} from '@ng-bootstrap/ng-bootstrap';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { EmailAddress } from '../models/EmailAddress';
import { Next } from '../models/Next';
import { Input } from '@angular/core';
import { NgModule } from '@angular/core';
import { CCService } from '../cc.service'
import { GetManyResponse } from '../models/GetManyResponse';
import { GetListsResponse } from '../models/GetListsResponse';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-input-form-add-remove',
  templateUrl: './input-form-add-remove.component.html',
  styleUrls: ['./input-form-add-remove.component.css']
})
export class InputFormAddRemoveComponent implements OnInit {

  closeResult: string = '';
  constructor(private modalService: NgbModal, private ccService: CCService) {}
  
  selectedList : Array<Contact>;
  selectedListName : string;
  selectedContact : Contact;
  isContactSelected : boolean = false;

  //public _reload = true;

  listOfContactLists : ContactList[];
  listCall : string[];

  //create set of test contacts
  test1 : Contact = new Contact;
  test2 : Contact = new Contact;
  test3 : Contact = new Contact;
  test4 : Contact = new Contact;
  test5 : Contact = new Contact;
  test6 : Contact = new Contact;
  test7 : Contact = new Contact;
  test8 : Contact = new Contact;

  list1 : ContactList = new ContactList;
  list2 : ContactList = new ContactList;
  list3 : ContactList = new ContactList;
  list4 : ContactList = new ContactList;
  list5 : ContactList = new ContactList;
  response : Observable<GetListsResponse>;

  ngOnInit(): void {

    //API CALL TO GRAB LIST OF CONTACT LISTS
    this.response = this.ccService.getContactLists();

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

    this.test6.first_name = "dmitri";
    this.test6.email_address = new EmailAddress();
    this.test6.email_address.address = "dmitri@gmail.com";

    this.test7.first_name = "balthazaar";
    this.test7.email_address = new EmailAddress();
    this.test7.email_address.address = "bzman@hotmail.com";

    this.test8.first_name = "kirby";
    this.test8.email_address = new EmailAddress();
    this.test8.email_address.address = "kbjr@outlook.com";

    //add them to the dummy lists
    this.list1.name = "testlist1";
    this.list1.contacts = [this.test1, this.test8, this.test6, this.test8];
    this.list2.name = "testlist2";
    this.list2.contacts = [this.test2, this.test4, this.test3];
    this.list3.name = "testlist3";
    this.list3.contacts = [this.test3, this.test6, this.test2];
    this.list4.name = "testlist4";
    this.list4.contacts = [this.test4, this.test1, this.test7, this.test8, this.test2, this.test3];
    this.list5.name = "testlist5";
    this.list5.contacts = [this.test5, this.test7];


    //add the dummy lists to the dummy list of lists
    this.listOfContactLists = [this.list1, this.list2,this.list3,this.list4,this.list5];
    this.selectedList = this.list1.contacts;
    this.selectedListName = this.list1.name



  }

  onListSelect(index : number){

    //change the name of the selected list
    this.selectedListName = this.listOfContactLists[index].name

    //API CALL TO GET LIST OF CONTACTS
    //this.listCall = [this.listOfContactLists[index].list_id]
    this.selectedList = this.listOfContactLists[index].contacts
    //this.ccService.getManyContacts(this.listCall, 100).subscribe((response : GetManyResponse) => {this.selectedList = response.contacts;});


    console.log('changed selected list');

  }

  onContactSelect(index : number){
    this.selectedContact = this.selectedList[index];
    console.log('changed selected user');
    this.isContactSelected = true;

  }

  open(content:any) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'}).result.then((result) => {
      this.closeResult = `Closed with: ${result}`;
    }, (reason) => {
      this.closeResult = `Dismissed ${this.getDismissReason(reason)}`;
    });
  }

  private getDismissReason(reason: any): string {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return  `with: ${reason}`;
    }
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
