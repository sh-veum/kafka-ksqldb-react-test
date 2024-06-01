import type { UserDto } from "@/models/UserDto";
import axios from "axios";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useRouter } from "vue-router";

const baseUrl = `${import.meta.env.VITE_API_URL}`;
const TOKEN_KEY = "authToken";

export const useAuthStore = defineStore("auth", () => {
  const router = useRouter();
  const token = ref<string>(localStorage.getItem(TOKEN_KEY) || "");
  const loading = ref(false);
  const error = ref<string | null>(null);

  const login = async (user: UserDto) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await axios.post(`${baseUrl}/login`, user);
      token.value = response.data.accessToken;
      localStorage.setItem(TOKEN_KEY, token.value);
      axios.defaults.headers.common["Authorization"] = `Bearer ${token.value}`;
      router.push("/"); // Redirect to home or another protected route
    } catch (err) {
      error.value = "Invalid email or password";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const register = async (user: UserDto) => {
    loading.value = true;
    error.value = null;

    try {
      await axios.post(`${baseUrl}/register`, user);
      await login(user); // Automatically log in after registration
    } catch (err) {
      error.value = "Registration failed";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const logout = () => {
    token.value = "";
    delete axios.defaults.headers.common["Authorization"];
    router.push("/login"); // Redirect to login
  };

  return {
    token,
    loading,
    error,
    login,
    register,
    logout,
  };
});
