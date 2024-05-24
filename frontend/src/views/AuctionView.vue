<template>
  <div>
    <h1>Auction {{ auctionId }}</h1>
    <v-dialog v-model="dialog" max-width="500">
      <template v-slot:activator="{ props: activatorProps }">
        <v-btn
          v-bind="activatorProps"
          color="surface-variant"
          text="Place Bid"
          variant="flat"
        ></v-btn>
      </template>

      <template v-slot:default="{ isActive }">
        <v-card>
          <v-card-title>Place Your Bid</v-card-title>
          <v-card-text>
            <InsertAuctionBid :auctionId="auctionId" />
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn text="Close" @click="isActive.value = false"></v-btn>
          </v-card-actions>
        </v-card>
      </template>
    </v-dialog>
    <v-list lines="one">
      <v-list-item
        v-for="(message, index) in auctionStore.messages"
        :key="index"
      >
        <div v-if="auctionStore.isAuctionBidsMessage(message)">
          {{ message.Timestamp }}: {{ message.Username }} with
          {{ message.Bid_Amount }}
        </div>
      </v-list-item>
    </v-list>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref } from "vue";
import { useRoute } from "vue-router";
import { useAuctionStore } from "@/stores/auctionStore";
import { WebPage } from "@/Enums/webPage";
import InsertAuctionBid from "@/components/auction/InsertAuctionBid.vue";

const route = useRoute();
const auctionStore = useAuctionStore();
const auctionId = route.params.auctionId as string;
const dialog = ref(false);

onMounted(() => {
  auctionStore.connectWebSocket(auctionId, WebPage.SpesificAuction);
});

onBeforeUnmount(() => {
  auctionStore.disconnectWebSocket();
});
</script>
