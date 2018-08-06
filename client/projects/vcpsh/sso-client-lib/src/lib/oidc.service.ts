import { Inject, Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { UserManager } from 'oidc-client';
import { Observable } from 'rxjs';
import {SsoConfig} from './config.model';
import { UserModel } from './user.model';
import { SsoConfigToken } from './config';
import * as Oidc from 'oidc-client';

@Injectable({
    providedIn: 'root'
})
export class OidcService implements CanActivate {
  private _manager: UserManager;
  private _userChanged: ((user: UserModel | null) => void)[] = [];
  private _lastUserJson = '';
  private _lastUser: UserModel | null = null;

  public get User(): Promise<UserModel | null> {
    return this.getUser();
  }

  public set UserChanged(callback: (user: UserModel | null) => void) {
    this._userChanged.push(callback);
  }

  constructor(
    private _router: Router,
    @Inject(SsoConfigToken) private _settings: SsoConfig,
  ) {
    if (this._settings.debug === true) {
      Oidc.Log.logger = console;
    }
    this._manager = new UserManager(_settings);
    this._manager.events.addSilentRenewError(ev => this.silentRenewError(ev));
    this._manager.events.addUserLoaded(ev => this.userLoaded(ev));
    this._manager.events.addUserUnloaded(ev => this.userUnloaded(ev));
    this._manager.events.addUserSignedOut(ev => this.userSignedOut(ev));
    this._manager.events.addAccessTokenExpired(ev =>
      this.accessTokenExpired(ev),
    );
    this._manager.events.addAccessTokenExpiring(ev =>
      this.accessTokenExpiring(ev),
    );
    this.getUser();
  }

  /**
   * Route guard for logged in users.
   */
  public canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): Observable<boolean> | Promise<boolean> | boolean {
    return this.getUser().then(user => {
      if (user === null) {
        this._router.navigateByUrl('/');
        return false;
      }
      return true;
    });
  }

  public login(): Promise<any> {
    return this._manager.signinRedirect();
  }

  public logout(): void {
    this._manager.signoutRedirect();
  }

  public signinCallback(): void {
    this._manager
      .signinRedirectCallback()
      .then(() => {
        this._router.navigateByUrl('/start');
      })
      .catch(e => console.error('RedirectError', e));
  }

  private getUser(): Promise<UserModel | null> {
    return this._manager.getUser().then(user => {
      if (this._lastUserJson !== JSON.stringify(user)) {
        this._lastUserJson = JSON.stringify(user);
        this._lastUser = user;
        this._userChanged.forEach(v => v(user));
      }
      return this._lastUser;
    });
  }

  private silentRenewError(...ev: any[]): void {
    this.log('SilentRenewError', ev);
    this.getUser();
  }

  private userLoaded(...ev: any[]): void {
    this.log('UserLoaded', ev);
    this.getUser();
  }

  private userUnloaded(...ev: any[]): void {
    this.log('UserUnloaded');
    this.getUser();
  }

  private userSignedOut(...ev: any[]): void {
    this.log('UserSignedOut');
    this._manager.removeUser().then(() => this.getUser());
  }

  private accessTokenExpired(...ev: any[]): void {
    this.log('AccessTokenExpired', ev);
    this._manager.removeUser().then(() => this.getUser());
  }

  private accessTokenExpiring(...ev: any[]): void {
    this.log('AccessTokenExpiring', ev);
    this.getUser();
  }

  private log(...objs: any[]) {
    if (this._settings.debug === true) {
      console.log(...objs);
    }
  }
}
