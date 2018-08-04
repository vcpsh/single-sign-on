import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot} from '@angular/router';
import {OidcService} from '@vcpsh/sso-client-lib';
import {Observable} from 'rxjs';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private _oidc: OidcService
  ) {}
  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    return this._oidc.User.then(u => {
      if (u === null) {
        this._oidc.login();
      } else {
        return true;
      }
    });
  }
}
