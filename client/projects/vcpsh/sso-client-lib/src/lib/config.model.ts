export interface DefaultConfig {
  authority: string;
  client_id: string;
  response_type: string;
  /** Route to switch to after the user was unloaded. */
  route_after_user_unloaded: string;
  scope: string;
  loadUserInfo: boolean;
  debug?: boolean;
}

export interface InternalConfig {
  redirect_uri: string;
  post_logout_redirect_uri: string;
  silent_redirect_uri: string;
}

export interface AutomaticRenew {
  automaticSilentRenew: true;
}

export type SsoConfig = DefaultConfig | DefaultConfig & AutomaticRenew;
export type InternalSsoConfig = DefaultConfig & InternalConfig | DefaultConfig & InternalConfig & AutomaticRenew;
