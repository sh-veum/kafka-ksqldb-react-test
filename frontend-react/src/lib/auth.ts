import { QueryClient } from "@tanstack/react-query";
import axios from "axios";

// Auth type definition
export type Auth = {
  login: (
    accessToken: string,
    refreshToken: string,
    username?: string,
    role?: string
  ) => void;
  logout: () => void;
  status: "loggedOut" | "loggedIn";
  username?: string;
  role?: string;
};

let queryClient: QueryClient;

export const setQueryClient = (client: QueryClient) => {
  queryClient = client;
};

export const auth: Auth = {
  status: "loggedOut",
  username: undefined,
  role: undefined,
  login: (
    accessToken: string,
    refreshToken: string,
    username?: string,
    role?: string
  ) => {
    queryClient.setQueryData(
      ["auth", "loginInfo"],
      (old: Auth | undefined) => ({
        ...old,
        status: "loggedIn",
        username: username,
        role: role,
      })
    );
    if (username) {
      localStorage.setItem("username", username);
    }

    if (role) {
      localStorage.setItem("role", role);
    }

    localStorage.setItem("accessToken", accessToken);
    localStorage.setItem("refreshToken", refreshToken);
    console.log("logged in", username, role);
  },
  logout: () => {
    console.log("logging out");
    queryClient.setQueryData(
      ["auth", "loginInfo"],
      (old: Auth | undefined) => ({
        ...old,
        status: "loggedOut",
        username: undefined,
        role: undefined,
      })
    );
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    delete axios.defaults.headers.common["Authorization"];
  },
};
