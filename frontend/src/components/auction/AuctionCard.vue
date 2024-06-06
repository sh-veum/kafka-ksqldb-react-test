<template>
  <v-card @click="navigateToAuction(props.auction.Auction_Id)">
    <v-card-title>{{ props.auction.Title }}</v-card-title>
    <!-- <v-card-subtitle
      >Number of Bids: {{ props.auction.Number_Of_Bids }}</v-card-subtitle
    > -->
    <v-card-subtitle
      >Created at: {{ props.auction.Created_At }}</v-card-subtitle
    >
    <!-- <v-card-text>Starting Price: {{ props.auction.Starting_Price }}</v-card-text> -->
    <v-card-text
      >Current Price: {{ props.auction.Current_Price }} NOK
    </v-card-text>
    <div class="ml-4" style="margin-top: -12px">
      <v-card-subtitle>{{ props.auction.Number_Of_Bids }} bids</v-card-subtitle>
    </div>
    <v-card-text>Description: </v-card-text>
    <div class="ml-4" style="margin-top: -12px; padding-bottom: 12px">
      <v-card-subtitle>{{ props.auction.Description }}</v-card-subtitle>
    </div>
    <!-- <v-card-text>Winner: {{ props.auction.Winner }}</v-card-text> -->
    <!-- <v-card-actions>
      <v-btn
        variant="outlined"
        @click="navigateToAuction(props.auction.Auction_Id)"
      >
        Go to Auction: {{ props.auction.Title }}
      </v-btn>
    </v-card-actions> -->
  </v-card>
</template>

<script setup lang="ts">
import { defineProps } from "vue";
import router from "@/router";

const props = defineProps<{ auction: any }>();

const navigateToAuction = (auctionId: string) => {
  router.push(`/auction/${auctionId}`);
};
</script>
