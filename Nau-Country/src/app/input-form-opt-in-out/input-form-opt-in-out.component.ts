import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { CCService } from '../cc.service'
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-input-form-opt-in-out',
  templateUrl: './input-form-opt-in-out.component.html',
  styleUrls: ['./input-form-opt-in-out.component.css']
})
export class InputFormOptInOutComponent implements OnInit {

  dropdownSettings:IDropdownSettings = {};
  contactLists: GetListsResponse;
  selectedIdsAdd: Array<string>;
  selectedIdsRemove: Array<string>;
  userOptIn: string;
  userOptOut: string;

  constructor(private ccService: CCService) {}



  ngOnInit(): void {
    this.GetLists();
    this.selectedIdsAdd = [];
    this.selectedIdsRemove = [];
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

  onItemSelectAdd(item: any) {
    this.selectedIdsAdd.push(item.list_id)
  }

  onItemDeSelectAdd(item: any) {
    this.selectedIdsAdd.forEach((value,index)=>{
      if(value==item.list_id) this.selectedIdsAdd.splice(index,1);
    });
  }

  onItemSelectRemove(item: any) {
    this.selectedIdsRemove.push(item.list_id)
  }

  onItemDeSelectRemove(item: any) {
    this.selectedIdsRemove.forEach((value,index)=>{
      if(value==item.list_id) this.selectedIdsRemove.splice(index,1);
    });
  }

  onOptInClick(){
    // needs method to return a single contact list

    // this.selectedIdsAdd.forEach(list => {
    //   let temp: ContactList = this.ccService.getContactLists()
    //   this.ccService.updateList()
    // });
  }

  onOptOutClick(){
    //needs method to return a single contact list
  }

  isDisabledAdd() {
    return !(this.selectedIdsAdd.length != 0 && this.userOptIn != '')
  }

  isDisabledRemove() {
    return !(this.selectedIdsRemove.length != 0 && this.userOptOut != '');
  }



}
