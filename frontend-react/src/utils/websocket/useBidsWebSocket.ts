import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Bid } from "@/models/Bid";

const baseUrl = import.meta.env.VITE_API_WEBSOCKET_URL;

const useBidsWebSocket = (auctionId: string, isEnabled: boolean) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!isEnabled) return;

    const webSocketUrl = `${baseUrl}?auctionId=${auctionId}&webSocketSubscription=${WebSocketSubscription.SpecificAuction}`;
    console.log("WebSocket URL:", webSocketUrl);
    const websocket = new WebSocket(webSocketUrl);

    websocket.onopen = () => {
      console.log("connected to bids websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("WebSocket message received:", data);

      queryClient.setQueryData(["auctionBids", auctionId], (oldData: Bid[]) => {
        return [...(oldData || []), data];
      });
    };

    websocket.onerror = (error) => {
      console.error("WebSocket error:", error);
    };

    websocket.onclose = (event) => {
      console.log("WebSocket closed:", event);
    };

    return () => {
      websocket.close();
    };
  }, [queryClient, auctionId, isEnabled]);
};

export default useBidsWebSocket;
