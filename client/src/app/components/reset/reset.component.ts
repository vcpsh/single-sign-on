import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';
import {PasswordValidator} from '../../util/password-validator';

@Component({
  selector: 'app-reset',
  templateUrl: './reset.component.html',
  styleUrls: ['./reset.component.scss']
})
export class ResetComponent extends BaseComponent {
  public Form: FormGroup;
  public TokenValid = false;
  public Success = false;

  constructor(
    fb: FormBuilder,
    private _service: AccountService,
    route: ActivatedRoute,
  ) {
    super();
    this.Form = fb.group({
      token: [''],
      password: ['', Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/)],
      confirmPassword: ['']
    }, {
      validator: PasswordValidator.matchPassword
    });

    this.addSub(route.queryParams.subscribe(params => {
      if (params.token) {
        const base64Url = params.token.split('.')[1];
        const base64 = base64Url.replace('-', '+').replace('_', '/');
        const token = JSON.parse(window.atob(base64));
        const utcNow = new Date().getTime();
        if (utcNow > token.exp) {
          this.Form.controls['token'].setValue(params.token);
          this.TokenValid = true;
        }
      }
    }));
  }

  public onResetClick() {
    if (this.Form.valid) {
      this._service.reset(this.Form.value)
        .then(res => {
          if (!res) {
            this.TokenValid = false;
          } else {
            this.Success = true;
          }
        });
    } else {
      this.Form.markAsTouched();
    }
  }
}
