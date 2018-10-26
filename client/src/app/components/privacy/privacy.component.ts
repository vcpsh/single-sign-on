import {Component} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {BaseComponent} from '@vcpsh/sso-client-lib';

@Component({
  selector: 'app-privacy',
  templateUrl: './privacy.component.html',
  styleUrls: ['./privacy.component.scss'],
})
export class PrivacyComponent extends BaseComponent {
  constructor(
    title: Title,
  ) {
    super();
    title.setTitle('Datenschutzerklärung - vcp.sh');
  }

}
