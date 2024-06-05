<template>
  <v-card>
    <h1>Drop Table</h1>
    <v-form @submit.prevent="dropTable">
      <v-text-field
        v-model="tableName"
        label="Table Name"
        required
      ></v-text-field>
      <v-btn type="submit" :loading="isLoading" color="error">Drop Table</v-btn>
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
import { useOverviewStore } from "@/stores/overviewStore";
import { ref, watch } from "vue";

const overviewStore = useOverviewStore();
const tableName = ref<string | null>(null);
const result = ref<object | null>(null);
const formattedResult = ref<string>("");
const errorMessage = ref<string | null>(null);
const isLoading = ref(true);

const dropTable = async () => {
  if (tableName.value !== null) {
    const { data, loading, error } = await overviewStore.dropTable(
      tableName.value
    );
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
