import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry, tap } from 'rxjs/operators';
import { GetManyResponse } from './models/GetManyResponse';
import { GetListsResponse } from './models/GetListsResponse';
import { Links } from './models/Links';
import { Contact } from './models/Contact';
import { ContactList } from './models/ContactList';

@Injectable({
  providedIn: 'root'
})
export class CCService {
  baseUrl : string = "http://96.3.55.1:45455/api/ConstantContact/";
  constructor(private http: HttpClient) { }

  setAuthorization(code : string) {
    let url : string = this.baseUrl + "authorize";
    const body = new HttpParams()
      .set('_code', code);

      return this.http.post(url,
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

  getManyContacts(lists : Array<string>, limit : number) : Observable<GetManyResponse> {
    let urlEncoded : string = "";
    lists.forEach((list, ii) => {
      urlEncoded += list + "%2B";
    });
    urlEncoded = urlEncoded.substring(0, urlEncoded.lastIndexOf("%2B"));

    let url : string = this.baseUrl + "getmany?tLists=" + urlEncoded + "&limit=" + limit;
    let re = / /gi;
    url = url.replace(re, "%20");

    return this.http.get<GetManyResponse>(url);
  }

  createContact(contact : Contact) {
    let url : string = this.baseUrl + "createcontact";
    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.http.post(url, contact, { headers : headers });
  }

  deleteContact(id: string){
    let url : string = this.baseUrl + "deletecontact";

    return this.http.delete(url, {body: id});
  }
}
