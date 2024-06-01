<template>
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
        v-for="chatMessage in chatStore.chatMessages"
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
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from "vue";
import { useChatStore } from "@/stores/chatStore";
import { useUserStore } from "@/stores/userStore";

const props = defineProps<{ auctionId: string }>();

const chatStore = useChatStore();
const userStore = useUserStore();
const chatLoading = ref(true);
const chatError = ref<string | null>(null);
const newMessage = ref("");

onMounted(async () => {
  chatLoading.value = !(await chatStore.getChatMessagesForAuction(
    props.auctionId
  ));
  chatStore.connectChatWebSocket(props.auctionId, onChatError);
});

onBeforeUnmount(() => {
  chatStore.disconnectChatWebSocket();
});

function onChatError(err: string) {
  chatLoading.value = false;
  chatError.value = err;
}

async function sendMessage() {
  if (newMessage.value.trim() !== "") {
    try {
      await chatStore.insertChatMessage({
        username: userStore.userInfo?.UserName ?? "anon",
        messageText: newMessage.value,
        auction_Id: props.auctionId,
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
