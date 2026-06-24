export { SignIn } from './feature-sign-in/sign-in';
export { SignUp } from './feature-sign-up/sign-up';

// Exposed so other domains (route guards in core/) can check session
// state and role without reaching into auth's internals.
export { AuthService } from './data-access/authentication';
export type { RegisterPayload, RegisterResponse } from './data-access/authentication';
