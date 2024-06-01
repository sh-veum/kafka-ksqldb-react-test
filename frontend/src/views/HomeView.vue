<template>
  <main>
    <v-container fluid>
      <h1>Auction Overview</h1>
      <div>
        <InsertAuction />
      </div>
      <div class="mt-4">
        <v-select
          v-model="sortOrder"
          :items="['Latest', 'Earliest']"
          label="Sort by"
          variant="outlined"
        ></v-select>
      </div>
      <v-row>
        <v-col cols="12" md="4" v-if="loading" v-for="n in 3" :key="n">
          <v-skeleton-loader
            type="heading, list-item-two-line, list-item@4, button"
          ></v-skeleton-loader>
        </v-col>
        <v-col
          v-else
          v-for="(auction, index) in sortedMessages"
          :key="index"
          cols="12"
          md="4"
        >
          <v-card>
            <v-card-title>{{ auction.Title }}</v-card-title>
            <v-card-subtitle>
              Number of Bids: {{ auction.Number_Of_Bids }}
            </v-card-subtitle>
            <v-card-subtitle>
              Created at: {{ auction.Created_At }}
            </v-card-subtitle>
            <v-card-text>
              Starting Price: {{ auction.Starting_Price }}
            </v-card-text>
            <v-card-text>
              Current Price: {{ auction.Current_Price }}
            </v-card-text>
            <v-card-text> Description: {{ auction.Description }} </v-card-text>
            <v-card-text> Winner: {{ auction.Winner }} </v-card-text>
            <v-card-actions>
              <v-btn
                variant="outlined"
                @click="navigateToAuction(auction.Auction_Id)"
              >
                Go to Auction: {{ auction.Title }}
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
import router from "@/router";
import InsertAuction from "@/components/debugComponents/InsertAuction.vue";

const auctionStore = useAuctionStore();
const sortOrder = ref("Latest");
const loading = ref(true);
const error = ref<string | null>(null);

const sortedMessages = computed(() => {
  return auctionStore.auctionMessages.sort((a, b) => {
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

onMounted(async () => {
  await auctionStore.getAllTables();
  loading.value = false;
  auctionStore.connectAuctionOverviewWebSocket(onError);
});

onBeforeUnmount(() => {
  auctionStore.disconnectAuctionOverviewWebSocket();
});

function onError(err: string) {
  console.log("Error occured");
  loading.value = false;
  error.value = err;
}
</script>
