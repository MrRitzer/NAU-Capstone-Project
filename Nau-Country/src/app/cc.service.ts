import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { GetManyResponse } from './models/GetManyResponse';

@Injectable({
  providedIn: 'root'
})
export class CCService {
  baseUrl : string = "http://192.168.112.1:5000/api/ConstantContact/";
  constructor(private http: HttpClient) { }

  setAuthorization(code : string) {
    alert("in2: " + code);
    let url : string = this.baseUrl + "authorize";
    const body = new HttpParams()
      .set('_code', code);
    return this.http.post(url,
      body.toString(),
      {
        headers: new HttpHeaders()
        .set('Content-Type', 'application/x-www-form-urlencoded')
      }
    )
  }

  getManyContacts(lists : Array<string>, limit : number) {
    //Turn lists into single string of format "list1+list2+...+listn"
    let urlEncoded : string = "";
    lists.forEach((list, ii) => {
      urlEncoded += list + "+";
    });
    urlEncoded = urlEncoded.substring(0, urlEncoded.lastIndexOf("+"));

    let url : string = this.baseUrl + "getmany/" + urlEncoded + "/" + limit;
    return this.http.get<GetManyResponse>(url);
  }
}
