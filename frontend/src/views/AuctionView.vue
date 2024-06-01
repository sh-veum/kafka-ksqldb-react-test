<template>
  <div style="max-height: 800px">
    <h1>Auction {{ auctionId }}</h1>
    <InsertBidDialog :auctionId="auctionId" :username="username" />
    <v-container fluid>
      <v-row>
        <BidsTable :auctionId="auctionId" />
        <ChatContainer :auctionId="auctionId" />
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "@/stores/userStore";
import InsertBidDialog from "@/components/bids/InsertBidDialog.vue";
import BidsTable from "@/components/bids/BidsTable.vue";
import ChatContainer from "@/components/chat/ChatContainer.vue";

const route = useRoute();
const userStore = useUserStore();
const auctionId = route.params.auctionId as string;
const username = userStore.userInfo?.UserName ?? "anon";

onMounted(async () => {
  await userStore.fetchUserInfo();
});
</script>
