<template>
  <header>
    <Navigation />
  </header>

  <div class="ma-2">
    <RouterView />
  </div>
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import Navigation from "@/components/Navigation.vue";
</script>
