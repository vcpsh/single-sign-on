export interface DefaultConfig {
  authority: string;
  client_id: string;
  response_type: string;
  scope: string;
  loadUserInfo: boolean;
  debug?: boolean;
}

export interface NonFactoryConfig {
  redirect_uri: string;
  post_logout_redirect_uri: string;
}

export interface FactoryConfig {
  post_logout_redirect_uri: string | (() => string);
  redirect_uri: string | (() => string);
}

export interface AutomaticRenew {
  automaticSilentRenew: true;
  silent_redirect_uri?: string;
}

export interface AutomaticRenewFactory {
  silent_redirect_uri?: string | (() => string);
  automaticSilentRenew: true;
}


export type SsoConfig = DefaultConfig & FactoryConfig & AutomaticRenewFactory;
export type InternalSsoConfig = DefaultConfig & NonFactoryConfig | DefaultConfig & NonFactoryConfig & AutomaticRenew;
