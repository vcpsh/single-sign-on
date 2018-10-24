import {Component} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';
import {Jwt} from '../../util/jwt';

@Component({
  selector: 'app-confirm',
  templateUrl: './confirm.component.html',
  styleUrls: ['./confirm.component.scss'],
})
export class ConfirmComponent extends BaseComponent {
  public Success = false;
  public Error = false;
  constructor(
    service: AccountService,
    route: ActivatedRoute,
    title: Title,
  ) {
    super();
    title.setTitle('Confirm - vcp.sh');
    this.addSub(route.queryParams.subscribe(params => {
      if (params.token) {
        const token = Jwt.parse(params.token);
        if (token === 'expired') {
          this.Error = true;
        } else {
          service.confirm(params.token).then(res => {
            if (res) {
              this.Success = true;
            } else {
              this.Error = true;
            }
          });
          console.log(token);
        }
      } else {
        this.Error = true;
      }
    }));
  }

}
