export interface LoginResponse {
  userId: number;
  username: string;
  role: string;
  token: string;
  expiration: string;
  requiresPasswordChange: boolean;
}
