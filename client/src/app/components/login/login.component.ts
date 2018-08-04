import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute} from '@angular/router';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseComponent {
  public Form: FormGroup;
  public ReturnUrl: string | null = null;
  constructor(
    fb: FormBuilder,
    route: ActivatedRoute,
    private _service: AccountService,
  ) {
    super();
    this.Form = fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      remember: [false],
      returnUrl: [''],
    });
    this.addSub(route.queryParams.subscribe(params => {
      if (params.returnUrl) {
        this.ReturnUrl = params.returnUrl;
        this.Form.controls['returnUrl'].setValue(this.ReturnUrl);
      }
    }),
      this.Form.controls['username'].valueChanges.subscribe(() => {
        this.Form.controls['password'].setErrors(null);
      }),
      this.Form.controls['password'].valueChanges.subscribe(() => {
        this.Form.controls['username'].setErrors(null);
      }));
  }

  public onLoginClick() {
    if (this.Form.valid) {
      this._service.login(this.Form.value).then(val => {
        if (!val) {
          this.Form.setErrors({ error: 'Ungültige Kombination aus Benutzername und Passwort'});
          this.Form.controls['username'].setErrors({ 'wrong': true});
          this.Form.controls['password'].setErrors({ 'wrong': true});
        }
      });
    } else {
      this.Form.markAsTouched();
    }
  }

  public onCancelClick() {
    this._service.cancel(this.ReturnUrl);
  }
}
