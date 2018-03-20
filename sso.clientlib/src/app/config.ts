import { InjectionToken } from "@angular/core";
import { UserManagerSettings } from "oidc-client";

export const SsoConfigToken = new InjectionToken<UserManagerSettings>(
  "SSO_CONFIG"
);
