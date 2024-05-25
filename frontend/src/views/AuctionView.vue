<template>
  <div style="max-height: 800px">
    <h1>Auction {{ auctionId }}</h1>
    <div>
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
              <InsertAuctionBid :auctionId="auctionId" />
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn text="Close" @click="isActive.value = false"></v-btn>
            </v-card-actions>
          </v-card>
        </template>
      </v-dialog>
    </div>
    <v-container fluid>
      <v-row>
        <v-col cols="12" md="6">
          <v-data-table
            :items="auctionStore.auctionBidsMessages"
            :loading="bidsLoading"
            item-key="BidId"
          >
            <template v-slot:loading>
              <v-skeleton-loader type="table-row@10"></v-skeleton-loader>
            </template>
            <!-- <template v-slot:body>
              <template v-if="error">
                <tr>
                  <td colspan="100%" class="text-center">{{ error }}</td>
                </tr>
              </template>
            </template> -->
          </v-data-table>
        </v-col>
        <v-col cols="12" md="6" class="d-flex flex-column">
          <div v-if="chatLoading">
            <v-skeleton-loader type="list-item-two-line@8"></v-skeleton-loader>
          </div>
          <div v-else-if="chatError">
            <v-alert type="error">{{ chatError }}</v-alert>
          </div>

          <v-container
            id="scroll-target"
            class="overflow-y-auto"
            style="max-height: 640px"
          >
            <template
              v-for="chatMessage in auctionStore.chatMessages"
              v-scroll:#scroll-target="onScroll"
            >
              <v-textarea
                :label="`${chatMessage.Username} - ${chatMessage.Timestamp}`"
                :model-value="chatMessage.MessageText"
                rows="1"
                variant="filled"
                auto-grow
                counter
                readonly
                hover
              >
              </v-textarea>
            </template>
          </v-container>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useRoute } from "vue-router";
import { useAuctionStore } from "@/stores/auctionStore";
import InsertAuctionBid from "@/components/auction/InsertAuctionBid.vue";

const route = useRoute();
const auctionStore = useAuctionStore();
const auctionId = route.params.auctionId as string;
const dialog = ref(false);
const bidsLoading = ref(true);
const bidsError = ref<string | null>(null);
const chatLoading = ref(true);
const chatError = ref<string | null>(null);

onMounted(() => {
  auctionStore.connectBidsWebSocket(auctionId, onFirstBidMessage, onBidError);
  auctionStore.connectChatWebSocket(auctionId, onFirstChatMessage, onChatError);
});

onBeforeUnmount(() => {
  auctionStore.disconnectBidsWebSocket();
  auctionStore.disconnectChatWebSocket();
});

function onFirstBidMessage() {
  bidsLoading.value = false;
}

function onBidError(err: string) {
  bidsLoading.value = false;
  bidsError.value = err;
}

function onFirstChatMessage() {
  chatLoading.value = false;
}

function onChatError(err: string) {
  chatLoading.value = false;
  chatError.value = err;
}
</script>
