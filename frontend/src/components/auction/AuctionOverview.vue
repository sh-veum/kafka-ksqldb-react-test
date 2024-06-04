<template>
  <v-row>
    <v-col cols="12" md="6" v-if="isLoading" v-for="n in 3" :key="n">
      <v-skeleton-loader
        type="heading, list-item-two-line, list-item@4, button"
      ></v-skeleton-loader>
    </v-col>
    <v-col
      v-else
      v-for="(auction, index) in sortedMessages"
      :key="index"
      cols="12"
      md="6"
    >
      <AuctionCard
        :auction="auction"
        @click="navigateToAuction(auction.Auction_Id)"
      />
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";
import router from "@/router";
import AuctionCard from "@/components/auction/AuctionCard.vue";

const auctionStore = useAuctionStore();
const isLoading = ref(true);
const errorMessage = ref<string | null>(null);
const props = defineProps<{ sortOrder: string }>();

const sortedMessages = computed(() => {
  console.log(`Sorting messages ${props.sortOrder}`);
  const messagesArray = Array.from(auctionStore.auctionMessages.values());
  return messagesArray.sort((a, b) => {
    const dateA = new Date(a.Created_At);
    const dateB = new Date(b.Created_At);
    if (props.sortOrder === "Latest") {
      return dateB.getTime() - dateA.getTime();
    } else {
      return dateA.getTime() - dateB.getTime();
    }
  });
});

const navigateToAuction = (auctionId: string) => {
  router.push(`/auction/${auctionId}`);
};

onMounted(async () => {
  const { loading, error } = await auctionStore.getAllTables();
  isLoading.value = loading;
  console.log("Error", error);
  auctionStore.connectAuctionOverviewWebSocket(onError);
});

onBeforeUnmount(() => {
  auctionStore.disconnectAuctionOverviewWebSocket();
});

function onError(err: string) {
  console.log("Error occurred");
  isLoading.value = false;
  errorMessage.value = err;
  console.error(err);
}
</script>
