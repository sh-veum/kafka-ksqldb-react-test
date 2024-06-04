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
        <v-col cols="12" md="8">
          <AuctionOverview :sortOrder="sortOrder" />
        </v-col>
        <v-col cols="12" md="4">
          <h2>Recent Bids</h2>
          <RecentBidsContainer />
        </v-col>
      </v-row>
    </v-container>
  </main>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import InsertAuction from "@/components/debugComponents/InsertAuction.vue";
import RecentBidsContainer from "@/components/bids/RecentBidsContainer.vue";
import AuctionOverview from "@/components/auction/AuctionOverview.vue";

const sortOrder = ref("Latest");

watch(sortOrder, (newOrder) => {
  sortOrder.value = newOrder;
});
</script>
