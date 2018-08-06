export interface JwtToken {
  exp: number;
  email: string;
  unique_name: string;
  nbf: number;
  iss: string;
}

export class Jwt {
  public static parse(rawToken: string): JwtToken | 'expired' {
    const base64Url = rawToken.split('.')[1];
    const base64 = base64Url.replace('-', '+').replace('_', '/');
    const token = JSON.parse(window.atob(base64)) as JwtToken;
    const utcNow = new Date().getTime();
    return utcNow < token.exp ? 'expired' : token;
  }
}
