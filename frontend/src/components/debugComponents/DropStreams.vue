<template>
  <v-card>
    <h1>Drop Stream</h1>
    <v-form @submit.prevent="dropStream">
      <v-text-field
        v-model="streamName"
        label="Stream Name"
        required
      ></v-text-field>
      <v-btn type="submit" :loading="overviewStore.loading" color="error"
        >Drop Stream</v-btn
      >
    </v-form>
    <v-textarea
      label="Error"
      v-if="overviewStore.error"
      v-model="overviewStore.error"
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
import { useOverviewStore } from "@/stores/auctionStore";

const overviewStore = useOverviewStore();
const streamName = ref<string | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");

const dropStream = async () => {
  if (streamName.value !== null) {
    const response = await overviewStore.dropStream(streamName.value);
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
