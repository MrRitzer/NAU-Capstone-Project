import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { GetManyResponse } from './models/GetManyResponse';
import { AuthorizationRequest } from './models/AuthorizationRequest';
import { AuthorizationResponse } from './models/AuthorizationResponse';

@Injectable({
  providedIn: 'root'
})
export class CCService {
  baseUrl : string = "http://192.168.112.1:45455/api/ConstantContact/";
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
    // .subscribe({
    //   error: error => {
    //       alert("Error setting authorization " + error.toString());
    //   }
    // });
  }

  Authorize(authorization : any) {
    alert("Response: " + authorization);
  }

  getManyContacts() {
      let url : string = "";
      return this.http.get<GetManyResponse>(url);
  }
}
