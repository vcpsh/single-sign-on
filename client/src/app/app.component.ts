import {Component} from '@angular/core';
import {MatDialog} from '@angular/material';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {VERSION} from '../environments/version';
import {PrivacyComponent} from './components/privacy/privacy.component';
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
    activatedRoute: ActivatedRoute,
    private _dialog: MatDialog,
  ) {
    super();
    const pendingRedirect = localStorage.getItem('pendingRedirect');
    localStorage.removeItem('pendingRedirect');
    if (pendingRedirect !== null) {
      window.location.href = pendingRedirect;
    }
    this.addSub(authGuard.IsActive.subscribe(value => this.AuthGuardActive = value));
  }

  public onPrivacyClick() {
    const dialogRef = this._dialog.open(PrivacyComponent, {
      disableClose: true,
    });
  }
}
