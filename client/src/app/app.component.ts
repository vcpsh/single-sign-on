import {Component} from '@angular/core';
import {BaseComponent } from '@vcpsh/sso-client-lib';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends BaseComponent {
  constructor() {
    super();
    const pendingRedirect = localStorage.getItem('pendingRedirect');
    localStorage.removeItem('pendingRedirect');
    if (pendingRedirect !== null) {
      window.location.href = pendingRedirect;
    }
  }

}
