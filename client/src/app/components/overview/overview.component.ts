import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {BaseComponent, OidcService} from '@vcpsh/sso-client-lib';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent extends BaseComponent {

  constructor(
    router: Router,
    oidc: OidcService,
  ) {
    super();
  }
}
