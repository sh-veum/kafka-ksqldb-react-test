<template>
  <!-- Navigation Drawer -->
  <v-navigation-drawer>
    <v-list>
      <v-list-item title="Navigation drawer"></v-list-item>
      <v-list-item>
        <RouterLink to="/">
          <v-btn variant="tonal">Home</v-btn>
        </RouterLink>
      </v-list-item>
      <v-list-item>
        <RouterLink to="/debug">
          <v-btn variant="tonal">Debug</v-btn>
        </RouterLink>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>

  <!-- Application Bar -->
  <v-app-bar>
    <v-toolbar-title>Auction Website</v-toolbar-title>
    <v-spacer></v-spacer>
    <div>
      <RouterLink to="/login">
        <v-btn variant="tonal">Login</v-btn>
      </RouterLink>
      <RouterLink to="/register">
        <v-btn variant="tonal">Register</v-btn>
      </RouterLink>
    </div>
  </v-app-bar>
</template>

<script setup lang="ts">
import { RouterLink } from "vue-router";
</script>
