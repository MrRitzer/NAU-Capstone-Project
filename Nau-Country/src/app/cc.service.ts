import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, tap } from 'rxjs/operators';
import { GetManyResponse } from './models/GetManyResponse';
import { GetListsResponse } from './models/GetListsResponse';
import { Links } from './models/Links';
import { Contact } from './models/Contact';
import { ContactList } from './models/ContactList';
import { EmailAddress } from './models/EmailAddress';

@Injectable({
  providedIn: 'root'
})
export class CCService {
  // baseUrl : string = "http://192.168.0.215:45455/api/ConstantContact/";
  baseUrl : string = "http://192.168.112.1:45455/api/ConstantContact/"; //Caleb's Desktop
  constructor(private http: HttpClient) { }

  setAuthorization(code : string) {
    let url : string = this.baseUrl + "authorize";
    const body = new HttpParams()
      .set('_code', code);

      return this.http.post<string>(url,
      body.toString(),
      {
        headers: new HttpHeaders()
        .set('Content-Type', 'application/x-www-form-urlencoded')
      }
    );
  }

  getContactLists(){
    let url : string = this.baseUrl + "getlists";
    return this.http.get<GetListsResponse>(url);
  }

  getContact(email: string) : Observable<Contact> {

    let url : string = this.baseUrl + "getcontact" + "?email=" + email;
    return this.http.get<Contact>(url);
  }

  getManyContacts(lists : Array<string>, limit : number) : Observable<GetManyResponse> {
    let urlEncoded : string = "";
    lists.forEach((list, ii) => {
      urlEncoded += list + "%2C";
    });
    urlEncoded = urlEncoded.substring(0, urlEncoded.lastIndexOf("%2C"));
    
    let url : string = this.baseUrl + "getmany?lists=" + urlEncoded + "&limit=" + limit;
    let re = / /gi;
    url = url.replace(re, "%20");

    return this.http.get<GetManyResponse>(url);
  }

  createContact(contact : Contact) : Observable<Contact> {
    let url : string = this.baseUrl + "createcontact";
    let headers = new HttpHeaders();
    headers.set('Content-Type', 'application/json');

    return this.http.post<Contact>(url, contact, { 'headers' : headers });
  }

  deleteContact(id: string) : Observable<string> {
    let url : string = this.baseUrl + "deletecontact" + "?contactId=" + id;

    return this.http.delete<string>(url);

  }

   updateContact(contact: Contact) : Observable<Contact>{
    let url : string = this.baseUrl + "updatecontact";
    contact.update_source = "Account";
    if (contact.birthday_day < 1) {
      contact.birthday_day = 1;
    }
    if (contact.birthday_month < 1) {
      contact.birthday_month = 1;
    }

    return this.http.put<Contact>(url, contact);
  }

  createList(list: ContactList) : Observable<ContactList> {
    let url: string = this.baseUrl + "createlist";
    let headers = new HttpHeaders();
    headers.set('Content-Type', 'application/json');

    return this.http.post<ContactList>(url, list, { 'headers' : headers });
  }

  updateList(list: ContactList) {
    let url: string = this.baseUrl + "updatelist";

    return this.http.put(url, list);
  }

  deleteList(id: string) {
    let url: string = this.baseUrl + "deletelist"+ "?listId=" + id;
    return this.http.delete(url);
  }
}
