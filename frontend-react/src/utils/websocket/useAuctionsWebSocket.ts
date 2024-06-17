import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Auction } from "@/models/Auction";
import { addOrUpdateData } from "@/utils/addOrUpdateData";
import { useEffect, useRef } from "react";
import { baseWebsocketUrl } from "@/lib/baseUrls";

/**
 * Custom hook to manage the WebSocket connection for auctions.
 *
 * @param {boolean} isEnabled - Whether the WebSocket should be enabled.
 */
const useAuctionsWebSocket = (isEnabled: boolean) => {
  const queryClient = useQueryClient();
  const websocketRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    if (!isEnabled || websocketRef.current) return;

    const webSocketUrl = `${baseWebsocketUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AuctionOverview}`;
    const websocket = new WebSocket(webSocketUrl);
    websocketRef.current = websocket;

    websocket.onopen = () => {
      console.log("connected to websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Auctions WebSocket message received:", data);

      queryClient.setQueryData(["allAuctions"], (oldData: Auction[]) => {
        return addOrUpdateData(oldData, data, (key) => key.Auction_Id);
      });
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
  }, [isEnabled, queryClient]);

  // Cleanup WebSocket on component unmount
  useEffect(() => {
    return () => {
      if (websocketRef.current) {
        websocketRef.current.close();
      }
    };
  }, []);
};

export default useAuctionsWebSocket;
