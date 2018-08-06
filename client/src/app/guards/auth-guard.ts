import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {OidcService} from '@vcpsh/sso-client-lib';
import {Observable, Subject} from 'rxjs';

@Injectable()
export class AuthGuard implements CanActivate {
  public IsActive = new Subject<boolean>();

  constructor(
    private _oidc: OidcService,
  ) {
    this.IsActive.next(false);
  }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    this.IsActive.next(true);
    return this._oidc.User.then(async u => {
      if (u === null) {
        await this._oidc.login();
        // this.IsActive.next(false); do not reactivate the rendering and wait for the redirect
        return false;
      } else {
        this.IsActive.next(false);
        return true;
      }
    });
  }
}
