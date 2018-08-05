import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Router} from '@angular/router';
import {OidcService} from '@vcpsh/sso-client-lib';

@Injectable()
export class AccountService {
  constructor(
    private _http: HttpClient,
    private _router: Router,
    private _oidc: OidcService
  ) {
  }

  public login(payload: { username: string; password: string, remember: boolean, returnUrl: string }): Promise<boolean> {
    return this._http.post('api/account/login', payload).toPromise()
      .then((data: { returnUrl: string }) => {
        localStorage.setItem('pendingRedirect', data.returnUrl);
        this._oidc.login();
        return true;
      })
      .catch(err => {
        return false;
      });
  }

  public cancel(returnUrl: string): Promise<boolean> {
    return this._http.post('api/account/cancel', { returnUrl }).toPromise()
      .then((data: { returnUrl: string}) => {
        localStorage.setItem('pendingRedirect', data.returnUrl);
        this._oidc.login();
        return true;
      })
      .catch(err => {
        return false;
      });
  }

  public logout(logoutId: string) {
    this._http.post('api/account/logout', { logoutId }).toPromise()
      .then((data: { returnUrl: string }) => {
        console.log(data);
      })
      .catch(err => {
        console.log(err);
      });
  }

  public forgot(value: { email: string}): Promise<boolean> {
    return this._http.post('api/account/forgot', value).toPromise()
      .then(() => {
        return true;
      })
      .catch(() => {
        return false;
      });
  }

  public reset(value: { token: string, password: string, confirmPassword: string}): Promise<boolean> {
    return this._http.post('api/account/reset', value).toPromise()
      .then(() => true)
      .catch(() => false);
  }
}
