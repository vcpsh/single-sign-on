import {AbstractControl} from '@angular/forms';

export class PasswordValidator {
  static matchPassword(AC: AbstractControl) {
    const password = AC.get('password').value;
    const confirmPassword = AC.get('confirmPassword').value;

    if (password !== confirmPassword) {
      AC.get('confirmPassword').setErrors( { matchPassword: true });
    }
  }
}
