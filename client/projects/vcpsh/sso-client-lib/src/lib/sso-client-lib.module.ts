import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {ModuleWithProviders, NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';
import {AuthInterceptorService} from './auth-interceptor.service';
import {SsoConfigToken} from './config';
import {SsoConfig} from './config.model';
import {OidcService} from './oidc.service';
import {SigninCallbackComponent} from './signin-callback.component';

const routes: Route[] = [
  {
    path: 'signin',
    component: SigninCallbackComponent,
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  declarations: [SigninCallbackComponent],
  exports: [
    RouterModule,
  ],
  providers: [
    OidcService,
  ],
})
export class SsoClientLibModule {
  static forRoot(ssoConfig: SsoConfig): ModuleWithProviders {
    return {
      ngModule: SsoClientLibModule,
      providers: [
        {
          provide: SsoConfigToken,
          useValue: ssoConfig,
        },
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AuthInterceptorService,
          multi: true,
        },
      ],
    };
  }
}
