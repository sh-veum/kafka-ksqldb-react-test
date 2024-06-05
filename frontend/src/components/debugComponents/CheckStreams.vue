<template>
  <v-card>
    <h1>Check Streams</h1>
    <v-btn @click="checkStreams" :loading="isLoading">Check Streams</v-btn>
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
import { useOverviewStore } from "@/stores/overviewStore";
import { ref, watch } from "vue";

const overviewStore = useOverviewStore();
const result = ref<object | null>(null);
const formattedResult = ref<string>("");
const errorMessage = ref<string | null>(null);
const isLoading = ref(true);

const checkStreams = async () => {
  const { data, loading, error } = await overviewStore.checkStreams();
  result.value = data;
  isLoading.value = loading;
  errorMessage.value = error;
};

watch(result, (newResult) => {
  if (newResult) {
    formattedResult.value = JSON.stringify(newResult, null, 2);
  } else {
    formattedResult.value = "";
  }
});
</script>
