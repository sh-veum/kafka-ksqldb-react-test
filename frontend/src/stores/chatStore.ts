import { defineStore } from "pinia";
import { ref } from "vue";
import axios from "axios";
import type { ChatMessage } from "@/models/ChatMessage";
import { webSocketService } from "@/lib/webSocket";
import { normalizeData } from "@/lib/normalizeData";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useChatStore = defineStore("chat", () => {
  const chatMessages = ref<ChatMessage[]>([]);
  const chatMessagesAlt = ref<ChatMessage[]>([]);
  const loading = ref(false);
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

  const getChatMessagesForAuction = async (
    auctionId: string
  ): Promise<boolean> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `${baseUrl}/api/chat/get_messages_for_auction?auction_Id=${auctionId}`
      );
      const normalizedData = response.data.map((dto: any) =>
        normalizeData(dto)
      );
      chatMessages.value = normalizedData;
      return true;
    } catch (err) {
      error.value = "Failed to retrieve chat messages";
      console.error(err);
      return false;
    } finally {
      loading.value = false;
    }
  };

  const getChatMessagesForAuctionAlt = async (
    auctionId: string
  ): Promise<boolean> => {
    loading.value = true;
    error.value = null;
    try {
      const response = await axios.get(
        `${baseUrl}/api/chat/get_messages_for_auction_alternative?auction_Id=${auctionId}`
      );
      const normalizedData = response.data.map((dto: any) =>
        normalizeData(dto)
      );
      chatMessagesAlt.value = normalizedData;
      return true;
    } catch (err) {
      error.value = "Failed to retrieve chat messages";
      console.error(err);
      return false;
    } finally {
      loading.value = false;
    }
  };

  function connectChatWebSocket(
    auctionId: string,
    onError: (err: string) => void
  ) {
    webSocketService.connectChat(
      auctionId,
      (data) => {
        chatMessages.value.push(data);
        chatMessagesAlt.value.push(data);
      },
      onError
    );
  }

  function disconnectChatWebSocket() {
    webSocketService.disconnectChat();
  }

  return {
    chatMessages,
    chatMessagesAlt,
    loading,
    error,
    insertChatMessage,
    getChatMessagesForAuction,
    getChatMessagesForAuctionAlt,
    connectChatWebSocket,
    disconnectChatWebSocket,
  };
});
