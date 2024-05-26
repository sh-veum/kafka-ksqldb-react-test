import { defineStore } from "pinia";
import { ref } from "vue";
import axios from "axios";
import type { ChatMessage } from "@/models/ChatMessage";
import { webSocketService } from "@/lib/webSocket";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useChatStore = defineStore("chat", () => {
  const chatMessages = ref<ChatMessage[]>([]);
  const error = ref<string | null>(null);

  const insertChatMessage = async (message: {
    username: string;
    messageText: string;
    auction_Id: string;
  }) => {
    try {
      await axios.post(`${baseUrl}/api/chat/insert_message`, message);
    } catch (err) {
      error.value = "Failed to send message";
      console.error(err);
    }
  };

  function connectChatWebSocket(
    auctionId: string,
    onFirstMessage: () => void,
    onError: (err: string) => void
  ) {
    webSocketService.connectChat(
      auctionId,
      (data) => {
        if (chatMessages.value.length === 0) {
          onFirstMessage();
        }
        chatMessages.value.push(data);
      },
      onError
    );
  }

  function disconnectChatWebSocket() {
    webSocketService.disconnectChat();
  }

  return {
    chatMessages,
    error,
    insertChatMessage,
    connectChatWebSocket,
    disconnectChatWebSocket,
  };
});
