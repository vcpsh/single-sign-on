import { InjectionToken } from '@angular/core';
import {SsoConfig} from './config.model';

export const SsoConfigToken = new InjectionToken<SsoConfig>('SSO_CONFIG');
