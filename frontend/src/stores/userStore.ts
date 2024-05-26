import { defineStore } from "pinia";
import axios from "axios";
import { ref } from "vue";
import type { UserInfo } from "@/models/UserInfo";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useUserStore = defineStore("user", () => {
  const userInfo = ref<UserInfo | null>(null);

  const fetchUserInfo = async () => {
    try {
      const response = await axios.get<UserInfo>(
        `${baseUrl}/api/user/userinfo`
      );
      userInfo.value = response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        userInfo.value = { email: "", userName: "anon", role: "" };
      } else {
        console.error("Error fetching user info", error);
      }
    }
  };

  return {
    userInfo,
    fetchUserInfo,
  };
});
