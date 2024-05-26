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
            :sort-by="[{ key: 'Timestamp', order: 'desc' }]"
          >
            <template v-slot:loading>
              <v-skeleton-loader type="table-row@10"></v-skeleton-loader>
            </template>
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
              v-for="chatMessage in reversedChatMessages"
              v-scroll:#scroll-target="onScroll"
            >
              <v-textarea
                :label="`${chatMessage.Username} - ${formatTimestamp(
                  chatMessage.Timestamp
                )}`"
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

          <v-text-field
            v-model="newMessage"
            label="Enter your message"
          ></v-text-field>
          <v-btn @click="sendMessage">Send</v-btn>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useRoute } from "vue-router";
import { useAuctionStore } from "@/stores/auctionStore";
import { useChatStore } from "@/stores/chatStore";
import { useUserStore } from "@/stores/userStore";
import InsertAuctionBid from "@/components/auction/InsertAuctionBid.vue";

const route = useRoute();
const auctionStore = useAuctionStore();
const chatStore = useChatStore();
const userStore = useUserStore();
const auctionId = route.params.auctionId as string;
const dialog = ref(false);
const bidsLoading = ref(true);
const bidsError = ref<string | null>(null);
const chatLoading = ref(true);
const chatError = ref<string | null>(null);
const newMessage = ref("");

const reversedChatMessages = computed(() => {
  return [...chatStore.chatMessages].reverse();
});

onMounted(async () => {
  auctionStore.connectBidsWebSocket(auctionId, onFirstBidMessage, onBidError);
  chatStore.connectChatWebSocket(auctionId, onFirstChatMessage, onChatError);
  await userStore.fetchUserInfo();
  console.log(userStore.userInfo);
});

onBeforeUnmount(() => {
  auctionStore.disconnectBidsWebSocket();
  chatStore.disconnectChatWebSocket();
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

async function sendMessage() {
  console.log(userStore.userInfo?.userName);
  if (newMessage.value.trim() !== "") {
    try {
      await chatStore.insertChatMessage({
        username: userStore.userInfo?.userName ?? "anon",
        messageText: newMessage.value,
        auction_Id: auctionId,
      });
      newMessage.value = "";
    } catch (error) {
      console.error("Failed to send message", error);
    }
  }
}

function formatTimestamp(timestamp: string): string {
  const messageDate = new Date(timestamp);
  const today = new Date();
  const isToday = messageDate.toDateString() === today.toDateString();
  const formattedTime = messageDate.toTimeString().split(" ")[0];
  if (isToday) {
    return `Today at ${formattedTime}`;
  } else {
    const formattedDate = messageDate.toLocaleDateString("en-GB");
    return `${formattedDate} ${formattedTime}`;
  }
}
</script>
