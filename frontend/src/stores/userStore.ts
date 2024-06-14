import type { UserInfo } from "@/models/UserInfo";
import axios from "axios";
import { defineStore } from "pinia";
import { ref, watch } from "vue";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useUserStore = defineStore("user", () => {
  const storedUserInfo = localStorage.getItem("userInfo");
  const userInfo = ref<UserInfo | null>(
    storedUserInfo ? JSON.parse(storedUserInfo) : null
  );

  const fetchUserInfo = async () => {
    try {
      const response = await axios.get<UserInfo>(
        `${baseUrl}/api/user/userinfo`
      );
      userInfo.value = response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 401) {
        userInfo.value = { Email: "", UserName: "anon", Role: "" };
      } else {
        console.error("Error fetching user info", error);
      }
    }
  };

  // Watch for changes in userInfo and update localStorage
  watch(userInfo, (newValue) => {
    if (newValue) {
      localStorage.setItem("userInfo", JSON.stringify(newValue));
    } else {
      localStorage.removeItem("userInfo");
    }
  });

  return {
    userInfo,
    fetchUserInfo,
  };
});
