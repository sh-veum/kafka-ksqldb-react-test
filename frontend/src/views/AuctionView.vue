<template>
  <div style="max-height: 800px; width: 80%" class="my-4 mx-4">
    <div>
      <div>
        <v-skeleton-loader
          v-if="isLoading"
          type="list-item"
        ></v-skeleton-loader>
        <div v-else-if="errorMessage">
          <h1 type="error">{{ errorMessage }}</h1>
        </div>
        <div v-else>
          <h1>Auction {{ currentAuction?.Title }}</h1>
        </div>
      </div>
      <InsertBidDialog :auctionId="auctionId" :username="username" />
      <v-container fluid>
        <v-row>
          <BidsTable :auctionId="auctionId" />
          <ChatContainer :auctionId="auctionId" />
        </v-row>
      </v-container>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "@/stores/userStore";
import InsertBidDialog from "@/components/bids/InsertBidDialog.vue";
import BidsTable from "@/components/bids/BidsTable.vue";
import ChatContainer from "@/components/chat/ChatContainer.vue";
import { useAuctionStore } from "@/stores/auctionStore";
import type { AuctionMessage } from "@/models/AuctionMessage";

const route = useRoute();
const userStore = useUserStore();
const auctionStore = useAuctionStore();
const auctionId = route.params.auctionId as string;
const username = userStore.userInfo?.UserName ?? "anon";
const currentAuction = ref<AuctionMessage | null>(null);
const isLoading = ref(true);
const errorMessage = ref<string | null>(null);

onMounted(async () => {
  await userStore.fetchUserInfo();
  const { data, loading, error } = await auctionStore.getAuctionById(auctionId);
  currentAuction.value = data;
  isLoading.value = loading;
  errorMessage.value = error;
});
</script>
