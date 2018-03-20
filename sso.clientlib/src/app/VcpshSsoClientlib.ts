import { CommonModule } from "@angular/common";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { InjectionToken, ModuleWithProviders, NgModule } from "@angular/core";
import { Route, RouterModule } from "@angular/router";
import "babel-polyfill";
import { UserManagerSettings } from "oidc-client";
import { SigninCallbackComponent } from "./components/signin-callback.component";
import { AuthInterceptorService } from "./services/auth-interceptor.service";
import { OidcService } from "./services/oidc.service";
import { SsoConfigToken } from "./config";

const routes: Route[] = [
  {
    path: "signin",
    component: SigninCallbackComponent
  }
];

@NgModule({
  declarations: [SigninCallbackComponent],
  imports: [CommonModule, RouterModule.forChild(routes)],
  providers: [
    OidcService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptorService,
      multi: true
    }
  ],
  exports: [RouterModule]
})
export class VcpshSsoClientlib {
  static forRoot(ssoConfig: UserManagerSettings): ModuleWithProviders {
    return {
      ngModule: VcpshSsoClientlib,
      providers: [
        {
          provide: SsoConfigToken,
          useValue: ssoConfig
        }
      ]
    };
  }
}
