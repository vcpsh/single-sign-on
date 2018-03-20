import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {OidcService} from '../services/oidc.service';

@Component({
  selector: 'sh-vcp-sso-client-lib-signin-callback',
  template: '',
  styleUrls: [],
})
export class SigninCallbackComponent implements OnInit {
  public ngOnInit(): void {
    this._oidc.signinCallback();
  }

  constructor(private _oidc: OidcService) {

  }
}
