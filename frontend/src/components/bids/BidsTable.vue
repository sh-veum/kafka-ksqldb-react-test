<template>
  <v-col cols="12" md="6">
    <v-data-table
      :items="auctionStore.auctionBidsMessages"
      :loading="isLoading"
      item-key="BidId"
      :sort-by="[{ key: 'Bid_Amount', order: 'desc' }]"
    >
      <template v-slot:loading>
        <v-skeleton-loader type="table-row@10"></v-skeleton-loader>
      </template>
    </v-data-table>
  </v-col>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const isLoading = ref(true);
const props = defineProps<{ auctionId: string }>();

onMounted(async () => {
  const { loading, error } = await auctionStore.getAuctionBidsForAuction(
    props.auctionId
  );
  isLoading.value = loading;
  if (error) {
    console.error("Failed to retrieve auction bids:", error);
  } else {
    auctionStore.connectBidsWebSocket(props.auctionId, onBidError);
  }
});

onBeforeUnmount(() => {
  auctionStore.disconnectBidsWebSocket();
});

function onBidError(err: string) {
  isLoading.value = false;
  console.error("Bid error:", err);
}
</script>
