<template>
  <v-col cols="12" md="6" class="d-flex flex-column">
    <div v-if="loading">
      <v-skeleton-loader type="list-item-two-line@8"></v-skeleton-loader>
    </div>
    <div v-else-if="error">
      <v-alert type="error">{{ error }}</v-alert>
    </div>
    <v-container
      id="scroll-target"
      class="overflow-y-auto"
      style="max-height: 640px"
    >
      <template
        v-for="bidMessage in recentBids"
        v-scroll:#scroll-target="onScroll"
        :key="bidMessage.Timestamp"
      >
        <v-textarea
          :label="formatTimestamp(bidMessage.Timestamp)"
          :model-value="formatBidMessage(bidMessage)"
          rows="1"
          variant="filled"
          auto-grow
          counter
          readonly
          hover
        >
        </v-textarea>
      </template>
    </v-container>
  </v-col>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";
import type { RecentBidMessage } from "@/models/RecentBidMessage";

const auctionStore = useAuctionStore();
const loading = ref(true);
const error = ref<string | null>(null);
const recentBids = ref(auctionStore.allRecentBidsMessages);

onMounted(() => {
  auctionStore.connectAllRecentBidsWebSocket(onError);
  loading.value = false;
});

onBeforeUnmount(() => {
  auctionStore.disconnectAllRecentBidsWebSocket();
});

function onError(err: string) {
  loading.value = false;
  error.value = err;
}

function formatTimestamp(timestamp: string): string {
  const messageDate = new Date(timestamp);
  const today = new Date();
  const isToday = messageDate.toDateString() === today.toDateString();
  const formattedTime = messageDate.toTimeString().split(" ")[0];
  if (isToday) {
    return `Today at ${formattedTime}`;
  } else {
    const formattedDate = messageDate.toLocaleDateString("en-GB");
    return `${formattedDate} ${formattedTime}`;
  }
}

function formatBidMessage(bidMessage: RecentBidMessage): string {
  return `${bidMessage.Username} bid ${bidMessage.Bid_Amount} on ${bidMessage.Title}!`;
}

function onScroll() {
  // TODO
}
</script>
