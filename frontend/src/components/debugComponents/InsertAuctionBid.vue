<template>
  <v-card>
    <h1>Insert Auction Bid</h1>
    <v-form @submit.prevent="insertAuctionBid">
      <v-text-field
        v-model="auctionId"
        label="Auction Id"
        type="string"
        required
      ></v-text-field>
      <v-text-field v-model="username" label="Username" required></v-text-field>
      <v-text-field
        v-model="bidAmount"
        label="Bid Amount"
        type="number"
        required
      ></v-text-field>
      <v-btn type="submit" :loading="isLoading">Insert Auction Bid</v-btn>
    </v-form>
    <v-textarea
      label="Error"
      v-if="errorMessage"
      v-model="errorMessage"
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
const auctionId = ref<string | null>(null);
const username = ref<string | null>(null);
const bidAmount = ref<number | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");
const isLoading = ref(false);
const errorMessage = ref<string | null>(null);

const insertAuctionBid = async () => {
  if (auctionId.value && username.value && bidAmount.value) {
    const { data, loading, error } = await auctionStore.insertAuctionBid({
      Auction_Id: auctionId.value,
      Username: username.value,
      Bid_Amount: bidAmount.value,
    });
    result.value = data;
    isLoading.value = loading;
    errorMessage.value = error;
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
