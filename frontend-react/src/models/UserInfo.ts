export interface UserInfo {
  status: "loggedOut" | "loggedIn";
  username: string;
  role: string;
}
