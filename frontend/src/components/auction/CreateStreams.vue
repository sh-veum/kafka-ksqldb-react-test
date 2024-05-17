<template>
  <v-card>
    <h1>Create Streams</h1>
    <v-btn @click="createStreams" :loading="auctionStore.loading"
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

    const createStreams = async () => {
      result.value = await auctionStore.createStreams();
    };

    return {
      auctionStore,
      createStreams,
      result,
    };
  },
});
</script>
