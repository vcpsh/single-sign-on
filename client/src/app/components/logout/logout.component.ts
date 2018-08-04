import { Component, OnInit } from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent, OidcService} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.scss'],
})
export class LogoutComponent extends BaseComponent {

  constructor(
    service: AccountService,
    route: ActivatedRoute,
  ) {
    super();
    this.addSub(
      route.queryParams.subscribe(params => {
      if (params.logoutId) {
        service.logout(params.logoutId);
      }
    }));
  }
}
