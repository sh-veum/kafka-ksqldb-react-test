<template>
  <v-layout class="rounded rounded-md">
    <header>
      <Navigation />
    </header>

    <v-main
      class="d-flex align-center justify-center"
      style="min-height: 300px"
    >
      <RouterView />
    </v-main>
  </v-layout>
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import Navigation from "@/components/Navigation.vue";
</script>
