import { useEffect } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { WebSocketSubscription } from "@/Enums/webSocketSubscription";
import { Auction } from "@/models/Auction";
import { addOrUpdateData } from "@/utils/addOrUpdateData";

const baseUrl = import.meta.env.VITE_API_WEBSOCKET_URL;

const useAuctionsWebSocket = (isEnabled: boolean) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    if (!isEnabled) return;

    const webSocketUrl = `${baseUrl}?auctionId=none&webSocketSubscription=${WebSocketSubscription.AuctionOverview}`;
    console.log("WebSocket URL:", webSocketUrl);
    const websocket = new WebSocket(webSocketUrl);

    websocket.onopen = () => {
      console.log("connected to auctions websocket");
    };

    websocket.onmessage = (event) => {
      const data = JSON.parse(event.data);
      console.log("Auctions WebSocket message received:", data);

      queryClient.setQueryData(["allAuctions"], (oldData: Auction[]) => {
        return addOrUpdateData(oldData, data, (auction) => auction.Auction_Id);
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
  }, [queryClient, isEnabled]);
};

export default useAuctionsWebSocket;
