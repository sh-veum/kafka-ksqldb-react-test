import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { ChatMessage } from "@/models/ChatMessage";
import { useEffect, useRef } from "react";
import { baseWebsocketUrl } from "@/lib/baseUrls";
import { addOrUpdateData } from "../addOrUpdateData";

/**
 * Custom hook to manage the WebSocket connection for chat messages.
 *
 * @param chatRoomId The ID of the chat room to listen for messages on.
 * @param isEnabled Whether the WebSocket should be enabled.
 */
const useChatWebSocket = (chatRoomId: string, isEnabled: boolean) => {
  const queryClient = useQueryClient();
  const websocketRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    if (!isEnabled || websocketRef.current) return;

    const webSocketUrl = `${baseWebsocketUrl}?auctionId=${chatRoomId}&webSocketSubscription=${WebSocketSubscription.Chat}`;
    const websocket = new WebSocket(webSocketUrl);
    websocketRef.current = websocket;

    websocket.onopen = () => {
      console.log("connected to websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Chat WebSocket message received:", data);

      queryClient.setQueryData(
        ["chatMessages", chatRoomId],
        (oldData: ChatMessage[]) => {
          return addOrUpdateData(oldData, data, (key) => key.Message_Id);
        }
      );
    };

    websocket.onerror = (error) => {
      console.error("WebSocket error:", error);
    };

    websocket.onclose = (event) => {
      console.log("WebSocket closed:", event);
      websocketRef.current = null;
    };

    return () => {
      websocket.close();
    };
  }, [isEnabled, chatRoomId, queryClient]);

  // Cleanup WebSocket on component unmount
  useEffect(() => {
    return () => {
      if (websocketRef.current) {
        websocketRef.current.close();
      }
    };
  }, []);
};

export default useChatWebSocket;
