import {Component} from '@angular/core';
import {ActivatedRoute, ActivationEnd, Router} from '@angular/router';
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
  public Wide = false;
  constructor(
    authGuard: AuthGuard,
    activatedRoute: ActivatedRoute,
    private _router: Router,
  ) {
    super();
    const pendingRedirect = localStorage.getItem('pendingRedirect');
    localStorage.removeItem('pendingRedirect');
    if (pendingRedirect !== null) {
      window.location.href = pendingRedirect;
    }
    this.addSub(authGuard.IsActive.subscribe(value => this.AuthGuardActive = value));
    this.addOnInit(() => this.onInit());
  }

  private onInit() {
    this.addSub(this._router.events.subscribe((val) => {
      if (val instanceof ActivationEnd) {
        this.Wide = val.snapshot.data.wide === true;
      }
    }));
  }

}
