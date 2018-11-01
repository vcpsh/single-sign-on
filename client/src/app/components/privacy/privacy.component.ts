import {Component} from '@angular/core';
import {MatDialogRef} from '@angular/material';
import {Title} from '@angular/platform-browser';
import {BaseComponent} from '@vcpsh/sso-client-lib';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.scss'],
})
export class PrivacyComponent extends BaseComponent {
  private _oldTitle: string;
  constructor(
    title: Title,
    private _dialogRef: MatDialogRef<PrivacyComponent>,
  ) {
    super();
    this._oldTitle = title.getTitle();
    title.setTitle('Datenschutzerklärung - vcp.sh');
    console.log('hi from dialog');
    this._dialogRef.afterClosed(() => title.setTitle(this._oldTitle));
  }

}
