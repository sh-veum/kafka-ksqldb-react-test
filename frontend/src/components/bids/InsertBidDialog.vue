<template>
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
          <v-form @submit.prevent="insertAuctionBid">
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
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text="Close" @click="isActive.value = false"></v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const dialog = ref(false);
const bidAmount = ref<number | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");

const props = defineProps<{ auctionId: string; username: string }>();

const insertAuctionBid = async () => {
  if (
    props.auctionId !== null &&
    props.username !== null &&
    bidAmount.value !== null
  ) {
    const response = await auctionStore.insertAuctionBid({
      Auction_Id: props.auctionId,
      Username: props.username,
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
