import type { User } from "@/models/User";
import axios from "axios";
import { defineStore } from "pinia";
import { ref } from "vue";
import { useRouter } from "vue-router";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useAuthStore = defineStore("auth", () => {
  const router = useRouter();
  const token = ref<string | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);

  const login = async (user: User) => {
    loading.value = true;
    error.value = null;

    try {
      const response = await axios.post(`${baseUrl}/login`, user);
      token.value = response.data.access_token;
      axios.defaults.headers.common["Authorization"] = `Bearer ${token.value}`;
      router.push("/"); // Redirect to home or another protected route
    } catch (err) {
      error.value = "Invalid email or password";
      console.error(err);
    } finally {
      loading.value = false;
    }
  };

  const register = async (user: User) => {
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
    token.value = null;
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
