import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { GetManyResponse } from './models/GetManyResponse';

@Injectable({
  providedIn: 'root'
})
export class CCService {
  baseUrl : string = "http://192.168.112.1:45455/api/ConstantContact/";
  constructor(private http: HttpClient) { }

  setAuthorization(code : string) {
    alert("in2");
    let url : string = this.baseUrl + "authorize";
    return this.http.post<string>(url, code);
  }

  getManyContacts() {
      let url : string = "";
      return this.http.get<GetManyResponse>(url);
  }
}
