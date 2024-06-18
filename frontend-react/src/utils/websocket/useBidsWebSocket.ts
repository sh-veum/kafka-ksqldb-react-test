import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Bid } from "@/models/Bid";
import { useEffect, useRef } from "react";
import { baseWebsocketUrl } from "@/lib/baseUrls";

/**
 * Custom hook to manage the WebSocket connection for auction bids.
 *
 * @param auctionId The ID of the auction to listen for bids on.
 * @param isEnabled Whether the WebSocket should be enabled.
 */
const useBidsWebSocket = (auctionId: string, isEnabled: boolean) => {
  const queryClient = useQueryClient();
  const websocketRef = useRef<WebSocket | null>(null);

  useEffect(() => {
    if (!isEnabled || websocketRef.current) return;

    const webSocketUrl = `${baseWebsocketUrl}?auctionId=${auctionId}&webSocketSubscription=${WebSocketSubscription.SpecificAuction}`;
    const websocket = new WebSocket(webSocketUrl);
    websocketRef.current = websocket;

    websocket.onopen = () => {
      console.log("connected to websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Bids WebSocket message received:", data);

      queryClient.setQueryData(["auctionBids", auctionId], (oldData: Bid[]) => {
        return [...(oldData || []), data];
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
  }, [isEnabled, auctionId, queryClient]);

  // Cleanup WebSocket on component unmount
  useEffect(() => {
    return () => {
      if (websocketRef.current) {
        websocketRef.current.close();
      }
    };
  }, []);
};

export default useBidsWebSocket;
