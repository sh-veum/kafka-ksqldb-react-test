<template>
  <v-card>
    <h1>Create Tables</h1>
    <v-btn @click="createTables" :loading="auctionStore.loading"
      >Create Tables</v-btn
    >
    <p v-if="auctionStore.error">{{ auctionStore.error }}</p>
    <p v-if="result">{{ result }}</p>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

export default defineComponent({
  setup() {
    const auctionStore = useAuctionStore();
    const result = ref<string | null>(null);

    const createTables = async () => {
      result.value = await auctionStore.createTables();
    };

    return {
      auctionStore,
      createTables,
      result,
    };
  },
});
</script>
