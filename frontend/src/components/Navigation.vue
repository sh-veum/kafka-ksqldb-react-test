<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { RouterLink } from "vue-router";

const auctionId = ref("");
const router = useRouter();

const navigateToAuction = () => {
  if (auctionId.value) {
    router.push(`/auction/${auctionId.value}`);
    auctionId.value = "";
  }
};
</script>

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
        <v-btn variant="tonal" @click="navigateToAuction">Auction</v-btn>
        <v-text-field
          v-model="auctionId"
          label="Auction ID"
          placeholder="Enter Auction ID"
          @keyup.enter="navigateToAuction"
        ></v-text-field>
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

<style scoped>
.nav-container {
  display: flex;
  justify-content: space-between;
}
</style>
