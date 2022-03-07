import { Component, OnInit, Inject } from '@angular/core';
import { DOCUMENT, Location } from '@angular/common';
import { Globals } from '../common/globals';
import { CCService } from '../cc.service';

@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {
  public code : string = "";
  public state : string = "";
  constructor(@Inject(DOCUMENT) private document: Document, private location: Location,
    private ccService : CCService) { }

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
      
      if (this.state != Globals.State)
      {
        //request was corrupted somehow. 
        //Do something
        alert('State not valid, OAuth request corrupted');
      }
      else
      {
        alert("in");
        this.ccService.setAuthorization(this.code)
          .subscribe(response => alert(response));
      }

      this.location.replaceState("/");
    }
    catch (e)
    {
      alert('Un-parsable URL' + e);
    }
  }
}
