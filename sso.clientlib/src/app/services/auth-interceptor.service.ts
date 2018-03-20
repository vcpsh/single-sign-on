import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs/Observable';
import {OidcService} from './oidc.service';
import 'rxjs/add/observable/fromPromise';
import 'rxjs/add/operator/mergeMap';

@Injectable()
export class AuthInterceptorService implements HttpInterceptor {
  constructor(private _oidc: OidcService) {

  }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return Observable.fromPromise(this._oidc.User).flatMap(user => {
      if (user !== null && req.url.startsWith('/api/')) {
        const authReq = req.clone({headers: req.headers.set('Authorization', 'Bearer ' + user.access_token)});
        return next.handle(authReq);
      } else {
        return next.handle(req);
      }
    });
  }
}
