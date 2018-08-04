export interface DefaultConfig {
  authority: string;
  client_id: string;
  redirect_uri: string;
  response_type: string;
  scope: string;
  post_logout_redirect_uri: string;
  loadUserInfo: boolean;
  debug?: boolean;
}

export interface AutomaticRenew {
  automaticSilentRenew: true;
  silent_redirect_uri: string;
}


export type SsoConfig = DefaultConfig | DefaultConfig & AutomaticRenew;
