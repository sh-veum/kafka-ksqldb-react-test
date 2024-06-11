<template>
  <v-app-bar>
    <!-- Application Bar -->
    <v-app-bar-nav-icon
      variant="text"
      @click.stop="drawer = !drawer"
    ></v-app-bar-nav-icon>

    <v-toolbar-title>Auction Website</v-toolbar-title>
    <v-spacer></v-spacer>
    <div>
      <RouterLink to="/login">
        <v-btn variant="text">Login</v-btn>
      </RouterLink>
      <RouterLink to="/register">
        <v-btn variant="text">Register</v-btn>
      </RouterLink>
    </div>
  </v-app-bar>

  <!-- Navigation Drawer -->
  <v-navigation-drawer
    :width="140"
    v-model="drawer"
    :location="$vuetify.display.mobile ? 'bottom' : undefined"
  >
    <v-list>
      <v-list-item title="Navigation"></v-list-item>
      <v-divider></v-divider>
      <v-list-item>
        <RouterLink to="/">
          <v-btn variant="text">Home</v-btn>
        </RouterLink>
      </v-list-item>
      <v-list-item>
        <RouterLink to="/debug">
          <v-btn variant="text">Debug</v-btn>
        </RouterLink>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { RouterLink } from "vue-router";

const drawer = ref(true);
</script>
