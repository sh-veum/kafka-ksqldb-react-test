import { QueryClient } from "@tanstack/react-query";

// Auth type definition
export type Auth = {
  login: (username: string, accessToken: string, refreshToken: string) => void;
  logout: () => void;
  status: "loggedOut" | "loggedIn";
  username?: string;
};

let queryClient: QueryClient;

export const setQueryClient = (client: QueryClient) => {
  queryClient = client;
};

export const auth: Auth = {
  status: "loggedOut",
  username: undefined,
  login: (username: string, accessToken: string, refreshToken: string) => {
    queryClient.setQueryData(
      ["auth", "loginInfo"],
      (old: Auth | undefined) => ({
        ...old,
        status: "loggedIn",
        username: username,
      })
    );
    localStorage.setItem("username", username);
    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    // console.log("logged in", username, accessToken, refreshToken);
  },
  logout: () => {
    console.log("logging out");
    queryClient.setQueryData(
      ["auth", "loginInfo"],
      (old: Auth | undefined) => ({
        ...old,
        status: "loggedOut",
        username: undefined,
      })
    );
    localStorage.removeItem("username");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    // console.log(
    //   "logged out",
    //   localStorage.getItem("username"),
    //   localStorage.getItem("accessToken"),
    //   localStorage.getItem("refreshToken")
    // );
  },
};
