import {User} from 'oidc-client';

export interface ProfileModel {
  amr: string[];
  auth_time: number;
  idp: string;
  name: string;
  preferred_username: string;
  sid: string;
  /**
   * Subject (equivalent to the userid/vcpid)
   */
  sub: string;
  division_member?: string | string[];
  is_division_lgs?: string | string[];
  tribe_member?: string | string[];
}

export interface UserModel extends User {
  profile: ProfileModel;
}
