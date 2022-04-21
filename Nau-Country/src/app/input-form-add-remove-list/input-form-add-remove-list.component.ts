import { Component, OnInit } from '@angular/core';
import { Contact } from '../models/Contact';
import { ContactList } from '../models/ContactList';
import { CCService } from '../cc.service'
import { GetListsResponse } from '../models/GetListsResponse';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-input-form-add-remove-list',
  templateUrl: './input-form-add-remove-list.component.html',
  styleUrls: ['./input-form-add-remove-list.component.css']
})
export class InputFormAddRemoveListComponent implements OnInit {
    
  dropdownSettings:IDropdownSettings = {};
  contactLists: GetListsResponse;
  selectedIds: Array<string>;
  newList: string;
  
  constructor(private ccService: CCService) {}

  ngOnInit(): void {
    this.GetLists();
    this.selectedIds = [];
    this.newList = '';

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

  onAddCLick(){
    let list: ContactList = new ContactList();
    list.contacts = new Array<Contact>();
    list.created_at = new Date().toDateString();
    list.description = "new list";
    list.membership_count = new Uint8Array();
    list.name = this.newList;
    list.updated_at = new Date().toDateString();
    // list.list_id = 'newListWoo';
    let createStatus: string = "";
    this.ccService.createList(list).subscribe(() => createStatus = 'Create Successful');
  }

  onRemoveClick(){
    this.selectedIds.forEach(id => {
      let deleteStatus : string = "";
      this.ccService.deleteList(id).subscribe(() => deleteStatus = 'Delete Successful');
    });
  }

  isDisabledAdd() {
    return !(this.newList != '');
  }

  isDisabledRemove() {
    return !(this.selectedIds.length != 0);
  }

}
