import {Inject, Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot} from '@angular/router';
import {User, UserManager, UserManagerSettings} from 'oidc-client';
import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {Observable} from 'rxjs/Observable';
import {UserModel} from '../models/user.model';
import {SsoConfigToken} from '../VcpshSsoClientlib';

@Injectable()
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
    @Inject(SsoConfigToken) settings: UserManagerSettings
  ) {
    this._manager = new UserManager(settings);
    this._manager.events.addSilentRenewError(ev => this.silentRenewError(ev));
    this._manager.events.addUserLoaded(ev => this.userLoaded(ev));
    this._manager.events.addUserUnloaded(ev => this.userUnloaded(ev));
    this._manager.events.addUserSignedOut(ev => this.userSignedOut(ev));
    this._manager.events.addAccessTokenExpired(ev => this.accessTokenEpired(ev));
    this._manager.events.addAccessTokenExpiring(ev => this.accessTokenExpiring(ev));
    this.getUser();
  }


  /**
   * Route guard for logged in users.
   */
  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this.getUser().then(user => {
      if (user === null) {
        this._router.navigate(['']);
        return false;
      }
      return true;
    });
  }

  public login(): void {
    this._manager.signinRedirect();
  }

  public logout(): void {
    this._manager.signoutRedirect();
  }

  public signinCallback(): void {
    this._manager.signinRedirectCallback()
      .then(() => {
        this._router.navigateByUrl('/start');
      })
      .catch(e => console.error('RedirectError', e));
  }

  private getUser(): Promise<UserModel | null> {
    return this._manager.getUser()
      .then(user => {
        if (this._lastUserJson !== JSON.stringify(user)) {
          this._lastUserJson = JSON.stringify(user);
          this._lastUser = user;
          this._userChanged.forEach(v => v(user));
        }
        return this._lastUser;
      });
  }

  private silentRenewError(...ev: any[]): void {
    console.error('SilentRenewError', ev);
    this.getUser();
  }

  private userLoaded(...ev: any[]): void {
    console.log('UserLoaded', ev);
    this.getUser();
  }

  private userUnloaded(...ev: any[]): void {
    console.log('UserUnloaded');
    this.getUser();
  }

  private userSignedOut(...ev: any[]): void {
    console.log('UserSignedOut');
    this._manager.removeUser().then(() => this.getUser());
  }

  private accessTokenEpired(...ev: any[]): void {
    console.log('AccessTokenExpired', ev);
    this._manager.removeUser().then(() => this.getUser());
  }

  private accessTokenExpiring(...ev: any[]): void {
    console.log('AccessTokenExpiring', ev);
    this.getUser();
  }
}
