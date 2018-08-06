import {Component} from '@angular/core';
import {BaseComponent } from '@vcpsh/sso-client-lib';
import {AuthGuard} from './guards/auth-guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends BaseComponent {
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
