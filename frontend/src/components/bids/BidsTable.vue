<template>
  <v-col cols="12" md="6">
    <v-data-table
      :items="sortedBids"
      :loading="bidsLoading"
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
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const bidsLoading = ref(true);
const props = defineProps<{ auctionId: string }>();

const sortedBids = computed(() => {
  return auctionStore.auctionBidsMessages.sort((a, b) => {
    return b.Bid_Amount - a.Bid_Amount;
  });
});

onMounted(async () => {
  bidsLoading.value = !(await auctionStore.getAuctionBidsForAuction(
    props.auctionId
  ));
  auctionStore.connectBidsWebSocket(props.auctionId, onBidError);
});

onBeforeUnmount(() => {
  auctionStore.disconnectBidsWebSocket();
});

function onBidError(err: string) {
  bidsLoading.value = false;
  console.error("Bid error:", err);
}
</script>
