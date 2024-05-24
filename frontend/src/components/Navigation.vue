<template>
  <div class="wrapper">
    <nav class="nav-container">
      <div class="nav-left">
        <RouterLink to="/">
          <v-btn variant="tonal">Home</v-btn>
        </RouterLink>
        <RouterLink to="/debug">
          <v-btn variant="tonal">Debug</v-btn>
        </RouterLink>
      </div>
      <div class="nav-right">
        <RouterLink to="/login">
          <v-btn variant="tonal">Login</v-btn>
        </RouterLink>
        <RouterLink to="/register">
          <v-btn variant="tonal">Register</v-btn>
        </RouterLink>
      </div>
    </nav>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from "vue-router";
</script>

<style scoped>
.nav-container {
  display: flex;
  justify-content: space-between;
}
</style>
