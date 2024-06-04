<template>
  <v-card>
    <h1>Create Tables</h1>
    <v-btn @click="createTables" :loading="isLoading">Create Tables</v-btn>
    <p v-if="errorMessage">{{ errorMessage }}</p>
    <p v-if="result">{{ result }}</p>
  </v-card>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const result = ref<string>();
const isLoading = ref(false);
const errorMessage = ref<string | null>();

const createTables = async () => {
  const { data, loading, error } = await auctionStore.createTables();
  result.value = data;
  isLoading.value = loading;
  errorMessage.value = error;
};
</script>
