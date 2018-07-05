import { InjectionToken } from "@angular/core";
import { UserManagerSettings } from "oidc-client";
import {ModuleConfig} from './models/config.model';

export const SsoConfigToken = new InjectionToken<ModuleConfig>(
  "SSO_CONFIG"
);
