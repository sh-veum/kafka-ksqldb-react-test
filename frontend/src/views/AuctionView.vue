<template>
  <div>
    <h1>Auction {{ auctionId }}</h1>
    <ul>
      <li v-for="message in auctionStore.messages" :key="message.id">
        {{ message }}
      </li>
    </ul>
  </div>
</template>

<script lang="ts">
import { defineComponent, onMounted, onBeforeUnmount } from "vue";
import { useRoute } from "vue-router";
import { useAuctionStore } from "@/stores/auctionStore";

export default defineComponent({
  setup() {
    const route = useRoute();
    const auctionStore = useAuctionStore();
    const auctionId = route.params.auctionId as string;

    onMounted(() => {
      auctionStore.connectWebSocket(auctionId);
    });

    onBeforeUnmount(() => {
      auctionStore.disconnectWebSocket();
    });

    return { auctionId, auctionStore };
  },
});
</script>
