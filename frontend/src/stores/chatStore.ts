import { defineStore } from "pinia";
import { ref } from "vue";
import { ChatService } from "@/services/chatService";
import type { ChatMessage } from "@/models/ChatMessage";
import { ChatWebSocketHandler } from "@/services/chatWebSocketHandler";

const baseUrl = `${import.meta.env.VITE_API_URL}`;

export const useChatStore = defineStore("chat", () => {
  const chatMessages = ref<ChatMessage[]>([]);
  const chatMessagesAlt = ref<ChatMessage[]>([]);

  const chatService = new ChatService(baseUrl);
  const webSocketHandler = new ChatWebSocketHandler(
    chatMessages,
    chatMessagesAlt
  );

  const insertChatMessage = async (message: {
    username: string;
    messageText: string;
    auction_Id: string;
  }) => {
    const { loading, error } = await chatService.insertChatMessage(message);
    return { loading, error };
  };

  const getChatMessagesForAuction = async (auctionId: string) => {
    const { data, loading, error } =
      await chatService.getChatMessagesForAuction(auctionId);
    if (data) {
      chatMessages.value = data;
    }
    return { loading, error };
  };

  const getChatMessagesForAuctionAlt = async (auctionId: string) => {
    const { data, loading, error } =
      await chatService.getChatMessagesForAuctionAlt(auctionId);
    if (data) {
      chatMessagesAlt.value = data;
    }
    return { loading, error };
  };

  return {
    chatMessages,
    chatMessagesAlt,
    insertChatMessage,
    getChatMessagesForAuction,
    getChatMessagesForAuctionAlt,
    connectChatWebSocket:
      webSocketHandler.connectChatWebSocket.bind(webSocketHandler),
    disconnectChatWebSocket:
      webSocketHandler.disconnectChatWebSocket.bind(webSocketHandler),
  };
});
