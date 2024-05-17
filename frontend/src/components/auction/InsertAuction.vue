<template>
  <v-card>
    <h1>Insert Auction</h1>
    <v-form @submit.prevent="insertAuction">
      <v-text-field
        v-model="auctionId"
        label="Auction ID"
        type="number"
        required
      ></v-text-field>
      <v-text-field v-model="title" label="Title" required></v-text-field>
      <v-btn type="submit" :loading="auctionStore.loading"
        >Insert Auction</v-btn
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
    const title = ref<string | null>(null);
    const result = ref<object | null>(null);
    const formattedResult = ref<string>("");

    const insertAuction = async () => {
      if (auctionId.value !== null && title.value !== null) {
        const response = await auctionStore.insertAuction({
          auction_Id: auctionId.value,
          title: title.value,
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
      title,
      insertAuction,
      result,
      formattedResult,
    };
  },
});
</script>
