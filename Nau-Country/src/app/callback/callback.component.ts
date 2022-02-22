import { Component, OnInit, Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {
  public code : string = "";
  public state : string = "";
  constructor(@Inject(DOCUMENT) private document: Document) { }

  ngOnInit(): void {
    //run this first :)
    try {
      let url : URL = new URL(this.document.location.href);
      let params : URLSearchParams = new URLSearchParams(url.search);
      let tCode : string = ""; //temp cuz i couldn't access code
      let tState : string = ""; //temp cuz i couldn't access state
      params.forEach(function(value, key) {
        if (key == "code")
          tCode = value;
        else if (key == "state")
          tState = value;
      });
      this.code = tCode;
      this.state = tState;
    }
    catch (e)
    {
      alert('Un-parsable URL' + e);
    }
  }
}
