import {HTTP_INTERCEPTORS} from '@angular/common/http';
import {ModuleWithProviders, NgModule} from '@angular/core';
import {Route, RouterModule} from '@angular/router';
import {AuthInterceptorService} from './auth-interceptor.service';
import {SsoConfigToken} from './config';
import {SsoConfig} from './config.model';
import {SigninCallbackComponent} from './signin-callback.component';
import {OidcService} from './oidc.service';

const routes: Route[] = [
  {
    path: 'signin',
    component: SigninCallbackComponent,
  }
  ];
@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  declarations: [SigninCallbackComponent],
  providers: [
    OidcService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true
    }
  ],
  exports: [
        RouterModule
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
      ],
    };
  }
}
