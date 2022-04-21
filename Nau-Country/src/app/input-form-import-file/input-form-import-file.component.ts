import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { CustomField } from '../models/CustomField';
import { PhoneNumber } from '../models/PhoneNumber';
import { StreetAddress } from '../models/StreetAddress';
import { CCService } from '../cc.service'
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { EmailAddress } from '../models/EmailAddress';

@Component({
  selector: 'app-input-form-import-file',
  templateUrl: './input-form-import-file.component.html',
  styleUrls: ['./input-form-import-file.component.css']
})
export class InputFormImportFileComponent implements OnInit {

  dropdownSettings:IDropdownSettings = {};
  contactLists: GetListsResponse;
  selectedIds: Array<string>;
  msSelectedIds: Array<string>;
  newList: string;
  tempFile: string;
  file: File;

  constructor(private ccService: CCService) { }

  ngOnInit(): void {
    this.GetLists();
    this.selectedIds = [];
    this.msSelectedIds = [];

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

  onItemSelect(item: any) {
    this.selectedIds.push(item.list_id)
  }

  onItemDeSelect(item: any) {
    this.selectedIds.forEach((value,index)=>{
      if(value==item.list_id) this.selectedIds.splice(index,1);
    });
  }
  
  onAddClick(){
    let reader = new FileReader();
    reader.readAsText(this.file);
    reader.onload = () => {
      let csvData = reader.result?.toString();
      let contactsList = csvData!.split(/\r?\n/);
      contactsList.pop();
      contactsList.forEach(contact => {
        let contactInfo = contact.split(',');
        let newEmail: EmailAddress = new EmailAddress();
        newEmail.address = contactInfo[0];
        newEmail.permission_to_send = "implicit";
        newEmail.created_at = new Date();
        newEmail.updated_at = new Date();
        newEmail.opt_in_source = "Contact";
        newEmail.opt_in_date = new Date();
        newEmail.opt_out_reason = "New contact";
        newEmail.confirm_status = "confirmed";
        
        let newContact: Contact = new Contact();
        newContact.first_name = contactInfo[1];
        newContact.last_name = contactInfo[2];
        newContact.email_address = newEmail;
        newContact.job_title = "";
        newContact.company_name = "";
        newContact.birthday_day = 1;
        newContact.birthday_month = 1;
        newContact.anniversary = "2000-01-01";
        newContact.update_source = "Account";
        newContact.create_source = "Account";
        newContact.custom_fields = new Array<CustomField>();
        newContact.phone_numbers = new Array<PhoneNumber>();
        newContact.street_addresses = new Array<StreetAddress>();
        newContact.list_memberships = new Array<string>();
        this.selectedIds.forEach(id => {
          newContact.list_memberships.push(id);
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
        this.ccService.createContact(newContact).subscribe(observer);
      });
    };
    reader.onerror = function () {
      console.log('error is occured while reading file!');
    };
    this.tempFile = "";
    this.msSelectedIds = [];
    this.selectedIds = [];
  }

  fileInput(event: any) {
    this.file = event.target.files[0];
  }

  isDisabled() {
    return !(this.selectedIds.length != 0 && this.file != null);
  }

}
