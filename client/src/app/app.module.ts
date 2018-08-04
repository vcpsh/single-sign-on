import {HttpClientModule, HttpClientXsrfModule} from '@angular/common/http';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatButtonModule, MatCardModule, MatCheckboxModule, MatFormFieldModule, MatInputModule} from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {RouterModule} from '@angular/router';
import {SsoClientLibModule} from '@vcpsh/sso-client-lib';
import {environment} from '../environments/environment';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './components/login/login.component';
import {AuthGuard} from './guards/auth-guard';
import {routes} from './routes';
import { PageNotFoundComponent } from './components/page-not-found-component/page-not-found.component';
import { RegisterComponent } from './components/register/register.component';
import { ForgotComponent } from './components/forgot/forgot.component';
import { OverviewComponent } from './components/overview/overview.component';
import { LogoutComponent } from './components/logout/logout.component';
import {AccountService} from './services/account.service';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    PageNotFoundComponent,
    RegisterComponent,
    ForgotComponent,
    OverviewComponent,
    LogoutComponent
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
      redirect_uri: environment.redirect_uri,
      response_type: 'id_token token',
      scope: 'openid profile sh.vcp.sso@1.0.0',
      automaticSilentRenew: true,
      post_logout_redirect_uri: environment.post_logout_redirect_uri,
      silent_redirect_uri: environment.silent_redirect_uri,
      loadUserInfo: true,
      debug: true,
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
    MatFormFieldModule,
    MatInputModule,
  ],
  providers: [
    AccountService,
    AuthGuard,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
