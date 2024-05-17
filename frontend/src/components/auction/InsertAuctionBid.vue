<template>
  <v-card>
    <h1>Insert Auction Bid</h1>
    <v-form @submit.prevent="insertAuctionBid">
      <v-text-field
        v-model="auctionId"
        label="Auction ID"
        type="number"
        required
      ></v-text-field>
      <v-text-field v-model="username" label="Username" required></v-text-field>
      <v-text-field
        v-model="bidAmount"
        label="Bid Amount"
        type="number"
        required
      ></v-text-field>
      <v-btn type="submit" :loading="auctionStore.loading"
        >Insert Auction Bid</v-btn
      >
    </v-form>
    <v-textarea
      label="Error"
      v-if="auctionStore.error"
      v-model="auctionStore.error"
      readonly
    ></v-textarea>
    <v-textarea
      label="Result"
      v-if="result"
      v-model="formattedResult"
      readonly
      auto-grow
    ></v-textarea>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, ref, watch } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

export default defineComponent({
  setup() {
    const auctionStore = useAuctionStore();
    const auctionId = ref<number | null>(null);
    const username = ref<string | null>(null);
    const bidAmount = ref<number | null>(null);
    const result = ref<object | null>(null);
    const formattedResult = ref<string>("");

    const insertAuctionBid = async () => {
      if (
        auctionId.value !== null &&
        username.value !== null &&
        bidAmount.value !== null
      ) {
        const response = await auctionStore.insertAuctionBid({
          auction_Id: auctionId.value,
          username: username.value,
          bid_Amount: bidAmount.value,
        });
        result.value = response;
      }
    };

    watch(result, (newResult) => {
      if (newResult) {
        formattedResult.value = JSON.stringify(newResult, null, 2);
      } else {
        formattedResult.value = "";
      }
    });

    return {
      auctionStore,
      auctionId,
      username,
      bidAmount,
      insertAuctionBid,
      result,
      formattedResult,
    };
  },
});
</script>
