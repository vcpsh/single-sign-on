import {Component} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
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
    title: Title,
  ) {
    super();
    title.setTitle('Logout - vcp.sh');
    this.addSub(
      route.queryParams.subscribe(params => {
      if (params.logoutId) {
        service.logout(params.logoutId);
      }
    }));
  }
}
