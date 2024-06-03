<template>
  <v-card>
    <h1>Drop Table</h1>
    <v-form @submit.prevent="dropTable">
      <v-text-field
        v-model="tableName"
        label="Table Name"
        required
      ></v-text-field>
      <v-btn type="submit" :loading="overviewStore.loading" color="error"
        >Drop Table</v-btn
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
import { useOverviewStore } from "@/stores/overviewStore";
import { ref, watch } from "vue";

const overviewStore = useOverviewStore();
const tableName = ref<string | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");

const dropTable = async () => {
  if (tableName.value !== null) {
    const response = await overviewStore.dropTable(tableName.value);
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
