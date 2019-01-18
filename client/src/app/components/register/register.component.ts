import {Component} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Title} from '@angular/platform-browser';
import {BaseComponent} from '@vcpsh/sso-client-lib';
import {AccountService} from '../../services/account.service';
import {PasswordValidator} from '../../util/password-validator';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent extends BaseComponent {
  public Form: FormGroup;
  public Success = false;
  constructor(
    fb: FormBuilder,
    title: Title,
    private _service: AccountService,
  ) {
    super();
    title.setTitle('Register - vcp.sh');
    this.Form = fb.group({
      id: ['', [Validators.required, Validators.pattern(/^[0-9]*$/)]],
      username: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9]*$/)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/)]],
      confirmPassword: ['', [Validators.required]],
    }, {
      validator: PasswordValidator.matchPassword
    });
  }

  public onRegisterClick() {
    if (this.Form.valid) {
      this._service.register(this.Form.value)
        .then((res: string | boolean) => {
          // this.Form.controls['password'].setValue('');
          // this.Form.controls['confirmPassword'].setValue('');
          switch (res) {
            case 'Unknown VCP-ID':
              this.Form.controls['id'].setErrors({ invalidId: true});
              break;
            case 'Email used by another account':
              this.Form.controls['email'].setErrors({ used: true});
              break;
            case 'Username used by another account':
              this.Form.controls['username'].setErrors({ used: true});
              break;
            case 'Account exists':
              this.Form.controls['id'].setErrors({ used: true});
              break;
            case true:
              this.Success = true;
              break;
            default:
              console.error(res);
              break;
          }
        })
        .catch(err => {
          console.error(err);
        });
    } else {
      this.Form.markAsTouched();
    }
  }
}
