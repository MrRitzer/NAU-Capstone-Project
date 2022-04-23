import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { CCService } from '../cc.service'
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-input-form-opt-in-out',
  templateUrl: './input-form-opt-in-out.component.html',
  styleUrls: ['./input-form-opt-in-out.component.css']
})
export class InputFormOptInOutComponent implements OnInit {

  dropdownSettings:IDropdownSettings = {};
  contactLists: GetListsResponse;
  selectedIds: Array<string>;
  msSelectedIds: Array<string>;
  userOptIn: string;
  userOptOut: string;

  constructor(private ccService: CCService) {}



  ngOnInit(): void {
    this.GetLists();
    this.selectedIds = [];
    this.msSelectedIds = [];
    this.userOptIn = '';
    this.userOptOut = '';

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
    this.selectedIds.push(item.list_id);
  }

  onItemDeSelect(item: any) {
    this.selectedIds.forEach((value,index)=>{
      if(value==item.list_id) this.selectedIds.splice(index,1);
    });
  }

  onOptInClick(){
    this.ccService.getContact(this.userOptIn).subscribe(data => { 
      if (data.list_memberships == null) {
        data.list_memberships = new Array<string>();
      }

      //add all lists that the contact isn't already part of
      this.selectedIds.forEach(id => {
        if (data.list_memberships.indexOf(id) < 0)
        {
          //data is not already part of this list
          data.list_memberships.push(id);
        }
      });

      this.ccService.updateContact(data).subscribe(updatedData => { console.log(updatedData.list_memberships)});
      this.selectedIds = [];
    });

    this.userOptIn = '';
    this.msSelectedIds = [];
  }

  onOptOutClick(){
    this.ccService.getContact(this.userOptOut).subscribe(data => { 
      if (data.list_memberships == null) {
        data.list_memberships = new Array<string>();
      }
      //Remove any selected ids from contact
      this.selectedIds.forEach(list => {
        data.list_memberships.forEach((value,index) => {
          if(value==list) {
            data.list_memberships.splice(index,1);
          }
        });
      });

      this.ccService.updateContact(data).subscribe(updatedData => { console.log(updatedData.list_memberships)});
      this.selectedIds = [];
    });

    this.userOptOut = '';
    this.msSelectedIds = [];
  }

  isDisabledAdd() {
    return !(this.selectedIds.length != 0 && this.userOptIn != '')
  }

  isDisabledRemove() {
    return !(this.selectedIds.length != 0 && this.userOptOut != '');
  }



}
