import {Component} from '@angular/core';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {VERSION} from '../environments/version';
import {AuthGuard} from './guards/auth-guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends BaseComponent {
  public Version = VERSION;
  public Date = {FullYear: new Date(Date.now()).getFullYear()};
  public AuthGuardActive = false;
  constructor(
    authGuard: AuthGuard,
  ) {
    super();
    const pendingRedirect = localStorage.getItem('pendingRedirect');
    localStorage.removeItem('pendingRedirect');
    if (pendingRedirect !== null) {
      window.location.href = pendingRedirect;
    }
    this.addSub(authGuard.IsActive.subscribe(value => this.AuthGuardActive = value));
  }

}
