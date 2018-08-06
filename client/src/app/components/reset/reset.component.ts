import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';
import {Jwt} from '../../util/jwt';
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
        const token = Jwt.parse(params.token);
        if (token === 'expired') {
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
