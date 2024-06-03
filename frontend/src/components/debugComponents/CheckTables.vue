<template>
  <v-card>
    <h1>Check Tables</h1>
    <v-btn @click="checkTables" :loading="overviewStore.loading"
      >Check Tables</v-btn
    >
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
import { useOverviewStore } from "@/stores/overviewStore";
import { ref, watch } from "vue";

const overviewStore = useOverviewStore();
const result = ref<object | null>(null);
const formattedResult = ref<string>("");

const checkTables = async () => {
  const response = await overviewStore.checkTables();
  result.value = response;
};

watch(result, (newResult) => {
  if (newResult) {
    formattedResult.value = JSON.stringify(newResult, null, 2);
  } else {
    formattedResult.value = "";
  }
});
</script>
