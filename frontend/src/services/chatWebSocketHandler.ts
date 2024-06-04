import { type Ref } from "vue";
import { webSocketService } from "@/lib/webSocket";
import type { ChatMessage } from "@/models/ChatMessage";

export class ChatWebSocketHandler {
  private chatMessages: Ref<ChatMessage[]>;
  private chatMessagesAlt: Ref<ChatMessage[]>;

  constructor(
    chatMessages: Ref<ChatMessage[]>,
    chatMessagesAlt: Ref<ChatMessage[]>
  ) {
    this.chatMessages = chatMessages;
    this.chatMessagesAlt = chatMessagesAlt;
  }

  connectChatWebSocket(auctionId: string, onError: (err: string) => void) {
    webSocketService.connectChat(
      auctionId,
      (data) => {
        this.chatMessages.value.push(data);
        this.chatMessagesAlt.value.push(data);
      },
      onError
    );
  }

  disconnectChatWebSocket() {
    webSocketService.disconnectChat();
  }
}
