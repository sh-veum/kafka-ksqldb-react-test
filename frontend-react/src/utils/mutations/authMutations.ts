import { auth } from "@/lib/auth";
import { baseUrl } from "@/lib/baseUrls";
import { LoginInfo } from "@/models/LoginInfo";
import { LoginResponse } from "@/models/LoginResponse";
import { RegisterInfo } from "@/models/RegisterInfo";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { toast } from "sonner";

export const useLoginMutation = () => {
  const toastId = "loginToast";

  const mutation = useMutation<LoginResponse, Error, LoginInfo, unknown>({
    mutationFn: async (loginInfo: LoginInfo): Promise<LoginResponse> => {
      const response = await axios.post(`${baseUrl}/login`, loginInfo);

      console.log("login response", response);

      if (response.status !== 200) {
        throw new Error("Login failed");
      }

      auth.login(response.data.accessToken, response.data.refreshToken);

      const userInfo = await axios.get(`${baseUrl}/api/user/userinfo`);

      auth.login(
        response.data.accessToken,
        response.data.refreshToken,
        userInfo.data.UserName,
        userInfo.data.Role
      );

      return response.data;
    },
    onSuccess: () => {
      toast.success("Logged in!", { id: toastId });
    },
    onError: (error) => {
      toast.error(`Login failed!`, {
        id: toastId,
        description: error.message,
      });
    },
  });

  return mutation;
};

export const useRegisterMutation = () => {
  const toastId = "registerToast";

  const mutation = useMutation<void, Error, RegisterInfo, unknown>({
    mutationFn: async (registerInfo: RegisterInfo) => {
      const response = await axios.post(
        `${baseUrl}/api/user/register`,
        registerInfo
      );

      console.log("register response", response);

      if (response.status !== 200) {
        throw new Error("Register failed");
      }
    },
    onSuccess: () => {
      toast.success("Registered!", { id: toastId });
    },
    onError: (error) => {
      toast.error(`Register failed!`, {
        id: toastId,
        description: error.message,
      });
    },
  });

  return mutation;
};
