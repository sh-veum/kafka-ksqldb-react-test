<template>
  <v-responsive class="mx-auto" max-width="344">
    <h1>Login</h1>
    <v-form @submit.prevent="submit">
      <div>
        <v-text-field
          clearable
          type="email"
          v-model="email"
          id="email"
          label="Email"
          hint="Enter your email address"
          :rules="[rules.required]"
        />
      </div>
      <div>
        <v-text-field
          clearable
          type="password"
          v-model="password"
          id="password"
          label="Password"
          hint="Enter your password"
          :rules="[rules.required]"
        />
      </div>
      <v-btn type="submit" :disabled="authStore.loading">Login</v-btn>
      <p v-if="authStore.error">{{ authStore.error }}</p>
    </v-form>
  </v-responsive>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useAuthStore } from "@/stores/authStore";

const email = ref("");
const password = ref("");
const authStore = useAuthStore();

const submit = async () => {
  await authStore.login({
    email: email.value,
    password: password.value,
  });
};

const rules = {
  required: (value: string) => !!value || "Required.",
};
</script>
