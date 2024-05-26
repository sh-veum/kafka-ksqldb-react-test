<template>
  <div>
    <v-btn @click="dialog = true" color="primary">Open Insert Auction</v-btn>

    <v-dialog v-model="dialog" max-width="500">
      <v-card>
        <v-card-title>Insert Auction</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="insertAuction">
            <v-text-field v-model="title" label="Title" required></v-text-field>
            <v-text-field
              v-model="description"
              label="Description"
              required
            ></v-text-field>
            <v-text-field
              v-model="startingPrice"
              label="Starting price"
              type="number"
              required
            ></v-text-field>
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
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn text="Close" @click="dialog = false"></v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from "vue";
import { useAuctionStore } from "@/stores/auctionStore";

const auctionStore = useAuctionStore();
const title = ref<string>("");
const description = ref<string>("");
const startingPrice = ref<number>(0);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");
const dialog = ref(false);

const insertAuction = async () => {
  if (
    title.value !== null &&
    description.value !== null &&
    startingPrice.value !== null
  ) {
    const response = await auctionStore.insertAuction({
      Title: title.value,
      Description: description.value,
      Starting_Price: startingPrice.value,
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
</script>
