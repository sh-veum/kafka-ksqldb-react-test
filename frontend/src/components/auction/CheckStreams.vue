<template>
  <v-card>
    <h1>Check Streams</h1>
    <v-btn @click="checkStreams" :loading="overviewStore.loading"
      >Check Streams</v-btn
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

<script lang="ts">
import { defineComponent, ref, watch } from "vue";
import { useOverviewStore } from "@/stores/auctionStore";

export default defineComponent({
  setup() {
    const overviewStore = useOverviewStore();
    const result = ref<object | null>(null);
    const formattedResult = ref<string>("");

    const checkStreams = async () => {
      const response = await overviewStore.checkStreams();
      result.value = response;
    };

    watch(result, (newResult) => {
      if (newResult) {
        formattedResult.value = JSON.stringify(newResult, null, 2);
      } else {
        formattedResult.value = "";
      }
    });

    return {
      overviewStore,
      checkStreams,
      result,
      formattedResult,
    };
  },
});
</script>
