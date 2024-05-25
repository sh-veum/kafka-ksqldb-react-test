<template>
  <main>
    <h1>Auction Overview</h1>
    <div>
      <InsertAuction />
    </div>
    <v-container fluid>
      <v-row>
        <v-col cols="12">
          <v-select
            v-model="sortOrder"
            :items="['Latest', 'Earliest']"
            label="Sort by"
            variant="outlined"
          ></v-select>
        </v-col>
        <v-col
          v-for="(message, index) in sortedMessages"
          :key="index"
          cols="12"
          md="4"
        >
          <v-card v-if="auctionStore.isAuctionMessage(message)">
            <v-card-title>{{ message.Title }}</v-card-title>
            <v-card-subtitle>
              Number of Bids: {{ message.Number_Of_Bids }}
            </v-card-subtitle>
            <v-card-subtitle>
              Created at: {{ message.Created_At }}
            </v-card-subtitle>
            <v-card-text>
              Starting Price: {{ message.Starting_Price }}
            </v-card-text>
            <v-card-text>
              Current Price: {{ message.Current_Price }}
            </v-card-text>
            <v-card-text> Description: {{ message.Description }} </v-card-text>
            <v-card-text> Winner: {{ message.Winner }} </v-card-text>
            <v-card-actions>
              <v-btn
                variant="outlined"
                @click="navigateToAuction(message.Auction_Id)"
              >
                Go to Auction: {{ message.Title }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </main>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";
import { WebPage } from "@/Enums/webPage";
import router from "@/router";
import InsertAuction from "@/components/auction/InsertAuction.vue";

const auctionStore = useAuctionStore();
const sortOrder = ref("Latest");

const sortedMessages = computed(() => {
  return auctionStore.messages
    .filter(auctionStore.isAuctionMessage)
    .sort((a, b) => {
      const dateA = new Date(a.Created_At);
      const dateB = new Date(b.Created_At);
      if (sortOrder.value === "Latest") {
        return dateB.getTime() - dateA.getTime();
      } else {
        return dateA.getTime() - dateB.getTime();
      }
    });
});

const navigateToAuction = (auctionId: string) => {
  router.push(`/auction/${auctionId}`);
};

onMounted(() => {
  auctionStore.connectWebSocket("none", WebPage.AuctionOverview);
});

onBeforeUnmount(() => {
  auctionStore.disconnectWebSocket();
});
</script>
