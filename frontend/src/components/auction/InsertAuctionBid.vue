<template>
  <v-card>
    <h1>Insert Auction Bid</h1>
    <v-form @submit.prevent="insertAuctionBid">
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

<script setup lang="ts">
import { ref, watch } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const username = ref<string | null>(null);
const bidAmount = ref<number | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");

const props = defineProps<{ auctionId: string }>();

const insertAuctionBid = async () => {
  if (
    props.auctionId !== null &&
    username.value !== null &&
    bidAmount.value !== null
  ) {
    const response = await auctionStore.insertAuctionBid({
      Auction_Id: props.auctionId,
      Username: username.value,
      Bid_Amount: bidAmount.value,
    });
    console.log(response);
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
</script>
