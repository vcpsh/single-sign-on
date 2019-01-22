import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from '@angular/common/http';
import {Injectable} from '@angular/core';
import { Observable, from } from 'rxjs';
import {OidcService} from './oidc.service';
import {flatMap} from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class AuthInterceptorService implements HttpInterceptor {
  private _baseUrl = `${window.location.protocol}//${window.location.host}/api/`;
  constructor(private _oidc: OidcService) {

  }

  public intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this._oidc.User).pipe(flatMap(user => {
      if (user !== null &&
        (req.url.startsWith(this._baseUrl) || req.url.startsWith('/api/'))
      ) {
        const authReq = req.clone({headers: req.headers.set('Authorization', 'Bearer ' + user.access_token)});
        return next.handle(authReq);
      } else {
        return next.handle(req);
      }
    }));
  }
}
