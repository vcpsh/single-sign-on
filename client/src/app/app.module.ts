import {HttpClientModule, HttpClientXsrfModule} from '@angular/common/http';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {
  MatButtonModule,
  MatCardModule,
  MatCheckboxModule,
  MatDialogModule,
  MatFormFieldModule,
  MatInputModule,
  MatProgressSpinnerModule,
} from '@angular/material';
import {MatToolbarModule} from '@angular/material/toolbar';
import {BrowserModule} from '@angular/platform-browser';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {RouterModule} from '@angular/router';
import {ServiceWorkerModule} from '@angular/service-worker';
import {SsoClientLibModule} from '@vcpsh/sso-client-lib';
import {environment} from '../environments/environment';

import {AppComponent} from './app.component';
import {ConfirmComponent} from './components/confirm/confirm.component';
import {ForgotComponent} from './components/forgot/forgot.component';
import {LoginComponent} from './components/login/login.component';
import {LogoutComponent} from './components/logout/logout.component';
import {OverviewComponent} from './components/overview/overview.component';
import {PageNotFoundComponent} from './components/page-not-found/page-not-found.component';
import {PrivacyComponent} from './components/privacy/privacy.component';
import {RegisterComponent} from './components/register/register.component';
import {ResetComponent} from './components/reset/reset.component';
import {AuthGuard} from './guards/auth-guard';
import {routes} from './routes';
import {AccountService} from './services/account.service';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    PageNotFoundComponent,
    RegisterComponent,
    ForgotComponent,
    OverviewComponent,
    LogoutComponent,
    ResetComponent,
    ConfirmComponent,
    PrivacyComponent,
  ],
  entryComponents: [
    PrivacyComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    RouterModule.forRoot(
      routes,
      {
    }),
    SsoClientLibModule.forRoot({
      authority: environment.authority,
      client_id: 'sh.vcp.sso-client@1.0.0',
      response_type: 'id_token token',
      scope: 'openid profile sh.vcp.sso@1.0.0',
      route_after_user_unloaded: '/login',
      automaticSilentRenew: true,
      loadUserInfo: true,
      debug: !environment.production,
    }),
    FormsModule,
    HttpClientModule,
    HttpClientXsrfModule.withOptions({
      cookieName: 'XSRF-TOKEN',
      headerName: 'X-XSRF-TOKEN',
    }),
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    ServiceWorkerModule.register('ngsw-worker.js', {enabled: environment.production}),
  ],
  providers: [
    AccountService,
    AuthGuard,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
